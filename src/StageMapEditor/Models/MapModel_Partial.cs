


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using StageMapEditor.ViewModels;

namespace StageMapEditor.Models
{
    public partial class MapModel 
    {
        #region Modelパラメータ

        Int32 _ID;
        public Int32 ID
        {
            get { return _ID; }
            set
            {
                if (_ID == value) return;
                _ID = value;
                RaisePropertyChanged("ID");
				EditWidthHeight();
            }
        }

        String _MapName;
        public String MapName
        {
            get { return _MapName; }
            set
            {
                if (_MapName == value) return;
                _MapName = value;
                RaisePropertyChanged("MapName");
				EditWidthHeight();
            }
        }

        String _Background;
        public String Background
        {
            get { return _Background; }
            set
            {
                if (_Background == value) return;
                _Background = value;
                RaisePropertyChanged("Background");
				EditWidthHeight();
            }
        }

        Int32 _MapCellWidth;
        public Int32 MapCellWidth
        {
            get { return _MapCellWidth; }
            set
            {
                if (_MapCellWidth == value) return;
                _MapCellWidth = value;
                RaisePropertyChanged("MapCellWidth");
				EditWidthHeight();
            }
        }

        Int32 _MapCellHeight;
        public Int32 MapCellHeight
        {
            get { return _MapCellHeight; }
            set
            {
                if (_MapCellHeight == value) return;
                _MapCellHeight = value;
                RaisePropertyChanged("MapCellHeight");
				EditWidthHeight();
            }
        }

        Int32 _BgNo;
        public Int32 BgNo
        {
            get { return _BgNo; }
            set
            {
                if (_BgNo == value) return;
                _BgNo = value;
                RaisePropertyChanged("BgNo");
				EditWidthHeight();
            }
        }

        Int32 _BGMNo;
        public Int32 BGMNo
        {
            get { return _BGMNo; }
            set
            {
                if (_BGMNo == value) return;
                _BGMNo = value;
                RaisePropertyChanged("BGMNo");
				EditWidthHeight();
            }
        }

        Int32 _ScrollSpeed;
        public Int32 ScrollSpeed
        {
            get { return _ScrollSpeed; }
            set
            {
                if (_ScrollSpeed == value) return;
                _ScrollSpeed = value;
                RaisePropertyChanged("ScrollSpeed");
				EditWidthHeight();
            }
        }

        Int32 _ScrollAngle;
        public Int32 ScrollAngle
        {
            get { return _ScrollAngle; }
            set
            {
                if (_ScrollAngle == value) return;
                _ScrollAngle = value;
                RaisePropertyChanged("ScrollAngle");
				EditWidthHeight();
            }
        }

        Int32 _MapChipType;
        public Int32 MapChipType
        {
            get { return _MapChipType; }
            set
            {
                if (_MapChipType == value) return;
                _MapChipType = value;
                RaisePropertyChanged("MapChipType");
				EditWidthHeight();
            }
        }

        Int32 _TimeLimit;
        public Int32 TimeLimit
        {
            get { return _TimeLimit; }
            set
            {
                if (_TimeLimit == value) return;
                _TimeLimit = value;
                RaisePropertyChanged("TimeLimit");
				EditWidthHeight();
            }
        }

		#endregion

        public void UpdateFromInput(EditMapViewModel editMapViewModel)
        {
			ID = Int32.Parse(editMapViewModel.InputID);
			MapName = editMapViewModel.InputMapName;
			Background = editMapViewModel.InputBackground;
			MapCellWidth = Int32.Parse(editMapViewModel.InputMapCellWidth);
			MapCellHeight = Int32.Parse(editMapViewModel.InputMapCellHeight);
			BgNo = Int32.Parse(editMapViewModel.InputBgNo);
			BGMNo = Int32.Parse(editMapViewModel.InputBGMNo);
			ScrollSpeed = Int32.Parse(editMapViewModel.InputScrollSpeed);
			ScrollAngle = Int32.Parse(editMapViewModel.InputScrollAngle);
			MapChipType = Int32.Parse(editMapViewModel.InputMapChipType);
			TimeLimit = Int32.Parse(editMapViewModel.InputTimeLimit);
        }

		/// <summary>
		/// EditMapViewModelからMapModelの値のセット
		/// </summary>
		/// <param name="inputViewModel"></param>
		/// <param name="parent"></param>
        private void SetProperties(EditMapViewModel inputViewModel, StageModel parent)
        {
			this.ID = System.Int32.Parse(inputViewModel.InputID);
			this.MapName = string.Format("World{0}-Stage{1}-Map{2}", parent.World, parent.Stage, ID);
			this.Background = inputViewModel.InputBackground;
			this.MapCellWidth = System.Int32.Parse(inputViewModel.InputMapCellWidth);
			this.MapCellHeight = System.Int32.Parse(inputViewModel.InputMapCellHeight);
			this.BgNo = System.Int32.Parse(inputViewModel.InputBgNo);
			this.BGMNo = System.Int32.Parse(inputViewModel.InputBGMNo);
			this.ScrollSpeed = System.Int32.Parse(inputViewModel.InputScrollSpeed);
			this.ScrollAngle = System.Int32.Parse(inputViewModel.InputScrollAngle);
			this.MapChipType = System.Int32.Parse(inputViewModel.InputMapChipType);
			this.TimeLimit = System.Int32.Parse(inputViewModel.InputTimeLimit);
        }
		
		/// <summary>
		/// MapModelPackからMapModelの値のセット
		/// </summary>
		/// <param name="mapModelPack"></param>
        private void SetProperties(MapModelPack mapModelPack)
        {
			this.ID = mapModelPack.ID;
			this.MapName = mapModelPack.MapName;
			this.Background = mapModelPack.Background;
			this.MapCellWidth = mapModelPack.Width;
			this.MapCellHeight = mapModelPack.Height;
			this.BgNo = mapModelPack.BgNo;
			this.BGMNo = mapModelPack.BGMNo;
			this.ScrollSpeed = mapModelPack.ScrollSpeed;
			this.ScrollAngle = mapModelPack.ScrollAngle;
			this.MapChipType = mapModelPack.MapChipType;
			this.TimeLimit = mapModelPack.TimeLimit;
        }
		
		/// <summary>
		/// MapModelからMessagePack保存用のデータを作成
		/// </summary>
        public MapModelPack ToMsgPack()
        {
            return new MapModelPack()
            {
				ID = ID,
				MapName = MapName,
				Background = Background,
				BgNo = BgNo,
				BGMNo = BGMNo,
				ScrollSpeed = ScrollSpeed,
				ScrollAngle = ScrollAngle,
				MapChipType = MapChipType,
				TimeLimit = TimeLimit,
                Width = MapCellWidth,
                Height = MapCellHeight,
                MapChipPack = MapChipModel.ToMagPack(),
                ObjectChipPack = ObjectChipModel.ToMsgPack(),
            };
        }
	}
}