using System.Text;

namespace StageMapEditor.Models.Commands
{
    /// <summary>
    /// Undo・Redo機能を管理
    /// </summary>
    public class Memento
    {
        private readonly IMapCommandStack _undo = new MapCommandStack();
        private readonly IMapCommandStack _redo = new MapCommandStack();
        public bool CanUndo { get { return !_undo.IsEmpty; } }
        public bool CanRedo { get { return !_redo.IsEmpty; } }

        public void AddUndoCommand(IMapCommand command)
        {
            _redo.Clear();
            _undo.Add(command);
        }

        public void AddRedoCommand(IMapCommand command)
        {
            _redo.Add(command);
        }

        public IMapCommand UndoCommand(MapModel model)
        {
            var command = _undo.Pop();
            _redo.Add(command.ReverseCommand(model));
            return command;
        }

        public IMapCommand RedoCommand(MapModel model)
        {
            var command = _redo.Pop();
            _undo.Add(command.ReverseCommand(model));
            return command;
        }

        public void Clear()
        {
            _undo.Clear();
            _redo.Clear();
        }
    }
}
