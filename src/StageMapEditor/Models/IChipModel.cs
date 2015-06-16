using System.Collections.Generic;
using System.Drawing;

namespace StageMapEditor.Models
{
    public interface IChipModel
    {
        void Delete(Point point);
        void Delete(int x, int y);
        //IEnumerable<string> GetOriginalString();
        string GetMD5String();
    }
}