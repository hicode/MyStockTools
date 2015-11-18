using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalyzePastData
{
    class StocksFilter
    {
        private List<Stock> stocks;
        private uint latestDay;
        private HashSet<Stock> selectStocks;

        public StocksFilter(List<Stock> stocks)
        {
            this.stocks = stocks;
            selectStocks = new HashSet<Stock>();
            foreach (var stock in stocks)
            {
                if (stock.DayLines.Count == 0) continue;
                var date = stock.DayLines[stock.DayLines.Count - 1].Date;
                latestDay = Utilities.DateLargerThan(date, latestDay) ? date : latestDay;
            }
        }

        public void GetUpDown(int up,int down ,float upPercent,float downPercent)
        {
            foreach (var stock in stocks)
            {
                var count = stock.DayLines.Count;
                if (count <= up + down) continue;
                if (Utilities.DateLargerThan(latestDay, stock.DayLines[count - 1].Date)) continue;
                if (stock.DayLines[count - down - 1].Close / stock.DayLines[count - up - down - 1].Close < 1 + upPercent) continue;
                if (stock.DayLines[count - 1].Close / stock.DayLines[count - down - 1].Close > 1 - downPercent) continue;
                selectStocks.Add(stock);
            }
        }

        public void OutputResult()
        {
            Utilities.WriteStocksToSelfblock(selectStocks);
        }
    }
}
