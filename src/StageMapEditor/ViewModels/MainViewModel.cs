using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Livet;
using Livet.EventListeners;
using StageMapEditor.Models.Chip;
using StageMapEditor.Models.MousePosition;

using StageMapEditor.Models;
using StageMapEditor.Models.Commands;
using StageMapEditor.Models.Drawing;
using StageMapEditor.Views;

namespace StageMapEditor.ViewModels
{
    /// <summary>
    /// MainWindowに関連づけられるViewModel
    /// </summary>
    public partial class MainViewModel : ViewModel, IMapControlViewModel
    {
        /// <summary>
        /// Model部分
        /// </summary>
        private readonly MainModel _model;

        /// <summary>
        /// このマップのオブジェクトの数
        /// </summary>
        public int MapObjectCount
        {
            get
            {
                if (CurrentMapIsNull)
                {
                    return 0;
                }

                return CurrentMap.ObjectChipModel.ListAll().Count();
            }
        }

        public ReadOnlyDispatcherCollection<StageViewModel> StageViewModels { get; private set; }
        public ReadOnlyDispatcherCollection<MapViewModel> MapViewModels { get; private set; }

        /// <summary>
        /// 現在選択中のMapModel
        /// </summary>
        private MapModel CurrentMap { get { return _model.CurrentMap; } }


        /// <summary>
        /// 現在のMapModelがNullかどうか
        /// </summary>
        public bool CurrentMapIsNull { get { return _model.CurrentMap == null; } }

        /// <summary>
        /// 現在選択中のMapModelのMapChipModel
        /// </summary>
        public MapChipModel MapChipModel { get { return CurrentMapIsNull ? null : CurrentMap.MapChipModel; } }

        /// <summary>
        /// 現在選択中のMapModelのObjectChipModel
        /// </summary>
        public ObjectChipModel ObjectChipModel { get { return CurrentMapIsNull ? null : CurrentMap.ObjectChipModel; } }

        IGeneralSettings IRenderingViewModel.Settings { get { return Settings; } }

        public string BackgroundImagePath { get { return CurrentMapIsNull ? null : Path.Combine(Settings.StageDataDirectory, CurrentMap.Background); } }
        public string BackgroundFilePath { get { return CurrentMapIsNull ? null : CurrentMap.BackgroundFilePath; } }
        public bool ExistsBackground { get { return !CurrentMapIsNull && CurrentMap.ExistsBackground; } }

        /// <summary>
        /// 編集を一時的にプールしておくリスト
        /// </summary>
        private readonly List<IMapCommand> _commandPool = new List<IMapCommand>();
        public List<IMapCommand> CommandPool { get { return _commandPool; } }

        /// <summary>
        /// 描画領域幅
        /// </summary>
        public int MapWidth { get { return _model.MapWidth; } }

        /// <summary>
        /// 描画領域高さ
        /// </summary>
        public int MapHeight { get { return _model.MapHeight; } }

        /// <summary>
        /// 現在のマップのレイヤー管理オブジェクト
        /// </summary>
        public LayerManager CurrentLayerManager
        {
            get
            {
                if (CurrentMapIsNull)
                {
                    return null;
                }

                return CurrentMap.LayerManager;
            }
        }

        public bool VisibleBorderLayer { get { return !CurrentMapIsNull && CurrentLayerManager.Border.Visible; } }
        public bool VisibleMapChipLayer { get { return !CurrentMapIsNull && CurrentLayerManager.MapChip.Visible; } }
        public bool VisibleObjectChipLayer { get { return !CurrentMapIsNull && CurrentLayerManager.ObjectChip.Visible; } }

        /// <summary>
        /// マップの横幅セル
        /// </summary>
        public int MapCellWidth { get { return _model.MapCellWidth; } }

        /// <summary>
        /// マップの縦幅セル
        /// </summary>
        public int MapCellHeight { get { return _model.MapCellHeight; } }

        //Mapのスクロールバーの位置
        public Point MapScrollPosition
        {
            get { return CurrentMap == null ? new Point() : CurrentMap.ScrollPosition; }
            set
            {
                if (CurrentMap == null) return;
                CurrentMap.ScrollPosition = value;
            }
        }

        /// <summary>
        /// 現在のマップの背景画像ファイル名
        /// </summary>
        public string CurrentBackground
        {
            get
            {
                if (CurrentMapIsNull)
                {
                    return null;
                }

                return CurrentMap.Background;
            }
        }

        /// <summary>
        /// Gridのサイズ
        /// </summary>
        public int GridSize { get { return _model.GridSize; } }
        public int ScaledGridSize { get { return (int)(_model.GridSize * Scale); } }

        /// <summary>
        /// 拡大縮小の倍率
        /// </summary>
        public float Scale { get { return CurrentMapIsNull ? 1f : CurrentMap.Scale; } }

        public Point CurrentPosition { get { return _model.CurrentPosition; } }
        public MousePosition CurrentMousePosition { get { return _model.CurrentMousePosition; } }
        public string CurrentPositionInformation { get { return _model.CurrentPositionInformation; } }
        public MapChip CurrentPositionMapChip { get { return _model.CurrentPositionMapChip; } }
        public ObjectChip CurrentPositionObjectChip { get { return _model.CurrentPositionObjectChip; } }

        /// <summary>
        /// 現在選択中のマップの選択状態
        /// </summary>
        public MapSelect MapSelect { get { return CurrentMap == null ? null : CurrentMap.MapSelect; } }

        public double TreeViewWidth { get { return Properties.Settings.Default.TreeViewWidth; } }
        public double PalletWidth { get { return Properties.Settings.Default.PalletWidth; } }
        public double WindowWidth { get { return Properties.Settings.Default.WindowWidth; } }
        public double WindowHeight { get { return Properties.Settings.Default.WindowHeight; } }

        public PalletViewModelManager PalletViewModelManager;
        public MapChipPalletViewModel MapChipPalletViewModel { get; set; }
        public ObjectChipPalletViewModel ObjectChipPalletViewModel { get; set; }

        #region FontSize
        private double _FontSize;
        public double FontSize
        {
            get { return _FontSize; }
            set
            {
                if (_FontSize != value)
                {
                    _FontSize = value;
                    RaisePropertyChanged("FontSize");
                }
            }
        }
        #endregion

        public Settings Settings { get; private set; }

        public PropertyChangedEventListener Listener { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainViewModel()
        {
            //設定ファイルの読み込み
            Settings = new Settings();

            //セレクターのサイズの初期化
            MouseSelectorPosition.GridSize = Settings.PalletCellSize;
            FontSize = 9;
            StageModel.Dir = new DirectoryInfo(Settings.StageDataDirectory);

            _model = new MainModel(Settings);

            MapChipPalletViewModel = new MapChipPalletViewModel(Settings);
            ObjectChipPalletViewModel = new ObjectChipPalletViewModel(Settings);

            MapChipPalletViewModel.PalletEnterEvent += chip => _model.CurrentPositionMapChip = chip.ID == 0 ? null : chip;
            ObjectChipPalletViewModel.PalletEnterEvent += chip => _model.CurrentPositionObjectChip = chip.ID == 0 ? null : chip;

            PalletViewModelManager = new PalletViewModelManager(MapChipPalletViewModel, ObjectChipPalletViewModel);

            StageViewModels = ViewModelHelper.CreateReadOnlyDispatcherCollection(
                _model.StageModels,
                m =>
                {
                    var vm = new StageViewModel(m, this);
                    vm.PropertyChanged += (sender, args) => RaiseModelPropertyChanged(args.PropertyName);
                    return vm;
                },
                DispatcherHelper.UIDispatcher);

            //TreeViewの要素リストを監視
            MapViewModels = ViewModelHelper.CreateReadOnlyDispatcherCollection(
                _model.MapModels,
                model =>
                {
                    var vm = new MapViewModel(model);
                    vm.PropertyChanged += (sender, args) => RaiseModelPropertyChanged(args.PropertyName);
                    return vm;
                },
                DispatcherHelper.UIDispatcher);

            //MainModelのプロパティ変更を監視
            Listener = new PropertyChangedEventListener(_model);
            Listener.RegisterHandler((sender, args) => RaisePropertyChanged(args.PropertyName));
            Listener.RegisterHandler("CurrentMap", (sender, args) => CurrentMapChenged());
            Listener.RegisterHandler("Scale", (sender, args) =>
            {
                RaisePropertyChanged("MapWidth");
                RaisePropertyChanged("MapHeight");
            });
            Listener.RegisterHandler("CurrentPosition", (sender, args) => _model.UpdateMousePositionProperty());
            CompositeDisposable.Add(Listener);

            ChangeMapInitialize();
        }

        public void RaiseModelPropertyChanged(string propertyName)
        {
            RaisePropertyChanged(propertyName);

            if (propertyName.StartsWith("MapCell"))
            {
                RaisePropertyChanged("MapWidth");
                RaisePropertyChanged("MapHeight");
            }
        }

        /// <summary>
        /// ファイルを開くとき
        /// </summary>
        /// <param name="filePath"></param>
        public void LoadFile(string filePath)
        {
            //XMLファイルでない場合は何もしない
            if (/*filePath.ToLower().EndsWith(".xml") || */ filePath.ToLower().EndsWith(".dsm"))
            {
                _model.ParseFile(filePath);
            }
        }

        /// <summary>
        /// レイヤー表示部分の更新とマップの再描画
        /// </summary>
        private void CurrentMapChenged()
        {
            RaisePropertyChanged("MapWidth");
            RaisePropertyChanged("MapHeight");
            RaisePropertyChanged("MapCellWidth");
            RaisePropertyChanged("MapCellHeight");

            RaisePropertyChanged("VisibleBorderLayer");
            RaisePropertyChanged("VisibleMapChipLayer");
            RaisePropertyChanged("VisibleObjectChipLayer");

            CommandCanExecuteChanged();
        }

        public System.Drawing.Point GetMapChipCropPosition(int id)
        {
            return MapChipPalletViewModel.GetPosition(id);
        }

        public System.Drawing.Point GetObjChipCropPosition(int id)
        {
            return ObjectChipPalletViewModel.GetPosition(id);
        }

        /// <summary>
        /// CurrentMapの切り換え後にDrawStateの初期化などを行う
        /// </summary>
        private void ChangeMapInitialize()
        {
            _commandPool.Clear();
            ChangeDrawState(DrawState.MapChip, MapChip.Empty.ID);
        }

        /// <summary>
        /// DrawStateを切り替える
        /// </summary>
        /// <param name="drawState"></param>
        /// <param name="id"> </param>
        public void ChangeDrawState(DrawState drawState, int id)
        {
            //現在のMapModelがNullだったら何もせず終了
            if (CurrentMap == null) return;

            PalletViewModelManager.ChangeDrawState(drawState, id);
        }

        /// <summary>
        /// MapTabControlのタブを変更。
        /// カレントのStageを変更する
        /// </summary>
        /// <param name="sender"></param>
        public void ChangeTab(TabControl sender)
        {
            var stageVM = (StageViewModel)sender.SelectedItem;

            _model.ChangeTab(stageVM);
        }
    }
}
