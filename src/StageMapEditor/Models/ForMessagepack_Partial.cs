

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StageMapEditor.Models
{
    /// <summary>
    /// MessagePack用の保存クラス
    /// </summary>
    public partial class StageModelPack
    {
        public Int32 World { get; set; }
        public Int32 Stage { get; set; }
        public String StageName { get; set; }
        public String StageDescription { get; set; }
    }

    public partial class MapModelPack
    {
        public Int32 ID { get; set; }
        public String MapName { get; set; }
        public String Background { get; set; }
        public Int32 BgNo { get; set; }
        public Int32 BGMNo { get; set; }
        public Int32 ScrollSpeed { get; set; }
        public Int32 ScrollAngle { get; set; }
        public Int32 MapChipType { get; set; }
        public Int32 TimeLimit { get; set; }
    }

    [DebuggerDisplay("ID = {ID}")]
    public partial class MapChipPack
    {
        public Int32 ID { get; set; }
    }

    public partial class ObjectChipPack
    {
        public Int32 ID { get; set; }
        public Single Status { get; set; }
        public Single Param { get; set; }
        public Single SubParam1 { get; set; }
        public Single SubParam2 { get; set; }
        public Int32 Trigger1 { get; set; }
        public Int32 Action1 { get; set; }
        public Int32 Trigger2 { get; set; }
        public Int32 Action2 { get; set; }
        public Int32 Trigger3 { get; set; }
        public Int32 Action3 { get; set; }
        public Int32 Item1 { get; set; }
        public Int32 Item2 { get; set; }
        public Int32 Item3 { get; set; }
        public Int32 Item4 { get; set; }
        public Int32 Item5 { get; set; }
    }
}