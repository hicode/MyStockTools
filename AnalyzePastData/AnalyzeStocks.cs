﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalyzePastData
{
    class AnalyzeStocks
    {
        List<Stock> stocks;
        public AnalyzeStocks(List<Stock> stocks)
        {
            this.stocks = stocks;
        }

        public float UpDownUp(uint startDate, uint endDate, bool limitUp, int nUp, float upPercent, int nDown, float downPercent, int hold, float targetPercent)
        {
            int pre = 0;
            int post = 0;
            foreach (var stock in stocks)
            {
                List<int> indexes = new List<int>();
                if (limitUp) indexes = getNDayLimitUp(stock, nUp, startDate, endDate, false);
                else indexes = getNDayUp(stock, nUp, startDate, endDate, upPercent);
                //List<int> preConditions = new List<int>();
                //foreach (var i in indexes)
                //{
                //    if (isUp(stock, nDown, i + nUp, downPercent)) preConditions.Add(i);
                //}
                var preConditions = from i in indexes
                                    where isUp(stock, nDown, i + nUp, downPercent)
                                    select i;
                var valid = from i in preConditions
                            where isUp(stock, hold, i + nUp + nDown, targetPercent)
                            select i;
                pre += preConditions.Count();
                post += valid.Count();
            }
            return (float)post / (float)pre;
        }

        public bool IsNDayLimitUp(Stock stock, int n, uint startDate, bool includeFlat)
        {
            int start = getIndex(startDate, stock);
            if (start <= 0) return false;
            if (start > stock.DayLines.Count - n) return false;
            bool isValid = true;
            for (int j = start; j < start + n && j < stock.DayLines.Count; j++)
            {
                if (!includeFlat && (stock.DayLines[j].Low == stock.DayLines[j].Close
                    || stock.DayLines[j].Close != limitUp(stock.DayLines[j - 1].Close))) isValid = false;
                if (includeFlat && stock.DayLines[j].Close != limitUp(stock.DayLines[j - 1].Close)) isValid = false;
            }
            return isValid;
        }

        private List<int> getNDayUp(Stock stock, int n, uint startDate, uint endDate, float percent)
        {
            var res = new List<int>();
            int start = getIndex(startDate, stock);
            int end = getIndex(endDate, stock);
            for (int i = start; i < end - n + 1; i++)
            {
                if (isUp(stock, n, i, percent)) res.Add(i);
            }
            return res;
        }


        public bool isUp(Stock stock, int n, int index, float percent)
        {
            if (index == 0 || index > stock.DayLines.Count - n) return false;
            if (n == 0) return true;
            float realPercent = stock.DayLines[index + n - 1].Close / stock.DayLines[index - 1].Close - 1;
            return (percent > 0 && realPercent > percent) || (percent < 0 && realPercent < percent);
        }

        public List<int> getNDayLimitUp(Stock stock, int n, uint startDate, uint endDate, bool includeFlat)
        {
            var res = new List<int>();
            int start = getIndex(startDate, stock);
            int end = getIndex(endDate, stock);
            for (int i = start; i < end - n + 1; i++)
            {
                if (i == 0) continue;
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

        public int getIndex(uint date, Stock stock)
        {
            if (stock.DayLines.Count != 0 && dateLargerThan(stock.DayLines[0].Date, date)) return 0;
            for (int i = 0; i < stock.DayLines.Count; i++)
            {
                if (stock.DayLines[i].Date == date) return i;
                if (i > 0 && dateLargerThan(stock.DayLines[i].Date, date) && dateLargerThan(date, stock.DayLines[i - 1].Date)) return i;
            }
            return stock.DayLines.Count - 1;
        }

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

        public uint DateToUint(int year, int month, int day)
        {
            uint y = (uint)year;
            uint m = (uint)month;
            uint d = (uint)day;
            //return (y << 16) + (m << 8) + d;
            return (d << 24) + (m << 16) + y;
        }

    }
}