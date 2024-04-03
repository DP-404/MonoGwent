using System.Collections.Generic;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace MonoGwent;

public enum RowType {
    MELEE,
    RANGE,
    SIEGE
}

public class Card
{
    public static readonly string HOVER_NAME = "card_hover";
    public static readonly string BACK_NAME = "card_back";
    public const int HEIGHT = 80;
    public const int WIDTH = 55;

    public string img_name;

    public Texture2D image;
    public static Texture2D hover_image;
    public static Texture2D back_image;

    public static Vector2 _GetRowPosition(int index, int count, int xpos, int ypos, int width) {
        var xConsumed = count * WIDTH;
        var xFree = (xConsumed > width)? 0 : (width - xConsumed) / 2;
        return new Vector2(
            xpos            // Row starting position
            +WIDTH*index    // Card position in row
            //-(WIDTH/2)      // Center card position
            +xFree,         // Center row position
            ypos
        );
    }
    public Vector2 GetRowPosition(int index, int count, int xpos, int ypos, int width) {
        return _GetRowPosition(index, count, xpos, ypos, width);
    }

    public void LoadContent(GraphicTools gt) {
        image = gt.content.Load<Texture2D>(img_name);
    }
    public void Draw(GraphicTools gt, int xpos, int ypos) {
        gt.spriteBatch.Begin();
        gt.spriteBatch.Draw(
            image,
            new Vector2(xpos, ypos),
            null,
            Color.Black,
            0.1074f
        );
        gt.spriteBatch.End();
    }
}

public class CardUnit : Card {

    public RowType[] types {get; init;}
    public virtual int power {get; set;}
    public bool is_hero {get; init;}
    public int effect {get; init;}
}

public class CardWeather : Card {

    public int penalty;
    public RowType[] types;
}

public class CardBoost : Card {

    public int bonus;
    public RowType type;
}

public class CardDispel : Card {

    public RowType[] types;
}

public class CardBait : CardUnit {

    public override int power {get => 0;}
}
