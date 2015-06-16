using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;

namespace StageMapEditor.Models.Chip
{
    /// <summary>
    /// マップチップを表すクラス
    /// </summary>
    [DebuggerDisplay("ID = {ID}")]
    public class MapChip : INotifyPropertyChanged, IChip, IMapChip
    {
        /// <summary>
        /// マップチップのID
        /// </summary>
        public int ID
        {
            get { return _id; }
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged("ID");
                }
            }
        }
        private int _id;

        public static MapChip Empty { get { return new MapChip(); } }

        public MapChip()
        {
            ID = 0;
        }

        public MapChip(int id)
        {
            ID = id;
        }

        public MapChip(MapChipPack mapChipPack)
        {
            ID = mapChipPack.ID;
        }

        public void Set(int id)
        {
            ID = id;
        }

        public MapChip Clone()
        {
            return new MapChip(ID);
        }

        public void Clear()
        {
            ID = 0;
        }

        public Point Position { get; set; }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged(null, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}