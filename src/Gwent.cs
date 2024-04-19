﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGwent;

public class GameTools{
    public GraphicsDeviceManager graphics;
    public SpriteBatch spriteBatch;
    public ContentManager content;
    public GameTools(GraphicsDeviceManager g, SpriteBatch s, ContentManager c) {
        graphics = g;
        spriteBatch = s;
        content = c;
    }

}

public class Gwent : Game
{
    private GraphicsDeviceManager graphics;
    private SpriteBatch spriteBatch;
    private GraphicTools graphicTools;

    private Point gameResolution = new (1024,720);
    private RenderTarget2D renderTarget;
    private Rectangle renderTargetDestination;
    private Color clearColor = Color.Black;

    private BattleManager bm;

    public Gwent()
    {
        graphics = new GraphicsDeviceManager(this)
        {
            IsFullScreen = false,
            PreferredBackBufferWidth = gameResolution.X,
            PreferredBackBufferHeight = gameResolution.Y
        };
        graphics.ApplyChanges();

        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        bm = new BattleManager();
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        base.Initialize();
        var deck1 = DecksDump.Deck1.GetDeck();
        var deck2 = DecksDump.Deck1.GetDeck();
        bm.Initialize(deck1, deck2);
    }

    protected override void LoadContent()
    {
        spriteBatch = new SpriteBatch(GraphicsDevice);
        graphicTools = new GameTools(graphics, spriteBatch, Content);

        renderTarget = new RenderTarget2D(GraphicsDevice, gameResolution.X, gameResolution.Y);
        renderTargetDestination = GetRenderTargetDestination(gameResolution, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

        // TODO: use this.Content to load your game content here

        bm.LoadContent(graphicTools);
    }

    protected override void Update(GameTime gameTime)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Escape)) 
            Exit();
        if (Keyboard.GetState().IsKeyDown(Keys.F4)) {
            ToggleFullScreen();
        }

        // TODO: Add your update logic here

        bm.Update();
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.SetRenderTarget(renderTarget);
        GraphicsDevice.Clear(clearColor);

        // TODO: Add your drawing code here

        graphicTools.spriteBatch.Begin();
        bm.Draw(graphicTools);
        graphicTools.spriteBatch.End();

        GraphicsDevice.SetRenderTarget(null);
        GraphicsDevice.Clear(clearColor);

        spriteBatch.Begin();
        spriteBatch.Draw(renderTarget, renderTargetDestination, Color.White);
        spriteBatch.End();

        base.Draw(gameTime);
    }

    void ToggleFullScreen()
    {
        if (!graphics.IsFullScreen)
        {
            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        }
        else
        {
            graphics.PreferredBackBufferWidth = gameResolution.X;
            graphics.PreferredBackBufferHeight = gameResolution.Y;
        }
        graphics.IsFullScreen = !graphics.IsFullScreen;
        graphics.ApplyChanges();

        renderTargetDestination = GetRenderTargetDestination(gameResolution, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
    }

    Rectangle GetRenderTargetDestination(Point resolution, int preferredBackBufferWidth, int preferredBackBufferHeight)
    {
        float resolutionRatio = (float)resolution.X / resolution.Y;
        float screenRatio;
        Point bounds = new Point(preferredBackBufferWidth, preferredBackBufferHeight);
        screenRatio = (float)bounds.X / bounds.Y;
        float scale;
        Rectangle rectangle = new Rectangle();

        if (resolutionRatio < screenRatio)
            scale = (float)bounds.Y / resolution.Y;
        else if (resolutionRatio > screenRatio)
            scale = (float)bounds.X / resolution.X;
        else
        {
            // Resolution and window/screen share aspect ratio
            rectangle.Size = bounds;
            return rectangle;
        }
        rectangle.Width = (int)(resolution.X * scale);
        rectangle.Height = (int)(resolution.Y * scale);
        return CenterRectangle(new Rectangle(Point.Zero, bounds), rectangle);
    }

    static Rectangle CenterRectangle(Rectangle outerRectangle, Rectangle innerRectangle)
    {
        Point delta = outerRectangle.Center - innerRectangle.Center;
        innerRectangle.Offset(delta);
        return innerRectangle;
    }
}
