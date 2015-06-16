using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows;
using Livet;
using Livet.Commands;
using Livet.EventListeners;
using Livet.Messaging;
using Livet.Messaging.Windows;
using StageMapEditor.Models;
using StageMapEditor.Models.Chip;
using StageMapEditor.Models.Commands;
using StageMapEditor.Models.Drawing;
using StageMapEditor.Models.MousePosition;
using StageMapEditor.RenderEngine;
using StageMapEditor.Views;

namespace StageMapEditor.ViewModels
{
    public partial class StageViewModel : ViewModel
    {
        /// <summary>
        /// Model部
        /// </summary>
        private readonly StageModel _model;

        public MainViewModel MainViewModel { get; set; }

        private bool CurrentMapIsNull { get { return _model.CurrentMap == null; } }

        public bool IsSelected { get { return _model.IsSelected; } }

        //コンストラクタ
        public StageViewModel(StageModel model, MainViewModel mainViewModel)
        {
            MainViewModel = mainViewModel;

            _model = model;
            _model.MapModels.CollectionChanged += (sender, args) =>
                {
                    _model.Parent.ResetMapModels();

                    if (args.Action == NotifyCollectionChangedAction.Add)
                    {
                        var maps = args.NewItems.Cast<MapModel>();
                        foreach (var m in maps)
                        {
                            var l = new PropertyChangedEventListener(m, (o, eventArgs) =>
                                {
                                    RaisePropertyChanged(eventArgs.PropertyName);
                                    RaisePropertyChanged("CurrentMap");
                                });

                            CompositeDisposable.Add(l);
                        }

                        if (CurrentMapIsNull) { _model.CurrentMap = maps.FirstOrDefault(); }

                        RaisePropertyChanged("CurrentMap");
                    }
                };

            CompositeDisposable.Add(new PropertyChangedEventListener(_model, (sender, e) => RaisePropertyChanged(e.PropertyName)));
        }


        /// <summary>
        /// Stageの追加
        /// </summary>
        private void CreateStage()
        {
            var inputVewModel = new EditStageViewModel(_model.Parent);
            Messenger.Raise(new TransitionMessage(inputVewModel, "TransitionStage"));
        }

        /// <summary>
        /// 引数のModelがこのViewModelのModelかどうかチェックする
        /// </summary>
        /// <param name="stageModel"></param>
        /// <returns></returns>
        public bool HasModel(StageModel stageModel)
        {
            return _model == stageModel;
        }

        public PropertyChangedEventListener Listener { get; set; }

        #region EditStageCommand Stageの編集
        ListenerCommand<MainViewModel> _EditStageCommand;

        public ListenerCommand<MainViewModel> EditStageCommand
        {
            get { return _EditStageCommand ?? (_EditStageCommand = new ListenerCommand<MainViewModel>(EditStage)); }
        }

        private void EditStage(MainViewModel parameter)
        {
            var inputVewModel = new EditStageViewModel(_model, _model.Parent);
            Messenger.Raise(new TransitionMessage(inputVewModel, "TransitionStage"));
        }
        #endregion

        #region CloseStageCommand
        private ListenerCommand<StageViewModel> _CloseStageCommand;
        public ListenerCommand<StageViewModel> CloseStageCommand { get { return _CloseStageCommand ?? (_CloseStageCommand = new ListenerCommand<StageViewModel>(CloseStage)); } }
        public void CloseStage(StageViewModel parameter)
        {
            if (_model.IsEdited())
            {
                var result = MessageBox.Show("Stageの変更を保存しますか？", "StageMapEditor", MessageBoxButton.YesNoCancel);
                switch (result)
                {
                    case MessageBoxResult.OK:
                    case MessageBoxResult.Yes:
                        _model.Save();
                        break;
                    case MessageBoxResult.Cancel:
                    case MessageBoxResult.None:
                        return;
                    case MessageBoxResult.No:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            _model.Parent.StageModels.Remove(_model);
        }
        #endregion
    }
}
