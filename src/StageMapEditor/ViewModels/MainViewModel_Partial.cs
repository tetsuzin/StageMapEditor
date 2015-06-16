using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.Messaging.Windows;
using Microsoft.Win32;
using StageMapEditor.Helper;
using StageMapEditor.Models;
using StageMapEditor.Models.Chip;
using StageMapEditor.Models.Commands;
using StageMapEditor.Models.Drawing;
using StageMapEditor.Models.MousePosition;
using StageMapEditor.Views;

namespace StageMapEditor.ViewModels
{
    public partial class MainViewModel
    {
        public void CommandCanExecuteChanged()
        {
            UndoCommand.RaiseCanExecuteChanged();
            RedoCommand.RaiseCanExecuteChanged();
            SaveCommand.RaiseCanExecuteChanged();
            CreateMapCommand.RaiseCanExecuteChanged();
            EditStageCommand.RaiseCanExecuteChanged();
            DeleteMapCommand.RaiseCanExecuteChanged();
            VisibleLayerCommand.RaiseCanExecuteChanged();
        }

        #region IMapControlViewModel
        public void Delete()
        {
            if (!MapSelect.IsEmpty)
            {
                //Undoコマンドの作成
                var command = new MapCommand(CurrentMap, MapSelect);
                CurrentMap.AddHistory(command);

                PalletViewModelManager.Delete(CurrentMap, MapSelect);
            }
        }

        public void MousePositionMoved(Point point)
        {
            //現在地の更新をする際に、マップから検索地点はみ出ていた場合は何もしない
            if (_model.CurrentMap.MapCellWidth < point.X && _model.CurrentMap.MapCellHeight < point.Y)
            {
                _model.CurrentPosition = new Point(point.X, point.Y);
            }
        }

        public void MouseGridPositionMoved(MousePosition position)
        {
            if (this.CurrentMap != null)
            {
                _model.CurrentMousePosition = position;
            }
        }

        public void MouseDoubleClick(MousePosition position)
        {
            var p = position.GridPoint;

            PalletViewModelManager.DoubleClick(
                CurrentMap, p,
                chip =>
                Messenger.Raise(new TransitionMessage(new ObjectChipViewModel((ObjectChip)chip), "TransitionObjectChip")));
        }

        /// <summary>
        /// 左クリックダウン
        /// </summary>
        /// <param name="position"></param>
        public void MouseLeftButtonDown(MousePosition position)
        {
            var p = position.GridPoint;

            if (PalletViewModelManager.CanDraw(CurrentMap, p))
            {
                //一時コマンドプールにコマンドを保存
                var command = new MapCommand(p, CurrentMap, MapSelect);
                _commandPool.Add(command);

                //DrawしてModelを更新
                PalletViewModelManager.Draw(CurrentMap, p, MapSelect);

                CommandCanExecuteChanged();
            }
        }

        /// <summary>
        /// 左クリックアップ
        /// </summary>
        /// <param name="position"></param>
        public void MouseLeftButtonUp(MousePosition position)
        {
            if (_commandPool.FirstOrDefault() == null) return;

            var command = new MapCommand(_commandPool);
            CurrentMap.AddHistory(command);
            _commandPool.Clear();

            CommandCanExecuteChanged();
        }

        /// <summary>
        /// 左クリックとマウス移動
        /// </summary>
        /// <param name="position"></param>
        public void MouseMoveWithLeftButtonDown(MousePosition position)
        {
            var p = position.GridPoint;

            if (PalletViewModelManager.CanDraw(CurrentMap, p))
            {
                var command = new MapCommand(p, CurrentMap, MapSelect);
                _commandPool.Add(command);

                //DrawしてModelを更新
                PalletViewModelManager.Draw(CurrentMap, p, MapSelect);

                //MapControl.Rendering();
            }
        }

        /// <summary>
        /// マウスが離れたとき
        /// </summary>
        public void MouseLeave()
        {
            if (_commandPool.FirstOrDefault() != null)
            {
                CurrentMap.AddHistory(new MapCommand(_commandPool));
                _commandPool.Clear();
            }
        }

        private void ScaleChangeCore(int parameter)
        {
            if (parameter == 0) { CurrentMap.ScaleNormal(); }
            else if (parameter > 0) { CurrentMap.ScaleUp(); }
            else if (parameter < 0) { CurrentMap.ScaleDown(); }
        }

        /// <summary>
        /// マウスホイール時に呼び出されるメソッド
        /// </summary>
        /// <param name="mouseDirection"></param>
        public void MouseWheel(int mouseDirection)
        {
            ScaleChangeCore(mouseDirection);
        }
        #endregion

        #region ScaleChangeCommand
        ListenerCommand<string> _ScaleChangeCommand;
        public ListenerCommand<string> ScaleChangeCommand
        {
            get { return _ScaleChangeCommand ?? (_ScaleChangeCommand = new ListenerCommand<string>(ScaleChange)); }
        }

        private void ScaleChange(string parameter)
        {
            if (!CurrentMapIsNull)
            {
                int p;
                if (int.TryParse(parameter, out p))
                {
                    ScaleChangeCore(p);
                }
            }
        }
        #endregion

        #region MouseRightButtonDownCommand
        private ListenerCommand<MousePosition> _MouseRightButtonDownCommand;

        public ListenerCommand<MousePosition> MouseRightButtonDownCommand
        {
            get
            {
                return _MouseRightButtonDownCommand ??
                       (_MouseRightButtonDownCommand = new ListenerCommand<MousePosition>(MouseRightButtonDown));
            }
        }

        /// <summary>
        /// 右クリックダウン
        /// スポイト動作も行う
        /// </summary>
        /// <param name="parameter"> </param>
        public void MouseRightButtonDown(MousePosition parameter)
        {
            var p = parameter.GridPoint;

            var mapChip = CurrentMap.MapChipModel.Get(p);
            var objChip = CurrentMap.ObjectChipModel.Get(p);

            if (!objChip.IsEmpty) //ObjectChipが存在する場合
            {
                //スポイト操作の前にDrawStateを変更する
                //オブジェクトチップのスポイト動作
                PalletViewModelManager.SpoilChip(DrawState.ObjChip, objChip);
            }
            else if (mapChip.ID != 0) //ObjectChipが空でMapChipが存在する場合
            {
                //スポイト操作の前にDrawStateを変更する
                //マップチップのスポイト動作
                PalletViewModelManager.SpoilChip(DrawState.MapChip, mapChip);
            }

            //MapSelectの更新
            MapSelect.Set(p.X, p.Y);

            //MapControl.Rendering();
        }
        #endregion

        #region MouseMoveWithRightButtonDownCommand
        private ListenerCommand<MousePosition> _MouseMoveWithRightButtonDownCommand;

        public ListenerCommand<MousePosition> MouseMoveWithRightButtonDownCommand
        {
            get
            {
                return _MouseMoveWithRightButtonDownCommand ??
                       (_MouseMoveWithRightButtonDownCommand = new ListenerCommand<MousePosition>(MouseMoveWithRightButtonDown));
            }
        }

        /// <summary>
        /// 右クリックとマウス移動
        /// </summary>
        /// <param name="parameter"> </param>
        public void MouseMoveWithRightButtonDown(MousePosition parameter)
        {
            var p = parameter.GridPoint;

            if (MapSelect.SelectedXEnd != p.X || MapSelect.SelectedYEnd != p.Y)
            {
                MapSelect.SetEnd(p.X, p.Y);

                //MapControl.Rendering();
            }
        }
        #endregion

        #region CreateMapCommand 新しいマップを追加する
        ViewModelCommand _CreateMapCommand;
        public ViewModelCommand CreateMapCommand
        {
            get { return _CreateMapCommand ?? (_CreateMapCommand = new ViewModelCommand(CreateMap, CanCreateMap)); }
        }

        private bool CanCreateMap()
        {
            return _model.CurrentStage != null;
        }

        private void CreateMap()
        {
            Messenger.Raise(new TransitionMessage(new EditMapViewModel(_model.CurrentStage), "TransitionMap"));

            if (UpdateCommand.CanExecute) { Update(); }
        }
        #endregion

        #region UpdateCommand マップの変更通知を発生させる
        ViewModelCommand _UpdateCommand;
        public ViewModelCommand UpdateCommand
        {
            get { return _UpdateCommand ?? (_UpdateCommand = new ViewModelCommand(Update, CanUpdate)); }
        }

        private bool CanUpdate()
        {
            if (_model.CurrentStage == null)
            {
                return false;
            }

            return true;
        }

        private void Update()
        {
            _model.CurrentStage.RaisePropertyChangedFromMapModel();
            RaisePropertyChanged("MapWidth");
            RaisePropertyChanged("MapHeight");
            RaisePropertyChanged("Scale");
        }
        #endregion


        #region DeleteMapCommand マップの削除を行う
        private ViewModelCommand _DeleteMapCommand;

        public ViewModelCommand DeleteMapCommand
        {
            get
            {
                if (_DeleteMapCommand == null)
                {
                    _DeleteMapCommand = new ViewModelCommand(DeleteMap, CanDeleteMap);
                }
                return _DeleteMapCommand;
            }
        }

        public bool CanDeleteMap()
        {
            return !CurrentMapIsNull;
        }

        public void DeleteMap()
        {
            CurrentMap.DeleteMap();

            CommandCanExecuteChanged();
        }
        #endregion

        #region CreateStageCommand Stageの追加
        ListenerCommand<MainViewModel> _CreateStageCommand;

        public ListenerCommand<MainViewModel> CreateStageCommand
        {
            get { return _CreateStageCommand ?? (_CreateStageCommand = new ListenerCommand<MainViewModel>(CreateStage)); }
        }

        private void CreateStage(MainViewModel parameter)
        {
            var inputVewModel = new EditStageViewModel(_model);

            Messenger.Raise(new TransitionMessage(inputVewModel, "TransitionStage"));

            if (_model.CurrentStage != null && _model.CurrentStage.MapModels.Count == 0)
            {
                CreateMapCommand.Execute();
            }
        }
        #endregion

        #region EditStageCommand Stageの編集
        ListenerCommand<MainViewModel> _EditStageCommand;

        public ListenerCommand<MainViewModel> EditStageCommand
        {
            get { return _EditStageCommand ?? (_EditStageCommand = new ListenerCommand<MainViewModel>(EditStage, CanEditStage)); }
        }

        private bool CanEditStage()
        {
            return _model.CurrentStage != null;
        }

        private void EditStage(MainViewModel parameter)
        {
            var inputVewModel = new EditStageViewModel(_model.CurrentStage, _model);

            Messenger.Raise(new TransitionMessage(inputVewModel, "TransitionStage"));
        }
        #endregion

        #region 現在編集中のマップを保存する
        ViewModelCommand _SaveCommand;

        public ViewModelCommand SaveCommand
        {
            get { return _SaveCommand ?? (_SaveCommand = new ViewModelCommand(Save, CanSave)); }
        }

        private bool CanSave()
        {
            return _model.CurrentStage != null;
        }

        private void Save()
        {
            _model.CurrentStage.Save();
        }
        #endregion

        #region 現在編集中のマップを保存する
        ViewModelCommand _SaveNewCommand;

        public ViewModelCommand SaveNewCommand
        {
            get { return _SaveNewCommand ?? (_SaveNewCommand = new ViewModelCommand(SaveNew, CanSaveNew)); }
        }

        private bool CanSaveNew()
        {
            return _model.CurrentStage != null;
        }

        private void SaveNew()
        {
            _model.CurrentStage.SaveNew();
        }
        #endregion

        #region CopyCommand 選択範囲のコピー動作
        ViewModelCommand _CopyCommand;
        public ViewModelCommand CopyCommand { get { return _CopyCommand ?? (_CopyCommand = new ViewModelCommand(_Copy, CanCopy)); } }
        public bool CanCopy() { return !CurrentMapIsNull; }
        public void Copy() { CopyCommand.Execute(); }
        private void _Copy()
        {
            PalletViewModelManager.Clipboard.Copy(CurrentMap);

            CommandCanExecuteChanged();
        }
        #endregion

        #region CutCommand 選択範囲のコピー動作
        ViewModelCommand _CutCommand;
        public ViewModelCommand CutCommand { get { return _CutCommand ?? (_CutCommand = new ViewModelCommand(_Cut, CanCut)); } }
        public bool CanCut() { return !CurrentMapIsNull; }
        public void Cut() { CutCommand.Execute(); }
        private void _Cut()
        {
            var command = new MapCommand(CurrentMap, MapSelect);
            CurrentMap.AddHistory(command);
            PalletViewModelManager.Clipboard.Cut(CurrentMap);

            CommandCanExecuteChanged();
        }
        #endregion

        #region PasteCommand 選択範囲のコピー動作
        ViewModelCommand _PasteCommand;
        public ViewModelCommand PasteCommand { get { return _PasteCommand ?? (_PasteCommand = new ViewModelCommand(_Paste, CanPaste)); } }
        public bool CanPaste() { return !CurrentMapIsNull; }
        public void Paste() { PasteCommand.Execute(); }
        private void _Paste()
        {
            var command = new MapCommand(CurrentMap, PalletViewModelManager.Clipboard);
            CurrentMap.AddHistory(command);
            PalletViewModelManager.Clipboard.Paste(CurrentMap);

            CommandCanExecuteChanged();
        }
        #endregion

        #region Undo 巻き戻しを実行
        ViewModelCommand _UndoCommand;
        public ViewModelCommand UndoCommand { get { return _UndoCommand ?? (_UndoCommand = new ViewModelCommand(_Undo, CanUndo)); } }
        private bool CanUndo() { return CurrentMap != null && CurrentMap.CanUndo; }
        public void Undo() { UndoCommand.Execute(); }
        private void _Undo()
        {
            CurrentMap.Undo();

            CommandCanExecuteChanged();
        }
        #endregion

        #region Redo やり直しを実行
        ViewModelCommand _RedoCommand;
        public ViewModelCommand RedoCommand { get { return _RedoCommand ?? (_RedoCommand = new ViewModelCommand(_Redo, CanRedo)); } }
        private bool CanRedo() { return CurrentMap != null && CurrentMap.CanRedo; }
        public void Redo() { RedoCommand.Execute(); }
        private void _Redo()
        {
            CurrentMap.Redo();

            CommandCanExecuteChanged();
        }
        #endregion

        #region VisibleLayerCommand レイヤーの表示非表示を切り替える
        private ListenerCommand<string> _VisibleLayerCommand;
        public ListenerCommand<string> VisibleLayerCommand
        {
            get
            {
                return _VisibleLayerCommand ??
                       (_VisibleLayerCommand = new ListenerCommand<string>(VisibleLayer, CanVisibleLayer));
            }
        }

        public bool CanVisibleLayer() { return CurrentMap != null; }

        public void VisibleLayer(string target)
        {
            switch (target)
            {
                case "Border":
                    CurrentMap.LayerManager.Border.FlipVisible();
                    RaisePropertyChanged("VisibleBorderLayer");
                    break;
                case "MapChip":
                    CurrentMap.LayerManager.MapChip.FlipVisible();
                    RaisePropertyChanged("VisibleMapChipLayer");
                    break;
                case "ObjectChip":
                    CurrentMap.LayerManager.ObjectChip.FlipVisible();
                    RaisePropertyChanged("VisibleObjectChipLayer");
                    break;
            }

            RaisePropertyChanged("ChangeLayer");
        }
        #endregion

        #region OpenStageCommand
        private ViewModelCommand _OpenStageCommand;

        public ViewModelCommand OpenStageCommand
        {
            get { return _OpenStageCommand ?? (_OpenStageCommand = new ViewModelCommand(OpenStage)); }
        }

        public void OpenStage()
        {
            var dialog = FileDialogHelpers.OpenStageFileDialog(Settings.StageDataDirectory);

            if (dialog.ShowDialog() == true)
            {
                LoadFile(dialog.FileName);

                //スクロール位置のリセット
                //MapScrollPosition.X
            }

            CommandCanExecuteChanged();
        }
        #endregion

        #region ChangeNextTabCommand 　タブの切り替えを行う
        private ListenerCommand<string> _ChangeNextTabCommand;

        public ListenerCommand<string> ChangeNextTabCommand
        {
            get
            {
                if (_ChangeNextTabCommand == null)
                {
                    _ChangeNextTabCommand = new ListenerCommand<string>(ChangeNextTab, CanChangeNextTab);
                }
                return _ChangeNextTabCommand;
            }
        }

        public bool CanChangeNextTab()
        {
            return _model.StageModels.Count > 1;
        }

        public void ChangeNextTab(string parameter)
        {
            var p = int.Parse(parameter);
            var current = _model.StageModels.IndexOf(_model.CurrentStage);
            var count = _model.StageModels.Count;

            var x = current + p;
            //移動先Indexがマイナスになったら先頭に、タブの数よりも大きくなったら0にする
            var next = x < 0 ? count - 1 :
                       x >= count ? 0 : x;
            _model.CurrentStage = _model.StageModels.ElementAt(next);

            CommandCanExecuteChanged();
        }
        #endregion

        #region FileDropCommand
        private ListenerCommand<string[]> _FileDropCommand;

        public ListenerCommand<string[]> FileDropCommand
        {
            get { return _FileDropCommand ?? (_FileDropCommand = new ListenerCommand<string[]>(FileDrop)); }
        }

        public void FileDrop(string[] filepathList)
        {
            var path = filepathList.FirstOrDefault();
            if (path != null)
            {
                foreach (var p in filepathList)
                {
                    LoadFile(p);
                }
            }
        }
        #endregion

        #region ApplicationClosedCommand アプリケーション終了時
        private ListenerCommand<Window> _ApplicationClosedCommand;
        public ListenerCommand<Window> ApplicationClosedCommand { get { return _ApplicationClosedCommand ?? (_ApplicationClosedCommand = new ListenerCommand<Window>(ApplicationClosed)); } }
        public void ApplicationClosed(Window sender)
        {
            var window = ((MainWindow)sender);

            Properties.Settings.Default.WindowHeight = window.ActualHeight;
            Properties.Settings.Default.WindowWidth = window.ActualWidth;
            Properties.Settings.Default.PalletWidth = window.SelectorCol.ActualWidth;
            Properties.Settings.Default.TreeViewWidth = window.StageMapTreeCol.ActualWidth;

            Properties.Settings.Default.Save();

            sender.Close();
        }
        #endregion

        /// <summary>
        /// 選択中のマスにあるチップの編集ウィンドウを開く
        /// </summary>
        public void OpenEditWindow()
        {
            if (MapSelect != null && MapSelect.IsSelectedSinglePoint)
            {
                var current = MapSelect.SinglePoiint;

                if (current.HasValue)
                {
                    PalletViewModelManager.DoubleClick(
                        CurrentMap, current.Value,
                        chip =>
                        Messenger.Raise(new TransitionMessage(new ObjectChipViewModel((ObjectChip)chip),
                                                              "TransitionObjectChip")));
                }
            }
        }

        #region FlipMapCommand
        private ViewModelCommand _FlipMapCommand;
        public ViewModelCommand FlipMapCommand { get { return _FlipMapCommand ?? (_FlipMapCommand = new ViewModelCommand(FlipMap)); } }
        public void FlipMap()
        {
            if (CurrentMapIsNull) { return; }
            //CurrentMap.FlipHorizontal();
        }
        #endregion

        #region FillMapCommand
        private ViewModelCommand _FillMapCommand;
        public ViewModelCommand FillMapCommand { get { return _FillMapCommand ?? (_FillMapCommand = new ViewModelCommand(FillMap)); } }
        public void FillMap()
        {
            if (CurrentMapIsNull) { return; }
            //CurrentMap.FillMapChip();
        }
        #endregion

        #region QuickPackCommand
        private ViewModelCommand _QuickPackCommand;
        public ViewModelCommand QuickPackCommand { get { return _QuickPackCommand ?? (_QuickPackCommand = new ViewModelCommand(QuickPack)); } }
        public void QuickPack()
        {
            if (CurrentMapIsNull) { return; }
            //CurrentMap.QuickPackChip();
        }
        #endregion

    }
}