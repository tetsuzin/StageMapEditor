using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Xml.Serialization;
using Livet;
using Livet.EventListeners;
using MsgPack.Serialization;
using StageMapEditor.Helper;
using StageMapEditor.ViewModels;

namespace StageMapEditor.Models
{
    //[SerializableAttribute]
    //public class StageModelXml
    //{
    //    public string StageName;
    //    public int World;
    //    public int Stage;
    //    public string StageDescription;
    //    public List<MapModelXml> MapModelList;
    //}

    public partial class StageModel : NotificationObject
    {
        public ObservableCollection<MapModel> MapModels { get; private set; }

        public string StageFileName { get; set; }
        public static DirectoryInfo Dir { get; set; }

        public MainModel Parent { get; private set; }

        private MapModel _CurrentMap;
        public MapModel CurrentMap
        {
            get { return _CurrentMap; }
            set
            {
                if (_CurrentMap == value) { return; }
                _CurrentMap = value;
                RaisePropertyChanged("CurrentMap");
            }
        }

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
        /// 設定ファイル
        /// </summary>
        public IGeneralSettings Settings { get { return Parent.Settings; } }

        private static MessagePackSerializer<StageModelPack> _serializer =
            MessagePackSerializer.Create<StageModelPack>();

        /// <summary>
        /// 保存ファイル情報
        /// </summary>
        public FileInfo FileInfo { get; set; }

        private static MD5 _md5 = MD5.Create();
        private static string ComputeHash(Stream st)
        {
            var bytes = new byte[st.Length];
            st.Read(bytes, 0, bytes.Length);
            return BitConverter.ToString(_md5.ComputeHash(bytes)).ToLower().Replace("-", "");
        }

        /// <summary>
        /// EditViewModelからSatgeModelを生成
        /// </summary>
        /// <param name="editViewModel"></param>
        /// <param name="parent"></param>
        public StageModel(EditStageViewModel editViewModel, MainModel parent)
        {
            Initialize(parent);

            SetProperties(editViewModel);

            World = Int32.Parse(editViewModel.InputWorld);
            Stage = Int32.Parse(editViewModel.InputStage);
            StageName = editViewModel.InputStageName;
            StageDescription = editViewModel.InputStageDescription;
            CurrentMap = MapModels.FirstOrDefault();
        }

        /// <summary>
        /// ファイルからStageModelを生成
        /// </summary>
        /// <param name="fi"></param>
        /// <param name="parent"></param>
        public StageModel(FileInfo fi, MainModel parent)
        {
            Initialize(parent);

            FileInfo = fi;
            SerializeMessagePack(FileInfo.FullName);
            CurrentMap = MapModels.FirstOrDefault();
        }

        private void Initialize(MainModel parent)
        {
            Parent = parent;
            MapModels = new ObservableCollection<MapModel>();
        }

        /// <summary>
        /// バイナリファイルからシリアライズして、値をセットする。
        /// </summary>
        /// <param name="path"></param>
        private void SerializeMessagePack(string path)
        {
            using (var fs = new FileStream(path, FileMode.Open))
            using (var ms = new MemoryStream())
            {
                var model = _serializer.Unpack(fs);

                StageFileName = path;
                SetProperties(model);
                World = model.World;
                Stage = model.Stage;
                StageName = model.StageName;
                StageDescription = model.StageDescription;
                MapModels = new ObservableCollection<MapModel>(model.MapModel.Select(x => new MapModel(x, this)));

                _serializer.Pack(ms, model);

                fs.Position = 0;
                StageHash = ComputeHash(ms);
            }
        }

        protected string StageHash { get; set; }

        /// <summary>
        /// MainModelにこのインスタンスが含まれているかチェック
        /// </summary>
        public bool IsInCludeCollection
        {
            get { return Parent.StageModels.Contains(this); }
        }

        /// <summary>
        /// MainModelにこのインスタンスを追加
        /// </summary>
        public void AddThisToCollection()
        {
            Parent.StageModels.Add(this);
        }

        /// <summary>
        /// 次のMapIDを取得
        /// </summary>
        /// <returns></returns>
        public int GetNextID()
        {
            return MapModels.Count == 0 ? 0 : MapModels.Max(x => x.ID) + 1;
        }

        /// <summary>
        /// 現在のインスタンスをファイルに保存
        /// </summary>
        public void SaveFile()
        {
            if (StageFileName == null)
            {
                var dialog = FileDialogHelpers.SaveStageFileDialog(Dir.FullName, String.Format("{0}_{1}.dsm", World, Stage));

                if (dialog.ShowDialog() == true)
                {
                    StageFileName = dialog.FileName;
                }
                else
                {
                    return;
                }
            }

            using (var fs = new FileStream(StageFileName, FileMode.OpenOrCreate))
            using (var ms = new MemoryStream())
            {
                var serializer = MessagePackSerializer.Create<StageModelPack>();
                var model = ToMsgPack();

                //ファイルに保存
                serializer.Pack(fs, model);

                serializer.Pack(ms, model);
                StageHash = ComputeHash(ms);

                //RaisePropertyChanged("CurrentMap");
            }

            MessageBox.Show("保存しました。");
            //XmlSerializerHelper.Serialize(FindRootData(), ToXml());
        }

        public StageModelPack ToMsgPack()
        {
            var sm = new StageModelPack()
                {
                    World = World,
                    Stage = Stage,
                    StageName = StageName,
                    StageDescription = StageDescription,
                    MapModel = MapModels.OrderBy(x => x.ID).Select(x => x.ToMsgPack()).ToArray()
                };

            return sm;
        }

        public bool IsEdited()
        {
            using (var ms = new MemoryStream())
            {
                var model = ToMsgPack();
                _serializer.Pack(ms, model);
                return StageHash != ComputeHash(ms);
            }
        }

        /// <summary>
        /// StageからMapをリムーブ
        /// </summary>
        /// <param name="mapModel"></param>
        public void Remove(MapModel mapModel)
        {
            MapModels.Remove(mapModel);
            //削除したあと、現在マップの変更
            CurrentMap = MapModels.FirstOrDefault();
        }

        /// <summary>
        /// マップの切り替えを行うメソッド
        /// </summary>
        /// <param name="mapModel"></param>
        public void ChangeMap(MapModel mapModel)
        {
            Parent.ChangeMap(mapModel);

            if (!MapModels.Contains(mapModel))
                throw new ArgumentException("この mapModel は MapList に含まれていません");

            CurrentMap = mapModel;
        }

        /// <summary>
        /// SaveCommandから呼び出される処理
        /// </summary>
        public void Save()
        {
            if (!IsInCludeCollection)
            {
                //Createの処理
                AddThisToCollection();
            }

            //Updateの処理
            SaveFile();
        }

        /// <summary>
        /// 新しい名前で保存
        /// </summary>
        public void SaveNew()
        {
            var dialog = FileDialogHelpers.SaveStageFileDialog(Dir.FullName, String.Format("{0}_{1}.dsm", World, Stage));

            if (dialog.ShowDialog() == true)
            {
                StageFileName = dialog.FileName;
            }
            else
            {
                return;
            }

            Save();
        }

        public void Save(MapModel mapModel)
        {
            if (!IsInCludeCollection)
            {
                //Createの処理
                AddThisToCollection();
            }

            //Updateの処理
            SaveFile();
        }

        public void ChangeStage()
        {
            Parent.CurrentStage = this;
        }

        public void RaisePropertyChangedFromMapModel()
        {
            RaisePropertyChanged("CurrentMap");
        }

        /// <summary>
        /// 対象のMapModelが含まれているかどうか
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Contains(MapModel model)
        {
            return MapModels.Contains(model);
        }
    }
}
