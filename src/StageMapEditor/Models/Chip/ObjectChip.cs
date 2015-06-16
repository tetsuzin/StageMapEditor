using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using MsgPack.Serialization;

namespace StageMapEditor.Models.Chip
{
    /// <summary>
    /// オブジェクトチップを表すクラス
    /// </summary>
    public partial class ObjectChip : INotifyPropertyChanged, IChip
    {
        /// <summary>
        /// デフォルト値が保存されている場所
        /// </summary>
        internal static Dictionary<int, ObjectChip> _originalDictionary;

        private ObjectChip Original
        {
            get
            {
                try { return _originalDictionary[ID]; }
                catch (Exception)
                {
                    throw new KeyNotFoundException(string.Format("{0} のIDを持つチップが登録されていません。", ID));
                }
            }
        }

        public bool IsEmpty { get { return ID == 0; } }

        public static ObjectChip Empty()
        {
            return new ObjectChip(0);
        }

        public static void SetOriginalDictionary(Dictionary<int, ObjectChip> paramDic)
        {
            _originalDictionary = paramDic;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged(null, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public string ToPositionString(System.Drawing.Point point)
        {
            return string.Format("{0} {1} {2}", point.X, point.Y, ToSaveString());
        }
    }
}