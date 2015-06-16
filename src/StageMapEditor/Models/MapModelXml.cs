using System;
using System.Xml.Serialization;

namespace StageMapEditor.Models
{
    [Serializable]
    public class MapModelXml
    {
        #region Xml要素
        //・背景	string
        //・BGM	int
        //・マップ名	string
        //・Xサイズ	int
        //・Yサイズ	int
        //・強制スクロール角度	int
        //・強制スクロール速度	float
        //"・Subparam
        //（予備情報　特殊状況の判定に）"	int

        public int ID;
        public string MapName;
        public string Background;
        [XmlElement("Width")]
        public int CellWidth;
        [XmlElement("Height")]
        public int CellHeight;

        #endregion

        //public MapModel ToModel(StageModel parent)
        //{
        //    return new MapModel(parent, this);
        //}
    }
}