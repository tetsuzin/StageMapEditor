using System.Drawing;

namespace StageMapEditor.Models.Chip
{
    /// <summary>
    /// Objectと座標がセットになっているクラス
    /// </summary>
    public class ObjectChipPoint
    {
        public Point Point { get; set; }
        public ObjectChip ObjectChip { get; set; }

        public ObjectChipPoint(Point point, ObjectChip chip)
        {
            Point = point;
            ObjectChip = chip;
        }

        public ObjectChipPoint CopyTo()
        {
            ObjectChip = ObjectChip.Clone();
            return this;
        }
    }
}