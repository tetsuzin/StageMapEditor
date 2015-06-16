using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Livet.Commands;
using SharpDXControl;
using StageMapEditor.Helper;
using StageMapEditor.Models;
using StageMapEditor.Models.MousePosition;
using StageMapEditor.RenderEngine;
using StageMapEditor.ViewModels;
using Point = System.Windows.Point;

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
    ///     <MyNamespace:MapControl/>
    ///
    /// </summary>
    [TemplatePart(Name = GridName, Type = typeof(Grid))]
    [TemplatePart(Name = MapScrollViewerName, Type = typeof(ScrollViewer))]
    [TemplatePart(Name = WrapperName, Type = typeof(Canvas))]
    [TemplatePart(Name = MapName, Type = typeof(DPFCanvas))]
    [TemplatePart(Name = RulerTopName, Type = typeof(RulerControl))]
    [TemplatePart(Name = RulerLeftName, Type = typeof(RulerControl))]
    public partial class MapControl : Control
    {
        private const string GridName = "Grid";
        private const string MapName = "Map";
        private const string MapScrollViewerName = "MapScrollViewer";
        private const string WrapperName = "Wrapper";
        private const string RulerTopName = "RulerTop";
        private const string RulerLeftName = "RulerLeft";

        public Grid Grid { get { return GetTemplateChild(GridName) as Grid; } }
        public ScrollViewer MapScrollViewer { get { return GetTemplateChild(MapScrollViewerName) as ScrollViewer; } }
        public Canvas Wrapper { get { return GetTemplateChild(WrapperName) as Canvas; } }
        public DPFCanvas Map { get { return GetTemplateChild(MapName) as DPFCanvas; } }
        public RulerControl RulerTop { get { return GetTemplateChild(RulerTopName) as RulerControl; } }
        public RulerControl RulerLeft { get { return GetTemplateChild(RulerLeftName) as RulerControl; } }

        public double ContentVerticalOffset { get { return MapScrollViewer.ContentVerticalOffset; } }
        public double ContentHorizontalOffset { get { return MapScrollViewer.ContentHorizontalOffset; } }

        public IMapControlViewModel ViewModel
        {
            get { return DataContext as IMapControlViewModel; }
        }

        static MapControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MapControl), new FrameworkPropertyMetadata(typeof(MapControl)));
        }

        public string _id;

        public MapControl()
        {
            _id = Guid.NewGuid().ToString();

            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }


        //private GameControlSource _gameControlSource;

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            Map.LayerList.Add(new MapLayer(this));

            Map.SetFrameRate(ViewModel.Settings.RenderFrameSecond);

            //マウス操作のイベントを設定
            SetMouseEvent();

            Canvas.SetLeft(Map, MapScrollViewer.ContentHorizontalOffset);
            Canvas.SetTop(Map, MapScrollViewer.ContentVerticalOffset);

            ScaleRebuild();
        }

        private void OnUnloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            //Map.Game.Components.Clear();
            //Map.Game.Dispose();
        }

        public int GetGamePanelPositionLeft()
        {
            return Map == null ? 0 : (int)Canvas.GetLeft(Map);
        }

        public int GetGamePanelPositionTop()
        {
            return Map == null ? 0 : (int)Canvas.GetTop(Map);
        }

        private void ViewModelOnOnRendering()
        {
            if (!ViewModel.CurrentMapIsNull)
            {
                ScrollTo(ViewModel.MapScrollPosition);
            }

            ScaleRebuild();
        }

        private void ScaleRebuild()
        {
            RulerTop.BuildNew();
            RulerLeft.BuildNew();
        }

        /// <summary>
        /// スクロールバーの位置をViewModelの現在選択中のModelに記録する
        /// </summary>
        public void SaveScrollPosition()
        {
            ViewModel.MapScrollPosition =
                new Point(MapScrollViewer.ContentHorizontalOffset,
                          MapScrollViewer.ContentVerticalOffset);
        }

        /// <summary>
        /// 現在地の取得
        /// </summary>
        /// <returns></returns>
        public MousePosition PresentMousePosition
        {
            get { return new MousePosition(this); }
        }


        private enum DrawDirection { Vertical, Horizontal, None }
        private Tuple<DrawDirection, MousePosition> GetDirectionAndParallelPosition(DrawDirection direction, MousePosition start, MousePosition current)
        {
            Func<DrawDirection> getDirection = () =>
            {
                var x = Math.Abs(start.GridPoint.X - current.GridPoint.X);
                var y = Math.Abs(start.GridPoint.Y - current.GridPoint.Y);

                return x == y ? DrawDirection.None :
                       x > y ? DrawDirection.Horizontal : DrawDirection.Vertical;
            };

            switch (direction)
            {
                case DrawDirection.Horizontal: //横方向への平行
                    return Tuple.Create(direction, new MousePosition(current.Position.X, start.Position.Y));
                case DrawDirection.Vertical: //縦方向への平行
                    return Tuple.Create(direction, new MousePosition(start.Position.X, current.Position.Y));
                case DrawDirection.None:
                    return Tuple.Create(getDirection(), current);
                default:
                    throw new ArgumentException("direction");
            }
        }

        #region このコントロールのイベント
        private void SetMouseEvent()
        {
            //スクロールバーが移動したときのイベント
            MapScrollViewer
                .ScrollAsObservable()
                .Subscribe(_ =>
                    {
                        Canvas.SetLeft(Map, MapScrollViewer.ContentHorizontalOffset);
                        Canvas.SetTop(Map, MapScrollViewer.ContentVerticalOffset);
                        ViewModel.MapScrollPosition = new Point(MapScrollViewer.ContentHorizontalOffset,
                                                                MapScrollViewer.ContentVerticalOffset);
                    });

            //ドラッグ移動
            Wrapper.MouseLeftButtonDownAsObservable().IsSpaceKeyDown().IsNotShiftKeyDown().IsNotRightClick()
                .Select(x => x.GetPosition(Wrapper))
                .SelectMany(down =>
                            Wrapper.MouseMoveAsObservable()
                                   .TakeUntil(Wrapper.MouseLeftButtonUpAsObservable()
                                   .Merge(Wrapper.MouseLeaveAsObservable())
                ).Select(x => new { down, move = x.GetPosition(Wrapper) })
            ).Subscribe(x => ScrollViewerScroll(x.down, x.move));


            //Dキー、Deleteキーを押した時に削除を実行
            this.KeyDownAsObservable().IsKeyDown(Key.D, Key.Delete).Do(x => ViewModel.Delete()).Subscribe();

            //Enterキーを押した時に編集ウィンドウを開く
            this.KeyDownAsObservable().IsKeyDown(Key.Enter).Select(x => PresentMousePosition).Do(x => ViewModel.OpenEditWindow()).Subscribe();

            //コピー・カット・ペースト
            this.KeyDownAsObservable().IsCtrlKeyDown().IsKeyDown(Key.C).Do(_ => ViewModel.Copy()).Subscribe();
            this.KeyDownAsObservable().IsCtrlKeyDown().IsKeyDown(Key.X).Do(_ => ViewModel.Cut()).Subscribe();
            this.KeyDownAsObservable().IsCtrlKeyDown().IsKeyDown(Key.V).Do(_ => ViewModel.Paste()).Subscribe();

            //スクロールバー操作
            Wrapper.KeyDownAsObservable().IsCtrlKeyDown().IsKeyDown(Key.Up).Subscribe(_ => ScrollToTop());
            Wrapper.KeyDownAsObservable().IsCtrlKeyDown().IsKeyDown(Key.Down).Subscribe(_ => ScrollToBottom());
            Wrapper.KeyDownAsObservable().IsCtrlKeyDown().IsKeyDown(Key.Left).Subscribe(_ => ScrollToLeftEnd());
            Wrapper.KeyDownAsObservable().IsCtrlKeyDown().IsKeyDown(Key.Right).Subscribe(_ => ScrollToRightEnd());

            //Ctrl+ホイール操作
            Wrapper.MouseWheelAsObservable().IsCtrlKeyDown().Do(x => ViewModel.MouseWheel(x.Delta)).Subscribe();

            //マウス移動による現在地情報の更新
            Map.MouseMoveAsObservable()
              .Select(_ => this.PresentMousePosition)
              .Scan(
                (new { pre = this.PresentMousePosition, now = this.PresentMousePosition }),
                (acc, x) => new { pre = acc.now, now = x })
              .Skip(1)
              .Do(x =>
                    ViewModel.MousePositionMoved(
                        new Point(x.now.Position.X - MapScrollViewer.ContentHorizontalOffset,
                                  x.now.Position.Y - MapScrollViewer.ContentVerticalOffset))
                    )
              .Where(x => x.pre.GridPoint != x.now.GridPoint)
              .Select(x => x.now)
                //.Where(x => MouseGridPositionMoved.CanExecute)
              .Subscribe(x => ViewModel.MouseGridPositionMoved(x));


            DrawDirection[] drawDirection = { DrawDirection.None };
            //キー入力無しの左クリック→移動→クリックを離すの一連のイベント
            Map.MouseLeftButtonDownAsObservable().IsNotSpaceKeyDown().IsNotRightClick()
              .Select(_ => this.PresentMousePosition)
              .Where(x => x.Position.X >= 0 && x.Position.Y >= 0)
              .Do(x => ViewModel.MouseLeftButtonDown(x))
              .SelectMany(
                down => //平行ドローイング方向とクリックの起点
                  Wrapper.MouseMoveAsObservable()
                    .TakeUntil(
                      Wrapper.MouseLeftButtonUpAsObservable()
                      .Merge(Wrapper.MouseLeaveAsObservable())
                    ) //左クリックを離すか、マウスが領域外に出るまで
                    .Select(x => this.PresentMousePosition)
                    .Scan(
                      (new { pre = this.PresentMousePosition, now = this.PresentMousePosition }),
                      (acc, now) => new { pre = acc.now, now = now })
                    .Skip(1)
                    .Where(x => x.pre.GridPoint != x.now.GridPoint)
                    .Select(x => x.now)
                    .Do(now =>
                        {
                            //Shiftが押されている場合
                            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                            {
                                var dp = GetDirectionAndParallelPosition(drawDirection[0], down, now);
                                drawDirection[0] = dp.Item1;
                                ViewModel.MouseMoveWithLeftButtonDown(dp.Item2);
                            }
                            else
                            {
                                ViewModel.MouseMoveWithLeftButtonDown(now);
                            }

                        })
                    )
                .Merge( // マウスを離すイベントをマージする（離したときに一度だけ起動する）
                Map.MouseLeftButtonUpAsObservable()
                    .Select(_ => this.PresentMousePosition)
                    .Do(up =>
                        {
                            drawDirection[0] = DrawDirection.None;
                            ViewModel.MouseLeftButtonUp(up);
                        }))
                .Subscribe();

            //選択範囲決定
            Map.MouseRightButtonDownAsObservable().IsNotSpaceKeyDown().IsNotShiftKeyDown().IsNotLeftClick()
                .Select(x => new MousePosition(x.GetPosition(Wrapper)))
                .Where(x => x.Position.X >= 0 && x.Position.Y >= 0)
                .Do(x => ViewModel.MouseRightButtonDown(x))
                .SelectMany(
                    down =>
                    Map.MouseMoveAsObservable()
                        .TakeUntil(Map.MouseRightButtonUpAsObservable().Merge(Map.MouseEnterAsObservable()))
                        .Select(x => new MousePosition(x.GetPosition(Wrapper)))
                        .Scan(
                          (new { pre = new MousePosition(), now = new MousePosition() }),
                          (acc, now) => new { pre = acc.now, now = now })
                        .Skip(1)
                        .Where(x => x.pre.GridPoint != x.now.GridPoint)
                        .Select(x => x.now)
                        .Do(x => ViewModel.MouseMoveWithRightButtonDown(x)))
                .Subscribe();

            //マウスがマップにインしたときの処理
            Map.MouseEnterAsObservable().Do(x => Keyboard.Focus(Map)).Subscribe();

            //マウスがマップからアウトしたときの処理
            Map.MouseLeaveAsObservable().Do(_ => ViewModel.MouseLeave()).Subscribe();

            //ダブルクリック
            Map.MouseRightButtonDownAsObservable()
               .Scan(new { pre = DateTime.Now, now = DateTime.Now },
                     (acc, now) => new { pre = acc.now, now = DateTime.Now })
               .Where(x => x.now - x.pre < TimeSpan.FromMilliseconds(300))
               .Subscribe(x => ViewModel.MouseDoubleClick(PresentMousePosition));
        }

        /// <summary>
        /// ScrollViewerをスクロールさせる
        /// </summary>
        private void ScrollViewerScroll(Point previous, Point now)
        {
            //スクロールの位置を取得する
            var pos = new Point(MapScrollViewer.ContentHorizontalOffset, MapScrollViewer.ContentVerticalOffset);

            var mx = previous.X - now.X;
            var my = previous.Y - now.Y;

            //スクロールの位置をセット
            MapScrollViewer.ScrollToHorizontalOffset(pos.X + mx);
            MapScrollViewer.ScrollToVerticalOffset(pos.Y + my);
        }

        public void ScrollToTop()
        {
            MapScrollViewer.ScrollToTop();
        }

        public void ScrollToBottom()
        {
            MapScrollViewer.ScrollToBottom();
        }

        public void ScrollToLeftEnd()
        {
            MapScrollViewer.ScrollToLeftEnd();
        }

        public void ScrollToRightEnd()
        {
            MapScrollViewer.ScrollToRightEnd();
        }

        public void ScrollTo(Point position)
        {
            if (MapScrollViewer != null)
            {
                MapScrollViewer.ScrollToHorizontalOffset(position.X);
                MapScrollViewer.ScrollToVerticalOffset(position.Y);
            }
        }

        #endregion
    }

    public class RoutedEventArgs<T> : RoutedEventArgs
    {
        public new T Source { get { return (T)base.Source; } }

        public RoutedEventArgs(RoutedEvent routedEvent, object source)
            : base(routedEvent, source) { }

    }
}
