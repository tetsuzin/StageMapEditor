


using System;
using System.Collections.Generic;

namespace StageMapEditor.ViewModels
{
	public partial class MapViewModel
	{
        #region Modelラッパープロパティ
	
		public Int32 ID
		{
			get { return _model.ID; }
			set { _model.ID = value; }
		}
			
		public String MapName
		{
			get { return _model.MapName; }
			set { _model.MapName = value; }
		}
			
		public String Background
		{
			get { return _model.Background; }
			set { _model.Background = value; }
		}
			
		public Int32 MapCellWidth
		{
			get { return _model.MapCellWidth; }
			set { _model.MapCellWidth = value; }
		}
			
		public Int32 MapCellHeight
		{
			get { return _model.MapCellHeight; }
			set { _model.MapCellHeight = value; }
		}
			
		public Int32 BgNo
		{
			get { return _model.BgNo; }
			set { _model.BgNo = value; }
		}
			
		public Int32 BGMNo
		{
			get { return _model.BGMNo; }
			set { _model.BGMNo = value; }
		}
			
		public Int32 ScrollSpeed
		{
			get { return _model.ScrollSpeed; }
			set { _model.ScrollSpeed = value; }
		}
			
		public Int32 ScrollAngle
		{
			get { return _model.ScrollAngle; }
			set { _model.ScrollAngle = value; }
		}
			
		public Int32 MapChipType
		{
			get { return _model.MapChipType; }
			set { _model.MapChipType = value; }
		}
			
		public Int32 TimeLimit
		{
			get { return _model.TimeLimit; }
			set { _model.TimeLimit = value; }
		}
			
		#endregion
	}
}