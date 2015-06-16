using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Imaging;

namespace StageMapEditor.ViewModels
{
    public class PalletItemViewModel : INotifyPropertyChanged
    {
        #region �R�}���h�ɑ��邽�߂̃A�C�e��ID
        private int _ItemID;
        public int ItemID
        {
            get { return _ItemID; }
            set
            {
                if (_ItemID != value)
                {
                    _ItemID = value;
                    OnPropertyChanged("ItemID");
                }
            }
        }
        #endregion

        #region �\������CroppedBitMap
        private CroppedBitmap _ImageSource;
        public CroppedBitmap ImageSource
        {
            get { return _ImageSource; }
            set
            {
                if (_ImageSource != value)
                {
                    _ImageSource = value;
                    OnPropertyChanged("ImageSource");
                }
            }
        }
        #endregion

        #region �\�����
        private Visibility _Visible;
        public Visibility Visible
        {
            get { return _Visible; }
            set
            {
                if (_Visible != value)
                {
                    _Visible = value;
                    OnPropertyChanged("Visible");
                }
            }
        }
        #endregion

        #region INotifyPropertyChanged�̎���
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        private void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            #endregion
        }
    }
}