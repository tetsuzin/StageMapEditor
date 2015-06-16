using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Livet;
using Livet.Commands;
using StageMapEditor.Models;
using StageMapEditor.Models.Chip;
using StageMapEditor.Models.Drawing;
using Brush = System.Windows.Media.Brush;
using Point = System.Drawing.Point;

namespace StageMapEditor.ViewModels
{
    public interface IPalletViewModel
    {
        /// <summary>
        /// �p���b�g���I���\��
        /// </summary>
        /// <returns></returns>
        bool CanPalletSelect();

        /// <summary>
        /// �p���b�g��I������
        /// </summary>
        /// <param name="id"></param>
        void PalletSelect(int id);

        /// <summary>
        /// �p���b�g�̑I����Ԃ��L�����Z������
        /// </summary>
        void SelectCancel();

        /// <summary>
        /// �p���b�g���_�u���N���b�N�����Ƃ��̓���
        /// </summary>
        /// <param name="mapModel"></param>
        /// <param name="point"></param>
        /// <param name="action"></param>
        void DoubleClick(MapModel mapModel, Point point, Action<IChip> action);

        /// <summary>
        /// �`�b�v��Draw���������C���X�^���X
        /// </summary>
        IDrawingChip Drawing { get; set; }
    }

    public abstract class AbstractPalletViewModel : NotificationObject, IPalletViewModel
    {
        #region Items

        private ObservableCollection<PalletItemViewModel> _Items;

        public ObservableCollection<PalletItemViewModel> Items
        {
            get { return _Items = _Items ?? new ObservableCollection<PalletItemViewModel>(); }
            set
            {
                if (_Items != value)
                {
                    _Items = value;
                    RaisePropertyChanged("Items");
                }
            }
        }

        #endregion

        #region BitmapSource

        private BitmapSource _BitmapSource;

        public BitmapSource BitmapSource
        {
            get { return _BitmapSource; }
            set
            {
                if (_BitmapSource != value)
                {
                    _BitmapSource = value;
                    RaisePropertyChanged("BitmapSource");
                }
            }
        }

        #endregion

        #region Scale

        private double _Scale = 1;

        public double Scale
        {
            get { return _Scale; }
            set
            {
                if (_Scale != value)
                {
                    _Scale = value;
                    GridSize = (int)(_baseGrisSize * _Scale);
                    RaisePropertyChanged("Scale");
                }
            }
        }

        #endregion

        #region �O���b�h���̃T�C�Y

        private int _GridSize;

        public int GridSize
        {
            get { return _GridSize; }
            set
            {
                if (_GridSize != value)
                {
                    _GridSize = value;
                    RaisePropertyChanged("GridSize");
                }
            }
        }

        #endregion

        #region �O���b�h���̐F

        private Brush _GridBrush = new SolidColorBrush(Colors.Black);

        public Brush GridBrush
        {
            get { return _GridBrush; }
            set
            {
                if (_GridBrush != value)
                {
                    _GridBrush = value;
                    RaisePropertyChanged("GridBrush");
                }
            }
        }

        #endregion

        #region �O���b�h���̓����x

        private double _GridOpacity = 1;

        public double GridOpacity
        {
            get { return _GridOpacity; }
            set
            {
                if (_GridOpacity != value)
                {
                    _GridOpacity = value;
                    RaisePropertyChanged("GridOpacity");
                }
            }
        }
        #endregion

        private int _baseGrisSize;

        public IDrawingChip Drawing { get; set; }

        #region PalletSelectCommand �N���b�N���ɊY���̃p���b�g��I����Ԃɂ���
        private ListenerCommand<int> _PalletSelectCommand;
        public ListenerCommand<int> PalletSelectCommand
        {
            get
            {
                if (_PalletSelectCommand == null)
                {
                    _PalletSelectCommand = new ListenerCommand<int>(PalletSelect, CanPalletSelect);
                }
                return _PalletSelectCommand;
            }
        }

        public bool CanPalletSelect()
        {
            return true;
        }

        public virtual void PalletSelect(int parameter)
        {
            foreach (var item in Items)
            {
                item.Visible = item.ItemID == parameter ? Visibility.Visible : Visibility.Hidden;
            }

        }
        #endregion

        #region �R���X�g���N�^
        protected AbstractPalletViewModel(IGeneralSettings settings)
        {
            GridSize = settings.CellSize;
            _baseGrisSize = GridSize;
            Scale = (double)settings.PalletCellSize / _baseGrisSize;

            GridOpacity = 0.4;
            GridBrush = new SolidColorBrush(new Color { A = 255, R = 41, G = 61, B = 250 });
        }
        #endregion

        /// <summary>
        /// �p���b�g�̑I����Ԃ��L�����Z������
        /// </summary>
        public void SelectCancel()
        {
            foreach (var item in Items)
            {
                item.Visible = Visibility.Hidden;
            }
        }

        public abstract void DoubleClick(MapModel mapModel, Point point, Action<IChip> action);
        public abstract Point GetPosition(int id);
    }
}