using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalyzePastData
{
    struct DayLine
    {
        public uint Date;
        public float Open;
        public float Close;
        public float High;
        public float Low;
        public uint Turnover;
        public double Volume;
    }
    class Stock : IComparable<Stock>
    {
        public string Code { get; private set; }
        public string Name { get; private set; }
        public List<DayLine> DayLines { get; set; }


        public int CompareTo(Stock other)
        {
            int a = int.Parse(this.Code);
            int b = int.Parse(other.Code);
            if (a < 100000 && b > 100000) return 1;
            if (a > 100000 && b < 100000) return -1;
            return a - b;
        }
    }
}
