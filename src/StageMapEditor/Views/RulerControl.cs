using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using StageMapEditor.ViewModels;

namespace StageMapEditor.Views
{
    /// <summary>
    /// このカスタム コントロールを XAML ファイルで使用するには、手順 1a または 1b の後、手順 2 に従います。
    ///
    /// 手順 1a) 現在のプロジェクトに存在する XAML ファイルでこのカスタム コントロールを使用する場合
    /// この XmlNamespace 属性を使用場所であるマークアップ ファイルのルート要素に
    /// 追加します:
    ///
    ///     xmlns:MyNamespace="clr-namespace:StageMapEditor.Views"
    ///
    ///
    /// 手順 1b) 異なるプロジェクトに存在する XAML ファイルでこのカスタム コントロールを使用する場合
    /// この XmlNamespace 属性を使用場所であるマークアップ ファイルのルート要素に
    /// 追加します:
    ///
    ///     xmlns:MyNamespace="clr-namespace:StageMapEditor.Views;assembly=StageMapEditor.Views"
    ///
    /// また、XAML ファイルのあるプロジェクトからこのプロジェクトへのプロジェクト参照を追加し、
    /// リビルドして、コンパイル エラーを防ぐ必要があります:
    ///
    ///     ソリューション エクスプローラーで対象のプロジェクトを右クリックし、
    ///     [参照の追加] の [プロジェクト] を選択してから、このプロジェクトを参照し、選択します。
    ///
    ///
    /// 手順 2)
    /// コントロールを XAML ファイルで使用します。
    ///
    ///     <MyNamespace:Ruler/>
    ///
    /// </summary>

    public enum MeasurePosition
    {
        Top, Right, Bottom, Left
    }

    [TemplatePart(Name = StackPanelName, Type = typeof(StackPanel))]
    [TemplatePart(Name = ScrollName, Type = typeof(ScrollViewer))]
    [TemplatePart(Name = CurrentPositionName, Type = typeof(ScrollViewer))]
    public class RulerControl : Control
    {
        #region TemplatePart
        private const string StackPanelName = "Wrapper";
        private StackPanel Wrapper { get { return GetTemplateChild(StackPanelName) as StackPanel; } }

        private const string ScrollName = "Scroll";
        private ScrollViewer Scroll { get { return GetTemplateChild(ScrollName) as ScrollViewer; } }

        private const string CurrentPositionName = "CurrentPosition";
        private Path CurrentPosition { get { return GetTemplateChild(CurrentPositionName) as Path; } }
        #endregion

        public static readonly DependencyProperty MeasurePositionProperty =
          DependencyProperty.Register("MeasurePosition", typeof(MeasurePosition), typeof(RulerControl), new PropertyMetadata(default(MeasurePosition)));

        public MeasurePosition MeasurePosition
        {
            get { return (MeasurePosition)GetValue(MeasurePositionProperty); }
            set { SetValue(MeasurePositionProperty, value); }
        }

        #region Orientation
        public static readonly DependencyProperty OrientationProperty =
          DependencyProperty.Register("Orientation", typeof(Orientation), typeof(RulerControl), new PropertyMetadata(Orientation.Vertical));

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }
        #endregion

        #region Distance

        public static readonly DependencyProperty DistanceProperty =
          DependencyProperty.Register("Distance", typeof(double), typeof(RulerControl), new PropertyMetadata(default(double), (o, args) => ((RulerControl)o).BuildNew()));
        public double Distance
        {
            get { return (double)GetValue(DistanceProperty); }
            set { SetValue(DistanceProperty, value); }
        }
        #endregion

        #region Scale

        public static readonly DependencyProperty ScaleProperty =
          DependencyProperty.Register("Scale", typeof(float), typeof(RulerControl), new PropertyMetadata(default(float), (o, args) => ((RulerControl)o).BuildNew()));
        public float Scale
        {
            get { return (float)GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }
        #endregion

        #region CellCount
        public static readonly DependencyProperty CellCountProperty =
          DependencyProperty.Register("CellCount", typeof(int), typeof(RulerControl), new PropertyMetadata(default(int), (o, args) => ((RulerControl)o).BuildNew()));

        public int CellCount
        {
            get { return (int)GetValue(CellCountProperty); }
            set { SetValue(CellCountProperty, value); }
        }
        #endregion

        #region ScrollHorizontalOffset
        public static readonly DependencyProperty ScrollHorizontalOffsetProperty =
          DependencyProperty.Register("ScrollHorizontalOffset", typeof(double), typeof(RulerControl), new PropertyMetadata(default(double), (o, args) => ((RulerControl)o).ScrollPosition()));

        public double ScrollHorizontalOffset
        {
            get { return (double)GetValue(ScrollHorizontalOffsetProperty); }
            set { SetValue(ScrollHorizontalOffsetProperty, value); }
        }
        #endregion

        #region ScrollVerticalOffset
        public static readonly DependencyProperty ScrollVerticalOffsetProperty =
          DependencyProperty.Register("ScrollVerticalOffset", typeof(double), typeof(RulerControl), new PropertyMetadata(default(double), (o, args) => ((RulerControl)o).ScrollPosition()));

        public double ScrollVerticalOffset
        {
            get { return (double)GetValue(ScrollVerticalOffsetProperty); }
            set { SetValue(ScrollVerticalOffsetProperty, value); }
        }
        #endregion

        #region
        public static readonly DependencyProperty CurrentMousePositionProperty =
          DependencyProperty.Register("CurrentMousePosition", typeof(Point), typeof(RulerControl), new PropertyMetadata(default(Point), (o, args) => ((RulerControl)o).CurrentMousePositionUpdate()));

        public Point CurrentMousePosition
        {
            get { return (Point)GetValue(CurrentMousePositionProperty); }
            set { SetValue(CurrentMousePositionProperty, value); }
        }

        private void CurrentMousePositionUpdate()
        {
            if (CurrentPosition == null)
                return;

            if (CurrentMousePosition.Y > 0 && CurrentMousePosition.Y > 0)
            {
                if (Orientation == Orientation.Horizontal)
                {
                    CurrentPosition.Data = new LineGeometry(new Point(CurrentMousePosition.X, 0), new Point(CurrentMousePosition.X, Scroll.ActualHeight));
                }
                else
                {
                    CurrentPosition.Data = new LineGeometry(new Point(0, CurrentMousePosition.Y), new Point(Scroll.ActualWidth, CurrentMousePosition.Y));
                }
            }
        }
        #endregion

        static RulerControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RulerControl), new FrameworkPropertyMetadata(typeof(RulerControl)));
        }

        public void ScrollPosition()
        {
            if (Scroll != null)
            {
                Scroll.ScrollToHorizontalOffset(ScrollHorizontalOffset);
                Scroll.ScrollToVerticalOffset(ScrollVerticalOffset);
            }
        }

        public void BuildNew()
        {
            if (Wrapper == null || FontSize <= 0 || Scale <= 0)
                return;

            Wrapper.Children.Clear();

            //var distance = Distance * Scale;
            var distance = Distance;
            var fontSize = FontSize * (1 - Math.Abs(Math.Pow(Scale - 1, 2)));

            for (int i = 0; i < CellCount; i++)
            {
                var canvas = new Canvas();
                var measure = new Border();
                var wrap = new Border();
                measure.BorderBrush = new SolidColorBrush(Colors.Black * 0.4f);

                //横方向の場合
                if (Orientation == Orientation.Horizontal)
                {
                    wrap.Width = distance;
                    canvas.Width = distance;

                    if (i != 0)
                    {
                        measure.BorderThickness = new Thickness(1, 0, 0, 0);
                    }
                    measure.Height = Wrapper.ActualHeight / 2;
                    measure.Margin = MeasurePosition == MeasurePosition.Top ? new Thickness(0, measure.Height, 0, 0) : new Thickness(0, 0, 0, 0);
                    measure.Width = distance;

                }
                else if (Orientation == Orientation.Vertical)
                {
                    wrap.Height = distance;
                    canvas.Height = distance;

                    if (i != 0)
                    {
                        measure.BorderThickness = new Thickness(0, 1, 0, 0);
                    }
                    measure.Width = Wrapper.ActualWidth / 2;
                    measure.Height = distance;
                    measure.Margin = MeasurePosition == MeasurePosition.Left ? new Thickness(measure.Width, 0, 0, 0) : new Thickness(0, 0, 0, 0);
                }

                //wrap.Background = new SolidColorBrush(Colors.Blue * 0.5f);
                //measure.Background = new SolidColorBrush(Colors.Red * 0.5f);

                var tb = new TextBlock();
                tb.Text = i.ToString();
                tb.HorizontalAlignment = HorizontalAlignment.Center;
                tb.VerticalAlignment = VerticalAlignment.Center;
                tb.FontWeight = FontWeight.FromOpenTypeWeight(700);
                tb.FontSize = fontSize;
                tb.Padding = new Thickness(2);

                wrap.Child = tb;

                canvas.Children.Add(measure);
                canvas.Children.Add(wrap);

                Wrapper.Children.Add(canvas);
            }
        }
    }
}
