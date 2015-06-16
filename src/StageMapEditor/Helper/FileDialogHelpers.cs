using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace StageMapEditor.Helper
{
    public static class FileDialogHelpers
    {
        /// <summary>
        /// Stageファイルを開くダイアログの表示
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        static public OpenFileDialog OpenStageFileDialog(string path = "./")
        {
            var dialog = new OpenFileDialog();
            var di = new System.IO.DirectoryInfo(path);

            dialog.Filter = "ステージRootファイル(*.xml,*.dsm)|*.xml;*.dsm";
            dialog.Title = "開くRootファイルを選択してください";
            dialog.RestoreDirectory = true;
            dialog.InitialDirectory = di.Exists ? di.FullName : "";

            return dialog;
        }

        static public SaveFileDialog SaveStageFileDialog(string dir, string path)
        {
            var dialog = new SaveFileDialog();
            var fi = new System.IO.FileInfo(System.IO.Path.Combine(dir, path));

            dialog.Filter = "ステージRootファイル(*.xml,*.dsm)|*.xml;*.dsm";
            dialog.Title = "開くRootファイルを選択してください";
            dialog.RestoreDirectory = true;
            dialog.InitialDirectory = dir;
            dialog.FileName = fi.Name;

            return dialog;
        }
    }
}
