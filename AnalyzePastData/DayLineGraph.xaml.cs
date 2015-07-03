using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private int numOfPreControl;
        private int num = 150;
        private int offset = 0;
        private float highest, lowest;
        private uint maxTurnover;
        private double width;
        private Stock stock;
        private int max;
        private bool mouseOn = false;
        private bool keyOn = false;
        private Rectangle singleLine = new Rectangle() { Stroke = Brushes.DarkGray, Width = 1, Height = 1200, SnapsToDevicePixels = true };
        private Rectangle singleLine1 = new Rectangle() { Stroke = Brushes.DarkGray, Width = 1, Height = 1200, SnapsToDevicePixels = true };

        private List<float> average5;
        private List<float> average10;
        private List<float> average20;
        private List<float> average60;
        private List<float> average120;
        private Path path5, path10, path20, path60, path120;

        private ObservableCollection<StockListData> list = new ObservableCollection<StockListData>();
        private bool upPercent = false;
        private bool up = false;
        private bool newPrice = false;
        private bool code = false;

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
            numOfPreControl = canvas.Children.Count;
            AddList();
            stock = stocks[0];
            AddGraph();
            GetAverageList();
            canvas.Focus();
            Keyboard.Focus(canvas);
        }

        private void GetAverageList()
        {
            average5 = GetAverage(5);
            average10 = GetAverage(10);
            average20 = GetAverage(20);
            average60 = GetAverage(60);
            average120 = GetAverage(120);
        }

        private void AddList()
        {
            for (int i = 0; i < stocks.Count; i++)
            {
                int n = stocks[i].DayLines.Count;
                if (n <= 1) continue;
                StockListData item = new StockListData();
                item.ID = i;
                item.Close = stocks[i].DayLines[n - 1].Close;
                item.Code = stocks[i].Code;
                item.Name = stocks[i].Name;
                item.Up = stocks[i].DayLines[n - 1].Close - stocks[i].DayLines[n - 2].Close;
                item.UpPercent = item.Up / stocks[i].DayLines[n - 2].Close;
                if (item.Up > 0) item.BrushClose = Brushes.Red;
                else if (item.Up < 0) item.BrushClose = Brushes.LightGreen;
                else item.BrushClose = Brushes.White;
                list.Add(item);
            }
            stockList.ItemsSource = list;
        }

        private void AddGraph()
        {
            for (int i = 0; i < max; i++)
            {
                canvas.Children.Add(new Rectangle() { SnapsToDevicePixels = true });
                canvas.Children.Add(new Rectangle() { SnapsToDevicePixels = true });
                Panel.SetZIndex(canvas.Children[canvas.Children.Count - 1], 1);
                Panel.SetZIndex(canvas.Children[canvas.Children.Count - 2], 0);
                canvasT.Children.Add(new Rectangle() { SnapsToDevicePixels = true });
            }
        }

        private void SetUnits()
        {
            SetBound();
            for (int i = stock.DayLines.Count - 1; i >= 0; i--)
            {
                Rectangle rect1 = canvas.Children[i * 2 + 1 + numOfPreControl] as Rectangle;
                double height = stock.DayLines[i].Open - stock.DayLines[i].Close;
                Brush brush = height <= 0 ? Brushes.Red : Brushes.Cyan;
                if (i > 0 && Math.Abs(height) < 0.0001) brush = stock.DayLines[i].Close >= stock.DayLines[i - 1].Close ? Brushes.Red : Brushes.Cyan;
                rect1.Stroke = brush;
                if (height > 0) rect1.Fill = brush;
                else rect1.Fill = Brushes.Black;
                rect1.Width = width * 4 / 5;
                double x = (highest - lowest) / canvas.ActualHeight;
                rect1.Height = Math.Abs(height) / x + 1;
                Canvas.SetTop(rect1, (highest - Math.Max(stock.DayLines[i].Open, stock.DayLines[i].Close)) / x);
                Canvas.SetLeft(rect1, width * (i - (stock.DayLines.Count - 1 - num) - 1 + offset));
                Rectangle rect2 = canvas.Children[i * 2 + numOfPreControl] as Rectangle;
                rect2.Height = (stock.DayLines[i].High - stock.DayLines[i].Low) / x;
                rect2.Width = 1;
                rect2.Stroke = brush;
                Canvas.SetTop(rect2, (highest - stock.DayLines[i].High) / x);
                Canvas.SetLeft(rect2, width * (i - (stock.DayLines.Count - 1 - num) - 1 + offset) + width * 2 / 5);
            }
            for (int i = stock.DayLines.Count; i < max; i++)
            {
                Rectangle rect1 = canvas.Children[i * 2 + 1 + numOfPreControl] as Rectangle;
                rect1.Height = 0;
                Rectangle rect2 = canvas.Children[i * 2 + numOfPreControl] as Rectangle;
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
                Canvas.SetLeft(rect, width * (i - (stock.DayLines.Count - 1 - num) - 1 + offset));
            }
        }

        private void SetBound()
        {
            highest = 0;
            lowest = float.MaxValue;
            for (int i = stock.DayLines.Count > num + offset ? stock.DayLines.Count - num - offset : 0; i < stock.DayLines.Count - offset; i++)
            {
                highest = Math.Max(stock.DayLines[i].High, highest);
                lowest = Math.Min(stock.DayLines[i].Low, lowest);
            }
            width = canvas.ActualWidth / num;
            SetScale();
        }

        private void SetScale()
        {
            double x = highest - lowest;
            scale1.Text = highest.ToString("#0.00");
            scale2.Text = (highest - x / 4).ToString("#0.00");
            scale3.Text = (highest - x / 2).ToString("#0.00");
            scale4.Text = (highest - x * 3 / 4).ToString("#0.00");
            scale5.Text = lowest.ToString("#0.00");
        }

        private void SetMaxTurnover()
        {
            maxTurnover = 0;
            for (int i = stock.DayLines.Count > num + offset ? stock.DayLines.Count - num - offset : 0; i < stock.DayLines.Count - offset; i++)
            {
                maxTurnover = Math.Max(maxTurnover, stock.DayLines[i].Turnover);
            }
        }

        private void canvas_Loaded(object sender, RoutedEventArgs e)
        {
            //SetUnits();
            ChangeStock();
            //AddAverage();
        }

        private void AddAverage()
        {
            AddAverageLine(average5, Brushes.DarkGray, 5, ref path5);
            AddAverageLine(average10, Brushes.Yellow, 10, ref path10);
            AddAverageLine(average20, Brushes.Purple, 20, ref path20);
            AddAverageLine(average60, Brushes.Blue, 60, ref path60);
            AddAverageLine(average120, Brushes.Green, 120, ref path120);
        }

        private void AddAverageLine(List<float> average, Brush brush, int n, ref Path path)
        {
            if (average.Count < 2) return;
            double x = (highest - lowest) / canvas.ActualHeight;
            PathFigure LineFigure = new PathFigure() { IsClosed = false };
            LineFigure.StartPoint = new Point(width * (num - stock.DayLines.Count + n - 1 + offset) + width * 2 / 5, (highest - average[0]) / x);
            PathSegmentCollection pathCollection = new PathSegmentCollection();
            for (int i = 1; i < average.Count; i++)
            {
                pathCollection.Add(new LineSegment(new Point(width * (i - (stock.DayLines.Count - 1 - num) - 1 + n - 1 + offset) + width * 2 / 5,
                    (highest - average[i]) / x), true));
            }
            LineFigure.Segments = pathCollection;
            PathGeometry lineGeometry = new PathGeometry();
            lineGeometry.Figures.Add(LineFigure);
            path = new Path() { Stroke = brush, StrokeThickness = 1, Data = lineGeometry };
            canvas.Children.Add(path);
        }

        private void ChangeStock()
        {
            DataToShow data = this.FindResource("rightPanel") as DataToShow;
            data.Code = stock.Code;
            data.Name = stock.Name;
            int count = stock.DayLines.Count;
            data.Open = (count == 0 ? 0 : stock.DayLines[count - 1].Open).ToString("#0.00");
            data.Close = (count == 0 ? 0 : stock.DayLines[count - 1].Close).ToString("#0.00");
            data.High = (count == 0 ? 0 : stock.DayLines[count - 1].High).ToString("#0.00");
            data.Low = (count == 0 ? 0 : stock.DayLines[count - 1].Low).ToString("#0.00");
            data.Turnover = count == 0 ? 0 : stock.DayLines[count - 1].Turnover;
            data.Volume = count == 0 ? 0 : stock.DayLines[count - 1].Volume;
            data.OpenColor = count < 2 ? Brushes.Cyan : SetColor(count - 1, stock.DayLines[count - 1].Open);
            data.CloseColor = count < 2 ? Brushes.Cyan : SetColor(count - 1, stock.DayLines[count - 1].Close);
            data.HighColor = count < 2 ? Brushes.Cyan : SetColor(count - 1, stock.DayLines[count - 1].High);
            data.LowColor = count < 2 ? Brushes.Cyan : SetColor(count - 1, stock.DayLines[count - 1].Low);
            data.Up = (count <= 1 ? 0 : stock.DayLines[count - 1].Close - stock.DayLines[count - 2].Close).ToString("#0.00");
            data.UpPercent = count <= 1 ? 0 : stock.DayLines[count - 1].Close / stock.DayLines[count - 2].Close - 1;
        }

        private Brush SetColor(int i, float value)
        {
            return value >= stock.DayLines[i - 1].Close ? Brushes.Red : Brushes.LightGreen;
        }

        private void canvas_KeyDown(object sender, KeyEventArgs e)
        {
            bool needToRedraw = false;
            switch (e.Key)
            {
                case Key.Up:
                    num = (int)(num * 0.75);
                    if (num < 20) num = 20;
                    needToRedraw = true;
                    break;
                case Key.Down:
                    num = (int)(num / 0.75);
                    needToRedraw = true;
                    break;
                case Key.Left:
                    if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                    {
                        offset++;
                        needToRedraw = true;
                        break;
                    }
                    if (!mouseOn) break;
                    keyOn = true;
                    double x = Canvas.GetLeft(singleLine);
                    if (x < width) break;
                    x = ((int)(x / width) - 1) * width + width * 2 / 5;
                    SetSingleLine(x);
                    int i = stock.DayLines.Count - (num - (int)(x / width));
                    SetAverageText(stock.DayLines.Count - 1 - i);
                    SetStatusPanel(i);
                    break;
                case Key.Right:
                    if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                    {
                        if (offset == 0) break;
                        offset--;
                        needToRedraw = true;
                        break;
                    }
                    if (!mouseOn) break;
                    keyOn = true;
                    double xx = Canvas.GetLeft(singleLine);
                    if (xx >= width * (num - 1)) break;
                    xx = ((int)(xx / width) + 1) * width + width * 2 / 5;
                    SetSingleLine(xx);
                    int ii = stock.DayLines.Count - (num - (int)(xx / width));
                    SetAverageText(stock.DayLines.Count - 1 - ii);
                    SetStatusPanel(ii);
                    break;
                case Key.Escape:
                    if (mouseOn) { canvas.Children.Remove(singleLine); canvasT.Children.Remove(singleLine1); }
                    mouseOn = false;
                    Status.Visibility = Visibility.Hidden;
                    break;
                default: return;
            }
            if (needToRedraw)
            {
                RemoveAverage();
                SetUnits();
                AddAverage();
            }

            e.Handled = true;
        }

        private void RemoveAverage()
        {
            canvas.Children.Remove(path5);
            canvas.Children.Remove(path10);
            canvas.Children.Remove(path20);
            canvas.Children.Remove(path60);
            canvas.Children.Remove(path120);
        }


        private void canvas_Resize(object sender, SizeChangedEventArgs e)
        {
            RemoveAverage();
            SetUnits();
            AddAverage();
            SetAverageText(0);
        }

        private void SetAverageText(int i)
        {
            i += offset;
            text5.Text = average5[average5.Count - 1 - i].ToString("#0.00");
            text10.Text = average5[average10.Count - 1 - i].ToString("#0.00");
            text20.Text = average5[average20.Count - 1 - i].ToString("#0.00");
            text60.Text = average5[average60.Count - 1 - i].ToString("#0.00");
            text120.Text = average5[average120.Count - 1 - i].ToString("#0.00");
        }


        private void stockList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (canvas.Children.Count < max) return;

            RemoveAverage();
            stock = stocks[(stockList.SelectedItem as StockListData).ID];
            SetUnits();
            ChangeStock();
            GetAverageList();
            AddAverage();
        }

        private void canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            canvas.Focus();
            Keyboard.Focus(canvas);
            int i = stock.DayLines.Count - (num - (int)(e.GetPosition(canvas).X / width));
            SetAverageText(stock.DayLines.Count - 1 - i);
            SetStatusPanel(i);
            if (!mouseOn) { canvas.Children.Add(singleLine); canvasT.Children.Add(singleLine1); }
            mouseOn = true;
            keyOn = false;
            double x = e.GetPosition(canvas).X;
            SetSingleLine(x);
        }

        private void SetSingleLine(double x)
        {
            Canvas.SetLeft(singleLine, x);
            Canvas.SetLeft(singleLine1, x);
        }

        private void SetStatusPanel(int i)
        {
            i -= offset;
            DataToShow data = this.FindResource("mousePanel") as DataToShow;
            uint date = stock.DayLines[i].Date;
            uint day = date >> 24;
            uint month = (date & 0x00FF0000) >> 16;
            uint year = date & 0x0000FFFF;
            data.Date = year + "/" + month + "/" + day;
            data.Open = (stock.DayLines[i].Open).ToString("#0.00");
            data.Close = (stock.DayLines[i].Close).ToString("#0.00");
            data.High = (stock.DayLines[i].High).ToString("#0.00");
            data.Low = (stock.DayLines[i].Low).ToString("#0.00");
            data.Turnover = stock.DayLines[i].Turnover;
            data.Volume = stock.DayLines[i].Volume;
            data.Up = (stock.DayLines[i].Close - stock.DayLines[i - 1].Close).ToString("#0.00");
            data.UpPercent = stock.DayLines[i].Close / stock.DayLines[i - 1].Close - 1;
            data.OpenColor = SetColor(i, stock.DayLines[i].Open);
            data.CloseColor = SetColor(i, stock.DayLines[i].Close);
            data.HighColor = SetColor(i, stock.DayLines[i].High);
            data.LowColor = SetColor(i, stock.DayLines[i].Low);
            Status.Visibility = Visibility.Visible;
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (!mouseOn) return;
            if (keyOn) return;
            keyOn = false;
            int i = stock.DayLines.Count - (num - (int)(e.GetPosition(canvas).X / width));
            SetAverageText(stock.DayLines.Count - 1 - i);
            SetStatusPanel(i);
            double x = e.GetPosition(canvas).X;
            SetSingleLine(x);
        }

        private void stockList_GotFocus(object sender, RoutedEventArgs e)
        {
            if (mouseOn) { canvas.Children.Remove(singleLine); canvasT.Children.Remove(singleLine1); }
            mouseOn = false;
            Status.Visibility = Visibility.Hidden;
        }

        private List<float> GetAverage(int n)
        {
            var list = new List<float>();
            int j = 0;
            float sum = 0;
            for (int i = 0; i < stock.DayLines.Count; i++)
            {
                if (i < n - 1) { sum += stock.DayLines[i].Close; continue; }
                sum += stock.DayLines[i].Close;
                list.Add(sum / n);
                sum -= stock.DayLines[j++].Close;
            }
            return list;
        }

        private void tbUpPercent_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SortStockList(ref upPercent, item => item.UpPercent);
        }

        private void tbUp_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SortStockList(ref up, item => item.Up);
        }

        private void tbNew_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SortStockList(ref newPrice, item => item.Close);
        }

        private void tbCode_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SortStockList(ref code, item => float.Parse(item.Code));
        }

        private void SortStockList( ref bool upDown, Func<StockListData, float> keySelector)
        {
            IEnumerable<StockListData> newList;
            if (upDown) newList = list.OrderBy(keySelector);
            else newList = list.OrderByDescending(keySelector);
            upDown = !upDown;
            list = new ObservableCollection<StockListData>(newList);
            stockList.ItemsSource = list;
        }
    }
}
