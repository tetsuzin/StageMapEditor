using System.Collections.Generic;

namespace StageMapEditor.Models.Commands
{
    /// <summary>
    /// MaoCommandスタック
    /// </summary>
    internal class MapCommandStack : IMapCommandStack
    {
        private readonly Stack<IMapCommand> _stack = new Stack<IMapCommand>();

        #region Implementation of IMapCommandStack

        public bool IsEmpty
        {
            get { return _stack.Count == 0; }
        }

        public void Add(IMapCommand command)
        {
            if (!IsEqual(command))
            {
                _stack.Push(command);
            }
        }

        public void Clear()
        {
            _stack.Clear();
        }

        public IMapCommand Pop()
        {
            return _stack.Pop();
        }

        #endregion

        private bool IsEqual(IMapCommand comamnd)
        {
            if (IsEmpty)
                return false;

            var current = _stack.Peek();
            return current.Equals(comamnd);
        }
    }
}