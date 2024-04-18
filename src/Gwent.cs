using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGwent;

public class GraphicTools{
    public GraphicsDeviceManager graphics;
    public SpriteBatch spriteBatch;
    public ContentManager content;
    public GraphicTools(GraphicsDeviceManager g, SpriteBatch s, ContentManager c) {
        graphics = g;
        spriteBatch = s;
        content = c;
    }

}

public class Gwent : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private GraphicTools graphicTools;

    private BattleManager bm;

    public Gwent()
    {
        _graphics = new GraphicsDeviceManager(this)
        {
            IsFullScreen = false,
            PreferredBackBufferWidth = 1024,
            PreferredBackBufferHeight = 720
        };
        _graphics.ApplyChanges();

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
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        graphicTools = new GraphicTools(_graphics, _spriteBatch, Content);

        // TODO: use this.Content to load your game content here

        bm.LoadContent(graphicTools);
    }

    protected override void Update(GameTime gameTime)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Escape)) 
            Exit();

        // TODO: Add your update logic here

        bm.Update();
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        // TODO: Add your drawing code here

        graphicTools.spriteBatch.Begin();
        bm.Draw(graphicTools);
        base.Draw(gameTime);
        graphicTools.spriteBatch.End();
    }
}
