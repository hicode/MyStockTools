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


            //Utilities.AddTodayToHistoryFile();

        }

        private void btnRestoreData_Click(object sender, RoutedEventArgs e)
        {
            Utilities.ParseHistoryFilesToOneFile();
            MessageBox.Show("done");

        }
    }
}
