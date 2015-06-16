using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using StageMapEditor.Models.Chip;

namespace StageMapEditor.Models.Commands
{
    /// <summary>
    /// オブジェクトチップの変更コマンド
    /// </summary>
    public class ObjectChipCommand
    {
        public ObjectChipPoint[] ObjectChipPointList { get; private set; }

        public ObjectChipCommand(Point point, ObjectChip chip)
        {
            ObjectChipPointList = new[] { new ObjectChipPoint(point, chip.Clone()), };
        }

        public ObjectChipCommand(ObjectChipPoint chip)
        {
            ObjectChipPointList = new[] { chip.CopyTo() };
        }

        public ObjectChipCommand(IEnumerable<ObjectChipPoint> chipList)
        {
            foreach (var x in chipList)
            {
                x.CopyTo();
            }

            ObjectChipPointList = chipList.ToArray();
        }

        public Point[] DrawArea
        {
            get { return ObjectChipPointList.Select(x => x.Point).ToArray(); }
        }

        public void Execute(MapModel mapModel)
        {
            foreach (var x in ObjectChipPointList)
            {
                mapModel.ObjectChipModel.Set(x.Point.X, x.Point.Y, x.ObjectChip);
            }
        }

        public void Execute(MapModel mapInfomation, Action callBack)
        {
            Execute(mapInfomation);
            callBack();
        }
    }
}