using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using Livet;
using StageMapEditor.Helper;
using StageMapEditor.Models.Chip;
using StageMapEditor.Models.Commands;
using StageMapEditor.ViewModels;

namespace StageMapEditor.Models
{
    [Serializable]
    public partial class MapModel : NotificationObject
    {
        private StageModel _parent;
        public LayerManager LayerManager = new LayerManager();
        public StageModel StageModel { get { return _parent; } }

        public MapChipModel MapChipModel { get; set; }
        public ObjectChipModel ObjectChipModel { get; set; }

        public List<IChipModel> ChipModels { get; private set; }

        /// <summary>
        /// ファイルからMapChipModelとObjectChipModelを読み込んだかどうか
        /// </summary>
        public bool IsLoaded;

        bool _IsSelected = true;
        public bool IsSelected
        {
            get { return _IsSelected; }
            set
            {
                if (_IsSelected != value)
                {
                    _IsSelected = value;
                    RaisePropertyChanged("IsSelected");
                }
            }
        }

        /// <summary>
        /// Mapの選択状態を表すクラス
        /// </summary>
        public MapSelect MapSelect { get; set; }

        /// <summary>
        /// 操作履歴を管理するクラス
        /// </summary>
        private Memento _memento;
        public void ClearHistory() { _memento.Clear(); }
        public bool CanUndo { get { return _memento.CanUndo; } }
        public bool CanRedo { get { return _memento.CanRedo; } }

        /// <summary>
        /// Mapのスクロールバーの位置
        /// </summary>
        public Point ScrollPosition { get; set; }

        /// <summary>
        /// 背景に表示する画像の絶対パス
        /// </summary>
        public string BackgroundFilePath { get; set; }

        /// <summary>
        /// 背景画像が存在するかどうか
        /// </summary>
        public bool ExistsBackground { get; set; }

        #region Scale
        private float _Scale = 1f;
        /// <summary>
        /// 拡大縮小の倍率
        /// </summary>
        public float Scale
        {
            get { return _Scale; }
            set
            {
                if (_Scale != value)
                {
                    _Scale = value;
                    RaisePropertyChanged("Scale");
                    RaisePropertyChanged("MapWidth");
                    RaisePropertyChanged("MapHeight");
                    RaisePropertyChanged("ScaledGridSize");
                }
            }
        }
        #endregion

        /// <summary>
        /// 何も無い状態のコンストラクタ
        /// </summary>
        /// <param name="inputViewModel"></param>
        /// <param name="parent"></param>
        public MapModel(EditMapViewModel inputViewModel, StageModel parent)
        {
            Initialize(parent);
            SetProperties(inputViewModel, parent);

            this.MapChipModel = new MapChipModel(this, MapCellWidth, MapCellHeight);
            this.ObjectChipModel = new ObjectChipModel(this);

            SetEvent();
            ChangeBackground();
        }

        /// <summary>
        /// MessagePackのデータを使って初期化を行う
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="mapModelPack"></param>
        public MapModel(MapModelPack mapModelPack, StageModel parent)
        {
            Initialize(parent);
            SetProperties(mapModelPack);

            MapChipModel = new MapChipModel(this, mapModelPack.MapChipPack);
            ObjectChipModel = new ObjectChipModel(this, mapModelPack.ObjectChipPack);

            SetEvent();
            ChangeBackground();
        }

        private void SetEvent()
        {
            this.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Background")
                {
                    ChangeBackground();
                }
            };
        }

        private void ChangeBackground()
        {
            var fi = new FileInfo(Path.Combine(_parent.Settings.StageDataDirectory, this.Background));

            ExistsBackground = fi.Exists;
            BackgroundFilePath = ExistsBackground ? fi.FullName : null;
            RaisePropertyChanged("CurrentMap");
        }

        /// <summary>
        /// 初期化メソッド
        /// </summary>
        /// <param name="parent"></param>
        private void Initialize(StageModel parent)
        {
            _parent = parent;
            _memento = new Memento();
            MapSelect = new MapSelect();
            ScrollPosition = new Point(0, 0);
        }

        /// <summary>
        /// 親インスタンスに自身が含まれているかチェック
        /// </summary>
        public bool IsInCludeCollection
        {
            get { return _parent.MapModels.Contains(this); }
        }

        /// <summary>
        /// 親インスタンスに自身を追加
        /// </summary>
        public void AddThisToCollection() { _parent.MapModels.Add(this); }

        /// <summary>
        /// 履歴にCommandを追加
        /// </summary>
        /// <param name="command"></param>
        public void AddHistory(IMapCommand command) { _memento.AddUndoCommand(command); }

        /// <summary>
        /// Undoの実行
        /// </summary>
        public void Undo()
        {
            _memento.UndoCommand(this).Execute(this);
            _parent.RaisePropertyChangedFromMapModel();
        }

        /// <summary>
        /// Redoの実行
        /// </summary>
        public void Redo()
        {
            _memento.RedoCommand(this).Execute(this);
            _parent.RaisePropertyChangedFromMapModel();
        }

        /// <summary>
        /// 幅と高さが変更になったときの処理
        /// </summary>
        public void EditWidthHeight()
        {
            if (MapChipModel == null || ObjectChipModel == null)
                return;

            MapChipModel.EditWidthHeight(MapCellWidth, MapCellHeight);
            ObjectChipModel.EditWidthHeight(MapCellWidth, MapCellHeight);
        }

        /// <summary>
        /// StageModelを更新
        /// </summary>
        public void UpdateParent()
        {
            _parent.SaveFile();
        }

        /// <summary>
        /// Mapの切り替え
        /// </summary>
        public void ChangeMap()
        {
            _parent.ChangeMap(this);

            foreach (var m in _parent.MapModels)
            {
                m.IsSelected = false;
            }

            IsSelected = true;
        }

        /// <summary>
        /// このMapModelをParentから削除する
        /// </summary>
        public void DeleteMap()
        {
            _parent.Remove(this);
            _parent.RaisePropertyChangedFromMapModel();
        }

        /// <summary>
        /// 現在の状態をファイルに保存
        /// </summary>
        private void SaveFiles()
        {
            _parent.Save(this);
        }

        public void Save()
        {
            if (!IsInCludeCollection)
            {
                AddThisToCollection();
            }

            EditWidthHeight();
            SaveFiles();

            RaisePropertyChanged("CurrentMap");
        }

        /// <summary>
        /// Map内のオブジェクトの左右反転を行う
        /// </summary>
        public void FlipHorizontal()
        {
            var command = new MapCommandAll(this);
            _memento.AddUndoCommand(command);

            MapChipModel.FlipHorizontal();
            ObjectChipModel.FlipHorizontal();
        }

        public IEnumerable<Tuple<System.Drawing.Point, MapChip, ObjectChip>> ListAllWithPosition()
        {
            return MapChipModel.ListAllWithPosition()
                                .Select(x => Tuple.Create(x.Item1, x.Item2, ObjectChipModel.Get(x.Item1)));
        }

        #region 拡大縮小関連

        public static List<float> ScaleList = new List<float> { 0.125f, 0.1667f, 0.25f, 0.3333f, 0.50f, 0.6667f, 1f, };

        /// <summary>
        /// Scaleの縮小
        /// </summary>
        public void ScaleDown()
        {
            if (CanScaleDown())
            {
                Scale = ScaleList.OrderByDescending(x => x).ToList().Find(x => x < Scale);
            }
        }

        /// <summary>
        /// Scaleの拡大
        /// </summary>
        public void ScaleUp()
        {
            if (CanScaleUp())
            {
                Scale = ScaleList.Find(x => x > Scale);
            }
        }

        /// <summary>
        /// Scaleの等倍
        /// </summary>
        public void ScaleNormal()
        {
            Scale = 1f;
        }

        /// <summary>
        /// Scaleダウンできるかどうか
        /// </summary>
        /// <returns></returns>
        public bool CanScaleDown() { return Scale > 0.125f; }

        /// <summary>
        /// Scaleアップできるかどうか
        /// </summary>
        /// <returns></returns>
        public bool CanScaleUp() { return Scale < 1f; }
        #endregion
    }
}