using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AnalyzePastData
{
    static class Utilities
    {
        public static bool dateLargerThan(uint date1, uint date2)
        {
            if ((date1 & 0x0000FFFF) > (date2 & 0x0000FFFF)) return true;
            if ((date1 & 0x0000FFFF) < (date2 & 0x0000FFFF)) return false;
            if ((date1 & 0x00FF0000) > (date2 & 0x00FF0000)) return true;
            if ((date1 & 0x00FF0000) < (date2 & 0x00FF0000)) return false;
            if ((date1 & 0xFF000000) > (date2 & 0xFF000000)) return true;
            return false;

        }

        public static uint DateToUint(int year, int month, int day)
        {
            uint y = (uint)year;
            uint m = (uint)month;
            uint d = (uint)day;
            return (d << 24) + (m << 16) + y;
        }

        public static List<Stock> getStocks(uint startDate, uint endDate)
        {
            List<Stock> list = new List<Stock>();
            Dictionary<int, string> map = new Dictionary<int, string>();
            FileStream name = new FileStream(@"G:\StockData\dataToAnlalyze\codeNameTable.txt", FileMode.Open, FileAccess.Read);
            StreamReader nameReader = new StreamReader(name);
            try
            {
                while (true)
                {
                    string str = nameReader.ReadLine();
                    if (str == null) break;
                    string[] cn = str.Split(',');
                    map[int.Parse(cn[0])] = cn[1];
                }
            }
            catch (IOException)
            { }
            nameReader.Close();
            name.Close();

            BufferedStream stocks = new BufferedStream(new FileStream(@"G:\StockData\dataToAnlalyze\history.cxs", FileMode.Open, FileAccess.Read));
            BinaryReader br = new BinaryReader(stocks);
            while (true)
            {
                try
                {
                    int code = br.ReadInt32();
                    string codeStr = "";
                    for (int i = 0; i < 6 - (code + "").Length; i++) codeStr = codeStr + "0";
                    codeStr = codeStr + code;
                    Stock stock = new Stock(codeStr, map[code]);
                    while (true)
                    {
                        uint date = br.ReadUInt32();
                        if (date == 0xFFFFFFFF) break;
                        float open = br.ReadSingle();
                        float high = br.ReadSingle();
                        float low = br.ReadSingle();
                        float close = br.ReadSingle();
                        uint turnover = br.ReadUInt32();
                        double volume = br.ReadDouble();
                        if (turnover == 0) continue;
                        if (dateLargerThan(date, endDate) || dateLargerThan(startDate, date)) continue;
                        stock.DayLines.Add(new DayLine(date, open, close, high, low, turnover, volume));
                    }
                    list.Add(stock);

                }
                catch (EndOfStreamException)
                {

                    break;
                }
            }
            br.Close();
            stocks.Close();
            return list;
        }

        public static void AddTodayToHistoryFile()
        {
            DateTime end = DateTime.Now;
            uint startDate = DateToUint(2007, 10, 1);
            uint endDate = DateToUint(end.Year, end.Month, end.Day);
            var stocks = getStocks(startDate, endDate);
            stocks.Sort((s1, s2) => int.Parse(s1.Code) - int.Parse(s2.Code));
            ApplicationClass app = new ApplicationClass();
            Workbook wb = app.Workbooks.Open(@"G:\StockData\newdata\Table.xls", Type.Missing, Type.Missing, Type.Missing,
                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                Type.Missing, Type.Missing, Type.Missing);
            Worksheet ws = (Worksheet)wb.Worksheets[1];
            using (BinaryWriter bw = new BinaryWriter(new BufferedStream(new FileStream(@"G:\StockData\testfile.cxs", FileMode.Create, FileAccess.Write))))
            {
                int old = 0, today = 1;
                while (true)
                {
                    Range cell = (Range)ws.Cells[today, 1];
                    if (cell == null) break;
                    if((string) cell.Value == stocks[old].Code)
                    {
                        
                    }
                    else
                    {

                    }
                }
            }
            //var value = (Range)ws.Cells[1, 1];
            //MessageBox.Show((string)value.Value);
            wb.Close();
            wb = null;
            app.Quit();
            app = null;
        }

        /// <summary>
        /// parse all the stock files from 通信达 to a single file.
        /// create a file contains stock code and name table.
        /// </summary>
        public static void ParseHistoryFilesToOneFile()
        {
            BufferedStream target = new BufferedStream(new FileStream(@"G:\StockData\dataToAnlalyze\history.cxs", FileMode.Create, FileAccess.Write));
            BinaryWriter bw = new BinaryWriter(target);
            BufferedStream codeNameTable = new BufferedStream(new FileStream(@"G:\StockData\dataToAnlalyze\codeNameTable.txt", FileMode.Create, FileAccess.Write));
            StreamWriter tw = new StreamWriter(codeNameTable);

            string path = @"G:\StockData\history";
            var files = Directory.GetFiles(path);
            foreach (var file in files)
            {
                BufferedStream source = new BufferedStream(new FileStream(file, FileMode.Open, FileAccess.Read));
                StreamReader sr = new StreamReader(source, Encoding.GetEncoding("gb2312"));
                string str = sr.ReadLine().Trim();
                char[] space = { ' ' };
                string[] name = str.Split(space, StringSplitOptions.RemoveEmptyEntries);
                tw.Write(name[0]);
                tw.Write(',');
                tw.WriteLine(name[1]);
                bw.Write(uint.Parse(name[0]));
                sr.ReadLine();

                while (true)
                {
                    str = sr.ReadLine();
                    string[] data = str.Split('\t');
                    if (data.Length != 7) break;
                    if (uint.Parse(data[5]) == 0) continue;
                    string[] date = data[0].Split('/');
                    bw.Write(short.Parse(date[0]));
                    bw.Write(byte.Parse(date[1]));
                    bw.Write(byte.Parse(date[2]));
                    bw.Write(float.Parse(data[1]));
                    bw.Write(float.Parse(data[2]));
                    bw.Write(float.Parse(data[3]));
                    bw.Write(float.Parse(data[4]));
                    bw.Write(uint.Parse(data[5]));
                    bw.Write(double.Parse(data[6]));
                }
                bw.Write(0xFFFFFFFF);
                sr.Close();
                source.Close();
            }


            codeNameTable.Flush();
            bw.Flush();
            tw.Close();
            codeNameTable.Close();
            bw.Close();
            target.Close();
        }

        /// <summary>
        /// Write stocks to 东方财富的自选股9
        /// </summary>
        /// <param name="stocks"></param>
        public static void WriteStocksToSelfblock(IEnumerable<Stock> stocks)
        {
            StringBuilder newStr = new StringBuilder();
            string file = @"D:\eastmoney\swc8\config\User\m5604094268132944\StockwayStock.ini";
            BufferedStream before = new BufferedStream(new FileStream(file, FileMode.Open, FileAccess.Read));
            StreamReader sr = new StreamReader(before);
            while (true)
            {
                string str = sr.ReadLine();
                if (str.Length >= 5 && str.Substring(0, 5) == "自选股9=") break;
                newStr.Append(str);
                newStr.Append("\r\n");
            }
            newStr.Append("自选股9=");
            foreach (var item in stocks)
            {
                int x = int.Parse(item.Code);
                if (x > 400000) newStr.Append("1.");
                else newStr.Append("0.");
                newStr.Append(item.Code + ",");
            }
            newStr.Append("\r\n");
            sr.Close();
            before.Close();
            BufferedStream after = new BufferedStream(new FileStream(file, FileMode.Create, FileAccess.Write));
            StreamWriter sw = new StreamWriter(after, Encoding.Unicode);
            sw.Write(newStr);
            sw.Flush();
            sw.Close();
            after.Close();
        }
    }
}
