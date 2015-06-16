using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using Livet.Commands;

namespace StageMapEditor.Views
{
    public class TextBoxBehavior : Behavior<TextBox>
    {
        public static readonly DependencyProperty PressEnterProperty =
            DependencyProperty.Register("PressEnter", typeof(ViewModelCommand), typeof(TextBoxBehavior), new PropertyMetadata(default(ViewModelCommand)));

        public ViewModelCommand PressEnter
        {
            get { return (ViewModelCommand)GetValue(PressEnterProperty); }
            set { SetValue(PressEnterProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.KeyDown += KeyPress;
            AssociatedObject.GotFocus += GotFocus;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.KeyDown -= KeyPress;
            AssociatedObject.GotFocus -= GotFocus;
        }

        private void GotFocus(object sender, RoutedEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb != null) tb.SelectAll();
        }

        private void KeyPress(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                PressEnter.Execute();
            }
        }
    }
}
