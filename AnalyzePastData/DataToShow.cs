using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace AnalyzePastData
{
    class DataToShow : DependencyObject
    {


        public string Code
        {
            get { return (string)GetValue(CodeProperty); }
            set { SetValue(CodeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Code.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CodeProperty =
            DependencyProperty.Register("Code", typeof(string), typeof(DataToShow), new PropertyMetadata("000001"));




        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Name.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NameProperty =
            DependencyProperty.Register("Name", typeof(string), typeof(DataToShow), new PropertyMetadata("平安银行"));

        

        public float Open
        {
            get { return (float)GetValue(OpenProperty); }
            set { SetValue(OpenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Open.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OpenProperty =
            DependencyProperty.Register("Open", typeof(float), typeof(DataToShow), new PropertyMetadata(default(float)));




        public float Close
        {
            get { return (float)GetValue(CloseProperty); }
            set { SetValue(CloseProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Close.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CloseProperty =
            DependencyProperty.Register("Close", typeof(float), typeof(DataToShow), new PropertyMetadata(default(float)));




        public float High
        {
            get { return (float)GetValue(HighProperty); }
            set { SetValue(HighProperty, value); }
        }

        // Using a DependencyProperty as the backing store for High.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HighProperty =
            DependencyProperty.Register("High", typeof(float), typeof(DataToShow), new PropertyMetadata(default(float)));



        public float Low
        {
            get { return (float)GetValue(LowProperty); }
            set { SetValue(LowProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Low.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LowProperty =
            DependencyProperty.Register("Low", typeof(float), typeof(DataToShow), new PropertyMetadata(default(float)));



        public uint Turnover
        {
            get { return (uint)GetValue(TurnoverProperty); }
            set { SetValue(TurnoverProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Turnover.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TurnoverProperty =
            DependencyProperty.Register("Turnover", typeof(uint), typeof(DataToShow), new PropertyMetadata(default(uint)));



        public double Volume
        {
            get { return (double)GetValue(VolumeProperty); }
            set { SetValue(VolumeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Volume.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VolumeProperty =
            DependencyProperty.Register("Volume", typeof(double), typeof(DataToShow), new PropertyMetadata(default(double)));



        public Brush OpenColor
        {
            get { return (Brush)GetValue(OpenColorProperty); }
            set { SetValue(OpenColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OpenColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OpenColorProperty =
            DependencyProperty.Register("OpenColor", typeof(Brush), typeof(DataToShow));

        public Brush CloseColor
        {
            get { return (Brush)GetValue(CloseColorProperty); }
            set { SetValue(CloseColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OpenColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CloseColorProperty =
            DependencyProperty.Register("CloseColor", typeof(Brush), typeof(DataToShow));

        public Brush HighColor
        {
            get { return (Brush)GetValue(HighColorProperty); }
            set { SetValue(HighColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OpenColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HighColorProperty =
            DependencyProperty.Register("HighColor", typeof(Brush), typeof(DataToShow));

        public Brush LowColor
        {
            get { return (Brush)GetValue(LowColorProperty); }
            set { SetValue(LowColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OpenColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LowColorProperty =
            DependencyProperty.Register("LowColor", typeof(Brush), typeof(DataToShow));
    }
}
