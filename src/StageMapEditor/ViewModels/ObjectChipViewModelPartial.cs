using System;
using System.Collections.Generic;
using System.Linq;

namespace StageMapEditor.ViewModels
{
	public partial class ObjectChipViewModel
	{
		// private string _head = "#PosX,PosY,ID,Status,Param,SubParam1,SubParam2,Trigger1,Action1,Trigger2,Action2,Trigger3,Action3,Item1,Item2,Item3,Item4,Item5";

        #region Modelラッパープロパティ
	
		public Int32 ID { get { return _model.ID; } set { _model.ID = value; } }
		public Single Status { get { return _model.Status; } set { _model.Status = value; } }
		public Single Param { get { return _model.Param; } set { _model.Param = value; } }
		public Single SubParam1 { get { return _model.SubParam1; } set { _model.SubParam1 = value; } }
		public Single SubParam2 { get { return _model.SubParam2; } set { _model.SubParam2 = value; } }
		public Int32 Trigger1 { get { return _model.Trigger1; } set { _model.Trigger1 = value; } }
		public Int32 Action1 { get { return _model.Action1; } set { _model.Action1 = value; } }
		public Int32 Trigger2 { get { return _model.Trigger2; } set { _model.Trigger2 = value; } }
		public Int32 Action2 { get { return _model.Action2; } set { _model.Action2 = value; } }
		public Int32 Trigger3 { get { return _model.Trigger3; } set { _model.Trigger3 = value; } }
		public Int32 Action3 { get { return _model.Action3; } set { _model.Action3 = value; } }
		public Int32 Item1 { get { return _model.Item1; } set { _model.Item1 = value; } }
		public Int32 Item2 { get { return _model.Item2; } set { _model.Item2 = value; } }
		public Int32 Item3 { get { return _model.Item3; } set { _model.Item3 = value; } }
		public Int32 Item4 { get { return _model.Item4; } set { _model.Item4 = value; } }
		public Int32 Item5 { get { return _model.Item5; } set { _model.Item5 = value; } }
		#endregion

        #region 入力用プロパティ

		private string _InputID;
		private string _InputStatus;
		private string _InputParam;
		private string _InputSubParam1;
		private string _InputSubParam2;
		private string _InputTrigger1;
		private string _InputAction1;
		private string _InputTrigger2;
		private string _InputAction2;
		private string _InputTrigger3;
		private string _InputAction3;
		private string _InputItem1;
		private string _InputItem2;
		private string _InputItem3;
		private string _InputItem4;
		private string _InputItem5;

		public string InputID
		{
			get { return _InputID; }
			set
			{
				_InputID = value;
				var valid = true;
				Int32 _val;
				valid = Int32.TryParse(value, out _val);
				if(string.IsNullOrEmpty(value) || !valid)
				{
					this["ID"] = "IDに正しい値が入力されていません";
				}
				else
				{
					this["ID"] = null;
				}
			}
		}
			
		public string InputStatus
		{
			get { return _InputStatus; }
			set
			{
				_InputStatus = value;
				var valid = true;
				Single _val;
				valid = Single.TryParse(value, out _val);
				if(string.IsNullOrEmpty(value) || !valid)
				{
					this["Status"] = "Statusに正しい値が入力されていません";
				}
				else
				{
					this["Status"] = null;
				}
			}
		}
			
		public string InputParam
		{
			get { return _InputParam; }
			set
			{
				_InputParam = value;
				var valid = true;
				Single _val;
				valid = Single.TryParse(value, out _val);
				if(string.IsNullOrEmpty(value) || !valid)
				{
					this["Param"] = "Paramに正しい値が入力されていません";
				}
				else
				{
					this["Param"] = null;
				}
			}
		}
			
		public string InputSubParam1
		{
			get { return _InputSubParam1; }
			set
			{
				_InputSubParam1 = value;
				var valid = true;
				Single _val;
				valid = Single.TryParse(value, out _val);
				if(string.IsNullOrEmpty(value) || !valid)
				{
					this["SubParam1"] = "SubParam1に正しい値が入力されていません";
				}
				else
				{
					this["SubParam1"] = null;
				}
			}
		}
			
		public string InputSubParam2
		{
			get { return _InputSubParam2; }
			set
			{
				_InputSubParam2 = value;
				var valid = true;
				Single _val;
				valid = Single.TryParse(value, out _val);
				if(string.IsNullOrEmpty(value) || !valid)
				{
					this["SubParam2"] = "SubParam2に正しい値が入力されていません";
				}
				else
				{
					this["SubParam2"] = null;
				}
			}
		}
			
		public string InputTrigger1
		{
			get { return _InputTrigger1; }
			set
			{
				_InputTrigger1 = value;
				var valid = true;
				Int32 _val;
				valid = Int32.TryParse(value, out _val);
				if(string.IsNullOrEmpty(value) || !valid)
				{
					this["Trigger1"] = "Trigger1に正しい値が入力されていません";
				}
				else
				{
					this["Trigger1"] = null;
				}
			}
		}
			
		public string InputAction1
		{
			get { return _InputAction1; }
			set
			{
				_InputAction1 = value;
				var valid = true;
				Int32 _val;
				valid = Int32.TryParse(value, out _val);
				if(string.IsNullOrEmpty(value) || !valid)
				{
					this["Action1"] = "Action1に正しい値が入力されていません";
				}
				else
				{
					this["Action1"] = null;
				}
			}
		}
			
		public string InputTrigger2
		{
			get { return _InputTrigger2; }
			set
			{
				_InputTrigger2 = value;
				var valid = true;
				Int32 _val;
				valid = Int32.TryParse(value, out _val);
				if(string.IsNullOrEmpty(value) || !valid)
				{
					this["Trigger2"] = "Trigger2に正しい値が入力されていません";
				}
				else
				{
					this["Trigger2"] = null;
				}
			}
		}
			
		public string InputAction2
		{
			get { return _InputAction2; }
			set
			{
				_InputAction2 = value;
				var valid = true;
				Int32 _val;
				valid = Int32.TryParse(value, out _val);
				if(string.IsNullOrEmpty(value) || !valid)
				{
					this["Action2"] = "Action2に正しい値が入力されていません";
				}
				else
				{
					this["Action2"] = null;
				}
			}
		}
			
		public string InputTrigger3
		{
			get { return _InputTrigger3; }
			set
			{
				_InputTrigger3 = value;
				var valid = true;
				Int32 _val;
				valid = Int32.TryParse(value, out _val);
				if(string.IsNullOrEmpty(value) || !valid)
				{
					this["Trigger3"] = "Trigger3に正しい値が入力されていません";
				}
				else
				{
					this["Trigger3"] = null;
				}
			}
		}
			
		public string InputAction3
		{
			get { return _InputAction3; }
			set
			{
				_InputAction3 = value;
				var valid = true;
				Int32 _val;
				valid = Int32.TryParse(value, out _val);
				if(string.IsNullOrEmpty(value) || !valid)
				{
					this["Action3"] = "Action3に正しい値が入力されていません";
				}
				else
				{
					this["Action3"] = null;
				}
			}
		}
			
		public string InputItem1
		{
			get { return _InputItem1; }
			set
			{
				_InputItem1 = value;
				var valid = true;
				Int32 _val;
				valid = Int32.TryParse(value, out _val);
				if(string.IsNullOrEmpty(value) || !valid)
				{
					this["Item1"] = "Item1に正しい値が入力されていません";
				}
				else
				{
					this["Item1"] = null;
				}
			}
		}
			
		public string InputItem2
		{
			get { return _InputItem2; }
			set
			{
				_InputItem2 = value;
				var valid = true;
				Int32 _val;
				valid = Int32.TryParse(value, out _val);
				if(string.IsNullOrEmpty(value) || !valid)
				{
					this["Item2"] = "Item2に正しい値が入力されていません";
				}
				else
				{
					this["Item2"] = null;
				}
			}
		}
			
		public string InputItem3
		{
			get { return _InputItem3; }
			set
			{
				_InputItem3 = value;
				var valid = true;
				Int32 _val;
				valid = Int32.TryParse(value, out _val);
				if(string.IsNullOrEmpty(value) || !valid)
				{
					this["Item3"] = "Item3に正しい値が入力されていません";
				}
				else
				{
					this["Item3"] = null;
				}
			}
		}
			
		public string InputItem4
		{
			get { return _InputItem4; }
			set
			{
				_InputItem4 = value;
				var valid = true;
				Int32 _val;
				valid = Int32.TryParse(value, out _val);
				if(string.IsNullOrEmpty(value) || !valid)
				{
					this["Item4"] = "Item4に正しい値が入力されていません";
				}
				else
				{
					this["Item4"] = null;
				}
			}
		}
			
		public string InputItem5
		{
			get { return _InputItem5; }
			set
			{
				_InputItem5 = value;
				var valid = true;
				Int32 _val;
				valid = Int32.TryParse(value, out _val);
				if(string.IsNullOrEmpty(value) || !valid)
				{
					this["Item5"] = "Item5に正しい値が入力されていません";
				}
				else
				{
					this["Item5"] = null;
				}
			}
		}
			
		#endregion

		private List<string> _ErrorList()
		{
            var errorPropertyList = new List<string>();
            if (!string.IsNullOrEmpty(this["InputID"])) { errorPropertyList.Add("InputID"); }
            if (!string.IsNullOrEmpty(this["InputStatus"])) { errorPropertyList.Add("InputStatus"); }
            if (!string.IsNullOrEmpty(this["InputParam"])) { errorPropertyList.Add("InputParam"); }
            if (!string.IsNullOrEmpty(this["InputSubParam1"])) { errorPropertyList.Add("InputSubParam1"); }
            if (!string.IsNullOrEmpty(this["InputSubParam2"])) { errorPropertyList.Add("InputSubParam2"); }
            if (!string.IsNullOrEmpty(this["InputTrigger1"])) { errorPropertyList.Add("InputTrigger1"); }
            if (!string.IsNullOrEmpty(this["InputAction1"])) { errorPropertyList.Add("InputAction1"); }
            if (!string.IsNullOrEmpty(this["InputTrigger2"])) { errorPropertyList.Add("InputTrigger2"); }
            if (!string.IsNullOrEmpty(this["InputAction2"])) { errorPropertyList.Add("InputAction2"); }
            if (!string.IsNullOrEmpty(this["InputTrigger3"])) { errorPropertyList.Add("InputTrigger3"); }
            if (!string.IsNullOrEmpty(this["InputAction3"])) { errorPropertyList.Add("InputAction3"); }
            if (!string.IsNullOrEmpty(this["InputItem1"])) { errorPropertyList.Add("InputItem1"); }
            if (!string.IsNullOrEmpty(this["InputItem2"])) { errorPropertyList.Add("InputItem2"); }
            if (!string.IsNullOrEmpty(this["InputItem3"])) { errorPropertyList.Add("InputItem3"); }
            if (!string.IsNullOrEmpty(this["InputItem4"])) { errorPropertyList.Add("InputItem4"); }
            if (!string.IsNullOrEmpty(this["InputItem5"])) { errorPropertyList.Add("InputItem5"); }
		
			return errorPropertyList;
		}

        private bool TryParse(string input, Type type)
        {
            var mi = type.GetMethods().FirstOrDefault(x => x.Name == "TryParse" && x.GetParameters().Length == 2);
            if (mi == null) { return true; }
			return (bool)mi.Invoke(null, new object[] { input, null });
        }

		private bool _CanSave()
		{
			if (!TryParse(InputID, typeof(Int32))) { return false; }
			if (!TryParse(InputStatus, typeof(Single))) { return false; }
			if (!TryParse(InputParam, typeof(Single))) { return false; }
			if (!TryParse(InputSubParam1, typeof(Single))) { return false; }
			if (!TryParse(InputSubParam2, typeof(Single))) { return false; }
			if (!TryParse(InputTrigger1, typeof(Int32))) { return false; }
			if (!TryParse(InputAction1, typeof(Int32))) { return false; }
			if (!TryParse(InputTrigger2, typeof(Int32))) { return false; }
			if (!TryParse(InputAction2, typeof(Int32))) { return false; }
			if (!TryParse(InputTrigger3, typeof(Int32))) { return false; }
			if (!TryParse(InputAction3, typeof(Int32))) { return false; }
			if (!TryParse(InputItem1, typeof(Int32))) { return false; }
			if (!TryParse(InputItem2, typeof(Int32))) { return false; }
			if (!TryParse(InputItem3, typeof(Int32))) { return false; }
			if (!TryParse(InputItem4, typeof(Int32))) { return false; }
			if (!TryParse(InputItem5, typeof(Int32))) { return false; }

			return true;
		}

		private void _UpdateByInput()
		{
			ID = Int32.Parse(InputID);
			Status = Single.Parse(InputStatus);
			Param = Single.Parse(InputParam);
			SubParam1 = Single.Parse(InputSubParam1);
			SubParam2 = Single.Parse(InputSubParam2);
			Trigger1 = Int32.Parse(InputTrigger1);
			Action1 = Int32.Parse(InputAction1);
			Trigger2 = Int32.Parse(InputTrigger2);
			Action2 = Int32.Parse(InputAction2);
			Trigger3 = Int32.Parse(InputTrigger3);
			Action3 = Int32.Parse(InputAction3);
			Item1 = Int32.Parse(InputItem1);
			Item2 = Int32.Parse(InputItem2);
			Item3 = Int32.Parse(InputItem3);
			Item4 = Int32.Parse(InputItem4);
			Item5 = Int32.Parse(InputItem5);
		}

		private void _InitializeInput()
		{
			InputID = _model.ID.ToString();
			InputStatus = _model.Status.ToString();
			InputParam = _model.Param.ToString();
			InputSubParam1 = _model.SubParam1.ToString();
			InputSubParam2 = _model.SubParam2.ToString();
			InputTrigger1 = _model.Trigger1.ToString();
			InputAction1 = _model.Action1.ToString();
			InputTrigger2 = _model.Trigger2.ToString();
			InputAction2 = _model.Action2.ToString();
			InputTrigger3 = _model.Trigger3.ToString();
			InputAction3 = _model.Action3.ToString();
			InputItem1 = _model.Item1.ToString();
			InputItem2 = _model.Item2.ToString();
			InputItem3 = _model.Item3.ToString();
			InputItem4 = _model.Item4.ToString();
			InputItem5 = _model.Item5.ToString();
		}
	}
}