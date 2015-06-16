using System;
using System.Diagnostics;
using System.Windows.Input;
using StageMapEditor.Views;
using Point = System.Drawing.Point;

namespace StageMapEditor.Models.MousePosition
{
    public struct MousePosition
    {
        public static MainModel Model { get; set; }
        public Point Position { get; set; }
        public Point GridPoint { get; set; }
        public System.Windows.Point ElementPosition { get; set; }
        private double Scale { get { return Model.CurrentMap == null ? 1 : Model.CurrentMap.Scale; } }

        public MousePosition(MapControl mapControl)
            : this()
        {
            var mappos = Mouse.GetPosition(mapControl.Wrapper);
            ElementPosition = mappos;
            Position = new Point((int)mappos.X, (int)mappos.Y);
            var g = Model.GridSize * Scale;
            GridPoint = new Point((int)(Position.X / g), (int)(Position.Y / g));
        }

        public MousePosition(int x, int y)
            : this()
        {
            ElementPosition = new System.Windows.Point(x, y);
            Position = new Point(x, y);
            var g = Model.GridSize * Scale;
            GridPoint = new Point((int)(Position.X / g), (int)(Position.Y / g));
        }

        public MousePosition(Point point)
            : this()
        {
            ElementPosition = new System.Windows.Point(point.X, point.Y);
            Position = point;
            var g = Model.GridSize * Scale;
            GridPoint = new Point((int)(Position.X / g), (int)(Position.Y / g));
        }

        public MousePosition(System.Windows.Point point)
            : this()
        {
            ElementPosition = point;
            Position = new Point((int)point.X, (int)point.Y);
            var g = Model.GridSize * Scale;
            GridPoint = new Point((int)(Position.X / g), (int)(Position.Y / g));
        }

        public override string ToString()
        {
            return String.Format("X = {0}, Y = {1}, GridX = {2}, GridY = {3}", this.Position.X, this.Position.Y, this.GridPoint.X, this.GridPoint.Y);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is MousePosition && Equals((MousePosition) obj);
        }

        public static bool operator ==(MousePosition left, MousePosition right)
        {
            return left.Position == right.Position;
        }

        public static bool operator !=(MousePosition left, MousePosition right)
        {
            return left.Position != right.Position;
        }

        public bool Equals(MousePosition other)
        {
            return Position.Equals(other.Position);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Position.GetHashCode();
                hashCode = (hashCode * 397) ^ GridPoint.GetHashCode();
                hashCode = (hashCode * 397) ^ ElementPosition.GetHashCode();
                return hashCode;
            }
        }
    }

}