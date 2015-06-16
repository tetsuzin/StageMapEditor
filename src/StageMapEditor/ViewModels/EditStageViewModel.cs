using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Livet;
using Livet.Commands;
using Livet.Messaging.Windows;
using StageMapEditor.Models;

namespace StageMapEditor.ViewModels
{
    public partial class EditStageViewModel : ViewModel, IDataErrorInfo
    {
        private MainModel _parent;
        private StageModel _model;

        /// <summary>
        /// ��̕ҏW��ʗpViewModel���쐬
        /// </summary>
        /// <param name="mainModel"></param>
        public EditStageViewModel(MainModel mainModel)
        {
            _parent = mainModel;

            Initialize();

            //�����l�ݒ�
            InputStageName = "���[���h 0 �X�e�[�W 0";
        }

        /// <summary>
        /// ���ɑ��݂���StageModel����ҏW��ʗpViewModel���쐬
        /// </summary>
        /// <param name="stageModel"></param>
        /// <param name="mainModel"></param>
        public EditStageViewModel(StageModel stageModel, MainModel mainModel)
        {
            _parent = mainModel;
            _model = stageModel;

            Initislize(stageModel);
        }

        #region SaveCommand Stage�̒ǉ�
        ViewModelCommand _SaveCommand;

        public ViewModelCommand SaveCommand
        {
            get { return _SaveCommand ?? (_SaveCommand = new ViewModelCommand(SaveStage, CanSaveStage)); }
        }

        private bool CanSaveStage()
        {
            return !_errors.Any();
        }

        private void SaveStage()
        {
            if (_parent.Contains(_model))
            {
                _model.UpdateFromInput(this);
            }
            else
            {
                _parent.StageModels.Add(new StageModel(this, _parent));
            }

            Messenger.Raise(new WindowActionMessage(WindowAction.Close, "Close"));
        }

        #endregion

        public void Cancel()
        {
            Messenger.Raise(new WindowActionMessage(WindowAction.Close, "Close"));
        }

        #region IDataErrorInfo

        /// <summary>
        /// �G���[���o�^�p
        /// </summary>
        private readonly Dictionary<string, string> _errors = new Dictionary<string, string>();

        public string Error
        {
            get
            {
                var errorPropertyList = _ErrorList();

                if (errorPropertyList.Count != 0)
                {
                    return string.Join("�E", errorPropertyList.ToArray()) + "���s���ł�";
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
            }
        }
        #endregion
    }
}