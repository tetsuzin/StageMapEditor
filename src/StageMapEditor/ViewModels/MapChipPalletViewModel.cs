using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Livet.Commands;
using StageMapEditor.Models;
using StageMapEditor.Models.Chip;
using StageMapEditor.Models.Drawing;
using Point = System.Drawing.Point;

namespace StageMapEditor.ViewModels
{
    public class MapChipPalletViewModel : AbstractPalletViewModel
    {
        public PalletViewModelManager Parent;
        public MapChipLibrary MapChipLibrary;

        public DrawMapState DrawMapState
        {
            get { return Drawing as DrawMapState; }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="settings"> </param>
        public MapChipPalletViewModel(IGeneralSettings settings)
            : base(settings)
        {
            MapChipLibrary = new MapChipLibrary(settings);
            Items = GetPalletItemViewModels(MapChipLibrary);
            Drawing = new DrawMapState(MapChipLibrary);
        }

        private ObservableCollection<PalletItemViewModel> GetPalletItemViewModels(MapChipLibrary mapChipLibrary)
        {
            var items =
                new ObservableCollection<PalletItemViewModel>(
                    mapChipLibrary.GetEnumerator()
                        .Select(
                            x =>
                            {
                                var bitmap = mapChipLibrary.GetBitMap(x.ID);
                                return new PalletItemViewModel()
                                           {
                                               ImageSource = bitmap,
                                               Visible = Visibility.Hidden,
                                               ItemID = x.ID,
                                           };
                            }));

            return items;
        }

        public override void PalletSelect(int parameter)
        {
            base.PalletSelect(parameter);

            Parent.ChangeDrawState(Models.Drawing.DrawState.MapChip, parameter);
        }

        public override void DoubleClick(MapModel mapModel, Point point, Action<IChip> action)
        {
            // 何もしない
        }

        public delegate void PalletEnterEventDelegate(MapChip chip);
        public PalletEnterEventDelegate PalletEnterEvent = delegate { };

        #region PalletEnterCommand
        private ListenerCommand<int> _PalletEnterCommand;

        public ListenerCommand<int> PalletEnterCommand
        {
            get { return _PalletEnterCommand ?? (_PalletEnterCommand = new ListenerCommand<int>(PalletEnter)); }
        }
        public void PalletEnter(int parameter)
        {
            var chip = MapChipLibrary.GetByID(parameter);
            PalletEnterEvent(chip);
        }
        #endregion

        public override Point GetPosition(int id)
        {
            return MapChipLibrary.GetPosition(id);
        }
    }
}
