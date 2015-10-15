using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows;

namespace WpfApplication1
{
    public class LineTrend : DependencyObject
    {
        public static readonly DependencyProperty NameProperty =
            DependencyProperty.Register("Name", typeof(string), typeof(LineTrend), new PropertyMetadata(default(string)));

        public string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public static readonly DependencyProperty TrendColorProperty =
            DependencyProperty.Register("TrendColor", typeof(Brush), typeof(LineTrend), new PropertyMetadata(default(Brush)));

        public Brush TrendColor
        {
            get { return (Brush)GetValue(TrendColorProperty); }
            set { SetValue(TrendColorProperty, value); }
        }

        public static readonly DependencyProperty PointThicknessProperty =
            DependencyProperty.Register("PointThickness", typeof(Thickness), typeof(LineTrend), new PropertyMetadata(default(Thickness)));

        public Thickness PointThickness
        {
            get { return (Thickness)GetValue(PointThicknessProperty); }
            set { SetValue(PointThicknessProperty, value); }
        }

        public Thickness LineThickness { get; set; }

        public static readonly DependencyProperty PointsProperty =
           DependencyProperty.Register("Points", typeof(ObservableCollection<TrendPoint>), typeof(LineTrend), new UIPropertyMetadata(default(ObservableCollection<TrendPoint>)));

        public ObservableCollection<TrendPoint> Points
        {
            get { return (ObservableCollection<TrendPoint>)GetValue(PointsProperty); }
            set { SetValue(PointsProperty, value); }
        }
    }

    public class TrendPoint : DependencyObject
    {
        public float X { get; set; }
        public float Y { get; set; }
        public Dictionary<String, String> AdditionalData { get; set; }

        public TrendPoint()
        {
            X = 0;
            Y = 0;
            AdditionalData = new Dictionary<string, string>();
        }
    }
}