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

        public Stock(string code, string name)
        {
            Code = code;
            Name = name;
            DayLines = new List<DayLine>();
        }

        public int CompareTo(Stock other)
        {
            int a = int.Parse(this.Code);
            int b = int.Parse(other.Code);
            if (a < 400000 && b > 400000) return 1;
            if (a > 400000 && b < 400000) return -1;
            return a - b;
        }
    }
}
