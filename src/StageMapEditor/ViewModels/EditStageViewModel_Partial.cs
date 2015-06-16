

using System;
using System.Collections.Generic;
using StageMapEditor.Models;

namespace StageMapEditor.ViewModels
{
	public partial class EditStageViewModel
	{
        private void Initialize()
        {
			_InputWorld = default(Int32).ToString();
			_InputStage = default(Int32).ToString();
			_InputStageName = String.Empty;
			_InputStageDescription = String.Empty;
        }

        private void Initislize(StageModel stageModel)
        {
			_InputWorld = stageModel.World.ToString();
			_InputStage = stageModel.Stage.ToString();
			_InputStageName = stageModel.StageName.ToString();
			_InputStageDescription = stageModel.StageDescription.ToString();
        }

        #region 入力用プロパティ
	
		private string _InputWorld;
		private string _InputStage;
		private string _InputStageName;
		private string _InputStageDescription;
			
		public string InputWorld
		{
			get { return _InputWorld; }
			set
			{
				_InputWorld = value;
				var valid = true;
				Int32 _val;
				valid = Int32.TryParse(value, out _val);
				if(string.IsNullOrEmpty(value) || !valid)
				{
					this["World"] = "Worldに正しい値が入力されていません";
				}
				else
				{
					_errors.Remove("World");
				}

                RaisePropertyChanged("InputWorld");
                SaveCommand.RaiseCanExecuteChanged();
			}
		}
			
		public string InputStage
		{
			get { return _InputStage; }
			set
			{
				_InputStage = value;
				var valid = true;
				Int32 _val;
				valid = Int32.TryParse(value, out _val);
				if(string.IsNullOrEmpty(value) || !valid)
				{
					this["Stage"] = "Stageに正しい値が入力されていません";
				}
				else
				{
					_errors.Remove("Stage");
				}

                RaisePropertyChanged("InputStage");
                SaveCommand.RaiseCanExecuteChanged();
			}
		}
			
		public string InputStageName
		{
			get { return _InputStageName; }
			set
			{
				_InputStageName = value;
				if(string.IsNullOrEmpty(value))
				{
					this["StageName"] = "StageNameに正しい値が入力されていません";
				}
				else
				{
					_errors.Remove("StageName");
				}

                RaisePropertyChanged("InputStageName");
                SaveCommand.RaiseCanExecuteChanged();
			}
		}
			
		public string InputStageDescription
		{
			get { return _InputStageDescription; }
			set
			{
				_InputStageDescription = value;
				if(string.IsNullOrEmpty(value))
				{
					this["StageDescription"] = "StageDescriptionに正しい値が入力されていません";
				}
				else
				{
					_errors.Remove("StageDescription");
				}

                RaisePropertyChanged("InputStageDescription");
                SaveCommand.RaiseCanExecuteChanged();
			}
		}
			
		#endregion

		private List<string> _ErrorList()
		{
            var errorPropertyList = new List<string>();
            if (!string.IsNullOrEmpty(this["InputWorld"]))
            {
                errorPropertyList.Add("InputWorld");
            }
            if (!string.IsNullOrEmpty(this["InputStage"]))
            {
                errorPropertyList.Add("InputStage");
            }
            if (!string.IsNullOrEmpty(this["InputStageName"]))
            {
                errorPropertyList.Add("InputStageName");
            }
            if (!string.IsNullOrEmpty(this["InputStageDescription"]))
            {
                errorPropertyList.Add("InputStageDescription");
            }
		
			return errorPropertyList;
		}

		private bool _CanSave()
		{
            if (string.IsNullOrEmpty(InputWorld))
            {
                return false;
            }
            if (string.IsNullOrEmpty(InputStage))
            {
                return false;
            }
            if (string.IsNullOrEmpty(InputStageName))
            {
                return false;
            }

			return true;
		}
	}
}