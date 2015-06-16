using System;

namespace StageMapEditor.Models.Commands
{
    public interface IMapCommand
    {
        MapChipCommand MapChipCommand { get; }
        ObjectChipCommand ObjectChipCommand { get; }
        void Execute(MapModel model);
        void Execute(MapModel model, Action callBack);
        IMapCommand ReverseCommand(MapModel model);
    }
}