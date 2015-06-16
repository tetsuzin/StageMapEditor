using System;
using System.Collections.Generic;
using System.Drawing;
using StageMapEditor.Models.Chip;
using StageMapEditor.Models.Commands;
using StageMapEditor.Models.Drawing;
using StageMapEditor.ViewModels;

namespace StageMapEditor.Models
{
    /// <summary>
    /// パレットViewModelをまとめるクラス。
    /// どれが選択状態にあるのかもここで管理
    /// </summary>
    public class PalletViewModelManager
    {
        public MapChipPalletViewModel MapChipPalletViewModel { get; private set; }
        public ObjectChipPalletViewModel ObjectChipPalletViewModel { get; private set; }

        /// <summary>
        /// 共通で使うクリップボード
        /// </summary>
        public Clipboard Clipboard { get; private set; }

        /// <summary>
        /// 現在の描画状態
        /// </summary>
        private IPalletViewModel _current;

        private HashSet<IPalletViewModel> PaletteViewModels;

        public PalletViewModelManager(MapChipPalletViewModel mapChipPaletteViewModel, ObjectChipPalletViewModel objectChipPaletteViewModel)
        {
            mapChipPaletteViewModel.Parent = this;
            objectChipPaletteViewModel.Parent = this;

            MapChipPalletViewModel = mapChipPaletteViewModel;
            ObjectChipPalletViewModel = objectChipPaletteViewModel;

            _current = MapChipPalletViewModel;

            PaletteViewModels = new HashSet<IPalletViewModel> { MapChipPalletViewModel, ObjectChipPalletViewModel };

            Clipboard = new Clipboard();
        }

        /// <summary>
        /// 選択されたパレットが有効範囲内かどうか
        /// </summary>
        /// <param name="drawState"></param>
        /// <param name="id"> </param>
        /// <returns></returns>
        private bool CanChangeDrawState(DrawState drawState, int id)
        {
            switch (drawState)
            {
                case DrawState.MapChip:
                    return MapChipPalletViewModel.DrawMapState.CanSetPallet(id);
                case DrawState.ObjChip:
                    return ObjectChipPalletViewModel.DrawObjectState.CanSetPallet(id);
                default:
                    return false;
            }
        }

        public void ChangeDrawState(DrawState drawState, int id)
        {
            //選択できる範囲外だったら何もせずに終了
            if (!CanChangeDrawState(drawState, id)) return;

            switch (drawState)
            {
                case DrawState.MapChip:
                    MapChipPalletViewModel.DrawMapState.Set(id);
                    ObjectChipPalletViewModel.SelectCancel();
                    _current = MapChipPalletViewModel;
                    break;

                case DrawState.ObjChip:
                    ObjectChipPalletViewModel.DrawObjectState.Set(id);
                    MapChipPalletViewModel.SelectCancel();
                    _current = ObjectChipPalletViewModel;
                    break;

                case DrawState.Empty:
                    //何もしない
                    break;
                default:
                    throw new ArgumentOutOfRangeException("drawState");
            }
        }

        /// <summary>
        /// 洗濯されている箇所を削除する
        /// </summary>
        /// <param name="model"></param>
        /// <param name="mapSelect"></param>
        public void Delete(MapModel model, MapSelect mapSelect)
        {
            foreach (var viewModel in PaletteViewModels)
            {
                viewModel.Drawing.Delete(model, mapSelect);
            }
        }

        public bool CanDraw(MapModel model, Point point)
        {
            return _current.Drawing.CanDraw(model, point);
        }

        public void Draw(MapModel model, Point point, MapSelect mapSelect)
        {
            _current.Drawing.Draw(model, point, mapSelect);
        }

        /// <summary>
        /// 取得したチップを元にスポイト動作をする
        /// </summary>
        public void SpoilChip(DrawState drawState, IChip chip)
        {
            switch (drawState)
            {
                case DrawState.MapChip:
                    MapChipPalletViewModel.PalletSelect(chip.ID);
                    MapChipPalletViewModel.DrawMapState.Set(chip.ID);
                    ObjectChipPalletViewModel.SelectCancel();
                    _current = MapChipPalletViewModel;
                    break;

                case DrawState.ObjChip:
                    ObjectChipPalletViewModel.PalletSelect(chip.ID);
                    ObjectChipPalletViewModel.DrawObjectState.SpoilObject(chip as ObjectChip);
                    MapChipPalletViewModel.SelectCancel();
                    _current = ObjectChipPalletViewModel;
                    break;

                case DrawState.Empty:
                    //何もしない
                    break;
                default:
                    throw new ArgumentOutOfRangeException("drawState");
            }
        }

        /// <summary>
        /// ダブルクリックされたときの動作
        /// </summary>
        /// <param name="mapModel"> </param>
        /// <param name="point"></param>
        /// <param name="action"></param>
        public void DoubleClick(MapModel mapModel, Point point, Action<IChip> action)
        {
            _current.DoubleClick(mapModel, point, action);
        }
    }
}