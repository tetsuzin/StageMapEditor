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
        public MapModelPack[] MapModel { get; set; }
    }

    public partial class MapModelPack
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public MapChipPack[] MapChipPack { get; set; }
        public ObjectChipPack[] ObjectChipPack { get; set; }
    }

    [DebuggerDisplay("ID = {ID}")]
    public partial class MapChipPack
    {
    }

    [DebuggerDisplay("ID = {ID}, Status = {Status}, Param = {Param}, SubParam1 = {SubParam1}, SubParam2 = {SubParam2}, Trigger1 = {Trigger1}, Action1 = {Action1}, Trigger2 = {Trigger2}, Action2 = {Action2}, Trigger3 = {Trigger3}, Action3 = {Action3}")]
    public partial class ObjectChipPack
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
}
