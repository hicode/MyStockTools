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
        private int num = 150;
        private float highest, lowest;
        private uint maxTurnover;
        private double width;
        private Stock stock;
        private int max;

        public DayLineGraph()
        {
            uint startDate = Utilities.DateToUint(2007, 10, 1);
            uint endDate = Utilities.DateToUint(2015, 6, 9);
            stocks = Utilities.getStocks(startDate, endDate);
            for (int i = 0; i < stocks.Count; i++)
            {
                max = Math.Max(max, stocks[i].DayLines.Count);
            }
            InitializeComponent();

            AddList();
            stock = stocks[stockList.SelectedIndex];
            AddGraph();
            canvas.Focus();
            Keyboard.Focus(canvas);
        }

        private void AddList()
        {
            for (int i = 0; i < stocks.Count; i++)
            {
                stockList.Items.Add(stocks[i].Code + " " + stocks[i].Name);
            }
        }

        private void AddGraph()
        {
            for (int i = 0; i < max; i++)
            {
                canvas.Children.Add(new Rectangle());
                canvas.Children.Add(new Rectangle());
                Panel.SetZIndex(canvas.Children[canvas.Children.Count - 1], 1);
                Panel.SetZIndex(canvas.Children[canvas.Children.Count - 2], 0);
                canvasT.Children.Add(new Rectangle());
            }
        }

        private void SetUnits()
        {
            SetBound();
            for (int i = stock.DayLines.Count - 1; i >= 0; i--)
            {
                Rectangle rect1 = canvas.Children[i * 2 + 1] as Rectangle;
                double height = stock.DayLines[i].Open - stock.DayLines[i].Close;
                Brush brush = height <= 0 ? Brushes.Red : Brushes.Cyan;
                rect1.Stroke = brush;
                if (height > 0) rect1.Fill = brush;
                else rect1.Fill = Brushes.Black;
                rect1.Width = width * 4 / 5;
                double x = (highest - lowest) / canvas.ActualHeight;
                rect1.Height = Math.Abs(height) / x + 1;
                Canvas.SetTop(rect1, (highest - Math.Max(stock.DayLines[i].Open, stock.DayLines[i].Close)) / x);
                Canvas.SetLeft(rect1, width * (i - (stock.DayLines.Count - 1 - num) - 1));
                Rectangle rect2 = canvas.Children[i * 2] as Rectangle;
                rect2.Height = (stock.DayLines[i].High - stock.DayLines[i].Low) / x;
                rect2.Width = 1;
                rect2.Stroke = brush;
                Canvas.SetTop(rect2, (highest - stock.DayLines[i].High) / x);
                Canvas.SetLeft(rect2, width * (i - (stock.DayLines.Count - 1 - num) - 1) + width * 2 / 5 );
            }
            for (int i = stock.DayLines.Count; i < max; i++)
            {
                Rectangle rect1 = canvas.Children[i * 2 + 1] as Rectangle;
                rect1.Height = 0;
                Rectangle rect2 = canvas.Children[i * 2] as Rectangle;
                rect2.Height = 0;
                Rectangle rect = canvasT.Children[i] as Rectangle;
                rect.Height = 0;
            }
            SetTurnover();
        }

        private void SetTurnover()
        {
            SetMaxTurnover();
            for (int i = stock.DayLines.Count - 1; i >= 0; i--)
            {
                Rectangle rect = canvasT.Children[i] as Rectangle;
                double x = maxTurnover / canvasT.ActualHeight;
                rect.Height = stock.DayLines[i].Turnover / x + 1;
                rect.Width = width * 4 / 5;
                if (i == 0) rect.Stroke = Brushes.Red;
                else
                {
                    if (stock.DayLines[i].Close >= stock.DayLines[i - 1].Close) { rect.Fill = Brushes.Black; rect.Stroke = Brushes.Red; }
                    else { rect.Fill = Brushes.Cyan; rect.Stroke = Brushes.Cyan; }
                }
                Canvas.SetTop(rect, (maxTurnover - stock.DayLines[i].Turnover) / x);
                Canvas.SetLeft(rect, width * (i - (stock.DayLines.Count - 1 - num) - 1));
            }
        }

        private void SetBound()
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

        private void SetMaxTurnover()
        {
            maxTurnover = 0;
            for (int i = stock.DayLines.Count > num ? stock.DayLines.Count - num : 0; i < stock.DayLines.Count; i++)
            {
                maxTurnover = Math.Max(maxTurnover, stock.DayLines[i].Turnover);
            }
        }

        private void canvas_Loaded(object sender, RoutedEventArgs e)
        {
            SetUnits();
            ChangeStock();
        }

        private void ChangeStock()
        {
            DataToShow data = this.FindResource("rightPanel") as DataToShow;
            data.Code = stock.Code;
            data.Name = stock.Name;
            int count = stock.DayLines.Count;
            data.Open = count == 0 ? 0 : stock.DayLines[count - 1].Open;
            data.Close = count == 0 ? 0 : stock.DayLines[count - 1].Close;
            data.High = count == 0 ? 0 : stock.DayLines[count - 1].High;
            data.Low = count == 0 ? 0 : stock.DayLines[count - 1].Low;
            data.Turnover = count == 0 ? 0 : stock.DayLines[count - 1].Turnover;
            data.Volume = count == 0 ? 0 : stock.DayLines[count - 1].Volume;
            data.OpenColor = count < 2 ? Brushes.Cyan : SetColor(count - 1, stock.DayLines[count - 1].Open);
            data.CloseColor = count < 2 ? Brushes.Cyan : SetColor(count - 1, stock.DayLines[count - 1].Close);
            data.HighColor = count < 2 ? Brushes.Cyan : SetColor(count - 1, stock.DayLines[count - 1].High);
            data.LowColor = count < 2 ? Brushes.Cyan : SetColor(count - 1, stock.DayLines[count - 1].Low);
        }

        private Brush SetColor(int i, float value)
        {
            return value >= stock.DayLines[i - 1].Close ? Brushes.Red : Brushes.Green;
        }

        private void canvas_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up: num = (int)(num * 0.75);
                    if (num < 20) num = 20;
                    break;
                case Key.Down: num = (int)(num / 0.75);
                    break;
                default: return;
            }
            SetUnits();
            e.Handled = true;
        }


        private void canvas_Resize(object sender, SizeChangedEventArgs e)
        {
            SetUnits();
        }


        private void stockList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (canvas.Children.Count < max) return;

            stock = stocks[stockList.SelectedIndex];
            SetUnits();
            ChangeStock();
        }

        private void canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            canvas.Focus();
            Keyboard.Focus(canvas);
        }

        
    }
}
