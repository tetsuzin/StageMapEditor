using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.Windows;
using StageMapEditor.Models;
using StageMapEditor.Models.Chip;

namespace StageMapEditor.ViewModels
{
    public partial class ObjectChipViewModel : ViewModel
    {
        /*コマンド、プロパティの定義にはそれぞれ 
         * 
         *  lvcom   : ViewModelCommand
         *  lvcomn  : ViewModelCommand(CanExecute無)
         *  llcom   : ListenerCommand(パラメータ有のコマンド)
         *  llcomn  : ListenerCommand(パラメータ有のコマンド・CanExecute無)
         *  lprop   : 変更通知プロパティ
         *  
         * を使用してください。
         */

        /*ViewModelからViewを操作したい場合は、
         * Messengerプロパティからメッセージ(各種InteractionMessage)を発信してください。
         */

        /*
         * UIDispatcherを操作する場合は、DispatcherHelperのメソッドを操作してください。
         * UIDispatcher自体はApp.xaml.csでインスタンスを確保してあります。
         */

        /*
         * Modelからの変更通知などの各種イベントをそのままViewModelで購読する事はメモリリークの
         * 原因となりやすく推奨できません。ViewModelHelperの各静的メソッドの利用を検討してください。
         */

        private ObjectChip _model;

        public ObjectChipViewModel(ObjectChip model)
        {
            _model = model;
            InitializeInput();
        }

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

            if (!_CanSave())
            {
                return false;
            }

            return true;
        }

        private void Save()
        {
            _UpdateByInput();
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
            //入力情報初期化
            InitializeInput();

            Messenger.Raise(new WindowActionMessage(WindowAction.Close, "Close"));
        }

        /// <summary>
        /// 入力情報を初期化
        /// </summary>
        private void InitializeInput()
        {
            _InitializeInput();
            _errors.Clear();
        }
        #endregion

        #region IDataErrorInfo
        /// <summary>
        /// エラー情報登録用
        /// </summary>
        private readonly Dictionary<string, string> _errors = new Dictionary<string, string>();

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
    }
}
