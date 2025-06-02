using System;
using Acorn.Trail.GFX;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Acorn.Trail;

public class MapComponent : Game
{
    private GraphicsDeviceManager _graphicsDeviceManager;
    private SpriteBatch _spriteBatch;
    private readonly IGraphicsManager _graphicsManger;

    private Texture2D? _ui = null;
    private readonly IGraphicsDeviceManagerProvider _graphicsDeviceManagerProvider;

    public MapComponent(IGraphicsDeviceManagerProvider graphicsDeviceManagerProvider, IGraphicsManager graphicsManager)
    {
        _graphicsManger = graphicsManager;
        _graphicsDeviceManager = new GraphicsDeviceManager(this);
        _graphicsDeviceManager.PreparingDeviceSettings += (sender, e) =>
        {
            e.GraphicsDeviceInformation.PresentationParameters.BackBufferWidth = 800;
            e.GraphicsDeviceInformation.PresentationParameters.BackBufferHeight = 600;
            e.GraphicsDeviceInformation.PresentationParameters.BackBufferFormat = SurfaceFormat.Color;
            e.GraphicsDeviceInformation.PresentationParameters.PresentationInterval = PresentInterval.Two;
        };
        graphicsDeviceManagerProvider.GraphicsDeviceManager = _graphicsDeviceManager;

        _graphicsDeviceManagerProvider = graphicsDeviceManagerProvider;
        
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        if (_graphicsDeviceManagerProvider.GraphicsDeviceManager?.GraphicsDevice is not null && _ui is null)
        {
            _ui = _graphicsManger.TextureFromResource(GFXTypes.PreLoginUI, 1);
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        _spriteBatch.Begin();

        if (_ui is not null)
        {
            _spriteBatch.Draw(_ui, new Rectangle(0, 0, _ui.Width, _ui.Height), Color.White);
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}