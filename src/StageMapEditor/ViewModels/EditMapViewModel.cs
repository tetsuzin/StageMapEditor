using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Livet;
using Livet.Commands;
using Livet.Messaging.Windows;
using StageMapEditor.Models;

namespace StageMapEditor.ViewModels
{
    public partial class EditMapViewModel : ViewModel, IDataErrorInfo
    {
        private MapModel _model;
        private StageModel _parent;

        /// <summary>
        /// 空のEditMapViewModelを作成
        /// </summary>
        /// <param name="parent"></param>
        public EditMapViewModel(StageModel parent)
        {
            _parent = parent;

            InputID = _parent.GetNextID().ToString();
            InputMapCellWidth = 40.ToString();
            InputMapCellHeight = 40.ToString();
            InputBackground = string.Format("{0}-{1}-{2}.png", _parent.World, _parent.Stage, InputID);
            InputMapName = string.Format("ワールド {0}, ステージ {1}, マップ {2}", _parent.World, _parent.Stage, InputID);

            Initialize();
        }

        #region IDataErrorInfo

        /// <summary>
        /// エラー情報登録用
        /// </summary>
        private Dictionary<string, string> _errors = new Dictionary<string, string>();

        public string Error
        {
            get
            {
                var errorPropertyList = _ErrorList();

                if (errorPropertyList.Count != 0)
                {
                    return string.Join("・", errorPropertyList.ToArray()) + "が不正です";
                }

                return null;
            }
        }

        public string this[string columnName]
        {
            get
            {
                if (_errors.Keys.Contains(columnName))
                {
                    return _errors[columnName];
                }

                return null;
            }
            set
            {
                _errors[columnName] = value;
                RaisePropertyChanged("Error");
                SaveCommand.RaiseCanExecuteChanged();
            }
        }
        #endregion

        #region SaveCommand
        ViewModelCommand _SaveCommand;

        public ViewModelCommand SaveCommand
        {
            get
            {
                if (_SaveCommand == null)
                    _SaveCommand = new ViewModelCommand(Save, CanSave);
                return _SaveCommand;
            }
        }

        private bool CanSave()
        {
            if (!string.IsNullOrEmpty(Error))
            {
                return false;
            }

            return true;
        }

        private void Save()
        {
            if (_parent.Contains(_model))
            {
                _model.UpdateFromInput(this);
            }
            else
            {
                _parent.MapModels.Add(new MapModel(this, _parent));
            }

            //Viewに画面遷移用メッセージを送信しています。
            //Viewは対応するメッセージキーを持つInteractionTransitionMessageTriggerでこのメッセージを受信します。
            Messenger.Raise(new WindowActionMessage(WindowAction.Close, "Close"));
        }
        #endregion

        #region CancelCommand
        ViewModelCommand _CancelCommand;

        public ViewModelCommand CancelCommand
        {
            get
            {
                if (_CancelCommand == null)
                    _CancelCommand = new ViewModelCommand(Cancel);
                return _CancelCommand;
            }
        }

        private void Cancel()
        {
            //Viewに画面遷移用メッセージを送信しています。
            //Viewは対応するメッセージキーを持つInteractionTransitionMessageTriggerでこのメッセージを受信します。
            Messenger.Raise(new WindowActionMessage(WindowAction.Close, "Close"));
        }

        #endregion
    }
}
