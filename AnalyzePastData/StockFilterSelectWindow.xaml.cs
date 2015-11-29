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
    /// Interaction logic for StockFilterSelect.xaml
    /// </summary>
    public partial class StockFilterSelectWindow : Window
    {
        public StockFilterSelectWindow()
        {
            InitializeComponent();
        }

        private void comboBoxUpDays_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBox combo = sender as ComboBox;
            int[] source = { 2, 3, 4 };
            combo.ItemsSource = source;
            combo.SelectedIndex = 0;
        }

        private void comboBoxUpPercent_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBox combo = sender as ComboBox;
            string[] source = { "10%", "15%", "20%" };
            combo.ItemsSource = source;
            combo.SelectedIndex = 1;
        }

        private void comboBoxLimitUp_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBox combo = sender as ComboBox;
            string[] source = { "是", "否" };
            combo.ItemsSource = source;
            combo.SelectedIndex = 1;
        }

        private void comboBoxDownDays_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBox combo = sender as ComboBox;
            int[] source = { 0, 1, 2, 3 };
            combo.ItemsSource = source;
            combo.SelectedIndex = 0;
        }

        private void comboBoxDownPercent_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBox combo = sender as ComboBox;
            string[] source = { "5%", "10%" };
            combo.ItemsSource = source;
            combo.SelectedIndex = 1;
        }

        private void comboBoxTurnover_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBox combo = sender as ComboBox;
            string[] source = { "无视", "放量", "持平", "缩量" };
            combo.ItemsSource = source;
            combo.SelectedIndex = 1;
        }
    }
}
