

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StageMapEditor.Models;

namespace StageMapEditor.ViewModels
{
    public partial class EditMapViewModel
    {
        /// <summary>
        /// 既存のMapModelからEditMapViewModelを作成
        /// </summary>
        /// <param name="model"></param>
        public EditMapViewModel(MapModel model)
        {
            _model = model;
            _parent = model.StageModel;

			InputID = model.ID.ToString();
			InputMapName = model.MapName.ToString();
			InputBackground = model.Background.ToString();
			InputMapCellWidth = model.MapCellWidth.ToString();
			InputMapCellHeight = model.MapCellHeight.ToString();
			InputBgNo = model.BgNo.ToString();
			InputBGMNo = model.BGMNo.ToString();
			InputScrollSpeed = model.ScrollSpeed.ToString();
			InputScrollAngle = model.ScrollAngle.ToString();
			InputMapChipType = model.MapChipType.ToString();
			InputTimeLimit = model.TimeLimit.ToString();
        }

        private void Initialize()
        {
			InputID = InputID ?? default(System.Int32).ToString();
			InputMapName = InputMapName ?? "";
			InputBackground = InputBackground ?? "";
			InputMapCellWidth = InputMapCellWidth ?? default(System.Int32).ToString();
			InputMapCellHeight = InputMapCellHeight ?? default(System.Int32).ToString();
			InputBgNo = InputBgNo ?? default(System.Int32).ToString();
			InputBGMNo = InputBGMNo ?? default(System.Int32).ToString();
			InputScrollSpeed = InputScrollSpeed ?? default(System.Int32).ToString();
			InputScrollAngle = InputScrollAngle ?? default(System.Int32).ToString();
			InputMapChipType = InputMapChipType ?? default(System.Int32).ToString();
			InputTimeLimit = InputTimeLimit ?? default(System.Int32).ToString();
        }

        #region 入力用プロパティ
	
		private string _InputID;			
		private string _InputMapName;			
		private string _InputBackground;			
		private string _InputMapCellWidth;			
		private string _InputMapCellHeight;			
		private string _InputBgNo;			
		private string _InputBGMNo;			
		private string _InputScrollSpeed;			
		private string _InputScrollAngle;			
		private string _InputMapChipType;			
		private string _InputTimeLimit;			
			
		public string InputID
		{
			get { return _InputID; }
			set
			{
				_InputID = value;
				var valid = true;
				Int32 _val;
				valid = System.Int32.TryParse(value, out _val);
				if(string.IsNullOrEmpty(value) || !valid)
				{
					this["ID"] = "IDに正しい値が入力されていません";
				}
				else
				{
					
					this["ID"] = null;
				}

				RaisePropertyChanged("InputID");
				SaveCommand.RaiseCanExecuteChanged();
			}
		}
			
		public string InputMapName
		{
			get { return _InputMapName; }
			set
			{
				_InputMapName = value;
					if(string.IsNullOrEmpty(value))
				{
					this["MapName"] = "MapNameに正しい値が入力されていません";
				}
				else
				{
					
					this["MapName"] = null;
				}
	
				RaisePropertyChanged("InputMapName");
				SaveCommand.RaiseCanExecuteChanged();
			}
		}
			
		public string InputBackground
		{
			get { return _InputBackground; }
			set
			{
				_InputBackground = value;
					if(string.IsNullOrEmpty(value))
				{
					this["Background"] = "Backgroundに正しい値が入力されていません";
				}
				else
				{
					
					this["Background"] = null;
				}
	
				RaisePropertyChanged("InputBackground");
				SaveCommand.RaiseCanExecuteChanged();
			}
		}
			
		public string InputMapCellWidth
		{
			get { return _InputMapCellWidth; }
			set
			{
				_InputMapCellWidth = value;
				var valid = true;
				Int32 _val;
				valid = System.Int32.TryParse(value, out _val);
				if(string.IsNullOrEmpty(value) || !valid)
				{
					this["MapCellWidth"] = "MapCellWidthに正しい値が入力されていません";
				}
				else
				{
					
					this["MapCellWidth"] = null;
				}

				RaisePropertyChanged("InputMapCellWidth");
				SaveCommand.RaiseCanExecuteChanged();
			}
		}
			
		public string InputMapCellHeight
		{
			get { return _InputMapCellHeight; }
			set
			{
				_InputMapCellHeight = value;
				var valid = true;
				Int32 _val;
				valid = System.Int32.TryParse(value, out _val);
				if(string.IsNullOrEmpty(value) || !valid)
				{
					this["MapCellHeight"] = "MapCellHeightに正しい値が入力されていません";
				}
				else
				{
					
					this["MapCellHeight"] = null;
				}

				RaisePropertyChanged("InputMapCellHeight");
				SaveCommand.RaiseCanExecuteChanged();
			}
		}
			
		public string InputBgNo
		{
			get { return _InputBgNo; }
			set
			{
				_InputBgNo = value;
				var valid = true;
				Int32 _val;
				valid = System.Int32.TryParse(value, out _val);
				if(string.IsNullOrEmpty(value) || !valid)
				{
					this["BgNo"] = "BgNoに正しい値が入力されていません";
				}
				else
				{
					
					this["BgNo"] = null;
				}

				RaisePropertyChanged("InputBgNo");
				SaveCommand.RaiseCanExecuteChanged();
			}
		}
			
		public string InputBGMNo
		{
			get { return _InputBGMNo; }
			set
			{
				_InputBGMNo = value;
				var valid = true;
				Int32 _val;
				valid = System.Int32.TryParse(value, out _val);
				if(string.IsNullOrEmpty(value) || !valid)
				{
					this["BGMNo"] = "BGMNoに正しい値が入力されていません";
				}
				else
				{
					
					this["BGMNo"] = null;
				}

				RaisePropertyChanged("InputBGMNo");
				SaveCommand.RaiseCanExecuteChanged();
			}
		}
			
		public string InputScrollSpeed
		{
			get { return _InputScrollSpeed; }
			set
			{
				_InputScrollSpeed = value;
				var valid = true;
				Int32 _val;
				valid = System.Int32.TryParse(value, out _val);
				if(string.IsNullOrEmpty(value) || !valid)
				{
					this["ScrollSpeed"] = "ScrollSpeedに正しい値が入力されていません";
				}
				else
				{
					
					this["ScrollSpeed"] = null;
				}

				RaisePropertyChanged("InputScrollSpeed");
				SaveCommand.RaiseCanExecuteChanged();
			}
		}
			
		public string InputScrollAngle
		{
			get { return _InputScrollAngle; }
			set
			{
				_InputScrollAngle = value;
				var valid = true;
				Int32 _val;
				valid = System.Int32.TryParse(value, out _val);
				if(string.IsNullOrEmpty(value) || !valid)
				{
					this["ScrollAngle"] = "ScrollAngleに正しい値が入力されていません";
				}
				else
				{
					
					this["ScrollAngle"] = null;
				}

				RaisePropertyChanged("InputScrollAngle");
				SaveCommand.RaiseCanExecuteChanged();
			}
		}
			
		public string InputMapChipType
		{
			get { return _InputMapChipType; }
			set
			{
				_InputMapChipType = value;
				var valid = true;
				Int32 _val;
				valid = System.Int32.TryParse(value, out _val);
				if(string.IsNullOrEmpty(value) || !valid)
				{
					this["MapChipType"] = "MapChipTypeに正しい値が入力されていません";
				}
				else
				{
					
					this["MapChipType"] = null;
				}

				RaisePropertyChanged("InputMapChipType");
				SaveCommand.RaiseCanExecuteChanged();
			}
		}
			
		public string InputTimeLimit
		{
			get { return _InputTimeLimit; }
			set
			{
				_InputTimeLimit = value;
				var valid = true;
				Int32 _val;
				valid = System.Int32.TryParse(value, out _val);
				if(string.IsNullOrEmpty(value) || !valid)
				{
					this["TimeLimit"] = "TimeLimitに正しい値が入力されていません";
				}
				else
				{
					
					this["TimeLimit"] = null;
				}

				RaisePropertyChanged("InputTimeLimit");
				SaveCommand.RaiseCanExecuteChanged();
			}
		}
			
		#endregion

		private List<string> _ErrorList()
		{
            var errorPropertyList = new List<string>();
            if (!string.IsNullOrEmpty(this["InputID"]))
            {
                errorPropertyList.Add("InputID");
            }
            if (!string.IsNullOrEmpty(this["InputMapName"]))
            {
                errorPropertyList.Add("InputMapName");
            }
            if (!string.IsNullOrEmpty(this["InputBackground"]))
            {
                errorPropertyList.Add("InputBackground");
            }
            if (!string.IsNullOrEmpty(this["InputMapCellWidth"]))
            {
                errorPropertyList.Add("InputMapCellWidth");
            }
            if (!string.IsNullOrEmpty(this["InputMapCellHeight"]))
            {
                errorPropertyList.Add("InputMapCellHeight");
            }
            if (!string.IsNullOrEmpty(this["InputBgNo"]))
            {
                errorPropertyList.Add("InputBgNo");
            }
            if (!string.IsNullOrEmpty(this["InputBGMNo"]))
            {
                errorPropertyList.Add("InputBGMNo");
            }
            if (!string.IsNullOrEmpty(this["InputScrollSpeed"]))
            {
                errorPropertyList.Add("InputScrollSpeed");
            }
            if (!string.IsNullOrEmpty(this["InputScrollAngle"]))
            {
                errorPropertyList.Add("InputScrollAngle");
            }
            if (!string.IsNullOrEmpty(this["InputMapChipType"]))
            {
                errorPropertyList.Add("InputMapChipType");
            }
            if (!string.IsNullOrEmpty(this["InputTimeLimit"]))
            {
                errorPropertyList.Add("InputTimeLimit");
            }
		
			return errorPropertyList;
		}
    }
}
