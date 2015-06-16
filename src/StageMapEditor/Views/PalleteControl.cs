using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using Livet.Commands;
using StageMapEditor.Helper;
using StageMapEditor.ViewModels;

namespace StageMapEditor.Views
{
  public class PalleteControl : Control
  {
    static PalleteControl()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(PalleteControl), new FrameworkPropertyMetadata(typeof(PalleteControl)));
    }
  }

  public class PalletItemBehavior : Behavior<Canvas>
  {
    #region
    public static readonly DependencyProperty PalletEnterProperty =
      DependencyProperty.Register("PalletEnter", typeof(ListenerCommand<int>), typeof(PalletItemBehavior),
                                  new PropertyMetadata(default(ICommand)));
    public ListenerCommand<int> PalletEnter
    {
      get { return (ListenerCommand<int>)GetValue(PalletEnterProperty); }
      set { SetValue(PalletEnterProperty, value); }
    }
    #endregion

    protected override void OnAttached()
    {
      base.OnAttached();

      AssociatedObject.Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
    {
      var vm = AssociatedObject.DataContext as PalletItemViewModel;

      AssociatedObject.MouseEnterAsObservable()
        .Subscribe(_ => PalletEnter.Execute(vm.ItemID));
    }
  }
}