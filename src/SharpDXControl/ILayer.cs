using System;
using SharpDX.Direct2D1;

namespace SharpDXControl
{
    public interface ILayerHost
    {
        SharpDX.Direct3D10.Device Device { get; }
        RenderTarget RenderTarget { get; }
        event EventHandler<RenderTarget> OnCreateAndBindTargetEnd;
    }

    public interface ILayer
    {
        bool Attached { get; set; }
        void Attach(ILayerHost host);
        void Detach();
        void Update(TimeSpan timeSpan);
        void Render();
    }
}
