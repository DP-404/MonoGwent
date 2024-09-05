using System.Collections.Generic;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.IO;

namespace MonoGwent;

public enum RowType {
    MELEE,
    RANGED,
    SIEGE
}

public abstract class Card
{
    public const int ACTUAL_WIDTH = 520;
    public const int ACTUAL_HEIGHT = 768;
    public const int WIDTH = 55;
    public const int HEIGHT = 80;
    public const string NEUTRAL_FACTION = "Neutral";
    public static string IMAGES_PATH = Path.Join("graphics","cards");
    public const string DEFAULT_IMAGE = "default";
    public static readonly Vector2 THUMB_SCALE =
    new Vector2((float)WIDTH/ACTUAL_WIDTH, (float)HEIGHT/ACTUAL_HEIGHT);
    public const int DEFAULT_POWER = 0;

    public CardBlueprint blueprint;
    public string name;
    public string description;
    public string faction = NEUTRAL_FACTION;
    public string owner;
    public RowType[] types {get; init;}
    public RowType? position = null;
    public abstract string type_name {get;}
    public List<IEffect> effects = new ();
    public int original_power = DEFAULT_POWER;
    protected int modified_power = DEFAULT_POWER;
    public int power {
        get => modified_power;
        set => modified_power = (value < 0)? 0 : value;
    }

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

    public virtual Card Copy() {
        var card = blueprint.GetCard();
        card.owner = owner;
        card.img = img;
        return card;
    }

    public virtual void Dispose() {
        return;
    }

    public abstract bool PlayCard(BattleManager bm);

}