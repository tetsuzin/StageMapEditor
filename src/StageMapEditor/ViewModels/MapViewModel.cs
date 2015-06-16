using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Livet;
using Livet.Commands;
using Livet.EventListeners;
using Livet.Messaging;
using Livet.Messaging.Windows;
using StageMapEditor.Models;

namespace StageMapEditor.ViewModels
{
    public partial class MapViewModel : ViewModel
    {
        private MapModel _model;

        public bool IsSelected { get { return _model.IsSelected; } }

        public MapViewModel(MapModel m)
        {
            _model = m;
            CompositeDisposable.Add(new PropertyChangedEventListener(_model, (sender, args) => RaisePropertyChanged(args.PropertyName)));
        }

        public void SelectNode()
        {
            _model.ChangeMap();
        }

        #region EditCommand Mapの編集ウィンドウを開くコマンド
        ViewModelCommand _EditCommand;
        public ViewModelCommand EditCommand { get { return _EditCommand ?? (_EditCommand = new ViewModelCommand(Edit)); } }
        private void Edit()
        {
            Messenger.Raise(new TransitionMessage(new EditMapViewModel(_model), "TransitionMap"));
        }
        #endregion

        #region DeleteCommand Mapの削除コマンド
        ListenerCommand<ConfirmationMessage> _DeleteCommand;
        public ListenerCommand<ConfirmationMessage> DeleteCommand { get { return _DeleteCommand ?? (_DeleteCommand = new ListenerCommand<ConfirmationMessage>(Delete)); } }

        private void Delete(ConfirmationMessage message)
        {
            if (message.Response.HasValue && message.Response.Value)
            {
                _model.DeleteMap();
                _model.UpdateParent();
            }
        }
        #endregion

        public void SelectMapList()
        {
            _model.ChangeMap();
        }
    }
}
