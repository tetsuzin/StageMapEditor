using System.Drawing;

namespace StageMapEditor.Models.Chip
{
    /// <summary>
    /// MapChipと座標がセットになっているクラス
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("ID:{MapChip.ID} X={Point.X} Y={Point.Y}")]
    public class MapChipPoint
    {
        public Point Point { get; set; }
        public MapChip MapChip { get; set; }

        public MapChipPoint(Point point, MapChip chip)
        {
            Point = point;
            MapChip = chip.Clone();
        }

        public MapChipPoint CopyTo()
        {
            MapChip = MapChip.Clone();
            return this;
        }
    }
}