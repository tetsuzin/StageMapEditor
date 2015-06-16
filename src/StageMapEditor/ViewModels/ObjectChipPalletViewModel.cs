using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Livet.Commands;
using StageMapEditor.Models;
using StageMapEditor.Models.Chip;
using StageMapEditor.Models.Drawing;
using Point = System.Drawing.Point;

namespace StageMapEditor.ViewModels
{
    public class ObjectChipPalletViewModel : AbstractPalletViewModel
    {
        public ObjectChipLibrary ObjectChipLibrary;
        public PalletViewModelManager Parent;

        public DrawObjectState DrawObjectState
        {
            get { return Drawing as DrawObjectState; }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="settings"> </param>
        public ObjectChipPalletViewModel(IGeneralSettings settings)
            : base(settings)
        {
            ObjectChipLibrary = new ObjectChipLibrary(settings);
            Items = GetPalletItemViewModels(ObjectChipLibrary);
            base.Drawing = new DrawObjectState(ObjectChipLibrary);
        }

        private static ObservableCollection<PalletItemViewModel> GetPalletItemViewModels(ObjectChipLibrary objectChipLibrary)
        {
            var items = new ObservableCollection<PalletItemViewModel>(
                   objectChipLibrary.GetEnumerator()
                       .Select(x =>
                       {
                           var bitmap = objectChipLibrary.GetBitMap(x.ID);
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

            Parent.ChangeDrawState(Models.Drawing.DrawState.ObjChip, parameter);
        }

        public override void DoubleClick(MapModel mapModel, Point point, Action<IChip> action)
        {
            if (mapModel == null)
                return;

            var objChip = mapModel.ObjectChipModel.Get(point);

            if (objChip.ID == 0)
                return;

            if (DrawObjectState.Selected.ID == objChip.ID)
            {
                action(objChip);
            }
        }

        public delegate void PalletEnterEventDelegate(ObjectChip chip);
        public PalletEnterEventDelegate PalletEnterEvent = delegate { };

        #region PalletEnterCommand
        private ListenerCommand<int> _PalletEnterCommand;

        public ListenerCommand<int> PalletEnterCommand
        {
            get { return _PalletEnterCommand ?? (_PalletEnterCommand = new ListenerCommand<int>(PalletEnter)); }
        }
        public void PalletEnter(int parameter)
        {
            var chip = ObjectChipLibrary.GetByID(parameter);
            PalletEnterEvent.Invoke(chip);
        }
        #endregion

        public override Point GetPosition(int id)
        {
            return ObjectChipLibrary.GetPosition(id);
        }
    }
}