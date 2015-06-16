using System;
using System.Windows;
using Livet.EventListeners;
using StageMapEditor.Models;
using StageMapEditor.Models.Chip;
using StageMapEditor.Models.MousePosition;
using StageMapEditor.ViewModels;

namespace StageMapEditor.Views
{
    public interface IRenderingViewModel
    {
        int MapCellWidth { get; }
        int MapCellHeight { get; }
        float Scale { get; }
        int GridSize { get; }
        int ScaledGridSize { get; }
        LayerManager CurrentLayerManager { get; }
        MapSelect MapSelect { get; }
        System.Drawing.Point GetObjChipCropPosition(int id);
        System.Drawing.Point GetMapChipCropPosition(int id);
        bool CurrentMapIsNull { get; }
        MapChipModel MapChipModel { get; }
        ObjectChipModel ObjectChipModel { get; }
        IGeneralSettings Settings { get; }
        string BackgroundFilePath { get; }
        bool ExistsBackground { get; }
        MousePosition CurrentMousePosition { get; }
    }

    public interface IMapControlViewModel : IRenderingViewModel
    {
        void Delete();
        void Copy();
        void Cut();
        void Paste();

        void Undo();
        void Redo();

        void MousePositionMoved(Point point);
        void MouseGridPositionMoved(MousePosition position);

        void MouseDoubleClick(MousePosition position);
        void MouseLeftButtonDown(MousePosition position);
        void MouseLeftButtonUp(MousePosition position);
        void MouseRightButtonDown(MousePosition position);

        void MouseMoveWithLeftButtonDown(MousePosition position);
        void MouseMoveWithRightButtonDown(MousePosition position);

        void MouseLeave();

        void MouseWheel(int mouseDirection);

        Point MapScrollPosition { get; set; }

        PropertyChangedEventListener Listener { get; set; }

        void OpenEditWindow();
    }
}