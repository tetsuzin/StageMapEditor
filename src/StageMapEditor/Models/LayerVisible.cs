using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace StageMapEditor.Models
{
    /// <summary>
    /// レイヤー情報
    /// 描画の有無を管理。
    /// 透明度も管理する予定
    /// </summary>
    public class Layer : INotifyPropertyChanged
    {
        public bool Visible { get; set; }

        public Layer() { Visible = true; }
        public void FlipVisible()
        {
            Visible = !Visible;
            OnPropertyChanged("Visible");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class LayerManager
    {
        public Layer Border;
        public Layer MapChip;
        public Layer ObjectChip;

        public LayerManager()
        {
            Border = new Layer();
            MapChip = new Layer();
            ObjectChip = new Layer();
        }
    }
}
