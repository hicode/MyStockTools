using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalyzePastData
{
    class Utilities
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
    }
}
