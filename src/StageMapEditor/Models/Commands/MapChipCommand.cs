using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using StageMapEditor.Models.Chip;

namespace StageMapEditor.Models.Commands
{
    /// <summary>
    /// マップチップの変更コマンド
    /// </summary>
    public class MapChipCommand
    {
        public MapChipPoint[] MapChipPointList { get; private set; }

        public MapChipCommand(Point point, MapChip chip)
        {
            MapChipPointList = new[] { new MapChipPoint(point, (MapChip)chip.Clone()), };

        }

        public MapChipCommand(MapChipPoint chip)
        {
            MapChipPointList = new[] { chip.CopyTo() };
        }

        public MapChipCommand(IEnumerable<MapChipPoint> chipList)
        {
            foreach (var x in chipList)
            {
                x.CopyTo();
            }

            MapChipPointList = chipList.ToArray();
        }

        public Point[] DrawArea
        {
            get { return MapChipPointList.Select(x => x.Point).ToArray(); }
        }

        public void Execute(MapModel mapModel)
        {
            MapChipPointList.ToList().ForEach(p => mapModel.MapChipModel.Set(p.Point.X, p.Point.Y, p.MapChip));
        }

        public void Execute(MapModel mapModel, Action callBack)
        {
            Execute(mapModel);
            callBack();
        }
    }
}