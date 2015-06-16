using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using StageMapEditor.Helper;
using StageMapEditor.Models.Chip;

namespace StageMapEditor.Models.Commands
{
    public class Clipboard
    {
        private MapChip[,] _mapChip;
        private ObjectChip[,] _objectChip;

        /// <summary>
        /// クリップボードが空であるかどうか
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return _mapChip.Length == 0 && _objectChip.Length == 0;
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Clipboard()
        {
            Clear();
        }

        /// <summary>
        /// クリップボードのクリア
        /// </summary>
        public void Clear()
        {
            _mapChip = new MapChip[,] { };
            _objectChip = new ObjectChip[,] { };
        }

        /// <summary>
        /// Modelからクリップボードに選択範囲をコピーする
        /// </summary>
        /// <param name="mapModel"></param>
        public void Copy(MapModel mapModel)
        {
            var mapSelect = mapModel.MapSelect;

            _mapChip = new MapChip[mapSelect.Height, mapSelect.Width];
            _objectChip = new ObjectChip[mapSelect.Height, mapSelect.Width];

            foreach (var p in mapSelect.GetSelectPointArray())
            {
                var x = p.X - mapSelect.StartX;
                var y = p.Y - mapSelect.StartY;
                _mapChip[y, x] = mapModel.MapChipModel.Get(p).Clone();
                _objectChip[y, x] = mapModel.ObjectChipModel.Get(p).Clone();
            }
        }


        /// <summary>
        /// Modelからクリップボードに選択範囲をカットする
        /// </summary>
        /// <param name="mapModel"></param>
        public void Cut(MapModel mapModel)
        {
            var mapSelect = mapModel.MapSelect;

            _mapChip = new MapChip[mapSelect.Height, mapSelect.Width];
            _objectChip = new ObjectChip[mapSelect.Height, mapSelect.Width];

            foreach (var p in mapSelect.GetSelectPointArray())
            {
                var x = p.X - mapSelect.StartX;
                var y = p.Y - mapSelect.StartY;
                _mapChip[y, x] = mapModel.MapChipModel.Get(p).Clone();
                _objectChip[y, x] = mapModel.ObjectChipModel.Get(p).Clone();
                mapModel.MapChipModel.Set(p, MapChip.Empty);
                mapModel.ObjectChipModel.Delete(p);
            }
        }

        public void Paste(MapModel mapModel)
        {
            var mapSelect = mapModel.MapSelect;

            if (IsEmpty || mapSelect.IsEmpty)
            {
                return;
            }

            _mapChip.Run(
                (m, y, x) =>
                    {
                        x = x + mapSelect.StartX;
                        y = y + mapSelect.StartY;
                        if (x < mapModel.MapCellWidth && y < mapModel.MapCellHeight)
                        {
                            mapModel.MapChipModel.Set(x, y, (MapChip)m.Clone());
                        }
                    });

            _objectChip.Run(
                (o, y, x) =>
                    {
                        x = x + mapSelect.StartX;
                        y = y + mapSelect.StartY;
                        if (x < mapModel.MapCellWidth && y < mapModel.MapCellHeight)
                        {
                            mapModel.ObjectChipModel.Set(x, y, (ObjectChip)o.Clone());
                        }
                    });

        }

        public Point[] GetPasteSelectArea(MapModel mapModel)
        {
            var mapSelect = mapModel.MapSelect;

            return _mapChip.Select(
                (_, y, x) =>
                    {
                        x = x + mapSelect.StartX;
                        y = y + mapSelect.StartY;
                        return new Point(x, y);
                    })
                .Where(p => p.X < mapModel.MapCellWidth && p.Y < mapModel.MapCellHeight)
                .ToArray();
        }
    }
}