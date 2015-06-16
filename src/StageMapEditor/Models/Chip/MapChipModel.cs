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
using Livet.Commands;
using Livet.Behaviors;
using StageMapEditor.Helper;

namespace StageMapEditor.Models.Chip
{
    /// <summary>
    /// MapごとのMapChipの一覧
    /// </summary>
    public class MapChipModel : NotificationObject, IChipModel
    {
        private MapChip[,] _mapChipList;
        private MapModel _parent;
        private static MD5CryptoServiceProvider _md5Provider = new MD5CryptoServiceProvider();

        public MapChipModel(MapModel parent, int width, int height)
        {
            _parent = parent;
            _mapChipList = new MapChip[height, width];
            _mapChipList.Run((_, y, x) => _mapChipList.SetValue(new MapChip(), y, x));
        }

        public MapChipModel(MapModel parent, MapChipPack[] mapChipPack)
        {
            _parent = parent;
            _mapChipList = new MapChip[parent.MapCellHeight, parent.MapCellWidth];

            for (int i = 0; i < mapChipPack.Length; i++)
            {
                var y = i / parent.MapCellWidth;
                var x = i % parent.MapCellWidth;
                _mapChipList[y, x] = new MapChip(mapChipPack[i]);
            }
        }

        private static IEnumerable<IEnumerable<int>> ParseFile(string fileName)
        {
            var lines = File.ReadAllLines(fileName);

            var reg = new Regex(" +");

            //空行を無視してからスペースで分割
            var result = lines.Select(
                x => reg.Split(x)
                    .Where(a => !string.IsNullOrWhiteSpace(a))
                    .Select(int.Parse)
                    .ToList())
                .ToList();

            return result;
        }

        private static void Initialize(MapChip[,] mapChipList)
        {
            var d1 = mapChipList.GetLength(0);
            var d2 = mapChipList.GetLength(1);

            for (var i = 0; i < d1; i++)
            {
                for (int j = 0; j < d2; j++)
                {
                    mapChipList[i, j] = new MapChip();
                }
            }
        }

        private string[] ToStringArray()
        {
            var d1 = _mapChipList.GetLength(0);
            var d2 = _mapChipList.GetLength(1);

            var lines = new string[d1];
            for (int i = 0; i < d1; i++)
            {
                var line = new int[d2];
                for (int j = 0; j < d2; j++)
                {
                    line[j] = _mapChipList[i, j].ID;
                }
                lines[i] = string.Join(" ", line);
            }

            return lines;
        }

        public string GetMD5String()
        {
            return _md5Provider.ComputeHashString(this.ToString());
        }

        #region データアクセス部分

        public IEnumerable<Tuple<Point, MapChip>> ListAllWithPosition()
        {
            return _mapChipList.Select((m, y, x) => Tuple.Create(new Point(x, y), m));
        }

        public MapChip Get(int x, int y)
        {
            return _mapChipList[y, x];
        }

        public MapChip Get(Point p)
        {
            return Get(p.X, p.Y);
        }

        public void Set(int x, int y, MapChip mapChip)
        {
            Set(new Point(x, y), mapChip);
        }

        public void Set(Point point, MapChip mapChip)
        {
            if (Get(point).ID != mapChip.ID)
            {
                _mapChipList[point.Y, point.X] = mapChip;
                RaisePropertyChanged("MapChipModel");
            }
        }

        public void Delete(int x, int y)
        {
            Delete(new Point(x, y));
        }

        public void Delete(Point point)
        {
            if (Get(point).ID != 0)
            {
                Set(point, MapChip.Empty);
                RaisePropertyChanged("MapChipModel");
            }
        }
        #endregion

        /// <summary>
        /// WidthとHeightが変更されたときに更新
        /// 余った部分部分は切り捨て
        /// 足りない部分は初期化
        /// </summary>
        /// <param name="newWidth"></param>
        /// <param name="newHeight"></param>
        public void EditWidthHeight(int newWidth, int newHeight)
        {
            //変更がなかったら何もしない
            if (_mapChipList.GetLength(0) == newHeight && _mapChipList.GetLength(1) == newWidth)
            {
                return;
            }

            var newMapList = new MapChip[newHeight, newWidth];
            Initialize(newMapList);

            var h = _mapChipList.GetLength(0);
            var w = _mapChipList.GetLength(1);

            newMapList.Run((c, y, x) =>
            {
                if (h <= y || w <= x)
                {
                    c.Clear();
                }
                else
                {
                    newMapList[y, x] = _mapChipList[y, x].Clone();
                }

            });

            _mapChipList = newMapList;
        }

        public override string ToString()
        {
            return String.Join(Environment.NewLine, ToStringArray());
        }

        public MapChipPack[] ToMagPack()
        {
            return _mapChipList.Select(x => new MapChipPack { ID = x.ID }).ToArray();
        }

        public void FlipHorizontal()
        {
            var h = _mapChipList.GetLength(0);
            var w = _mapChipList.GetLength(1);

            var newMapList = new MapChip[h, w];
            Initialize(newMapList);

            newMapList.Run((c, y, x) =>
                               {
                                   if (x == 0 || y == 0 || x == w - 1 || y == h - 1)
                                   {
                                       newMapList[y, x] = _mapChipList[y, x].Clone();
                                   }
                                   else
                                   {
                                       var _x = Math.Abs(x - (w - 1));
                                       newMapList[y, _x] = _mapChipList[y, x].Clone();
                                   }
                               });

            _mapChipList = newMapList;
        }

        public int[,] GetArray2D()
        {
            var h = _mapChipList.GetLength(0);
            var w = _mapChipList.GetLength(1);

            var newMapList = new int[h, w];

            newMapList.Run((m, y, x) => newMapList[y, x] = _mapChipList[y, x].ID);

            return newMapList;
        }

        public void ReplaceByArray2D(int[,] newMapList)
        {
            _mapChipList.Run((m, y, x) => _mapChipList[y, x] = new MapChip(newMapList[y, x]));
        }
    }
}
