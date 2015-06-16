using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using StageMapEditor.Models.Chip;

namespace StageMapEditor.Models.Commands
{
    public class MapCommandAll : IMapCommand
    {
        private Tuple<Point, MapChip, ObjectChip>[] _old;
        public MapChipCommand MapChipCommand { get; private set; }
        public ObjectChipCommand ObjectChipCommand { get; private set; }

        public MapCommandAll(MapModel model)
        {
            _old = model.ListAllWithPosition()
                     .ToArray();
        }

        public void Execute(MapModel model)
        {
            foreach (var t in _old)
            {
                var p = t.Item1;
                model.MapChipModel.Set(p, t.Item2.Clone());
                model.ObjectChipModel.Set(p, t.Item3.Clone());
            }

            foreach (var t in model.ObjectChipModel.ListAllWithPosition().ToArray())
            {
                if (t.Item2.ID == 0)
                {
                    model.ObjectChipModel.Delete(t.Item1);
                }
            }
        }

        public void Execute(MapModel model, Action callBack)
        {
            Execute(model);
            callBack();
        }

        public IMapCommand ReverseCommand(MapModel model)
        {
            return new MapCommandAll(model);
        }
    }

    public class MapCommand : IMapCommand
    {
        public MapChipCommand MapChipCommand { get; set; }
        public ObjectChipCommand ObjectChipCommand { get; set; }

        #region コンストラクタ

        /// <summary>
        /// 特定の地点のMapCommandを作成する
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="model"></param>
        public MapCommand(int x, int y, MapModel model) : this(new Point(x, y), model) { }

        /// <summary>
        /// 特定の地点のMapCommandを作成する
        /// </summary>
        /// <param name="point"></param>
        /// <param name="model"></param>
        public MapCommand(Point point, MapModel model)
        {
            MapChipCommand = new MapChipCommand(point, model.MapChipModel.Get(point));
            ObjectChipCommand = new ObjectChipCommand(point, model.ObjectChipModel.Get(point));
        }

        public MapCommand(int x, int y, MapModel model, MapSelect mapSelect) : this(new Point(x, y), model, mapSelect) { }
        public MapCommand(Point point, MapModel model, MapSelect mapSelect)
        {
            //追加場所が選択範囲ならば、その範囲を記録したコマンドを生成
            if (mapSelect.Contains(point))
            {
                var mapCommand = mapSelect.GetMapChipList(model.MapChipModel);
                var objCommand = mapSelect.GetObjectChipList(model.ObjectChipModel);

                MapChipCommand = new MapChipCommand(mapCommand);
                ObjectChipCommand = new ObjectChipCommand(objCommand);
            }
            else
            {
                MapChipCommand = new MapChipCommand(point, model.MapChipModel.Get(point));
                ObjectChipCommand = new ObjectChipCommand(point, model.ObjectChipModel.Get(point));
            }
        }

        public MapCommand(MapModel model, MapSelect mapSelect)
        {
            var mapCommand = mapSelect.GetSelectPointArray().Select(p => new MapChipPoint(p, model.MapChipModel.Get(p)));
            var objCommand = mapSelect.GetSelectPointArray().Select(p => new ObjectChipPoint(p, model.ObjectChipModel.Get(p)));

            MapChipCommand = new MapChipCommand(mapCommand);
            ObjectChipCommand = new ObjectChipCommand(objCommand);
        }

        public MapCommand(IEnumerable<IMapCommand> commandPool)
        {
            if (commandPool == null) return;

            var commands = commandPool.ToArray();
            var mapComamnd = commands.SelectMany(x => x.MapChipCommand.MapChipPointList);
            var objComamnd = commands.SelectMany(x => x.ObjectChipCommand.ObjectChipPointList);

            MapChipCommand = new MapChipCommand(mapComamnd);
            ObjectChipCommand = new ObjectChipCommand(objComamnd);
        }

        public MapCommand(MapModel model, Clipboard clipBoard)
        {
            var pasteArea = clipBoard.GetPasteSelectArea(model);

            var mapCommand = pasteArea.Select(p => new MapChipPoint(p, model.MapChipModel.Get(p))).ToArray();
            var objCommand = pasteArea.Select(p => new ObjectChipPoint(p, model.ObjectChipModel.Get(p))).ToArray();

            MapChipCommand = new MapChipCommand(mapCommand);
            ObjectChipCommand = new ObjectChipCommand(objCommand);
        }

        #endregion

        public void Execute(MapModel model)
        {
            if (MapChipCommand != null)
            {
                MapChipCommand.Execute(model);
            }
            if (ObjectChipCommand != null)
            {
                ObjectChipCommand.Execute(model);
            }
        }

        public void Execute(MapModel model, Action callBack)
        {
            Execute(model);
            callBack();
        }

        /// <summary>
        /// 引数のMapModelのスタックされているコマンドの適用範囲をコマンド化する
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IMapCommand ReverseCommand(MapModel model)
        {
            var mapCommands = MapChipCommand.MapChipPointList.Select(p => new MapCommand(p.Point, model));
            var objCommands = ObjectChipCommand.ObjectChipPointList.Select(p => new MapCommand(p.Point, model));

            return new MapCommand(mapCommands.Concat(objCommands));
        }
    }
}