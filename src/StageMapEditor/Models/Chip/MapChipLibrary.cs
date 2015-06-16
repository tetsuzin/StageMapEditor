using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using StageMapEditor.Helper;
using StageMapEditor.Models.Chip;
using StageMapEditor.ViewModels;
using Point = System.Drawing.Point;

namespace StageMapEditor.Models.Chip
{
    public class MapChipLibrary
    {
        private readonly int _cropSize;
        private MapChip[,] _mapChipList;
        private BitmapSource _imageSource;
        private IGeneralSettings _settings;

        public MapChipLibrary(IGeneralSettings settings)
        {
            _settings = settings;
            _cropSize = _settings.CellSize;
            LoadMapChipList();
        }

        public int Length1 { get { return _mapChipList.GetLength(0); } }
        public int Length2 { get { return _mapChipList.GetLength(1); } }

        private void LoadMapChipList()
        {
            var path = new DirectoryInfo(Path.Combine(_settings.ResourceDataDirectory, _settings.MapChip));
            _imageSource = new BitmapImage(new Uri(path.FullName));

            var fi = new FileInfo(Path.Combine(_settings.ResourceDataDirectory, _settings.MapChipList));
            var lines = File.ReadAllLines(fi.FullName);
            var list = lines.Select(x => x.Split(' ').Select(int.Parse).Select(a => new MapChip(a)).ToArray()).ToArray();

            int d1 = list.Count(),
                d2 = list.ElementAt(0).Count();

            _mapChipList = new MapChip[d1, d2];

            for (int i = 0; i < d1; i++)
            {
                for (int j = 0; j < d2; j++)
                {
                    _mapChipList[i, j] = list[i][j];
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

        public IEnumerable<MapChip> GetEnumerator()
        {
            return _mapChipList.Select(m => m);
        }

        public IEnumerable<Tuple<MapChip, int, int>> GetEnumeratorWithPosition()
        {
            return _mapChipList.Select((m, y, x) => Tuple.Create(m, y, x));
        }

        public Point GetPosition(int id)
        {
            return _mapChipList.Select((m, y, x) => Tuple.Create(new Point(x, y), m)).First(x => x.Item2.ID == id).Item1;
        }

        public MapChip GetByID(int id)
        {
            return GetEnumerator().First(x => x.ID == id);
        }

        public MapChip GetChip(int x, int y)
        {
            return _mapChipList[y, x];
        }

        public MapChip GetChip(Point select)
        {
            return _mapChipList[select.Y, select.X];
        }

        public MapChip GetChip(MousePosition.MousePosition select)
        {
            return _mapChipList[select.GridPoint.Y, select.GridPoint.X];
        }
    }
}