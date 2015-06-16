using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace StageMapEditor.Helper
{
    public static partial class WpfControlExtensions
    {
        public static IObservable<MouseEventArgs> MouseMoveAsObservable(this FrameworkElement control)
        {
            return Observable.FromEventPattern<MouseEventHandler, MouseEventArgs>(
                h => new MouseEventHandler(h),
                h => control.MouseMove += h,
                h => control.MouseMove -= h)
                .Select(e => e.EventArgs);
        }

        public static IObservable<MouseEventArgs> MouseEnterAsObservable(this FrameworkElement control)
        {
            return Observable.FromEventPattern<MouseEventHandler, MouseEventArgs>(
                h => new MouseEventHandler(h),
                h => control.MouseEnter += h,
                h => control.MouseEnter -= h)
                .Select(e => e.EventArgs);
        }

        public static IObservable<MouseEventArgs> MouseLeaveAsObservable(this FrameworkElement control)
        {
            return Observable.FromEventPattern<MouseEventHandler, MouseEventArgs>(
                h => new MouseEventHandler(h),
                h => control.MouseLeave += h,
                h => control.MouseLeave -= h)
                .Select(e => e.EventArgs);
        }

        public static IObservable<MouseButtonEventArgs> MouseLeftButtonDownAsObservable(this FrameworkElement control)
        {
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(
                h => new MouseButtonEventHandler(h),
                h => control.MouseLeftButtonDown += h,
                h => control.MouseLeftButtonDown -= h)
                .Select(e => e.EventArgs);
        }

        public static IObservable<MouseEventArgs> MouseLeftButtonUpAsObservable(this FrameworkElement control)
        {
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(
                h => new MouseButtonEventHandler(h),
                h => control.MouseLeftButtonUp += h,
                h => control.MouseLeftButtonUp -= h)
                .Select(e => e.EventArgs);
        }

        public static IObservable<MouseEventArgs> MouseRightButtonDownAsObservable(this FrameworkElement control)
        {
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(
                h => new MouseButtonEventHandler(h),
                h => control.MouseRightButtonDown += h,
                h => control.MouseRightButtonDown -= h)
                .Select(e => e.EventArgs);
        }

        public static IObservable<MouseEventArgs> MouseRightButtonUpAsObservable(this FrameworkElement control)
        {
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(
                h => new MouseButtonEventHandler(h),
                h => control.MouseRightButtonUp += h,
                h => control.MouseRightButtonUp -= h)
                .Select(e => e.EventArgs);
        }

        public static IObservable<MouseEventArgs> MouseDownAsObservable(this FrameworkElement control)
        {
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(
                h => new MouseButtonEventHandler(h),
                h => control.MouseDown += h,
                h => control.MouseDown -= h)
                .Select(e => e.EventArgs);
        }

        public static IObservable<MouseEventArgs> MouseUpAsObservable(this FrameworkElement control)
        {
            return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(
                h => new MouseButtonEventHandler(h),
                h => control.MouseUp += h,
                h => control.MouseUp -= h)
                .Select(e => e.EventArgs);
        }

        //public static IObservable<MouseEventArgs> MouseDoubleClickAsObservable(this FrameworkElement control)
        //{
        //    return Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(
        //        h => new MouseButtonEventHandler(h),
        //        h => control.MouseDoubleClick += h,
        //        h => control.MouseDoubleClick -= h)
        //        .Select(e => e.EventArgs);
        //}

        public static IObservable<MouseEventArgs> MouseLeftDrag(this FrameworkElement control)
        {
            return from down in control.MouseLeftButtonDownAsObservable()
                   from move in control.MouseMoveAsObservable().TakeUntil(control.MouseLeftButtonUpAsObservable())
                   select move;
        }

        public static IObservable<MouseEventArgs> MouseRightDrag(this FrameworkElement control)
        {
            return from down in control.MouseRightButtonDownAsObservable()
                   from move in control.MouseMoveAsObservable().TakeUntil(control.MouseRightButtonUpAsObservable())
                   select move;
        }

        public static IObservable<MouseWheelEventArgs> MouseWheelAsObservable(this FrameworkElement control)
        {
            return Observable.FromEventPattern<MouseWheelEventHandler, MouseWheelEventArgs>(
                h => new MouseWheelEventHandler(h),
                h => control.MouseWheel += h,
                h => control.MouseWheel -= h)
                .Select(e => e.EventArgs);
        }

        /// <summary>
        /// スクロールバーが動いたときのイベント
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static IObservable<ScrollChangedEventArgs> ScrollAsObservable(this ScrollViewer control)
        {
            return Observable.FromEventPattern<ScrollChangedEventHandler, ScrollChangedEventArgs>(
                h => new ScrollChangedEventHandler(h),
                h => control.ScrollChanged += h,
                h => control.ScrollChanged -= h)
                .Select(e => e.EventArgs);
        }

        public static IObservable<KeyEventArgs> KeyDownAsObservable(this FrameworkElement control)
        {
            return Observable.FromEventPattern<KeyEventHandler, KeyEventArgs>(
                h => new KeyEventHandler(h),
                h => control.KeyDown += h,
                h => control.KeyDown -= h)
                .Select(e => e.EventArgs);
        }

        public static IObservable<TEventArgs> IsKeyDown<TEventArgs>(this IObservable<TEventArgs> source, Key key)
        {
            return source.Where(_ => Keyboard.IsKeyDown(key));
        }

        public static IObservable<TEventArgs> IsKeyDown<TEventArgs>(this IObservable<TEventArgs> source, Key key1, Key key2)
        {
            return source.Where(_ => Keyboard.IsKeyDown(key1) || Keyboard.IsKeyDown(key2));
        }

        public static IObservable<TEventArgs> IsKeyDown<TEventArgs>(this IObservable<TEventArgs> source, Key key1, Key key2, Key key3)
        {
            return source.Where(_ => Keyboard.IsKeyDown(key1) || Keyboard.IsKeyDown(key2) || Keyboard.IsKeyDown(key3));
        }

        public static IObservable<TEventArgs> IsKeyDown<TEventArgs>(this IObservable<TEventArgs> source, Key key1, Key key2, Key key3, Key key4)
        {
            return source.Where(_ => Keyboard.IsKeyDown(key1) || Keyboard.IsKeyDown(key2) || Keyboard.IsKeyDown(key3) || Keyboard.IsKeyDown(key4));
        }

        public static IObservable<TEventArgs> IsNotKeyDown<TEventArgs>(this IObservable<TEventArgs> source, Key key)
        {
            return source.Where(_ => !Keyboard.IsKeyDown(key));
        }

        /// <summary>
        /// 左クリックがされていない
        /// </summary>
        /// <typeparam name="TEventArgs"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IObservable<TEventArgs> IsNotLeftClick<TEventArgs>(this IObservable<TEventArgs> source)
        {
            return source.Where(_ => Mouse.LeftButton != MouseButtonState.Pressed);
        }

        /// <summary>
        /// 右クリックがされていない
        /// </summary>
        /// <typeparam name="TEventArgs"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IObservable<TEventArgs> IsNotRightClick<TEventArgs>(this IObservable<TEventArgs> source)
        {
            return source.Where(_ => Mouse.RightButton != MouseButtonState.Pressed);
        }

        /// <summary>
        /// スペースキーが押されている
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IObservable<TEventArgs> IsSpaceKeyDown<TEventArgs>(this IObservable<TEventArgs> source)
        {
            return source.Where(_ => Keyboard.IsKeyDown(Key.Space));
        }

        /// <summary>
        /// シフトキーが押されている
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IObservable<TEventArgs> IsShiftKeyDown<TEventArgs>(this IObservable<TEventArgs> source)
        {
            return source.Where(_ => Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift));
        }

        /// <summary>
        /// シフトキーが押されている
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IObservable<TEventArgs> IsCtrlKeyDown<TEventArgs>(this IObservable<TEventArgs> source)
        {
            return source.Where(_ => Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl));
        }

        /// <summary>
        /// スペースキーが押されていない
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IObservable<TEventArgs> IsNotSpaceKeyDown<TEventArgs>(this IObservable<TEventArgs> source)
        {
            return source.Where(_ => !Keyboard.IsKeyDown(Key.Space));
        }

        /// <summary>
        /// シフトキーが押されていない
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IObservable<TEventArgs> IsNotShiftKeyDown<TEventArgs>(this IObservable<TEventArgs> source)
        {
            return source.Where(_ => !Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.RightShift));
        }

        /// <summary>
        /// コントロールキーが押されていない
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IObservable<TEventArgs> IsNotCtrlKeyDown<TEventArgs>(this IObservable<TEventArgs> source)
        {
            return source.Where(_ => !Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.IsKeyDown(Key.RightCtrl));
        }
    }
}
