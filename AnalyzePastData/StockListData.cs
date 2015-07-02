using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace AnalyzePastData
{
    class StockListData
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public float Close { get; set; }
        public float Up { get; set; }
        public float UpPercent { get; set; }
        public Brush BrushClose { get; set; }
    }
}
