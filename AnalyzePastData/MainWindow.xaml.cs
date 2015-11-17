﻿using Microsoft.Office.Interop.Excel;
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


            //Cursor = Cursors.Wait;
            //var kLineGraph = new DayLineGraph();
            //kLineGraph.Show();
            //Cursor = null;


            Cursor = Cursors.Wait;
            uint startDate = Utilities.DateToUint(2015, 10, 1);
            uint endDate = Utilities.DateToUint(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            var stocks = Utilities.getStocks(startDate, endDate);
            var test = new StocksFilter(stocks);
            test.GetUpDown(3, 1, 0.15f, 0.02f);
            test.OutputResult();
            Cursor = null;
            MessageBox.Show("Done");



        }

        private void btnRestoreData_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            Utilities.ParseHistoryFilesToOneFile();
            Cursor = null;
            MessageBox.Show("done");

        }
    }
}
