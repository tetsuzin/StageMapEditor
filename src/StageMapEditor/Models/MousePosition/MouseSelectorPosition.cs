using System;
using System.Windows;
using System.Windows.Input;
using Point = System.Drawing.Point;

namespace StageMapEditor.Models.MousePosition
{
    /// <summary>
    /// MapChipパレット・ObjectChipパレットにおけるマウス位置を抽象化
    /// </summary>
    public class MouseSelectorPosition
    {
        private Point _point;
        public static int GridSize;

        public bool IsEmpty { get { return _point.IsEmpty; } }

        #region constructor

        public MouseSelectorPosition()
        {
            _point = Point.Empty;
        }

        public MouseSelectorPosition(Point point)
        {
            _point = point;
        }

        public MouseSelectorPosition(System.Windows.Point point)
        {
            _point = new Point((int)point.X, (int)point.Y);
        }

        public MouseSelectorPosition(int x, int y)
        {
            _point = new Point(x, y);
        }

        public MouseSelectorPosition(double x, double y)
        {
            _point = new Point((int)x, (int)y);
        }

        #endregion

        #region Setメソッド

        public void Set(Point point)
        {
            _point = point;
        }

        public void Set(System.Windows.Point point)
        {
            _point = new Point((int)point.X, (int)point.Y);
        }

        public void Set(int x, int y)
        {
            _point = new Point(x, y);
        }

        public void Set(double x, double y)
        {
            _point = new Point((int)x, (int)y);
        }

        #endregion

        #region Getメソッド

        public int X { get { return _point.X; } }
        public int Y { get { return _point.Y; } }

        public int GridX { get { return _point.X / GridSize; } }
        public int GridY { get { return _point.Y / GridSize; } }

        public Point Get
        {
            get { return _point; }
        }

        public Point GetGridPosition
        {
            get { return new Point(_point.X / GridSize, _point.Y / GridSize); }
        }

        /// <summary>
        /// Map内のマウス位置をセル単位で取得
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public static MouseSelectorPosition MousePositionOnSelector(IInputElement control)
        {
            if (control == null) throw new ArgumentNullException("control");

            var p = Mouse.GetPosition(control);
            return new MouseSelectorPosition((int)(p.X), (int)(p.Y));
        }

        #endregion
    }
}