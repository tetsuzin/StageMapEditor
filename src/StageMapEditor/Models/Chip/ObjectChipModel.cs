using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Livet;
using StageMapEditor.Helper;

namespace StageMapEditor.Models.Chip
{
    /// <summary>
    /// MapごとのObjectChip一覧
    /// </summary>
    public class ObjectChipModel : NotificationObject, IChipModel
    {
        //private string _filepath;
        //private string RecentlyFilePath { get { return _parent.ObjectChipFileName; } }

        private Dictionary<Point, ObjectChip> _objectData;
        private MapModel _parent;

        private static MD5CryptoServiceProvider _md5Provider = new MD5CryptoServiceProvider();

        public ObjectChipModel(MapModel parent)
        {
            _parent = parent;
            _objectData = new Dictionary<Point, ObjectChip>();
        }

        public ObjectChipModel(MapModel parent, ObjectChipPack[] objectChipPack)
        {
            _parent = parent;
            _objectData = new Dictionary<Point, ObjectChip>();

            _objectData = objectChipPack.ToDictionary(x => new Point(x.X, x.Y), x => new ObjectChip(x));
        }

        #region データアクセス部分

        public IEnumerable<Tuple<Point, ObjectChip>> ListAllWithPosition()
        {
            return _objectData.Select(x => Tuple.Create(x.Key, x.Value));
        }

        public ObjectChip Get(Point p)
        {
            if (!_objectData.ContainsKey(p))
            {
                return ObjectChip.Empty();
            }

            return _objectData[p];
        }

        public ObjectChip Get(int x, int y)
        {
            return Get(new Point(x, y));
        }

        public IEnumerable<KeyValuePair<Point, ObjectChip>> ListAll()
        {
            return _objectData.Select(x => new KeyValuePair<Point, ObjectChip>(x.Key, x.Value));
        }

        public void Set(int x, int y, ObjectChip objectChip)
        {
            Set(new Point(x, y), objectChip);
        }

        public void Set(Point point, ObjectChip objectChip)
        {
            _objectData[point] = objectChip.Clone();
            RaisePropertyChanged("ObjectChipModel");
        }

        public void Delete(int x, int y)
        {
            Delete(new Point(x, y));
        }

        public void Delete(Point point)
        {
            if (_objectData.ContainsKey(point))
            {
                _objectData.Remove(point);
                RaisePropertyChanged("ObjectChipModel");
            }
        }
        #endregion

        /// <summary>
        /// 幅と高さ更新されたときにはみ出た部分を削除する
        /// </summary>
        /// <param name="newWidth"></param>
        /// <param name="newHeight"></param>
        public void EditWidthHeight(int newWidth, int newHeight)
        {
            var deleteList = _objectData.Where(o =>
            {
                var p = o.Key;
                return (p.X >= newWidth || p.Y >= newHeight);
            })
            .Select(x => x.Key)
            .ToList();

            deleteList.ForEach(x => _objectData.Remove(x));
        }

        public string GetMD5String()
        {
            return _md5Provider.ComputeHashString(this.ToString());
        }

        public override string ToString()
        {
            return string.Join(Environment.NewLine, _objectData.Select(ConvertKeyValuePair));
        }

        private string ConvertKeyValuePair(KeyValuePair<Point, ObjectChip> kv)
        {
            return kv.Value.ToPositionString(kv.Key);
        }

        public ObjectChipPack[] ToMsgPack()
        {
            return _objectData.Select(x => new ObjectChipPack()
                {
                    X = x.Key.X,
                    Y = x.Key.Y,
                    ID = x.Value.ID,
                    Status = x.Value.Status,
                    Param = x.Value.Param,
                    SubParam1 = x.Value.SubParam1,
                    SubParam2 = x.Value.SubParam2,
                    Trigger1 = x.Value.Trigger1,
                    Action1 = x.Value.Action1,
                    Trigger2 = x.Value.Trigger2,
                    Action2 = x.Value.Action2,
                    Trigger3 = x.Value.Trigger3,
                    Action3 = x.Value.Action3,
                    Item1 = x.Value.Item1,
                    Item2 = x.Value.Item2,
                    Item3 = x.Value.Item3,
                    Item4 = x.Value.Item4,
                    Item5 = x.Value.Item5
                }).ToArray();
        }

        public void FlipHorizontal()
        {
            var width = _parent.MapCellWidth;

            var newObjData =
                _objectData.Select(x => Tuple.Create(new Point(Math.Abs(x.Key.X - (width - 1)), x.Key.Y), x.Value))
                           .ToDictionary(x => x.Item1, x => x.Item2);

            _objectData = newObjData;
        }
    }
}
