﻿using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AnalyzePastData
{
    enum BuyAndSell
    {
        CloseAndClose, OpenAndClose, LowAndClose, CloseAndHigh, OpenAndHigh, LowAndHigh
    }

    enum Turnover
    {
        Discard, Greater, Equal, Less
    }

    class AnalyzeStocks
    {
        List<Stock> stocks;
        private uint startDate;
        private uint endDate;

        string[][] para = new string[9][]
        {
            new string[2] { "true", "false" },
            new string[4] { "1","2", "3", "4" },
            new string[3] { "0.15", "0.2", "0.25" },
            new string[4] { "0", "1", "2", "3" },
            new string[4] { "0.05", "0.10", "0.15", "0.20" },
            new string[4] { "1", "2", "3", "4" },
            new string[7] { "0.1", "0.05", "0.02", "0.0001", "-0.02", "-0.05", "-0.1" },
            new string[6] { "0", "1", "2", "3","4","5"  },
            new string[4] { "0", "1", "2", "3" }
        };

        public AnalyzeStocks(uint startDate, uint endDate)
        {
            this.startDate = startDate;
            this.endDate = endDate;
            this.stocks = getStocks();
        }

        public void StartAnalyze()
        {
            List<List<string>> list = new List<List<string>>();
            DFS(0, list, new List<string>());
            MessageBox.Show(list.Count.ToString());
        }

        private void DFS(int i, List<List<string>> list, List<string> res)
        {
            for (int j = 0; j < para[i].Length; j++)
            {
                Console.WriteLine("{0} {1}", i, j);
                res.Add(para[i][j]);
                if (i == 8)
                {
                    float rate = RateOf(bool.Parse(res[0]), int.Parse(res[1]), float.Parse(res[2]), int.Parse(res[3]),
                        float.Parse(res[4]), int.Parse(res[5]), float.Parse(res[6]), (BuyAndSell)int.Parse(res[7]), (Turnover)int.Parse(res[8]));
                    res.Add(rate.ToString());
                    list.Add(new List<string>(res));
                    res.RemoveAt(res.Count - 1);
                    res.RemoveAt(res.Count - 1);
                    continue;
                }
                DFS(i + 1, list, res);
                res.RemoveAt(res.Count - 1);
            }
        }

        public float RateOf(bool limitUp, int nUp, float upPercent, int nDown, float downPercent, int hold, float targetPercent, BuyAndSell strategy, Turnover op)
        {
            int pre = 0;
            int post = 0;
            Func<Stock, int, int, float, bool> achieve = null;
            switch (strategy)
            {
                case BuyAndSell.CloseAndClose: achieve = isUp; break;
                case BuyAndSell.OpenAndClose: achieve = openAndClose; break;
                case BuyAndSell.LowAndClose: achieve = lowAndClose; break;
                case BuyAndSell.CloseAndHigh: achieve = closeAndHigh; break;
                case BuyAndSell.OpenAndHigh: achieve = openAndHigh; break;
                case BuyAndSell.LowAndHigh: achieve = lowAndHigh; break;
            }
            foreach (var stock in stocks)
            {
                var preConditions = upDown(limitUp, nUp, upPercent, nDown, downPercent, stock, op);
                var valid = from i in preConditions
                            where achieve(stock, hold, i + nUp + nDown, targetPercent)
                            select i;
                pre += preConditions.Count();
                post += valid.Count();
            }
            return (float)post / (float)pre;
        }

        private void WriteToExcelFile()
        {
            ApplicationClass app = new ApplicationClass();
            if (app == null) MessageBox.Show("null");
            Workbooks workbooks = app.Workbooks;
            Workbook workbook = workbooks.Add(XlWBATemplate.xlWBATWorksheet);
            Worksheet worksheet = (Worksheet)workbook.Worksheets[1];
            //Range range;
            worksheet.Cells[1, 1] = "1";
            worksheet.Cells[1, 2] = "2";
            worksheet.Cells[1, 3] = "3";
            worksheet.Cells[2, 1] = "4";
            worksheet.Cells[2, 2] = "5";
            worksheet.Cells[2, 3] = "6";
            workbook.Saved = true;
            workbook.SaveCopyAs(@"G:\StockData\test.xlsx");
            workbook.Close(true, Type.Missing, Type.Missing);
            workbook = null;
            app.Quit();
            app = null;
        }

        private IEnumerable<int> upDown(bool limitUp, int nUp, float upPercent, int nDown, float downPercent, Stock stock, Turnover op)
        {
            List<int> indexes = new List<int>();
            if (limitUp) indexes = getNDayLimitUp(stock, nUp, false);
            else indexes = getNDayUp(stock, nUp, upPercent);
            var afterTurnover = from i in indexes
                                where compareTureover(stock, nDown, i + nUp, op)
                                select i;
            var preConditions = from i in afterTurnover
                                where isUp(stock, nDown, i + nUp, downPercent)
                                select i;
            return preConditions;
        }

        private bool compareTureover(Stock stock, int n, int index, Turnover op)
        {
            if (index == 0 || index > stock.DayLines.Count - n) return false;
            if (op == Turnover.Discard) return true;
            for (int i = 0; i < n; i++)
            {
                if (op == Turnover.Greater)
                {
                    if (stock.DayLines[index + i].Turnover <= stock.DayLines[index - 1].Turnover * 1.15) return false;
                }
                else if (op == Turnover.Less)
                {
                    if (stock.DayLines[index + i].Turnover >= stock.DayLines[index - 1].Turnover * 0.85) return false;
                }
                else if (op == Turnover.Equal)
                {
                    if (stock.DayLines[index + i].Turnover > stock.DayLines[index - 1].Turnover * 1.15
                        || stock.DayLines[index + i].Turnover < stock.DayLines[index - 1].Turnover * 0.85) return false;
                }
            }
            return true;
        }

        //public bool IsNDayLimitUp(Stock stock, int n, uint startDate, bool includeFlat)
        //{
        //    int start = getIndex(startDate, stock);
        //    if (start <= 0) return false;
        //    if (start > stock.DayLines.Count - n) return false;
        //    bool isValid = true;
        //    for (int j = start; j < start + n && j < stock.DayLines.Count; j++)
        //    {
        //        if (!includeFlat && (stock.DayLines[j].Low == stock.DayLines[j].Close
        //            || stock.DayLines[j].Close != limitUp(stock.DayLines[j - 1].Close))) isValid = false;
        //        if (includeFlat && stock.DayLines[j].Close != limitUp(stock.DayLines[j - 1].Close)) isValid = false;
        //    }
        //    return isValid;
        //}

        private List<int> getNDayUp(Stock stock, int n, float percent)
        {
            var res = new List<int>();
            for (int i = 0; i < stock.DayLines.Count - n + 1; i++)
            {
                if (isUp(stock, n, i, percent)) res.Add(i);
            }
            return res;
        }


        private bool isUp(Stock stock, int n, int index, float percent)
        {
            if (index == 0 || index > stock.DayLines.Count - n) return false;
            if (n == 0) return true;
            float realPercent = stock.DayLines[index + n - 1].Close / stock.DayLines[index - 1].Close - 1;
            return (percent > 0 && realPercent > percent) || (percent < 0 && realPercent < percent);
        }

        private bool openAndClose(Stock stock, int n, int index, float percent)
        {
            if (index == 0 || index > stock.DayLines.Count - n) return false;
            if (n <= 1) return false;
            if (Math.Abs(stock.DayLines[index].Open / limitUp(stock.DayLines[index - 1].Close)) < 0.001
                && stock.DayLines[index].Close == stock.DayLines[index].Low) return false;
            float realPercent = stock.DayLines[index + n - 1].Close / stock.DayLines[index].Open - 1;
            return (percent > 0 && realPercent > percent) || (percent < 0 && realPercent < percent);
        }

        private bool lowAndClose(Stock stock, int n, int index, float percent)
        {
            if (index == 0 || index > stock.DayLines.Count - n) return false;
            if (n <= 1) return false;
            if (Math.Abs(stock.DayLines[index].Open / limitUp(stock.DayLines[index - 1].Close)) < 0.001
                && stock.DayLines[index].Close == stock.DayLines[index].Low) return false;
            float realPercent = stock.DayLines[index + n - 1].Close / stock.DayLines[index].Low - 1;
            return (percent > 0 && realPercent > percent) || (percent < 0 && realPercent < percent);
        }

        private bool closeAndHigh(Stock stock, int n, int index, float percent)
        {
            if (index == 0 || index > stock.DayLines.Count - n) return false;
            if (n == 0) return true;
            float realPercent = stock.DayLines[index + n - 1].High / stock.DayLines[index - 1].Close - 1;
            return (percent > 0 && realPercent > percent) || (percent < 0 && realPercent < percent);
        }

        private bool openAndHigh(Stock stock, int n, int index, float percent)
        {
            if (index == 0 || index > stock.DayLines.Count - n) return false;
            if (n <= 1) return false;
            if (Math.Abs(stock.DayLines[index].Open / limitUp(stock.DayLines[index - 1].Close)) < 0.001
                && stock.DayLines[index].Close == stock.DayLines[index].Low) return false;
            float realPercent = stock.DayLines[index + n - 1].High / stock.DayLines[index].Open - 1;
            return (percent > 0 && realPercent > percent) || (percent < 0 && realPercent < percent);
        }

        private bool lowAndHigh(Stock stock, int n, int index, float percent)
        {
            if (index == 0 || index > stock.DayLines.Count - n) return false;
            if (n <= 1) return false;
            if (Math.Abs(stock.DayLines[index].Open / limitUp(stock.DayLines[index - 1].Close)) < 0.001
                && stock.DayLines[index].Close == stock.DayLines[index].Low) return false;
            float realPercent = stock.DayLines[index + n - 1].High / stock.DayLines[index].Low - 1;
            return (percent > 0 && realPercent > percent) || (percent < 0 && realPercent < percent);
        }

        public List<int> getNDayLimitUp(Stock stock, int n, bool includeFlat)
        {
            var res = new List<int>();
            for (int i = 1; i < stock.DayLines.Count - n + 1; i++)
            {
                bool isValid = true;
                for (int j = i; j < i + n; j++)
                {
                    if (!includeFlat && (stock.DayLines[j].Low == stock.DayLines[j].Close
                        || Math.Abs(stock.DayLines[j].Close - limitUp(stock.DayLines[j - 1].Close)) / stock.DayLines[j - 1].Close > 0.001)) isValid = false;
                    if (includeFlat && Math.Abs(stock.DayLines[j].Close - limitUp(stock.DayLines[j - 1].Close)) / stock.DayLines[j - 1].Close > 0.001) isValid = false;
                }
                if (isValid) res.Add(i);
            }
            return res;
        }

        //public int getIndex(uint date, Stock stock)
        //{
        //    if (stock.DayLines.Count != 0 && dateLargerThan(stock.DayLines[0].Date, date)) return 0;
        //    for (int i = 0; i < stock.DayLines.Count; i++)
        //    {
        //        if (stock.DayLines[i].Date == date) return i;
        //        if (i > 0 && dateLargerThan(stock.DayLines[i].Date, date) && dateLargerThan(date, stock.DayLines[i - 1].Date)) return i;
        //    }
        //    return stock.DayLines.Count - 1;
        //}

        private float limitUp(float price)
        {
            double x = price * 1.10;
            x = x * 100 + 0.5;
            return (float)(Math.Floor(x) / 100);
        }

        private bool dateLargerThan(uint date1, uint date2)
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

        private List<Stock> getStocks()
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
