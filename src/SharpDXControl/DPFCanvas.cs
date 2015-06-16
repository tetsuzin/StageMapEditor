using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SharpDX.Collections;
using SharpDX.Direct2D1;

using ObjectModel = System.Collections.ObjectModel;

namespace SharpDXControl
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using SharpDX;
    using SharpDX.Direct3D10;
    using SharpDX.DXGI;
    using Device = SharpDX.Direct3D10.Device1;

    public partial class DPFCanvas : Image, ILayerHost
    {
        private Device Device;
        private Texture2D RenderTarget;
        private DX10ImageSource D3DSurface;
        private Stopwatch RenderTimer;

        private SharpDX.Direct2D1.Factory D2DFactory;
        private SharpDX.Direct2D1.RenderTarget D2DRenderTarget;

        public Color4 ClearColor = SharpDX.Color.Black;

        public int _frameRate = 1;
        private const int _baseFrameSecond = 60;
        private int _frameCount = 0;

        public DPFCanvas()
        {
            this.RenderTimer = new Stopwatch();
            this.Loaded += this.Window_Loaded;
            this.Unloaded += this.Window_Closing;
            this.LayerList = new ObjectModel.ObservableCollection<ILayer>();
        }

        public void SetFrameRate(int renderFrameSecond)
        {
            if (renderFrameSecond > 60 || renderFrameSecond < 1)
            {
                _frameRate = 1;
            }

            _frameRate = _baseFrameSecond / renderFrameSecond;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (DPFCanvas.IsInDesignMode)
                return;

            this.StartD3D();
            this.StartRendering();
        }

        private void Window_Closing(object sender, RoutedEventArgs e)
        {
            if (DPFCanvas.IsInDesignMode)
                return;

            this.StopRendering();
            this.EndD3D();
        }

        private void StartD3D()
        {
            this.Device = new Device(DriverType.Hardware, DeviceCreationFlags.BgraSupport, FeatureLevel.Level_10_0);

            this.D3DSurface = new DX10ImageSource();
            this.D3DSurface.IsFrontBufferAvailableChanged += OnIsFrontBufferAvailableChanged;

            this.CreateAndBindTargets();

            this.Source = this.D3DSurface;
        }

        private void EndD3D()
        {
            if (this.LayerList != null)
            {
                foreach (var scene in this.LayerList)
                {
                    scene.Detach();
                }
            }

            this.D3DSurface.IsFrontBufferAvailableChanged -= OnIsFrontBufferAvailableChanged;
            this.Source = null;

            Disposer.RemoveAndDispose(ref this.D3DSurface);
            Disposer.RemoveAndDispose(ref this.D2DRenderTarget);
            Disposer.RemoveAndDispose(ref this.D2DFactory);
            Disposer.RemoveAndDispose(ref this.RenderTarget);
            Disposer.RemoveAndDispose(ref this.Device);
        }

        private void CreateAndBindTargets()
        {
            this.D3DSurface.SetRenderTargetDX10(null);

            Disposer.RemoveAndDispose(ref this.D2DRenderTarget);
            Disposer.RemoveAndDispose(ref this.D2DFactory);
            Disposer.RemoveAndDispose(ref this.RenderTarget);

            int width = Math.Max((int)base.ActualWidth, 100);
            int height = Math.Max((int)base.ActualHeight, 100);

            Texture2DDescription colordesc = new Texture2DDescription
            {
                BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                Format = Format.B8G8R8A8_UNorm,
                Width = width,
                Height = height,
                MipLevels = 1,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                OptionFlags = ResourceOptionFlags.Shared,
                CpuAccessFlags = CpuAccessFlags.None,
                ArraySize = 1
            };

            this.RenderTarget = new Texture2D(this.Device, colordesc);
            Surface surface = this.RenderTarget.QueryInterface<Surface>();

            D2DFactory = new SharpDX.Direct2D1.Factory();
            var rtp = new RenderTargetProperties(new SharpDX.Direct2D1.PixelFormat(Format.Unknown, SharpDX.Direct2D1.AlphaMode.Premultiplied));
            D2DRenderTarget = new RenderTarget(D2DFactory, surface, rtp);

            this.D3DSurface.SetRenderTargetDX10(this.RenderTarget);

            OnOnCreateAndBindTargetEnd(this.D2DRenderTarget);
            Debug.Print("OnOnCreateAndBindTargetEnd");
        }

        private void StartRendering()
        {
            if (this.RenderTimer.IsRunning)
                return;

            CompositionTarget.Rendering += OnRendering;
            this.RenderTimer.Start();
        }

        private void StopRendering()
        {
            if (!this.RenderTimer.IsRunning)
                return;

            CompositionTarget.Rendering -= OnRendering;
            this.RenderTimer.Stop();
        }

        private void OnRendering(object sender, EventArgs e)
        {
            if (!this.RenderTimer.IsRunning)
                return;

            if (_frameRate <= _frameCount)
            {
                this.Render(this.RenderTimer.Elapsed);
                this.D3DSurface.InvalidateD3DImage();
                _frameCount = 0;
            }

            _frameCount++;
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            this.CreateAndBindTargets();
            base.OnRenderSizeChanged(sizeInfo);
        }

        void Render(TimeSpan sceneTime)
        {
            SharpDX.Direct3D10.Device device = this.Device;
            if (device == null)
                return;

            Texture2D renderTarget = this.RenderTarget;
            if (renderTarget == null)
                return;

            int targetWidth = renderTarget.Description.Width;
            int targetHeight = renderTarget.Description.Height;
            device.Rasterizer.SetViewports(new Viewport(0, 0, targetWidth, targetHeight, 0.0f, 1.0f));

            D2DRenderTarget.BeginDraw();
            D2DRenderTarget.Clear(ClearColor);

            if (this.LayerList != null)
            {
                foreach (var scene in this.LayerList)
                {
                    if (!scene.Attached)
                    {
                        scene.Attach(this);
                    }

                    scene.Update(this.RenderTimer.Elapsed);
                    scene.Render();
                }
            }

            D2DRenderTarget.EndDraw();

            device.Flush();
        }

        private void OnIsFrontBufferAvailableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // this fires when the screensaver kicks in, the machine goes into sleep or hibernate
            // and any other catastrophic losses of the d3d device from WPF's point of view
            if (this.D3DSurface.IsFrontBufferAvailable)
                this.StartRendering();
            else
                this.StopRendering();
        }

        /// <summary>
        /// Gets a value indicating whether the control is in design mode
        /// (running in Blend or Visual Studio).
        /// </summary>
        public static bool IsInDesignMode
        {
            get
            {
                DependencyProperty prop = DesignerProperties.IsInDesignModeProperty;
                bool isDesignMode = (bool)DependencyPropertyDescriptor.FromProperty(prop, typeof(FrameworkElement)).Metadata.DefaultValue;
                return isDesignMode;
            }
        }

        public System.Collections.ObjectModel.ObservableCollection<ILayer> LayerList { get; set; }


        #region ILayereHostの実装

        SharpDX.Direct3D10.Device ILayerHost.Device { get { return Device; } }

        SharpDX.Direct2D1.RenderTarget ILayerHost.RenderTarget { get { return D2DRenderTarget; } }

        public event EventHandler<RenderTarget> OnCreateAndBindTargetEnd;

        protected void OnOnCreateAndBindTargetEnd(RenderTarget e)
        {
            EventHandler<RenderTarget> handler = OnCreateAndBindTargetEnd;
            if (handler != null) handler(RenderTarget, e);
        }

        #endregion
    }
}
