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
            //Cursor = Cursors.Wait;
            //uint startDate = Utilities.DateToUint(2007, 10, 1);
            //uint endDate = Utilities.DateToUint(2015, 11, 20);
            //AnalyzeStocks analyze = new AnalyzeStocks(startDate, endDate);
            //analyze.StartAnalyze();
            //Cursor = null;


            //Cursor = Cursors.Wait;
            //uint startDate = Utilities.DateToUint(2015, 10, 1);
            //uint endDate = Utilities.DateToUint(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            //var stocks = Utilities.getStocks(startDate, endDate);
            //var test = new StocksFilter(stocks);
            //test.GetUpDown(3, 2, 0.15f, 0.05f);
            //test.GetUpDown(3, 3, 0.20f, 0.10f);
            //test.OutputResult();
            //Cursor = null;
            //MessageBox.Show("Done");


            var selectWin = new StockFilterSelectWindow();
            selectWin.Show();
        }

        private void btnRestoreData_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            Utilities.ParseHistoryFilesToOneFile();
            Cursor = null;
            MessageBox.Show("Done");

        }

        private void btnKLine_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            var kLineGraph = new DayLineGraph();
            kLineGraph.Show();
            Cursor = null;

        }

        private void btnDZHBlocks_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            uint startDate = Utilities.DateToUint(2014, 10, 1);
            uint endDate = Utilities.DateToUint(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            var stocks = Utilities.getStocks(startDate, endDate);
            Utilities.OrganizeSelfSelectedStocksDZH(stocks);
            Cursor = null;
            MessageBox.Show("Done");
        }

        private void btnUpdateOneDrive_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            Utilities.UpdateOneDriveDZHFiles();
            Cursor = null;
            MessageBox.Show("Done");
        }
    }
}
