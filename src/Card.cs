using System;
using System.Linq;
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

public abstract class Card
{
    public const int ACTUAL_WIDTH = 520;
    public const int ACTUAL_HEIGHT = 768;
    public const int WIDTH = 55;
    public const int HEIGHT = 80;
    public static readonly Vector2 THUMB_SCALE =
    new Vector2((float)WIDTH/ACTUAL_WIDTH, (float)HEIGHT/ACTUAL_HEIGHT);

    public string name;
    public string description;
    public RowType[] types {get; init;}
    public abstract string type_name {get;}
    public IEffect effect = new EffectNone();

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
        var xConsumed = ((count != 0)? count : 1) * WIDTH;
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

    public abstract bool PlayCard(BattleManager bm);

}

public class CardUnit : Card {
    public const int POWER_DECOY = 0;
    private const string TYPE_UNIT_NAME = "Unit";
    private const string TYPE_SILVER_NAME = " (Silver)";
    private const string TYPE_GOLDEN_NAME = " (Golden)";
    private const string TYPE_DECOY_NAME = "Decoy";

    public override string type_name {get =>
    (!is_decoy)? (TYPE_UNIT_NAME + (is_hero? TYPE_GOLDEN_NAME : TYPE_SILVER_NAME))
    : TYPE_DECOY_NAME;}
    public bool is_hero {get; init;}
    public int power;

    public bool is_decoy {get => power == POWER_DECOY? true : false;}

    public int GetPower(CardWeather weather, CardBoost boost) {
        if (is_hero) return power;
        var actual_power = power;
        if (boost is not null) actual_power += boost.bonus;
        if (weather is not null) actual_power = Math.Min(actual_power, weather.penalty);
        return actual_power;
    }

    public override bool PlayCard(BattleManager bm)
    {
        if (((CardUnit)bm.HandCard).is_decoy) {
            var takeback_card = (CardUnit)bm.Current.GetFieldCard(
                (RowType)bm.Cursor.field,
                bm.Cursor.index
                );

            // If takeback card is Hero > Return
            if (takeback_card.is_hero) return false;

            // Take field card to hand
            bm.Current.rows[(RowType)bm.Cursor.field].Remove(takeback_card);
            bm.Current.hand.Add(takeback_card);

            // Place card on field card's place
            bm.Current.rows[(RowType)bm.Cursor.field].Insert(bm.Cursor.index, (CardUnit)bm.HandCard);
            bm.Current.hand.Remove(bm.HandCard);
        } else {
            // Place card on field
            bm.Current.rows[(RowType)bm.Cursor.index].Add((CardUnit)bm.HandCard);
            bm.Current.hand.Remove(bm.HandCard);
        }

        bm.UseCardEffect(bm.Current, this);
        return true;
    }
}

public class CardLeader : Card {
    public override string type_name {get => "Leader";}
    public bool used = false;

    public override bool PlayCard(BattleManager bm)
    {
        if (!used) {
            used = true;
            bm.UseCardEffect(bm.Current, this);
            return true;
        }
        return false;
    }
}

public class CardWeather : Card {
    public const int DISPEL_PENALTY = -1;
    public const int DEFAULT_PENALTY = 1;

    public override string type_name {get => (!is_dispel)? "Weather" : "Dispel";}
    public int penalty = DEFAULT_PENALTY;
    public bool is_dispel {get => penalty == DISPEL_PENALTY;}

    public override bool PlayCard(BattleManager bm)
    {
        RowType field;
        if (bm.Cursor.field == Cursor.NONE) {
            field = (RowType)bm.Cursor.index;
        } else {
            field = (RowType)bm.Cursor.field;
        }

        // Card is Weather
        if (!((CardWeather)bm.HandCard).is_dispel) {
            bm.Current.hand.Remove(bm.HandCard);
            var old_weather = bm.Weathers[field];
            if (old_weather.Item1 is not null) {
                old_weather.Item2.graveyard.Add(old_weather.Item1);
            }
            bm.Weathers[field] = new ((CardWeather)bm.HandCard, bm.Current);
        }
        // Card is Dispel
        else {
            var exists_weather = false;
            var is_single = bm.HandCard.types.Length != 0;
            if (is_single) {
                exists_weather = bm.Weathers[field].Item1 is not null;
            } else {
                exists_weather = Enum.GetValues(typeof(RowType))
                    .Cast<RowType>()
                    .Select(row => bm.Weathers[row].Item1)
                    .Any(w => w is not null);
            }

            if (!exists_weather) return false;

            bm.Current.graveyard.Add(bm.HandCard);
            bm.Current.hand.Remove(bm.HandCard);

            // Dispel All
            if (bm.HandCard.types.Length == 0) {bm.ClearAllWeathers();}
            // Dispel Single
            else {bm.ClearWeather(field);}
        }

        return true;
    }
}

public class CardBoost : Card {

    public override string type_name {get => "Boost";}
    public int bonus;

    public override bool PlayCard(BattleManager bm)
    {
        if (bm.Current.boosts[(RowType)bm.Cursor.index] is not null) return false;

        bm.Current.boosts[(RowType)bm.Cursor.index] = (CardBoost)bm.HandCard;
        bm.Current.hand.Remove(bm.HandCard);
        return true;
    }
}
