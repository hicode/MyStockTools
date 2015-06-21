using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AnalyzePastData
{
    /// <summary>
    /// Interaction logic for DayLineGraph.xaml
    /// </summary>
    public partial class DayLineGraph : Window
    {
        private List<Stock> stocks;
        //private List<UnitRects> units;
        private int num = 50;
        private float highest, lowest;
        private double width;

        public DayLineGraph()
        {
            InitializeComponent();
            //Rectangle rect = new Rectangle();
            //rect.Stroke = Brushes.Red;
            //rect.Height = 300;
            //rect.Width = 300;
            //canvas.Children.Add(rect);

            uint startDate = Utilities.DateToUint(2007, 10, 1);
            uint endDate = Utilities.DateToUint(2015, 6, 9);
            stocks = Utilities.getStocks(startDate, endDate);

            //xxx.Rect.Stroke = Brushes.Red;
            //xxx.Rect.Fill = Brushes.Black;
            //xxx.Rect.Width = 20;
            //xxx.Rect.Height = 60;
            //Canvas.SetTop(xxx, 100);
            //Canvas.SetLeft(xxx, 100);
            //xxx.Line.Stroke = Brushes.Red;
            //xxx.Line.Width = 1;
            //xxx.Line.Height = 90;
            //xxx.Rect.Margin = new Thickness(0, 10, 0, 0);
            //xxx.Line.Margin = new Thickness(10, 0, 0, 0);
        }

        private void AddUnits(Stock stock)
        {
            setBound(stock);
            for (int i = stock.DayLines.Count - 1; i >= 0; i--)
            {
                Rectangle rect1 = new Rectangle();
                //units.Add(unit);
                canvas.Children.Add(rect1);
                double height = stock.DayLines[i].Open - stock.DayLines[i].Close;
                Brush brush = height <= 0 ? Brushes.Red : Brushes.LightBlue;
                rect1.Stroke = brush;
                if (height > 0) rect1.Fill = brush;
                rect1.Width = width * 9 / 10;
                double x = (highest - lowest) / canvas.ActualHeight;
                rect1.Height = Math.Abs(height) / x;
                Canvas.SetTop(rect1, (highest - Math.Max(stock.DayLines[i].Open, stock.DayLines[i].Close)) / x);
                Canvas.SetLeft(rect1, width * (stock.DayLines.Count - 1 - i));
            }
        }

        private void setBound(Stock stock)
        {
            highest = 0;
            lowest = float.MaxValue;
            for (int i = stock.DayLines.Count > num ? stock.DayLines.Count - num : 0; i < stock.DayLines.Count; i++)
            {
                highest = Math.Max(stock.DayLines[i].High, highest);
                lowest = Math.Min(stock.DayLines[i].Low, lowest);
            }
            width = canvas.ActualWidth / num;
        }

        private void canvas_Loaded(object sender, RoutedEventArgs e)
        {
            AddUnits(stocks[0]);

        }
    }
}
