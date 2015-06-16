using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace StageMapEditor.Views
{
    [TemplatePart(Name = CanvasName, Type = typeof(Canvas))]
    public class GridCanvas : Control
    {
        #region TemplatePart
        private const string CanvasName = "Canvas";
        private Canvas Canvas { get { return GetTemplateChild(CanvasName) as Canvas; } }
        #endregion

        #region グリッドのカラー
        public static readonly DependencyProperty GridBrushProperty =
            DependencyProperty.Register("GridBrush", typeof(Brush), typeof(GridCanvas), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        public Brush GridBrush
        {
            get { return (Brush)GetValue(GridBrushProperty); }
            set { SetValue(GridBrushProperty, value); }
        }
        #endregion

        #region 線の太さ
        public static readonly DependencyProperty GridThicknessProperty =
            DependencyProperty.Register("GridThickness", typeof(int), typeof(GridCanvas),
            new PropertyMetadata(1, (d, e) => ((GridCanvas)d).BuildNew()));

        public int GridThickness
        {
            get { return (int)GetValue(GridThicknessProperty); }
            set { SetValue(GridThicknessProperty, value); }
        }
        #endregion

        #region グリッドのサイズ
        public static readonly DependencyProperty GridSizeProperty =
            DependencyProperty.Register("GridSize", typeof(int), typeof(GridCanvas), new PropertyMetadata(40));

        public int GridSize
        {
            get { return (int)GetValue(GridSizeProperty); }
            set { SetValue(GridSizeProperty, value); }
        }
        #endregion

        #region グリッド線の拡大率
        public static readonly DependencyProperty ScaleValueProperty =
            DependencyProperty.Register("ScaleValue", typeof(double), typeof(GridCanvas),
            new UIPropertyMetadata(1.0d, (d, e) => ((GridCanvas)d).BuildNew()));
        public double ScaleValue
        {
            get { return (double)GetValue(ScaleValueProperty); }
            set { SetValue(ScaleValueProperty, value); }
        }
        #endregion

        #region GridOpacity

        public static readonly DependencyProperty GridOpacityProperty =
            DependencyProperty.Register("GridOpacity", typeof(double), typeof(GridCanvas), new PropertyMetadata(1.0));

        public double GridOpacity
        {
            get { return (double)GetValue(GridOpacityProperty); }
            set { SetValue(GridOpacityProperty, value); }
        }
        #endregion

        static GridCanvas()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GridCanvas), new FrameworkPropertyMetadata(typeof(GridCanvas)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Canvas.Loaded += (s, e) => BuildNew();
            Canvas.SizeChanged += (s, e) => BuildNew();

        }

        public void BuildNew()
        {
            Canvas.Children.Clear();

            var w = (int)Canvas.ActualWidth / (int)(GridSize * ScaleValue);
            var h = (int)Canvas.ActualHeight / (int)(GridSize * ScaleValue);

            for (int i = 1; i < w + 1; i++)
            {
                var g = i * GridSize;
                var p = new Path
                {
                    Data = new LineGeometry(new Point(g, 0), new Point(g, Canvas.ActualHeight)),
                    Stroke = GridBrush,
                    StrokeThickness = GridThickness,
                    Opacity = GridOpacity,
                };
                Canvas.Children.Add(p);
            }

            for (int i = 1; i < h + 1; i++)
            {
                var g = i * GridSize;
                var p = new Path
                {
                    Data = new LineGeometry(new Point(0, g), new Point(Canvas.ActualWidth, g)),
                    Stroke = GridBrush,
                    StrokeThickness = GridThickness,
                    Opacity = GridOpacity,
                };
                Canvas.Children.Add(p);
            }
        }
    }
}
