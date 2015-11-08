using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AnalyzePastData
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //uint startDate = AnalyzeStocks.DateToUint(2007, 10, 1);
            //uint endDate = AnalyzeStocks.DateToUint(2015, 6, 9);
            //AnalyzeStocks analyze = new AnalyzeStocks(startDate, endDate);
            //analyze.StartAnalyze();
            this.Cursor = Cursors.Wait;
            var kLineGraph = new DayLineGraph();
            this.Cursor = null;
            kLineGraph.Show();
        }


        /// <summary>
        /// Write stocks to 东方财富的自选股9
        /// </summary>
        /// <param name="stocks"></param>
        private void WriteStocksToSelfblock(IEnumerable<Stock> stocks)
        {
            StringBuilder newStr = new StringBuilder();
            string file = @"D:\eastmoney\swc8\config\User\m5604094268132944\StockwayStock.ini";
            BufferedStream before = new BufferedStream(new FileStream(file, FileMode.Open, FileAccess.Read));
            StreamReader sr = new StreamReader(before);
            while (true)
            {
                string str = sr.ReadLine();
                if (str.Length >= 5 && str.Substring(0, 5) == "自选股9=") break;
                newStr.Append(str);
                newStr.Append("\r\n");
            }
            newStr.Append("自选股9=");
            foreach (var item in stocks)
            {
                int x = int.Parse(item.Code);
                if (x > 400000) newStr.Append("1.");
                else newStr.Append("0.");
                newStr.Append(item.Code + ",");
            }
            newStr.Append("\r\n");
            sr.Close();
            before.Close();
            BufferedStream after = new BufferedStream(new FileStream(file, FileMode.Create, FileAccess.Write));
            StreamWriter sw = new StreamWriter(after, Encoding.Unicode);
            sw.Write(newStr);
            sw.Flush();
            sw.Close();
            after.Close();
        }

        /// <summary>
        /// parse all the stock files from 通信达 to a single file.
        /// create a file contains stock code and name table.
        /// </summary>
        private void ParseHistoryFilesToOneFile()
        {
            BufferedStream target = new BufferedStream(new FileStream(@"G:\StockData\history.txt", FileMode.Create, FileAccess.Write));
            BinaryWriter bw = new BinaryWriter(target);
            BufferedStream codeNameTable = new BufferedStream(new FileStream(@"G:\StockData\codeNameTable.txt", FileMode.Create, FileAccess.Write));
            StreamWriter tw = new StreamWriter(codeNameTable);

            string path = @"G:\StockData\history";
            var files = Directory.GetFiles(path);
            foreach (var file in files)
            {
                BufferedStream source = new BufferedStream(new FileStream(file, FileMode.Open, FileAccess.Read));
                StreamReader sr = new StreamReader(source, Encoding.GetEncoding("gb2312"));
                string str = sr.ReadLine().Trim();
                char[] space = { ' ' };
                string[] name = str.Split(space, StringSplitOptions.RemoveEmptyEntries);
                tw.Write(name[0]);
                tw.Write(',');
                tw.WriteLine(name[1]);
                bw.Write(uint.Parse(name[0]));
                sr.ReadLine();

                while (true)
                {
                    str = sr.ReadLine();
                    string[] data = str.Split('\t');
                    if (data.Length != 7) break;
                    if (uint.Parse(data[5]) == 0) continue;
                    string[] date = data[0].Split('/');
                    bw.Write(short.Parse(date[0]));
                    bw.Write(byte.Parse(date[1]));
                    bw.Write(byte.Parse(date[2]));
                    bw.Write(float.Parse(data[1]));
                    bw.Write(float.Parse(data[2]));
                    bw.Write(float.Parse(data[3]));
                    bw.Write(float.Parse(data[4]));
                    bw.Write(uint.Parse(data[5]));
                    bw.Write(double.Parse(data[6]));
                }
                bw.Write(0xFFFFFFFF);
                sr.Close();
                source.Close();
            }


            codeNameTable.Flush();
            bw.Flush();
            tw.Close();
            codeNameTable.Close();
            bw.Close();
            target.Close();
        }
    }
}
