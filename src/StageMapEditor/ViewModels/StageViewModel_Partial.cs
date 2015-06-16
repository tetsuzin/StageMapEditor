

using System;
using System.Collections.Generic;

namespace StageMapEditor.ViewModels
{
	public partial class StageViewModel
	{
        #region Modelラッパープロパティ
	
		public Int32 World
		{
			get { return _model.World; }
			set { _model.World = value; }
		}
			
		public Int32 Stage
		{
			get { return _model.Stage; }
			set { _model.Stage = value; }
		}
			
		public String StageName
		{
			get { return _model.StageName; }
			set { _model.StageName = value; }
		}
			
		public String StageDescription
		{
			get { return _model.StageDescription; }
			set { _model.StageDescription = value; }
		}
			
		#endregion
	}
}
