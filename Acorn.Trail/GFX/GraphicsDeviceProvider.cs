using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Acorn.Trail.GFX;

public class GraphicsDeviceManagerProvider(GraphicsDevice? graphicsDevice, GraphicsDeviceManager? graphicsDeviceManager)
    : IGraphicsDeviceManagerProvider
{
    public GraphicsDeviceManager? GraphicsDeviceManager { get; set; } = graphicsDeviceManager;
}

public interface IGraphicsDeviceManagerProvider
{
    GraphicsDeviceManager? GraphicsDeviceManager { get; set; }
}