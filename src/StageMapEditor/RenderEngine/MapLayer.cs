using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDXControl;
using StageMapEditor.Models.Chip;
using StageMapEditor.ViewModels;
using StageMapEditor.Views;

namespace StageMapEditor.RenderEngine
{
    internal class MapLayer : ILayer
    {
        private IGeneralSettings _settings;
        private Bitmap _border;
        private Bitmap _map;
        private Bitmap _obj;
        private Bitmap _select;
        private Bitmap _star;
        private Bitmap _current;

        public IRenderingViewModel _viewModel;
        private readonly Color4 _white = Color.White;
        private Color _drawBordercolor = Color.White * 0.6f;
        private MapControl _control;
        public float Scale { get { return _viewModel.Scale; } }
        public int ScaledGridSize { get { return _viewModel.ScaledGridSize; } }

        public int PositionX { get { return _control.GetGamePanelPositionLeft(); } }
        public int PositionY { get { return _control.GetGamePanelPositionTop(); } }

        private MapChipModel MapChipList { get { return _viewModel.MapChipModel; } }
        private ObjectChipModel ObjectChipList { get { return _viewModel.ObjectChipModel; } }
        private bool ExistsBackground { get { return _viewModel.ExistsBackground; } }
        private string BackgroundFilePath { get { return _viewModel.BackgroundFilePath; } }

        private static IDictionary<string, Bitmap> _backgroundTextureDic;
        private bool _renderBackground;

        private ILayerHost _host;

        public bool Attached { get; set; }

        public MapLayer(MapControl control)
        {
            _control = control;
            _viewModel = _control.ViewModel;
            _settings = control.ViewModel.Settings;
            _renderBackground = control.ViewModel.Settings.RenderBackground;
        }


        public void Attach(ILayerHost host)
        {
            _host = host;
            host.OnCreateAndBindTargetEnd += HostOnOnCreateAndBindTargetEnd;
            Attached = true;
        }

        private void HostOnOnCreateAndBindTargetEnd(object sender, RenderTarget renderTarget)
        {
            LoadContent(renderTarget);
        }

        private void LoadContent(RenderTarget target)
        {
            _map = ImageLoader.LoadFromFile(target, Path.Combine("./Resource", _settings.MapChip));
            _obj = ImageLoader.LoadFromFile(target, Path.Combine("./Resource", _settings.ObjectChip));
            _star = ImageLoader.LoadFromFile(target, Path.Combine("./Resource", "star.png"));
            var test = ImageLoader.LoadFromFile(target, Path.Combine("./Resource", "test.png"));

            _border = ImageLoader.Grid(target, "border", new Size2(_viewModel.GridSize, _viewModel.GridSize), new Color4(40.0f / 255.0f, 61.0f / 255.0f, 250.0f / 255.0f, 1f));
            _select = ImageLoader.Cell(target, "select", new Size2(_viewModel.GridSize, _viewModel.GridSize), new Color4(0f, 1f, 0f, 1f));
            _current = ImageLoader.Cell(target, "current", new Size2(_viewModel.GridSize, _viewModel.GridSize), new Color4(0f, 0f, 1f, 1f));

            _backgroundTextureDic = new Dictionary<string, Bitmap>();

            var dir = new DirectoryInfo(_settings.StageDataDirectory);
            var imgs =
                dir.GetFiles().Where(
                    x => x.Name.EndsWith(".jpg") || x.Name.EndsWith(".png") || x.Name.EndsWith(".bmp"));

            foreach (var img in imgs)
            {
                var t = ImageLoader.LoadFromFile(target, Path.Combine(dir.FullName, img.FullName));
                if (!_backgroundTextureDic.ContainsKey(img.FullName))
                {
                    _backgroundTextureDic.Add(img.FullName, t);
                }
            }

        }

        public void Detach()
        {
            Attached = false;
        }

        public void Update(TimeSpan timeSpan)
        {
        }

        public void Render()
        {
            if (_viewModel.CurrentMapIsNull)
                return;

            if (_settings.RenderBackground)
            {
                RenderBackground();
            }

            RenderMapChip();

            RenderObjectChip();

            RenderSelectArea();

            RenderCellBorder();

            if (_settings.DrawCurrentCell)
            {
                RenderCurrentPosition();
            }
        }

        private void RenderBackground()
        {
            if (!_renderBackground || !ExistsBackground) { return; }

            var t = _backgroundTextureDic[BackgroundFilePath];

            var rect = new RectangleF(-PositionX,
                                      -PositionY,
                                      t.PixelSize.Width,
                                      t.PixelSize.Height);

            //調整
            var adJustrect = new RectangleF(rect.Left -180,
                                  rect.Top -180,
                                  rect.Width,
                                  rect.Height
                                  );

            _host.RenderTarget.DrawBitmap(t, adJustrect, 1.0f, BitmapInterpolationMode.Linear, null);
        }

        private void RenderMapChip()
        {
            if (MapChipList == null || !_viewModel.CurrentLayerManager.MapChip.Visible)
                return;

            int len = _viewModel.GridSize;

            var list = MapChipList.ListAllWithPosition();

            foreach (var m in list)
            {
                if (m.Item2.ID != 0)
                {
                    var p = _viewModel.GetMapChipCropPosition(m.Item2.ID);
                    var dx = (int)(m.Item1.X * ScaledGridSize - PositionX);
                    var dy = (int)(m.Item1.Y * ScaledGridSize - PositionY);
                    var dest = new RectangleF(dx, dy, ScaledGridSize, ScaledGridSize);
                    var source = new RectangleF(p.X * len, p.Y * len, len, len);

                    _host.RenderTarget.DrawBitmap(_map, dest, 1.0f, BitmapInterpolationMode.Linear, source);
                }
            }
        }

        private void RenderObjectChip()
        {
            if (ObjectChipList == null || !_viewModel.CurrentLayerManager.ObjectChip.Visible)
                return;

            int len = _viewModel.GridSize;

            var list = ObjectChipList.ListAllWithPosition();

            list.ToList()
                .ForEach(m =>
                {
                    //リソースから切り取る位置
                    var p = _viewModel.GetObjChipCropPosition(m.Item2.ID);
                    var dx = (int)(m.Item1.X * ScaledGridSize - PositionX);
                    var dy = (int)(m.Item1.Y * ScaledGridSize - PositionY);
                    var dest = new RectangleF(dx, dy, ScaledGridSize, ScaledGridSize);
                    var source = new RectangleF(p.X * len, p.Y * len, len, len);

                    _host.RenderTarget.DrawBitmap(_obj, dest, 1.0f, BitmapInterpolationMode.Linear, source);

                    if (m.Item2.IsEdited)
                    {
                        int sx = (int)((m.Item1.X * len) * Scale - PositionX + (len - 16) * Scale),
                            sy = (int)((m.Item1.Y * len) * Scale - PositionY);
                        var pos = new RectangleF(sx, sy,
                                                 _star.PixelSize.Width,
                                                 _star.PixelSize.Height);
                        _host.RenderTarget.DrawBitmap(_star, pos, 1.0f, BitmapInterpolationMode.Linear, null);
                    }
                });
        }

        private void RenderSelectArea()
        {
            var select = _viewModel.MapSelect;
            if (select == null || select.IsEmpty)
            {
                return;
            }

            int len = ScaledGridSize;

            foreach (var p in select.GetSelectPointArray())
            {
                int dx = (int)((p.X * len) - PositionX),
                    dy = (int)((p.Y * len) - PositionY);
                var dest = new RectangleF(dx, dy, len, len);
                _host.RenderTarget.DrawBitmap(_select, dest, 0.4f, BitmapInterpolationMode.Linear, null);
            }
        }

        private void RenderCellBorder()
        {
            if (!_viewModel.CurrentLayerManager.Border.Visible)
                return;

            var len = ScaledGridSize;

            float w = _host.RenderTarget.PixelSize.Width / len,
                  h = _host.RenderTarget.PixelSize.Height / len;

            var rect = new RectangleF(0, 0, (int)len, (int)len); // 正常位置

            int adjustx = (PositionX % _viewModel.GridSize),
                adjusty = (PositionY % _viewModel.GridSize);

            for (int x = 0; x <= w; x++)
            {
                for (int y = 0; y <= h; y++)
                {
                    float px = (x * len) - adjustx,
                          py = (y * len) - adjusty;
                    int r = (int)len / 2;

                    // テクスチャ・切り取る位置・描画位置・描画開始位置
                    _host.RenderTarget.DrawBitmap(_border, new RectangleF(px, py, len, len), 0.6f, BitmapInterpolationMode.Linear, rect);
                }
            }
        }

        private void RenderCurrentPosition()
        {
            int len = ScaledGridSize;
            var pos = _viewModel.CurrentMousePosition.GridPoint;
            int dx = (int)(pos.X * len - PositionX),
                dy = (int)(pos.Y * len - PositionY);
            var dest = new RectangleF(dx, dy, len, len);

            _host.RenderTarget.DrawBitmap(_current, dest, 0.2f, BitmapInterpolationMode.Linear, null);
        }
    }
}