using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SupervisorModbusWinForms
{
    public class SupervisorModbusServer
    {

        public event Action<string>? LogGenerated;//產生日誌時觸發，例如「收到連線從 192.168.1.100」
                                                  //「寫入 40004 = 1」「發生錯誤：連接超時」
        public event Action<string>? StatusChanged;//伺服器整體狀態改變時觸發，例如「已啟動」「正在監聽埠 1502」
                                                   //「已停止」「錯誤：埠被占用」

        private readonly ushort[] HoldingRegisters = new ushort[200];//模擬的保持暫存器陣列
        private readonly object RegisterLock = new object();//保護 HoldingRegisters 陣列的多執行緒存取
        private readonly object DataLock = new object();//用來保護其他共用資料（例如連線清單、計數器、log 緩衝區等）
                                                        //lock可以避免A在寫入的時候，B去取值

        private bool _serverRunning = false;
        private TcpListener? _listener;//TcpListener 物件，用來監聽 TCP 連線
        private Thread? _serverThread;//主執行緒，負責執行「接受客戶端連線」的無限迴圈
        private Thread? _refreshThread;//背景執行緒，用來模擬設備行為（例如自動增加計數、模擬檢測完成、更新時間）

        public int Port { get; private set; } = 1502;//唯讀屬性

        // ===== Register Map =====
        const int REG_READY = 40001;
        const int REG_TOTAL = 40002;
        const int REG_INDEX = 40003;
        const int REG_LOAD_CMD = 40004;
        const int REG_STATUS = 40005;

        const int REG_YEAR = 40010;
        const int REG_MONTH = 40011;
        const int REG_DAY = 40012;

        const int REG_MEASURE_ID = 40020;
        const int LEN_MEASURE_ID = 8;

        const int REG_WORKORDER = 40030;
        const int LEN_WORKORDER = 4;

        const int REG_SERIAL = 40040;
        const int LEN_SERIAL = 11;

        const int REG_RECIPE = 40060;
        const int REG_DIRECTION = 40061;
        const int LEN_DIRECTION = 2;

        const int REG_POS_Y = 40069;
        const int REG_WIDTH = 40070;
        const int REG_HEIGHT = 40071;

        const int REG_RESULT = 40080;

        const int REG_SAVE_PATH = 40090;
        const int LEN_SAVE_PATH = 34;

        const int REG_USER = 40130;
        const int LEN_USER = 20;

        public LatestData CurrentData { get; private set; } = new LatestData
        {
            Date = DateTime.Now,
            MeasureId = "MID0001",
            WorkOrderNo = "WO001",
            SerialNo = "SN000000001",
            Recipe = "標A",
            Direction = "L",
            PositionY = "200",
            Width = "50",
            Height = "60",
            Result = "OK",
            SavePath = @"D:\DATA\IMG\TEST_0001.BMP",
            User = "SUPERVISOR"
        };

        public void Start(int port)
        {
            if (_serverRunning) return;

            Port = port;
            _serverRunning = true;

            lock (RegisterLock)
            {
                Array.Clear(HoldingRegisters, 0, HoldingRegisters.Length);
                WriteU16(REG_READY, 0);
                WriteU16(REG_TOTAL, 1);
                WriteU16(REG_INDEX, 0);
                WriteU16(REG_LOAD_CMD, 0);
                WriteU16(REG_STATUS, 0);
            }

            _refreshThread = new Thread(RefreshLoop) { IsBackground = true };//當主程式（WinForms）關閉時，
                                                                             //這個執行緒會自動被終止，不會阻
                                                                             //礙應用程式結束。
            _refreshThread.Start();

            _serverThread = new Thread(ServerLoop) { IsBackground = true };
            _serverThread.Start();

            Log($"Supervisor Modbus TCP Server started. Port={Port}");//推測是內部輔助方法，
                                                                      //會呼叫 LogGenerated?.Invoke(...)
                                                                      //把訊息送給 UI（例如加到 TextBox）。
            StatusChanged?.Invoke("Server Started");
        }

        public void Stop()
        {
            _serverRunning = false;

            try
            {
                _listener?.Stop();//使用 ?.（null-conditional operator）避免
                                  //_listener 還沒初始化時發生 NullReferenceException
            }
            catch { }

            Log("Server stopped.");
            StatusChanged?.Invoke("Server Stopped");
        }

        public void UpdateData(LatestData data)
        {
            lock (DataLock)
            {
                CurrentData = data.Clone();
            }
            Log("CurrentData updated from UI.");
        }

        public LatestData GetSnapshot()//在多執行緒環境下，安全地提供目前資料的「唯讀副本」
                                       //給外部使用（例如 UI、記錄、測試、外部介面等），
                                       //而不會讓外部直接拿到原始資料的參考
        {
            lock (DataLock)
            {
                return CurrentData.Clone();
            }
        }

        public ushort[] GetHoldingRegistersSnapshot()//提供目前所有保持暫存器（HoldingRegisters 陣列）
                                                     //的一個完整複製快照
                                                     //回傳「複製」而不是「參考」
        {
            lock (RegisterLock)
            {
                ushort[] copy = new ushort[HoldingRegisters.Length];
                Array.Copy(HoldingRegisters, copy, HoldingRegisters.Length);
                return copy;
            }
        }

        private void ServerLoop()//整個 Modbus TCP 模擬伺服器中最核心的
                                 //「接受客戶端連線並處理請求」的無限迴圈邏輯
        {
            try
            {
                _listener = new TcpListener(IPAddress.Any, Port);//建立並啟動監聽器
                _listener.Start();

                while (_serverRunning)
                {
                    try
                    {
                        Log("Waiting for client...");
                        using TcpClient client = _listener.AcceptTcpClient();//★ 這裡是阻塞點 ★
                                                                             // 會卡住直到有客戶端連進來，
                                                                             // 或是 Stop() 被呼叫導致例外
                        Log("Client connected.");

                        try//處理這個客戶端的 Modbus 通訊
                           //注意：這裡是同步呼叫 HandleClient()，會阻塞直到該客戶端斷線
                        {
                            HandleClient(client);
                        }
                        catch (Exception ex)//HandleClient() 內讀寫資料時發生錯誤（斷線、格式錯等）
                        {
                            Log($"Client error: {ex.Message}");
                        }

                        Log("Client disconnected.");
                    }
                    catch (SocketException)//如果 _serverRunning 還是 true，代表不是正常停止，而是真的錯誤
                    {
                        if (_serverRunning)
                            Log("Socket error in ServerLoop.");
                    }
                    catch (Exception ex)// 其他一般例外
                    {
                        Log($"ServerLoop error: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)//最外層 catch：如果連 TcpListener 都啟動失敗（例如埠被占用）
            {
                Log($"Server start failed: {ex.Message}");
                StatusChanged?.Invoke("Server Error");
            }
        }

        private string DecodeModbus(byte[] data, int len, string direction)//Modbus TCP 解析器。它接收位元組陣列（Byte Array），
                                                                           //根據 功能碼 (Function Code) 解析其內容，
                                                                           //並輸出易於閱讀的除錯資訊。
        {
            if (len < 8) return "";

            byte fc = data[7];
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"{direction} Decode:");

            switch (fc)
            {
                case 0x06: // Write Single Register
                    if (len >= 12)
                    {
                        ushort addr = (ushort)((data[8] << 8) | data[9]);//|（位元或 OR）：用來合併兩個數值。
                                                                         //(Big-Endian)
                                                                         //<<:左移位運算符，將高位移到前面，低位移到後面。 
                        ushort value = (ushort)((data[10] << 8) | data[11]);

                        sb.AppendLine("→ FC06 Write Single Register");
                        sb.AppendLine($"→ Addr: {addr + 40001}");
                        sb.AppendLine($"→ Value: {value}");
                    }
                    break;

                case 0x03: // Read Holding Registers
                    if (direction == "RX" && len >= 12)
                    {
                        ushort addr = (ushort)((data[8] << 8) | data[9]);
                        ushort qty = (ushort)((data[10] << 8) | data[11]);

                        sb.AppendLine("→ FC03 Read Request");
                        sb.AppendLine($"→ Start Addr: {addr + 40001}");
                        sb.AppendLine($"→ Quantity: {qty}");
                    }
                    else if (direction == "TX" && len >= 9)
                    {
                        byte byteCount = data[8];
                        sb.AppendLine("→ FC03 Read Response");
                        sb.AppendLine($"→ Byte Count: {byteCount}");
                    }
                    break;

                case 0x10: // Write Multiple Registers
                    if (direction == "RX" && len >= 13)
                    {
                        ushort addr = (ushort)((data[8] << 8) | data[9]);
                        ushort qty = (ushort)((data[10] << 8) | data[11]);

                        sb.AppendLine("→ FC16 Write Multiple Registers");
                        sb.AppendLine($"→ Start Addr: {addr + 40001}");
                        sb.AppendLine($"→ Quantity: {qty}");
                    }
                    else if (direction == "TX" && len >= 12)
                    {
                        ushort addr = (ushort)((data[8] << 8) | data[9]);
                        ushort qty = (ushort)((data[10] << 8) | data[11]);

                        sb.AppendLine("→ FC16 Write Response");
                        sb.AppendLine($"→ Start Addr: {addr + 40001}");
                        sb.AppendLine($"→ Quantity: {qty}");
                    }
                    break;

                default:
                    sb.AppendLine($"→ Unknown FC: {fc}");
                    break;
            }

            return sb.ToString();
        }
        private void RefreshLoop()
        {
            while (_serverRunning)
            {
                try
                {
                    LatestData snapshot;

                    lock (DataLock)
                    {
                        snapshot = CurrentData.Clone();
                    }

                    lock (RegisterLock)
                    {
                        WriteU16(REG_READY, 0);
                        WriteU16(REG_STATUS, 0);

                        ClearDataArea();
                        WriteDataToRegisters(snapshot);

                        WriteU16(REG_STATUS, 1);
                        WriteU16(REG_READY, 1);
                    }
                }
                catch (Exception ex)
                {
                    Log($"RefreshLoop error: {ex.Message}");

                    lock (RegisterLock)
                    {
                        ClearDataArea();
                        WriteU16(REG_READY, 0);
                        WriteU16(REG_STATUS, 3);
                    }
                }

                Thread.Sleep(200);
            }
        }

        private void HandleClient(TcpClient client)//Modbus TCP 伺服器 (Slave) 的通訊核心處理邏輯。
                                                   //它負責處理單一用戶端的連線，循環讀取請求、解析指令
                                                   //、執行對應邏輯並回傳結果。
        {
            using NetworkStream stream = client.GetStream();//使用了 using 關鍵字，確保處理完後會自動掛斷（釋放資源）。
            byte[] request = new byte[260];//Modbus RTU/TCP 協定規範的最長封包大約就是 250 多個 Byte

            while (_serverRunning)
            {
                int len;
                try
                {
                    len = stream.Read(request, 0, request.Length);//阻塞式 (Blocking) 操作，沒人傳資料時，程式會停在這裡等

                }
                catch
                {
                    break;
                }

                if (len == 0) break;
                if (len < 8) continue;

                byte unitId = request[6];
                byte functionCode = request[7];

                Log("RX: " + BytesToHex(request, len));
                Log(DecodeModbus(request, len, "RX"));

                byte[] response = functionCode switch
                {
                    0x03 => HandleReadHoldingRegisters(request, len, unitId),
                    0x06 => HandleWriteSingleRegister(request, len, unitId),
                    0x10 => HandleWriteMultipleRegisters(request, len, unitId),
                    _ => BuildExceptionResponse(request, unitId, functionCode, 0x01)//錯誤代碼 (0x01)， _ 是其餘的意思
                };

                Log(DecodeModbus(response, response.Length, "TX"));
                stream.Write(response, 0, response.Length);
                Log("TX: " + BytesToHex(response, response.Length));
            }
        }

        private byte[] HandleReadHoldingRegisters(byte[] req, int len, byte unitId)
        {
            if (len < 12)
                return BuildExceptionResponse(req, unitId, 0x03, 0x03);

            ushort startAddr = ReadUInt16BE(req, 8);
            ushort quantity = ReadUInt16BE(req, 10);

            if (quantity == 0 || quantity > 125)
                return BuildExceptionResponse(req, unitId, 0x03, 0x03);

            if (startAddr + quantity > HoldingRegisters.Length)
                return BuildExceptionResponse(req, unitId, 0x03, 0x02);

            int byteCount = quantity * 2;
            ushort lengthField = (ushort)(3 + byteCount);//在 Modbus TCP 中，MBAP Header 的第 5 與第 6 個 Byte
                                                         //（即 resp[4] 和 resp[5]）定義為 「長度欄位 (Length Field)」
                                                         //從 Unit ID 開始計算，直到封包結束的所有 Byte 總數
                                                         //1 (Unit ID)+ 1 (FC)+ 1  (Byte Count) + n{(byteCount (Data)} = 3 + (byteCount)

            byte[] resp = new byte[9 + byteCount];
            resp[0] = req[0];
            resp[1] = req[1];
            resp[2] = 0x00;
            resp[3] = 0x00;
            resp[4] = (byte)(lengthField >> 8);
            resp[5] = (byte)(lengthField & 0xFF);//11111111
            resp[6] = unitId;
            resp[7] = 0x03;
            resp[8] = (byte)byteCount;

            int idx = 9;
            lock (RegisterLock)
            {
                for (int i = 0; i < quantity; i++)
                {
                    ushort value = HoldingRegisters[startAddr + i];
                    resp[idx++] = (byte)(value >> 8);
                    resp[idx++] = (byte)(value & 0xFF);
                }
            }

            return resp;
        }

        private byte[] HandleWriteSingleRegister(byte[] req, int len, byte unitId)
        {
            if (len < 12)
                return BuildExceptionResponse(req, unitId, 0x06, 0x03);

            ushort addr = ReadUInt16BE(req, 8);
            ushort value = ReadUInt16BE(req, 10);

            if (addr >= HoldingRegisters.Length)
                return BuildExceptionResponse(req, unitId, 0x06, 0x02);

            lock (RegisterLock)
            {
                HoldingRegisters[addr] = value;
            }

            byte[] resp = new byte[12];
            Array.Copy(req, resp, 12);
            return resp;
        }

        private byte[] HandleWriteMultipleRegisters(byte[] req, int len, byte unitId)
        {
            if (len < 13)
                return BuildExceptionResponse(req, unitId, 0x10, 0x03);

            ushort startAddr = ReadUInt16BE(req, 8);
            ushort quantity = ReadUInt16BE(req, 10);
            byte byteCount = req[12];

            if (quantity == 0 || quantity > 123)
                return BuildExceptionResponse(req, unitId, 0x10, 0x03);

            if (startAddr + quantity > HoldingRegisters.Length)
                return BuildExceptionResponse(req, unitId, 0x10, 0x02);

            if (byteCount != quantity * 2)
                return BuildExceptionResponse(req, unitId, 0x10, 0x03);

            if (len < 13 + byteCount)
                return BuildExceptionResponse(req, unitId, 0x10, 0x03);

            lock (RegisterLock)
            {
                int dataIdx = 13;
                for (int i = 0; i < quantity; i++)
                {
                    HoldingRegisters[startAddr + i] =
                        (ushort)((req[dataIdx] << 8) | req[dataIdx + 1]);
                    dataIdx += 2;
                }
            }

            byte[] resp = new byte[12];
            resp[0] = req[0];
            resp[1] = req[1];
            resp[2] = 0x00;
            resp[3] = 0x00;
            resp[4] = 0x00;
            resp[5] = 0x06;
            resp[6] = unitId;
            resp[7] = 0x10;
            resp[8] = req[8];
            resp[9] = req[9];
            resp[10] = req[10];
            resp[11] = req[11];
            return resp;
        }

        private void WriteDataToRegisters(LatestData d)
        {
            WriteU16(REG_YEAR, (ushort)d.Date.Year);
            WriteU16(REG_MONTH, (ushort)d.Date.Month);
            WriteU16(REG_DAY, (ushort)d.Date.Day);

            WriteAscii(REG_MEASURE_ID, LEN_MEASURE_ID, d.MeasureId);
            WriteAscii(REG_WORKORDER, LEN_WORKORDER, d.WorkOrderNo);
            WriteAscii(REG_SERIAL, LEN_SERIAL, d.SerialNo);
            WriteAscii(REG_DIRECTION, LEN_DIRECTION, d.Direction);
            WriteAscii(REG_SAVE_PATH, LEN_SAVE_PATH, d.SavePath);
            WriteAscii(REG_USER, LEN_USER, d.User);

            WriteU16(REG_RECIPE, RecipeToCode(d.Recipe));
            WriteU16(REG_RESULT, ResultToCode(d.Result));

            WriteU16(REG_POS_Y, ParseNumericOrNA(d.PositionY));
            WriteU16(REG_WIDTH, ParseNumericOrNA(d.Width));
            WriteU16(REG_HEIGHT, ParseNumericOrNA(d.Height));
        }

        private void ClearDataArea()
        {
            for (int addr = 40010; addr <= 40149; addr++)
            {
                int offset = AddressToOffset(addr);
                if (offset >= 0 && offset < HoldingRegisters.Length)
                    HoldingRegisters[offset] = 0;
            }
        }

        private static ushort RecipeToCode(string recipe) => recipe switch
        {
            "標A" => 1,
            "標B" => 2,
            "標C" => 3,
            _ => 0
        };

        private static ushort ResultToCode(string result) =>
            result.Equals("OK", StringComparison.OrdinalIgnoreCase) ? (ushort)1 : (ushort)0;

        private static ushort ParseNumericOrNA(string text)
        {
            if (string.IsNullOrWhiteSpace(text) || text.Equals("NA", StringComparison.OrdinalIgnoreCase))
                return 0;

            return ushort.TryParse(text, out ushort value) ? value : (ushort)0;
        }

        private static int AddressToOffset(int modbusAddress) => modbusAddress - 40001;

        private void WriteU16(int modbusAddress, ushort value)
        {
            int offset = AddressToOffset(modbusAddress);
            if (offset >= 0 && offset < HoldingRegisters.Length)
                HoldingRegisters[offset] = value;
        }

        private void WriteAscii(int startAddress, int registerLength, string text)
        {
            int startOffset = AddressToOffset(startAddress);
            int maxBytes = registerLength * 2;

            byte[] src = Encoding.ASCII.GetBytes(text ?? string.Empty);
            byte[] buf = new byte[maxBytes];
            Array.Copy(src, buf, Math.Min(src.Length, buf.Length));

            for (int i = 0; i < registerLength; i++)
            {
                byte high = buf[i * 2];
                byte low = buf[i * 2 + 1];
                HoldingRegisters[startOffset + i] = (ushort)((high << 8) | low);
            }
        }

        private static ushort ReadUInt16BE(byte[] data, int offset)
        {
            return (ushort)((data[offset] << 8) | data[offset + 1]);
        }


        private static byte[] BuildExceptionResponse(byte[] req, byte unitId, byte functionCode, byte exceptionCode)
        {
            byte[] resp = new byte[9];
            resp[0] = req.Length > 0 ? req[0] : (byte)0x00;
            resp[1] = req.Length > 1 ? req[1] : (byte)0x00;
            resp[2] = 0x00;
            resp[3] = 0x00;
            resp[4] = 0x00;
            resp[5] = 0x03;
            resp[6] = unitId;
            resp[7] = (byte)(functionCode | 0x80);
            resp[8] = exceptionCode;
            return resp;
        }

        private static string BytesToHex(byte[] data, int len)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < len; i++)
            {
                sb.Append(data[i].ToString("X2"));
                if (i < len - 1) sb.Append(' ');
            }
            return sb.ToString();
        }

        private void Log(string msg)
        {
            LogGenerated?.Invoke($"[{DateTime.Now:HH:mm:ss}] {msg}");
        }

        public class LatestData
        {
            public DateTime Date { get; set; } = DateTime.Now;
            public string MeasureId { get; set; } = "";
            public string WorkOrderNo { get; set; } = "";
            public string SerialNo { get; set; } = "";
            public string Recipe { get; set; } = "";
            public string Direction { get; set; } = "";
            public string PositionY { get; set; } = "";
            public string Width { get; set; } = "";
            public string Height { get; set; } = "";
            public string Result { get; set; } = "";
            public string SavePath { get; set; } = "";
            public string User { get; set; } = "";

            public LatestData Clone()
            {
                return new LatestData
                {
                    Date = this.Date,
                    MeasureId = this.MeasureId,
                    WorkOrderNo = this.WorkOrderNo,
                    SerialNo = this.SerialNo,
                    Recipe = this.Recipe,
                    Direction = this.Direction,
                    PositionY = this.PositionY,
                    Width = this.Width,
                    Height = this.Height,
                    Result = this.Result,
                    SavePath = this.SavePath,
                    User = this.User
                };
            }

        }
    }
}