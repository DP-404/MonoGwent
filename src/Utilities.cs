using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGwent;

static class Utilities {
    public static void CreateBorder( this Texture2D texture,  int borderWidth, Color borderColor ) {
        Color[] colors = new Color[ texture.Width * texture.Height ];

        for ( int x = 0; x < texture.Width; x++ ) {
            for ( int y = 0; y < texture.Height; y++ ) {
                bool colored = false;
                for ( int i = 0; i <= borderWidth; i++ ) {
                    if ( x == i || y == i || x == texture.Width - 1 - i || y == texture.Height - 1 - i ) {
                        colors[x + y * texture.Width] = borderColor;
                        colored = true;
                        break;
                    }
                }

                if(colored == false)
                    colors[ x + y * texture.Width ] = Color.Transparent;
            }
        }

        texture.SetData( colors );
    }
    public static void Draw (this SpriteBatch spriteBatch, Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, float scale) {
        spriteBatch.Draw(
            texture,
            position,
            sourceRectangle,
            color,
            0,
            new Vector2(0,0),
            scale,
            SpriteEffects.None,
            0
        );
    }
    public static void Draw (this SpriteBatch spriteBatch, Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color, Vector2 scale) {
        spriteBatch.Draw(
            texture,
            position,
            sourceRectangle,
            color,
            0,
            new Vector2(0,0),
            scale,
            SpriteEffects.None,
            0
        );
    }
}
