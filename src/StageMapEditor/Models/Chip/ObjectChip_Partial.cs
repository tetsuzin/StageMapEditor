

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace StageMapEditor.Models.Chip
{

    [DebuggerDisplay("ID = {ID}, Status = {Status}, Param = {Param}, SubParam1 = {SubParam1}, SubParam2 = {SubParam2}, Trigger1 = {Trigger1}, Action1 = {Action1}, Trigger2 = {Trigger2}, Action2 = {Action2}, Trigger3 = {Trigger3}, Action3 = {Action3}, Item1 = {Item1}, Item2 = {Item2}, Item3 = {Item3}, Item4 = {Item4}, Item5 = {Item5}")]
    public partial class ObjectChip
	{
        private System.Int32 _id;
        public System.Int32 ID
        {
            get { return _id; }
            set { if (_id != value) _id = value; OnPropertyChanged("ID"); }
        }

        private System.Single _status;
        public System.Single Status
        {
            get { return _status; }
            set { if (_status != value) _status = value; OnPropertyChanged("Status"); }
        }

        private System.Single _param;
        public System.Single Param
        {
            get { return _param; }
            set { if (_param != value) _param = value; OnPropertyChanged("Param"); }
        }

        private System.Single _subparam1;
        public System.Single SubParam1
        {
            get { return _subparam1; }
            set { if (_subparam1 != value) _subparam1 = value; OnPropertyChanged("SubParam1"); }
        }

        private System.Single _subparam2;
        public System.Single SubParam2
        {
            get { return _subparam2; }
            set { if (_subparam2 != value) _subparam2 = value; OnPropertyChanged("SubParam2"); }
        }

        private System.Int32 _trigger1;
        public System.Int32 Trigger1
        {
            get { return _trigger1; }
            set { if (_trigger1 != value) _trigger1 = value; OnPropertyChanged("Trigger1"); }
        }

        private System.Int32 _action1;
        public System.Int32 Action1
        {
            get { return _action1; }
            set { if (_action1 != value) _action1 = value; OnPropertyChanged("Action1"); }
        }

        private System.Int32 _trigger2;
        public System.Int32 Trigger2
        {
            get { return _trigger2; }
            set { if (_trigger2 != value) _trigger2 = value; OnPropertyChanged("Trigger2"); }
        }

        private System.Int32 _action2;
        public System.Int32 Action2
        {
            get { return _action2; }
            set { if (_action2 != value) _action2 = value; OnPropertyChanged("Action2"); }
        }

        private System.Int32 _trigger3;
        public System.Int32 Trigger3
        {
            get { return _trigger3; }
            set { if (_trigger3 != value) _trigger3 = value; OnPropertyChanged("Trigger3"); }
        }

        private System.Int32 _action3;
        public System.Int32 Action3
        {
            get { return _action3; }
            set { if (_action3 != value) _action3 = value; OnPropertyChanged("Action3"); }
        }

        private System.Int32 _item1;
        public System.Int32 Item1
        {
            get { return _item1; }
            set { if (_item1 != value) _item1 = value; OnPropertyChanged("Item1"); }
        }

        private System.Int32 _item2;
        public System.Int32 Item2
        {
            get { return _item2; }
            set { if (_item2 != value) _item2 = value; OnPropertyChanged("Item2"); }
        }

        private System.Int32 _item3;
        public System.Int32 Item3
        {
            get { return _item3; }
            set { if (_item3 != value) _item3 = value; OnPropertyChanged("Item3"); }
        }

        private System.Int32 _item4;
        public System.Int32 Item4
        {
            get { return _item4; }
            set { if (_item4 != value) _item4 = value; OnPropertyChanged("Item4"); }
        }

        private System.Int32 _item5;
        public System.Int32 Item5
        {
            get { return _item5; }
            set { if (_item5 != value) _item5 = value; OnPropertyChanged("Item5"); }
        }


        #region コンストラクタ
        public ObjectChip(int id)
        {
		    Set(id);
        }

        public ObjectChip(Int32 id, Single status, Single param, Single subparam1, Single subparam2, Int32 trigger1, Int32 action1, Int32 trigger2, Int32 action2, Int32 trigger3, Int32 action3, Int32 item1, Int32 item2, Int32 item3, Int32 item4, Int32 item5)
        {
		    Set(id, status, param, subparam1, subparam2, trigger1, action1, trigger2, action2, trigger3, action3, item1, item2, item3, item4, item5);
        }

        public ObjectChip(ObjectChipPack objectChipPack)
        {
            ID = objectChipPack.ID;
            Status = objectChipPack.Status;
            Param = objectChipPack.Param;
            SubParam1 = objectChipPack.SubParam1;
            SubParam2 = objectChipPack.SubParam2;
            Trigger1 = objectChipPack.Trigger1;
            Action1 = objectChipPack.Action1;
            Trigger2 = objectChipPack.Trigger2;
            Action2 = objectChipPack.Action2;
            Trigger3 = objectChipPack.Trigger3;
            Action3 = objectChipPack.Action3;
            Item1 = objectChipPack.Item1;
            Item2 = objectChipPack.Item2;
            Item3 = objectChipPack.Item3;
            Item4 = objectChipPack.Item4;
            Item5 = objectChipPack.Item5;
        }
		#endregion

		//ID以外は初期値でセット
        public void Set(int id)
        {
            ID = default(Int32);
            Status = default(Single);
            Param = default(Single);
            SubParam1 = default(Single);
            SubParam2 = default(Single);
            Trigger1 = default(Int32);
            Action1 = default(Int32);
            Trigger2 = default(Int32);
            Action2 = default(Int32);
            Trigger3 = default(Int32);
            Action3 = default(Int32);
            Item1 = default(Int32);
            Item2 = default(Int32);
            Item3 = default(Int32);
            Item4 = default(Int32);
            Item5 = default(Int32);

			ID = id;
        }

        public void Set(Int32 id, Single status, Single param, Single subparam1, Single subparam2, Int32 trigger1, Int32 action1, Int32 trigger2, Int32 action2, Int32 trigger3, Int32 action3, Int32 item1, Int32 item2, Int32 item3, Int32 item4, Int32 item5)
        {
            ID = id;
            Status = status;
            Param = param;
            SubParam1 = subparam1;
            SubParam2 = subparam2;
            Trigger1 = trigger1;
            Action1 = action1;
            Trigger2 = trigger2;
            Action2 = action2;
            Trigger3 = trigger3;
            Action3 = action3;
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
            Item4 = item4;
            Item5 = item5;
        }

        public void SetOriginal(int id)
        {
            ID = Original.ID;
            Status = Original.Status;
            Param = Original.Param;
            SubParam1 = Original.SubParam1;
            SubParam2 = Original.SubParam2;
            Trigger1 = Original.Trigger1;
            Action1 = Original.Action1;
            Trigger2 = Original.Trigger2;
            Action2 = Original.Action2;
            Trigger3 = Original.Trigger3;
            Action3 = Original.Action3;
            Item1 = Original.Item1;
            Item2 = Original.Item2;
            Item3 = Original.Item3;
            Item4 = Original.Item4;
            Item5 = Original.Item5;
        }

		//新しいObjectChipインスタンスを作成する
        public ObjectChip Clone()
        {
            return new ObjectChip(ID, Status, Param, SubParam1, SubParam2, Trigger1, Action1, Trigger2, Action2, Trigger3, Action3, Item1, Item2, Item3, Item4, Item5);
        }

		//値を初期値に戻す
        public void Clear()
        {
            Set(Original.ID, Original.Status, Original.Param, Original.SubParam1, Original.SubParam2, Original.Trigger1, Original.Action1, Original.Trigger2, Original.Action2, Original.Trigger3, Original.Action3, Original.Item1, Original.Item2, Original.Item3, Original.Item4, Original.Item5);
        }

		//引数のインスタンスと同じパラメータかどうか
		public bool IsSame(ObjectChip target)
		{
		    return ID == target.ID && Status == target.Status && Param == target.Param && SubParam1 == target.SubParam1 && SubParam2 == target.SubParam2 && Trigger1 == target.Trigger1 && Action1 == target.Action1 && Trigger2 == target.Trigger2 && Action2 == target.Action2 && Trigger3 == target.Trigger3 && Action3 == target.Action3 && Item1 == target.Item1 && Item2 == target.Item2 && Item3 == target.Item3 && Item4 == target.Item4 && Item5 == target.Item5;
		}
		

        /// <summary>
        /// ファイル出力用にスペース区切りの文字列を作成
        /// </summary>
        /// <returns></returns>
		public string ToSaveString()
		{
		    return string.Join( " ", new [] { ID, Status, Param, SubParam1, SubParam2, Trigger1, Action1, Trigger2, Action2, Trigger3, Action3, Item1, Item2, Item3, Item4, Item5 } );
		}

        public bool IsEdited
        {
            get
            {
                if (IsEmpty) { return false; }
                return !(ID == Original.ID && Status == Original.Status && Param == Original.Param && SubParam1 == Original.SubParam1 && SubParam2 == Original.SubParam2 && Trigger1 == Original.Trigger1 && Action1 == Original.Action1 && Trigger2 == Original.Trigger2 && Action2 == Original.Action2 && Trigger3 == Original.Trigger3 && Action3 == Original.Action3 && Item1 == Original.Item1 && Item2 == Original.Item2 && Item3 == Original.Item3 && Item4 == Original.Item4 && Item5 == Original.Item5);
            }
        }

		public static ObjectChip Convert(string[] line)
		{
            var p0 = Int32.Parse(line[0]);
            var p1 = Single.Parse(line[1]);
            var p2 = Single.Parse(line[2]);
            var p3 = Single.Parse(line[3]);
            var p4 = Single.Parse(line[4]);
            var p5 = Int32.Parse(line[5]);
            var p6 = Int32.Parse(line[6]);
            var p7 = Int32.Parse(line[7]);
            var p8 = Int32.Parse(line[8]);
            var p9 = Int32.Parse(line[9]);
            var p10 = Int32.Parse(line[10]);
            var p11 = Int32.Parse(line[11]);
            var p12 = Int32.Parse(line[12]);
            var p13 = Int32.Parse(line[13]);
            var p14 = Int32.Parse(line[14]);
            var p15 = Int32.Parse(line[15]);

            return new ObjectChip(p0, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15);
		}
    }
}