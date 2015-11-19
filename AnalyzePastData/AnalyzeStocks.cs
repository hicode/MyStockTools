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
        private List<int>[] first;
        private List<int>[] second;
        private List<int>[] third;

        string[][] para = new string[9][]
        {
            new string[2] { "true", "false" },
            new string[3] { "1","2", "3" },
            new string[2] { "0.15", "0.2" },
            new string[3] { "0", "1", "2"},
            new string[4] { "0", "1", "2", "3" },
            new string[2] { "0.05", "0.10"},
            new string[2] { "2", "3" },
            new string[4] { "0.1", "0.05", "0.0001", "-0.02" },
            new string[4] { "0", "1", "3", "4" }
        };

        public AnalyzeStocks(uint startDate, uint endDate)
        {
            this.startDate = startDate;
            this.endDate = endDate;
            this.stocks = Utilities.getStocks(startDate, endDate);
        }

        public void StartAnalyze()
        {
            List<List<string>> list = new List<List<string>>();
            DFS(0, list, new List<string>());
            WriteToExcelFile(list);
            MessageBox.Show(list.Count.ToString());
        }

        private void DFS(int i, List<List<string>> list, List<string> res)
        {
            if (i == 2 && res[0] == "true")
            {
                res.Add("0");
                DFS(i + 1, list, res);
                res.RemoveAt(res.Count - 1);
                return;
            }
            if (i == 2 && res[0] == "false" && res[1] != "3") return;
            if (i == 3)
            {
                getUp(bool.Parse(res[0]), int.Parse(res[1]), float.Parse(res[2]));
            }
            if (i == 4 && res[3] == "0")
            {
                res.Add("0");
                res.Add("0");
                second = first;
                DFS(i + 2, list, res);
                res.RemoveAt(res.Count - 1);
                res.RemoveAt(res.Count - 1);
                return;
            }
            if (i == 5)
            {
                if (res[4] != "0") getTOCompare(int.Parse(res[1]), int.Parse(res[3]), (Turnover)int.Parse(res[4]));
                else second = first;
            }
            if (i == 6)
            {
                if (res[3] == "1" && res[5] == "0.10") return;
                getDown(int.Parse(res[1]), int.Parse(res[3]), float.Parse(res[5]));
            }
            for (int j = 0; j < para[i].Length; j++)
            {
                res.Add(para[i][j]);
                if (i == 8)
                {
                    int pre, post;
                    float rate = finaleRate(int.Parse(res[1]),
                        int.Parse(res[3]), int.Parse(res[6]), float.Parse(res[7]), (BuyAndSell)int.Parse(res[8]), out pre, out post);
                    if (pre < 100)
                    {
                        res.RemoveAt(res.Count - 1);
                        continue;
                    }
                    res.Add(pre.ToString());
                    res.Add(post.ToString());
                    res.Add(rate.ToString());
                    for (int k = 0; k < res.Count; k++)
                    {
                        Console.Write(res[k] + " ");
                    }
                    Console.WriteLine();
                    list.Add(new List<string>(res));
                    res.RemoveAt(res.Count - 1);
                    res.RemoveAt(res.Count - 1);
                    res.RemoveAt(res.Count - 1);
                    res.RemoveAt(res.Count - 1);
                    continue;
                }
                DFS(i + 1, list, res);
                res.RemoveAt(res.Count - 1);
            }
        }

        private void getUp(bool limitUp, int n, float upPercent)
        {
            first = new List<int>[stocks.Count];
            for (int i = 0; i < stocks.Count; i++)
            {
                var list = new List<int>();
                if (limitUp) list = getNDayLimitUp(stocks[i], n, false);
                else list = getNDayUp(stocks[i], n, upPercent);
                first[i] = list;
            }
        }

        private void getTOCompare(int nUp, int nDown, Turnover op)
        {
            second = new List<int>[stocks.Count];
            for (int i = 0; i < first.Length; i++)
            {
                //var newList = from j in before[i]
                //              where compareTureover(stocks[i], nDown, j + nUp, op)
                //              select j;
                List<int> newList = new List<int>();
                foreach (var j in first[i])
                {
                    if (compareTureover(stocks[i], nDown, j + nUp, op)) newList.Add(j);
                }
                second[i] = newList;
            }
        }

        private void getDown(int nUp, int nDown, float downPercent)
        {
            third = new List<int>[stocks.Count];
            for (int i = 0; i < second.Length; i++)
            {
                //var newList = from j in before[i]
                //              where isUp(stocks[i], nDown, j + nUp, downPercent)
                //              select j;
                List<int> newList = new List<int>();
                foreach (var j in second[i])
                {
                    if (isUp(stocks[i], nDown, j + nUp, -downPercent)) newList.Add(j);
                }
                third[i] = newList;
            }
        }

        private float finaleRate(int nUp, int nDown, int nHold, float targetPercent, BuyAndSell strategy, out int pre, out int post)
        {
            pre = 0;
            post = 0;
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
            for (int i = 0; i < third.Length; i++)
            {
                pre += third[i].Count();
                var valid = from j in third[i]
                            where achieve(stocks[i], nHold, j + nUp + nDown, targetPercent)
                            select j;
                post += valid.Count();
            }
            return (float)post / (float)pre;
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

        private void WriteToExcelFile(List<List<string>> list)
        {
            ApplicationClass app = new ApplicationClass();
            if (app == null) MessageBox.Show("null");
            Workbooks workbooks = app.Workbooks;
            Workbook workbook = workbooks.Add(XlWBATemplate.xlWBATWorksheet);
            Worksheet worksheet = (Worksheet)workbook.Worksheets[1];
            //Range range;
            //worksheet.Cells[1, 1] = "1";
            //worksheet.Cells[1, 2] = "2";
            //worksheet.Cells[1, 3] = "3";
            //worksheet.Cells[2, 1] = "4";
            //worksheet.Cells[2, 2] = "5";
            //worksheet.Cells[2, 3] = "6";
            worksheet.Cells[1, 1] = "涨停";
            worksheet.Cells[1, 2] = "连涨(天)";
            worksheet.Cells[1, 3] = "涨幅";
            worksheet.Cells[1, 4] = "连跌(天)";
            worksheet.Cells[1, 5] = "换手";
            worksheet.Cells[1, 6] = "跌幅";
            worksheet.Cells[1, 7] = "持有(天)";
            worksheet.Cells[1, 8] = "预期收益";
            worksheet.Cells[1, 9] = "买卖";
            worksheet.Cells[1, 10] = "符合条件";
            worksheet.Cells[1, 11] = "达到预期";
            worksheet.Cells[1, 12] = "成功率";
            var sorted = list.OrderByDescending(item => float.Parse(item[7]))
                             .ThenByDescending(item => float.Parse(item[11]));
            int i = 0;
            foreach (var item in sorted)
            {
                for (int j = 0; j < 12; j++)
                {
                    if (j == 4) worksheet.Cells[i + 2, j + 1] = ((Turnover)int.Parse(item[j])).ToString();
                    else if (j == 8) worksheet.Cells[i + 2, j + 1] = ((BuyAndSell)int.Parse(item[j])).ToString();
                    else worksheet.Cells[i + 2, j + 1] = item[j];
                }
                i++;
            }
            //for (int i = 0; i < list.Count; i++)
            //{
            //    for (int j = 0; j < 10; j++)
            //    {
            //        if (j == 4) worksheet.Cells[i + 2, j + 1] = ((Turnover)int.Parse(list[i][j])).ToString();
            //        else if (j == 8) worksheet.Cells[i + 2, j + 1] = ((BuyAndSell)int.Parse(list[i][j])).ToString();
            //        else worksheet.Cells[i + 2, j + 1] = list[i][j];
            //    }
            //}
            workbook.Saved = true;
            workbook.SaveCopyAs(@"G:\StockData\StocksAnalyzeResult.xlsx");
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
            if (Math.Abs(stock.DayLines[index].Open / Utilities.LimitUp(stock.DayLines[index - 1].Close)) < 0.001
                && stock.DayLines[index].Close == stock.DayLines[index].Low) return false;
            float realPercent = stock.DayLines[index + n - 1].Close / stock.DayLines[index].Open - 1;
            return (percent > 0 && realPercent > percent) || (percent < 0 && realPercent < percent);
        }

        private bool lowAndClose(Stock stock, int n, int index, float percent)
        {
            if (index == 0 || index > stock.DayLines.Count - n) return false;
            if (n <= 1) return false;
            if (Math.Abs(stock.DayLines[index].Open / Utilities.LimitUp(stock.DayLines[index - 1].Close)) < 0.001
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
            if (Math.Abs(stock.DayLines[index].Open / Utilities.LimitUp(stock.DayLines[index - 1].Close)) < 0.001
                && stock.DayLines[index].Close == stock.DayLines[index].Low) return false;
            float realPercent = stock.DayLines[index + n - 1].High / stock.DayLines[index].Open - 1;
            return (percent > 0 && realPercent > percent) || (percent < 0 && realPercent < percent);
        }

        private bool lowAndHigh(Stock stock, int n, int index, float percent)
        {
            if (index == 0 || index > stock.DayLines.Count - n) return false;
            if (n <= 1) return false;
            if (Math.Abs(stock.DayLines[index].Open / Utilities.LimitUp(stock.DayLines[index - 1].Close)) < 0.001
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
                        || Math.Abs(stock.DayLines[j].Close - Utilities.LimitUp(stock.DayLines[j - 1].Close)) / stock.DayLines[j - 1].Close > 0.001)) isValid = false;
                    if (includeFlat && Math.Abs(stock.DayLines[j].Close - Utilities.LimitUp(stock.DayLines[j - 1].Close)) / stock.DayLines[j - 1].Close > 0.001) isValid = false;
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

    }
}
