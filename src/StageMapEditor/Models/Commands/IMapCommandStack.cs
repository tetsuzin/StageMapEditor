namespace StageMapEditor.Models.Commands
{
    internal interface IMapCommandStack
    {
        bool IsEmpty { get; }
        void Add(IMapCommand command);
        void Clear();
        IMapCommand Pop();
    }
}