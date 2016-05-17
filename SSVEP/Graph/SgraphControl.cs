using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace WpfApplication1
{
    [TemplatePart(Name = "PART_TrendsViewPort", Type = typeof(Canvas))]
    public class SgraphControl : Control
    {
        static SgraphControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SgraphControl), new FrameworkPropertyMetadata(typeof(SgraphControl)));
        }

        public SgraphControl()
            : base()
        {
            SetValue(TrendsPropertyKey, new FreezableCollection<LineTrend>());
        }

        public void clear()
        {
            SetValue(TrendsPropertyKey, new FreezableCollection<LineTrend>());
        }

        private static readonly DependencyPropertyKey TrendsPropertyKey = DependencyProperty.RegisterReadOnly(
            "Trends",
            typeof(FreezableCollection<LineTrend>),
            typeof(SgraphControl),
            new PropertyMetadata(new FreezableCollection<LineTrend>())
            );

        public static readonly DependencyProperty TrendsProperty = TrendsPropertyKey.DependencyProperty;

        public FreezableCollection<LineTrend> Trends
        {
            get { return (FreezableCollection<LineTrend>)GetValue(TrendsProperty); }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var trendsViewPort = GetTemplateChild("PART_TrendsViewPort") as Canvas;
            if (trendsViewPort != null && Trends != null)
            {
                foreach (var trend in Trends)
                {
                    DrawTrend(trendsViewPort, trend);
                }
            }
        }

        private void DrawTrend(Canvas drawingCanvas, LineTrend Trend)
        {
            var trend = Trend as LineTrend;
            if (trend != null && trend.Points != null)
            {
                for (int i = 1; i < trend.Points.Count; i++)
                {
                    var toDraw = new Line
                    {
                        X1 = trend.Points[i - 1].X,
                        Y1 = trend.Points[i - 1].Y,
                        X2 = trend.Points[i].X,
                        Y2 = trend.Points[i].Y,
                        StrokeThickness = 2,
                        Stroke = trend.TrendColor
                    };
                    drawingCanvas.Children.Add(toDraw);
                }
            }
        }
    }
}