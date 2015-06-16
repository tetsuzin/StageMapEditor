using System.Drawing;
using System.Linq;
using StageMapEditor.Models.Chip;

namespace StageMapEditor.Models.Drawing
{
    public class DrawMapState : IDrawingChip
    {
        private readonly MapChipLibrary _mapChipLibrary;

        /// <summary>
        /// 現在選択しているパレット
        /// </summary>
        private int _selected;

        public DrawMapState(MapChipLibrary mapChipLibrary)
        {
            _mapChipLibrary = mapChipLibrary;
        }

        /// <summary>
        /// チップのIDからパレットにセット
        /// </summary>
        /// <param name="id"></param>
        public void Set(int id)
        {
            _selected = id;
        }


        #region Draw関連
        public void Draw(MapModel model, int x, int y, MapSelect mapSelect)
        {
            if (mapSelect.Contains(x, y))
            {
                //範囲を塗りつぶす
                DrawArea(model, mapSelect);
            }
            else
            {
                //すでに同じチップが置かれている場合はDrawを行わない
                var chip = _mapChipLibrary.GetByID(_selected);
                model.MapChipModel.Set(x, y, chip);
            }
        }

        public void Draw(MapModel model, Point point, MapSelect mapSelect)
        {
            Draw(model, point.X, point.Y, mapSelect);
        }

        /// <summary>
        /// 範囲を塗りつぶす
        /// </summary>
        private void DrawArea(MapModel model, MapSelect mapSelect)
        {
            mapSelect.GetSelectPointArray()
                .ToList()
                .ForEach(p => model.MapChipModel.Set(p.X, p.Y, _mapChipLibrary.GetByID(_selected)));
        }
        #endregion

        public bool CanDraw(MapModel model, Point point)
        {
            return model != null && model.MapChipModel.Get(point).ID != _selected;
        }

        /// <summary>
        /// パレットのその位置を選択できるかどうか
        /// </summary>
        /// <returns></returns>
        public bool CanSetPallet(int id)
        {
            return _mapChipLibrary.GetEnumerator().Select(x => x.ID).Contains(id);
        }

        public void Delete(MapModel model, MapSelect mapSelect)
        {
            mapSelect.GetSelectPointArray()
                .ToList()
                .ForEach(p => model.MapChipModel.Set(p.X, p.Y, _mapChipLibrary.GetChip(0, 0)));
        }
    }
}