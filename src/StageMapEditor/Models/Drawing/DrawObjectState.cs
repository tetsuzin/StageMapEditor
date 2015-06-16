using System.Drawing;
using System.Linq;
using StageMapEditor.Models.Chip;

namespace StageMapEditor.Models.Drawing
{
    public class DrawObjectState : IDrawingChip
    {
        private readonly ObjectChipLibrary _objectChipLibrary;

        public DrawObjectState(ObjectChipLibrary objectChipLibrary)
        {
            _objectChipLibrary = objectChipLibrary;
        }

        /// <summary>
        /// 現在選択しているパレット
        /// </summary>
        private ObjectChip _selected;
        public ObjectChip Selected { get { return _selected; } }

        public void Draw(MapModel model, int x, int y, MapSelect mapSelect)
        {
            //IDが0の場合は何もしない
            if (_selected.ID != 0)
            {
                model.ObjectChipModel.Set(x, y, _selected);
            }
        }

        public void Draw(MapModel model, Point point, MapSelect mapSelect)
        {
            Draw(model, point.X, point.Y, mapSelect);
        }

        public bool CanDraw(MapModel model, Point point)
        {
            var r = !model.ObjectChipModel.Get(point).IsSame(_selected);
            
            return r;
        }

        public void Set(int id)
        {
            _selected = _objectChipLibrary.GetByID(id);
        }

        public void SpoilObject(ObjectChip objectChip)
        {
            _selected = objectChip.Clone();
        }

        /// <summary>
        /// パレットのその位置を選択できるかどうか
        /// </summary>
        /// <returns></returns>
        public bool CanSetPallet(int id)
        {
            return _objectChipLibrary.GetEnumerator().Select(x => x.ID).Contains(id);
        }

        public void Delete(MapModel model, MapSelect mapSelect)
        {
            mapSelect.GetSelectPointArray()
                .ToList()
                .ForEach(p => model.ObjectChipModel.Set(p.X, p.Y, ObjectChip.Empty()));
        }
    }
}