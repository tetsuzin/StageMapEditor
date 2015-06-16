using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media.Imaging;
using StageMapEditor.Helper;
using StageMapEditor.ViewModels;
using Point = System.Drawing.Point;

namespace StageMapEditor.Models.Chip
{
    public class ObjectChipLibrary
    {
        private ObjectChip[,] _objectChipList;
        private readonly int _cropSize;
        private BitmapSource _imageSource { get; set; }
        private IGeneralSettings _settings;

        public ObjectChipLibrary(IGeneralSettings settings)
        {
            _settings = settings;
            _cropSize = _settings.CellSize;
            LoadObjectChipList();
        }

        public int Length1 { get { return _objectChipList.GetLength(0); } }
        public int Length2 { get { return _objectChipList.GetLength(1); } }

        private void LoadObjectChipList()
        {
            var path = new FileInfo(Path.Combine(_settings.ResourceDataDirectory, _settings.ObjectChip));
            _imageSource = new BitmapImage(new Uri(path.FullName));

            var fi = new FileInfo(Path.Combine(_settings.ResourceDataDirectory, _settings.ObjectChipList));
            var pfi = new FileInfo(Path.Combine(_settings.ResourceDataDirectory, _settings.ObjectChipPrototype));

            var ip = new Func<string, int>(int.Parse);
            var fp = new Func<string, float>(float.Parse);

            var reg = new Regex(" ");

            //デフォルトパラメータのDictionary
            var objPrototype = File.ReadAllLines(pfi.FullName)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Where(x => !x.StartsWith("#"))
                .Select(x => reg.Split(x))
                .GroupBy(x => x[0])
                .Select(x => x.First())
                .Select(x => ObjectChip.Convert(x))
                .ToDictionary(x => x.ID);

            var lines = File.ReadAllLines(fi.FullName);
            var list = lines.Select(
                x => x.Split(' ')
                    .Select(int.Parse)
                    .Select(id =>
                        //プロトタイプが存在したらプロトタイプからコピー
                        objPrototype.ContainsKey(id) ?
                        objPrototype[id].Clone() :
                        new ObjectChip(id)
                    ).ToArray()).ToArray();

            //デフォルト値を記憶
            ObjectChip.SetOriginalDictionary(list.SelectMany(_ => _).ToDictionary(x => x.ID, x => x));

            int d1 = list.Count(),
                d2 = list.ElementAt(0).Count();

            _objectChipList = new ObjectChip[d1, d2];

            for (int i = 0; i < d1; i++)
            {
                for (int j = 0; j < d2; j++)
                {
                    _objectChipList[i, j] = list[i][j];
                }
            }
        }

        public CroppedBitmap GetBitMap(int id)
        {
            var p = GetPosition(id);
            var bitmap = new CroppedBitmap();
            bitmap.BeginInit();
            bitmap.Source = _imageSource;
            bitmap.SourceRect = new Int32Rect(p.X * _cropSize, p.Y * _cropSize, _cropSize, _cropSize);
            bitmap.EndInit();
            return bitmap;
        }

        public ObjectChip GetByID(int id)
        {
            return GetEnumerator().First(x => x.ID == id);
        }

        public IEnumerable<ObjectChip> GetEnumerator()
        {
            return _objectChipList.Select(m => m);
        }

        public IEnumerable<Tuple<ObjectChip, int, int>> GetEnumeratorWithPosition()
        {
            return _objectChipList.Select((m, y, x) => Tuple.Create(m, y, x));
        }

        public Point GetPosition(int id)
        {
            return _objectChipList.Select((m, y, x) => Tuple.Create(new Point(x, y), m)).First(x => x.Item2.ID == id).Item1;
        }

        public ObjectChip GetChip(int x, int y)
        {
            return _objectChipList[y, x];
        }

        public ObjectChip GetChip(Point select)
        {
            return _objectChipList[select.Y, select.X];
        }
    }
}
