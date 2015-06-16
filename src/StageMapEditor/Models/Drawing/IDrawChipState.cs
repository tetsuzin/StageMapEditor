using System.Drawing;

namespace StageMapEditor.Models.Drawing
{
    public interface IDrawingChip
    {
        void Draw(MapModel model, int x, int y, MapSelect mapSelect);
        void Draw(MapModel model, Point point, MapSelect mapSelect);
        bool CanDraw(MapModel model, Point point);
        bool CanSetPallet(int id);
        void Delete(MapModel model, MapSelect mapSelect);

    }
}