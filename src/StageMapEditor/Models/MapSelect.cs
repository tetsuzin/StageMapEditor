using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using StageMapEditor.Helper;
using StageMapEditor.Models.Chip;

namespace StageMapEditor.Models
{
    public class MapSelect
    {
        /// <summary>
        /// 現在選択中のマス横開始
        /// </summary>
        public int SelectedX { get; set; }

        /// <summary>
        /// 現在選択中のマス縦開始
        /// </summary>
        public int SelectedY { get; set; }

        /// <summary>
        /// 現在選択中のマス横終わり
        /// </summary>
        public int SelectedXEnd { get; set; }

        /// <summary>
        /// 現在選択中のマス縦開始
        /// </summary>
        public int SelectedYEnd { get; set; }

        public int StartX { get { return SelectedX <= SelectedXEnd ? SelectedX : SelectedXEnd; } }
        public int StartY { get { return SelectedY <= SelectedYEnd ? SelectedY : SelectedYEnd; } }
        public int EndX { get { return SelectedX <= SelectedXEnd ? SelectedXEnd : SelectedX; } }
        public int EndY { get { return SelectedY <= SelectedYEnd ? SelectedYEnd : SelectedY; } }

        /// <summary>
        /// 選択中であるかどうか
        /// </summary>
        public bool IsEmpty { get; set; }

        /// <summary>
        /// 一カ所しか選択されていないか
        /// </summary>
        public bool IsSelectedSinglePoint { get { return IsEmpty == false && Width == 1 && Height == 1; } }

        /// <summary>
        /// 一カ所選択されている場合のみその場所を返す
        /// </summary>
        public Point? SinglePoiint
        {
            get { return IsSelectedSinglePoint ? (Point?) new Point(StartX, StartY) : null; }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MapSelect() : this(0, 0, 0, 0) { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MapSelect(int x, int y, int endX, int endY)
        {
            SelectedX = x;
            SelectedY = y;

            SelectedXEnd = endX;
            SelectedYEnd = endY;

            IsEmpty = true;
        }

        public void Clear()
        {
            SelectedX = 0;
            SelectedY = 0;

            SelectedXEnd = 0;
            SelectedYEnd = 0;

            IsEmpty = true;
        }

        public void Set(int x, int y)
        {
            SelectedX = x;
            SelectedY = y;

            SelectedXEnd = x;
            SelectedYEnd = y;

            IsEmpty = false;
        }

        public void Set(int x, int y, int endX, int endY)
        {
            SelectedX = x;
            SelectedY = y;

            SelectedXEnd = endX;
            SelectedYEnd = endY;

            IsEmpty = false;
        }

        /// <summary>
        /// 選択範囲の終端を決定
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SetEnd(int x, int y)
        {
            SelectedXEnd = x;
            SelectedYEnd = y;

            IsEmpty = false;
        }

        /// <summary>
        /// 指定したの座標が選択範囲に含まれているか
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Contains(int x, int y)
        {
            return (StartX <= x && x <= EndX) && (StartY <= y && y <= EndY);
        }

        public bool Contains(MousePosition.MousePosition point)
        {
            return Contains(point.GridPoint.X, point.GridPoint.Y);
        }

        public bool Contains(Point point)
        {
            return Contains(point.X, point.Y);
        }

        public int Width { get { return Math.Abs(SelectedX - SelectedXEnd) + 1; } }
        public int Height { get { return Math.Abs(SelectedY - SelectedYEnd) + 1; } }

        public int[] GetXRange()
        {
            return Enumerable.Range(StartX, Math.Abs(SelectedX - SelectedXEnd) + 1).ToArray();
        }

        public int[] GetYRange()
        {
            return Enumerable.Range(StartY, Height).ToArray();
        }

        public Point[] GetSelectPointArray()
        {
            return GetYRange().MultiSelect(GetXRange(), (y, x) => new Point(x, y)).ToArray();
        }

        public override string ToString()
        {
            return string.Format("X={0}, Y={1}, EndX={2}, EndY={3}", SelectedX, SelectedY, SelectedXEnd, SelectedYEnd);
        }

        /// <summary>
        /// 選択範囲内に存在するMapChipPointのリストを返す
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IEnumerable<MapChipPoint> GetMapChipList(MapChipModel model)
        {
            return GetSelectPointArray().Select(p => new MapChipPoint(p, model.Get(p))).ToArray();
        }

        /// <summary>
        /// 選択範囲内に存在するObjectChipPointのリストを返す
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IEnumerable<ObjectChipPoint> GetObjectChipList(ObjectChipModel model)
        {
            return GetSelectPointArray().Select(p => new ObjectChipPoint(p, model.Get(p))).ToArray();
        }
    }
}