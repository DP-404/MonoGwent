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

public abstract class Card
{
    public const int ACTUAL_WIDTH = 520;
    public const int ACTUAL_HEIGHT = 768;
    public const int WIDTH = 55;
    public const int HEIGHT = 80;
    public static readonly Vector2 THUMB_SCALE =
    new Vector2((float)WIDTH/ACTUAL_WIDTH, (float)HEIGHT/ACTUAL_HEIGHT);

    public RowType[] types {get; init;}
    public abstract string type_name {get;}

    public string img_name;
    public Texture2D img;
    public static Texture2D img_back;
    public static Texture2D img_power_normal;
    public static Texture2D img_power_hero;
    public static Texture2D img_weather;
    public static Texture2D img_dispel;
    public static Texture2D img_boost;
    public static Texture2D img_decoy;
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
    public const int POWER_DECOY = 0;
    private const string TYPE_UNIT_NAME = "Unit";
    private const string TYPE_DECOY_NAME = "Decoy";

    public override string type_name {get => (!is_decoy)? TYPE_UNIT_NAME : TYPE_DECOY_NAME;}
    public bool is_hero {get; init;}
    public int power {get; set;}
    public int effect {get; init;}

    public bool is_decoy {get => power == POWER_DECOY? true : false;}

    public int GetPower(CardWeather weather, CardBoost boost) {
        if (is_hero) return power;
        var actual_power = power;
        if (boost is not null) actual_power += boost.bonus;
        if (weather is not null) actual_power = Math.Min(actual_power, weather.penalty);
        return actual_power;
    }
}

public class CardLeader : Card {
    public override string type_name {get => "Leader";}
    public LeaderEffect effect;
    public bool used = false;
}

public class CardWeather : Card {
    public const int DISPEL_PENALTY = -1;
    public const int DEFAULT_PENALTY = 1;

    public override string type_name {get => "Weather";}
    public int penalty = DEFAULT_PENALTY;
    public bool is_dispel {get => penalty == DISPEL_PENALTY;}

}

public class CardBoost : Card {

    public override string type_name {get => "Boost";}
    public int bonus;
}
