using System;
using System.Xml.Serialization;

namespace StageMapEditor.Models
{
    [Serializable]
    public class MapModelXml
    {
        #region Xml�v�f
        //�E�w�i	string
        //�EBGM	int
        //�E�}�b�v��	string
        //�EX�T�C�Y	int
        //�EY�T�C�Y	int
        //�E�����X�N���[���p�x	int
        //�E�����X�N���[�����x	float
        //"�ESubparam
        //�i�\�����@����󋵂̔���Ɂj"	int

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