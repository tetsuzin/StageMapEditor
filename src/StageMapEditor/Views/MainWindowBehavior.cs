using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Input;

namespace StageMapEditor.Views
{
    public class MainWindowBehavior : Behavior<Window>
    {
        #region DependencyProperty FileDrop
        public static readonly DependencyProperty FileDropProperty =
            DependencyProperty.Register("FileDrop", typeof(ICommand), typeof(MainWindowBehavior), new PropertyMetadata(default(ICommand)));

        public ICommand FileDrop
        {
            get { return (ICommand)GetValue(FileDropProperty); }
            set { SetValue(FileDropProperty, value); }
        }
        #endregion

        #region DependencyProperty ApplicationClosed
        public static readonly DependencyProperty ApplicationClosedProperty =
            DependencyProperty.Register("ApplicationClosed", typeof(ICommand), typeof(MainWindowBehavior), new PropertyMetadata(default(ICommand)));

        public ICommand ApplicationClosed
        {
            get { return (ICommand)GetValue(ApplicationClosedProperty); }
            set { SetValue(ApplicationClosedProperty, value); }
        }
        #endregion

        protected override void OnAttached()
        {
            base.OnAttached();

            if (AssociatedObject != null)
            {
                AssociatedObject.PreviewDrop += Drop;
                AssociatedObject.Closed += WindowClosed;
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (AssociatedObject != null)
            {
                AssociatedObject.PreviewDrop -= Drop;
                AssociatedObject.Closed -= WindowClosed;
            }
        }

        private void Drop(object sender, DragEventArgs args)
        {
            if (!(args.Data is DataObject))
            {
                return;
            }

            var dataObject = ((DataObject)args.Data);

            if (dataObject.ContainsFileDropList())
            {
                FileDrop.Execute((dataObject.GetFileDropList()).Cast<string>().ToArray());
            }
        }

        private void WindowClosed(object sender, EventArgs e)
        {
            ApplicationClosed.Execute(sender);
        }
    }
}