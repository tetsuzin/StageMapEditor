using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using Livet;
using Livet.EventListeners;
using StageMapEditor.Models.Chip;
using StageMapEditor.ViewModels;
using StageMapEditor.Models.MousePosition;

namespace StageMapEditor.Models
{
    public class MainModel : NotificationObject
    {
        /*
         * NotificationObjectはプロパティ変更通知の仕組みを実装したオブジェクトです。
         */

        /*
         * ModelからViewModelへのイベントを発行する場合はNotificatorを使用してください。
         *
         * Notificatorはイベント代替手段です。コードスニペット lnev でCLRイベントと同時に定義できます。
         *
         * ViewModelへNotificatorを使用した通知を行う場合はViewModel側でViewModelHelperを使用して受信側の登録をしてください。
         */

        public ObservableCollection<StageModel> StageModels { get; private set; }
        public ObservableCollection<MapModel> MapModels { get; private set; }
        public IGeneralSettings Settings { get; set; }

        #region 選択中のStageModel
        StageModel _CurrentStage;

        public StageModel CurrentStage
        {
            get { return _CurrentStage; }
            set
            {
                if (_CurrentStage == value) { return; }
                _CurrentStage = value;
                RaisePropertyChanged("CurrentStage");

                if (_CurrentStage != null)
                {
                    RaisePropertyChanged("CurrentMap");
                }

                CurrentMap = _CurrentStage == null ? null : _CurrentStage.CurrentMap;

                ResetMapModels(_CurrentStage);
            }
        }
        #endregion

        #region 現在選択中のMapModel
        MapModel _CurrentMap;

        public MapModel CurrentMap
        {
            get { return _CurrentMap; }
            set
            {
                if (_CurrentMap == value) { return; }
                _CurrentMap = value;
                RaisePropertyChanged("CurrentMap");

                if (CurrentMap != null)
                {
                    foreach (var m in MapModels)
                        m.IsSelected = false;

                    CurrentMap.IsSelected = true;
                }
            }
        }
        #endregion

        #region IRenderingViewModel
        /// <summary>
        /// 現在のMapの横幅（セル数）
        /// </summary>
        public int MapCellWidth { get { return CurrentMap == null ? 0 : CurrentMap.MapCellWidth; } }

        /// <summary>
        /// 現在のMapの縦幅（セル数）
        /// </summary>
        public int MapCellHeight { get { return CurrentMap == null ? 0 : CurrentMap.MapCellHeight; } }

        /// <summary>
        /// ベースとなるGridのサイズ
        /// </summary>
        public int GridSize = 40; //40はデフォルト値。コンストラクタで設定ファイルから読み込む。

        /// <summary>
        /// 拡大縮小状態
        /// </summary>
        public float Scale { get { return CurrentMap == null ? 1f : CurrentMap.Scale; } }

        /// <summary>
        /// 拡大縮小適用後のグリッドサイズ
        /// </summary>
        public int ScaledGridSize { get { return (int)(GridSize * Scale); } }

        /// <summary>
        /// マップの横幅（px）
        /// </summary>
        public int MapWidth { get { return CurrentMap == null ? 0 : CurrentMap.MapCellWidth * ScaledGridSize; } }

        /// <summary>
        /// マップの縦幅（px）
        /// </summary>
        public int MapHeight { get { return CurrentMap == null ? 0 : CurrentMap.MapCellHeight * ScaledGridSize; } }
        #endregion


        #region マウスの現在地座標とその情報
        private Point _CurrentPosition;
        public Point CurrentPosition
        {
            get { return _CurrentPosition; }
            set
            {
                if (_CurrentPosition == value) { return; }
                _CurrentPosition = value;
                RaisePropertyChanged("CurrentPosition");
            }
        }

        private MousePosition.MousePosition _CurrentMousePosition;
        public MousePosition.MousePosition CurrentMousePosition
        {
            get { return _CurrentMousePosition; }
            set
            {
                if (_CurrentMousePosition == value) { return; }
                _CurrentMousePosition = value;
                RaisePropertyChanged("CurrentMousePosition");
            }
        }

        private string _CurrentPositionInformation;
        public string CurrentPositionInformation
        {
            get { return _CurrentPositionInformation; }
            set
            {
                if (_CurrentPositionInformation == value) { return; }
                _CurrentPositionInformation = value;
                RaisePropertyChanged("CurrentPositionInformation");
            }
        }

        //現在マウスの位置にあるマップチップ
        MapChip _CurrentPositionMapChip;
        public MapChip CurrentPositionMapChip
        {
            get
            { return _CurrentPositionMapChip; }
            set
            {
                if (_CurrentPositionMapChip == value)
                    return;
                _CurrentPositionMapChip = value;
                RaisePropertyChanged("CurrentPositionMapChip");
            }
        }

        //現在マウスの位置にあるオブジェクトチップ
        ObjectChip _CurrentPositionObjectChip;
        public ObjectChip CurrentPositionObjectChip
        {
            get { return _CurrentPositionObjectChip; }
            set
            {
                if (_CurrentPositionObjectChip == value)
                    return;
                _CurrentPositionObjectChip = value;
                RaisePropertyChanged("CurrentPositionObjectChip");
            }
        }
        #endregion

        /// <summary>
        /// マウス位置更新時に実行されるメソッド
        /// </summary>
        public void UpdateMousePositionProperty()
        {
            if (CurrentMap == null)
                return;

            var pos = CurrentMousePosition.GridPoint;

            if (MapCellWidth > pos.X && MapCellHeight > pos.Y)
            {
                var mapChip = CurrentMap.MapChipModel.Get(CurrentMousePosition.GridPoint);
                var objChip = CurrentMap.ObjectChipModel.Get(CurrentMousePosition.GridPoint);

                //現在地のセル情報の更新
                CurrentPositionMapChip = mapChip.ID == 0 ? null : mapChip;
                CurrentPositionObjectChip = objChip.ID == 0 ? null : objChip;

                CurrentPositionInformation = GetCurrentPositionInformation(CurrentPositionMapChip, CurrentPositionObjectChip);
            }
        }

        private string GetCurrentPositionInformation(MapChip mapchip, ObjectChip objectChip)
        {
            var pos = string.Format("横 : {0}  縦 : {1}", MapCellWidth, MapCellHeight);
            var mc = mapchip == null ? "" : string.Format("MapChipID : {0}", mapchip.ID);
            var oc = objectChip == null ? "" : string.Format("ObjectChipID : {0}  Status : {1}  Parameter : {2}  SubParam1 : {3}  SubParam2 : {4}",
                objectChip.ID, objectChip.Status, objectChip.Param, objectChip.SubParam1, objectChip.SubParam2);

            return string.Join("  ", new[] { pos, mc, oc });
        }

        public MainModel(IGeneralSettings settings)
        {
            Settings = settings;
            StageModels = new ObservableCollection<StageModel>();
            MapModels = new ObservableCollection<MapModel>();

            StageModels.CollectionChanged += (sender, args) => ResetMapModels();

            MapModels.CollectionChanged += (sender, args) =>
                {
                    if (args.Action == NotifyCollectionChangedAction.Add)
                        CurrentMap = args.NewItems.Cast<MapModel>().FirstOrDefault();
                };

            //Gridの初期値の設定
            GridSize = Settings.CellSize;

            //MousePositionのターゲットとするMainModelの引き渡し
            MousePosition.MousePosition.Model = this;
        }

        /// <summary>
        /// 指定したStageModelでMapリストのリセット
        /// </summary>
        /// <param name="stageModel"></param>
        public void ResetMapModels(StageModel stageModel)
        {
            MapModels.Clear();

            foreach (var stage in StageModels)
                stage.IsSelected = stage == stageModel;

            if (stageModel != null)
            {
                foreach (var m in stageModel.MapModels)
                    MapModels.Add(m);
            }
        }

        /// <summary>
        /// 現在のStageでMapリストのリセット
        /// </summary>
        public void ResetMapModels()
        {
            MapModels.Clear();

            if (CurrentStage != null)
            {
                foreach (var m in CurrentStage.MapModels)
                    MapModels.Add(m);

                CurrentMap = CurrentStage.MapModels.FirstOrDefault();
            }
        }

        /// <summary>
        /// マップの変更
        /// </summary>
        /// <param name="mapModel"></param>
        public void ChangeMap(MapModel mapModel)
        {
            CurrentMap = mapModel;
        }

        /// <summary>
        /// ステージファイルのパース
        /// </summary>
        /// <param name="filePath"></param>
        public void ParseFile(string filePath)
        {
            var stage = new StageModel(new FileInfo(filePath), this);

            AddStage(stage);
        }
        /// <summary>
        /// StageModelのリストに読み込んだファイルを追加。
        /// 読み込んだステージをカレントに設定。
        /// </summary>
        /// <param name="stage"></param>
        public void AddStage(StageModel stage)
        {
            //すでに含まれていたら何もしない
            if (Contains(stage)) return;

            StageModels.Add(stage);
            CurrentStage = stage;

            //初期選択のMapを設定する
            CurrentMap = CurrentStage.MapModels.OrderBy(x => x.ID).FirstOrDefault();
        }

        /// <summary>
        /// StageModelsに既にそのステージが含まれているかどうか
        /// </summary>
        /// <param name="stageModel"></param>
        /// <returns></returns>
        public bool Contains(StageModel stageModel)
        {
            return StageModels.Contains(stageModel);
        }

        /// <summary>
        /// タブの変更イベント
        /// </summary>
        /// <param name="stageVm"></param>
        public void ChangeTab(StageViewModel stageVm)
        {
            CurrentStage = stageVm == null || StageModels.Count == 0 ? null : StageModels.FirstOrDefault(stageVm.HasModel);
        }
    }
}
