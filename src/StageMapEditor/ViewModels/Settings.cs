using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace StageMapEditor.ViewModels
{
    public interface IGeneralSettings
    {
        bool RenderBackground { get; }
        string MapChip { get; }
        string ObjectChip { get; }
        string MapChipList { get; }
        string ObjectChipList { get; }
        string ObjectChipPrototype { get; }
        int CellSize { get; }
        int PalletCellSize { get; }
        bool ReadTextureFromXNB { get; }
        string StageDataDirectory { get; }
        string ResourceDataDirectory { get; }
        bool DrawCurrentCell { get; set; }
        int RenderFrameSecond { get; set; }
    }

    public class Settings : IGeneralSettings
    {
        private string _filePath;
        public bool RenderBackground { get; set; }
        public string MapChip { get; set; }
        public string ObjectChip { get; set; }
        public string MapChipList { get; set; }
        public string ObjectChipList { get; set; }
        public string ObjectChipPrototype { get; set; }
        public int CellSize { get; set; }
        public int PalletCellSize { get; set; }
        public bool ReadTextureFromXNB { get; set; }
        public string StageDataDirectory { get; set; }
        public string ResourceDataDirectory { get; set; }
        public bool DrawCurrentCell { get; set; }
        public int RenderFrameSecond { get; set; }

        public Settings()
        {
            var directoryInfo = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory;
            if (directoryInfo == null)
            {
                throw new Exception("Assembly");
            }

            var loc = directoryInfo.FullName;
            _filePath = Path.Combine(loc, "setting.txt");

            if (!File.Exists(_filePath))
            {
                throw new FileNotFoundException("設定ファイル setting.txt が見つかりません。");
            }

            Load(_filePath);
        }

        public Settings(string path)
        {
            var fi = new FileInfo(path);
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("設定ファイル {0} が見つかりません。", fi.Name);
            }

            _filePath = path;

            Load(_filePath);
        }

        private void Load(string path)
        {
            Initialize();

            var groupRegex = new Regex(@"\[([^\]]+)\]([^\[\]]+)?", RegexOptions.Singleline);

            //コメントを排除
            var stxt = string.Join("\n", File.ReadAllLines(path, Encoding.UTF8).Where(x => !Regex.IsMatch(x, @"\s*#")));

            var sdic = groupRegex.Matches(stxt)
                        .Cast<Match>()
                        .ToDictionary(m => m.Groups[1].Value, m => m.Groups[2].Value);

            ParseGeneralSetting(sdic, "General");
            //ParseObjectParameterSetting(sdic, "ObjectChipParameters");
        }

        private void ParseGeneralSetting(Dictionary<string, string> settingDic, string groupName)
        {
            if (!settingDic.ContainsKey(groupName)) { throw new Exception("設定ファイルに [General] が存在しません"); }

            var lineRegex = new Regex(@"^(.+):(.*)$");

            var dic = Regex.Split(settingDic[groupName], @"[(\n)(\r)(\r\n)]")
                    .Select(x => lineRegex.Match(x))
                    .ToDictionary(
                        x => x.Groups[1].Value.Trim(),
                        x => x.Groups[2].Value.Trim()
                    );

            Func<Type, string, object> convert = (t, v) =>
            {
                //指定された型の Parse メソッドを取得
                var mi = t.GetMethods()
                          .Where(x => x.Name == "Parse")
                          .FirstOrDefault(x => x.GetParameters().Length == 1);
                return mi == null ? v : mi.Invoke(null, new object[] { v });
            };

            foreach (var p in typeof(IGeneralSettings).GetProperties())
            {
                var n = p.Name;
                var t = p.PropertyType;
                if (!dic.ContainsKey(n))
                {
                    throw new Exception(String.Format("{0} が見つかりません。", p));
                }

                GetType().GetProperty(n).SetValue(this, convert(t, dic[n]), null);
            }
        }

        private void ParseObjectParameterSetting(Dictionary<string, string> dic, string groupName)
        {
            if (!dic.ContainsKey(groupName)) { throw new Exception("設定ファイルに [ObjectChip] が存在しません"); }

            var lineRegex = new Regex(@"(.+):([^#]+?)\n");

            //ObjectParameters =
            //    lineRegex.Matches(dic[groupName]).Cast<Match>().Select(x =>
            //    {
            //        var name = x.Groups[1].Value.Trim();
            //        var type = GetTypeFromString(x.Groups[2].Value.Trim());
            //        return new ObjectChipParameter(name, type);
            //    }).ToList();
        }

        private static Type GetTypeFromString(string typeName)
        {
            var allTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes());
            return allTypes.First(x =>
            {
                var t =
                    typeName == "int" ? "System.Int32" :
                    typeName == "float" ? "System.Single" :
                    typeName == "double" ? "System.Double" :
                    typeName == "decimal" ? "System.Decimal" :
                    typeName == "string" ? "System.String" :
                    typeName;
                return x.FullName == t || x.AssemblyQualifiedName == t;
            });
        }

        /// <summary>
        /// 初期値の設定
        /// </summary>
        private void Initialize()
        {
            RenderBackground = false;
            ReadTextureFromXNB = false;
            StageDataDirectory = "./Stage";
            ResourceDataDirectory = "./Resource";
        }
    }
}
