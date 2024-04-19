using System.Collections.Generic;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using System;

namespace MonoGwent;

public enum RowType {
    MELEE,
    RANGE,
    SIEGE
}

public enum LeaderEffect {
    DRAW_EXTRA_CARD,
}

public class Card
{
    public const int ACTUAL_WIDTH = 520;
    public const int ACTUAL_HEIGHT = 768;
    public const int WIDTH = 55;
    public const int HEIGHT = 80;
    public static readonly Vector2 THUMB_SCALE =
    new Vector2((float)WIDTH/ACTUAL_WIDTH, (float)HEIGHT/ACTUAL_HEIGHT);

    public RowType[] types {get; init;}

    public string img_name;
    public Texture2D img;
    public static Texture2D img_back;
    public static Texture2D img_power_normal;
    public static Texture2D img_power_hero;
    public static Texture2D img_weather;
    public static Texture2D img_dispel;
    public static Texture2D img_melee;
    public static Texture2D img_range;
    public static Texture2D img_siege;
    public static Dictionary<RowType,Texture2D> img_rows;

    public static Vector2 _GetRowPosition(int index, int count, int xpos, int ypos, int width) {
        var xConsumed = count * WIDTH;
        var xUsed = width - xConsumed;
        var xFree = (xUsed < 0)? 0 : xUsed / 2;
        return new Vector2(
            xpos            // Row starting position
            +WIDTH*index    // Card position in row
            +((xUsed > 0)? xFree : xUsed / count * index),         // Center row position
            ypos
        );
    }
    public Vector2 GetRowPosition(int index, int count, int xpos, int ypos, int width) {
        return _GetRowPosition(index, count, xpos, ypos, width);
    }

}

public class CardUnit : Card {

    public virtual bool is_hero {get; init;}
    public virtual int power {get; set;}
    public virtual int effect {get; init;}

    public bool is_decoy {get => power == 0? true : false;}

    public int ApplyWeather(CardWeather weather) {
        if (weather is null) return power;
        return Math.Min(power, weather.penalty);
    }
}

public class CardLeader : Card {
    public LeaderEffect effect;
    public bool used = false;
}

public class CardWeather : Card {
    public const int DISPEL_PENALTY = -1;
    public const int DEFAULT_PENALTY = 1;

    public int penalty = DEFAULT_PENALTY;
    public bool is_dispel {get => penalty == DISPEL_PENALTY;}

}

public class CardBoost : Card {

    public int bonus;
}
