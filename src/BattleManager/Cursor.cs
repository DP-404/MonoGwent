using Microsoft.Xna.Framework.Graphics;

namespace MonoGwent;

public enum Section {
    HAND,
    FIELD,
    ROW,
    LEADER,
}

public class Cursor {
    private const int DEFAULT_INDEX = 0;
    public const int NONE = -1;
    public Section section = Section.HAND;
    public int index = DEFAULT_INDEX;
    public int hand = NONE;
    public int field = NONE;
    public bool holding = false;

    public Texture2D mark_card_hovered;
    public Texture2D mark_card_selected;
    public Texture2D mark_card_enabled;
    public Texture2D mark_card_disabled;
    public Texture2D mark_card_hovered_disabled;
    public Texture2D mark_row_hovered;
    public Texture2D mark_row_enabled;
    public Texture2D mark_row_disabled;
    public Texture2D mark_row_hovered_disabled;

    public void Move(Section s, int i, int h, int f, bool hold=true) {
        section = s;
        index = i;
        hand = h;
        field = f;
        if (hold) Hold();
    }
    public void Move(Section s, int i, bool hold=true) {
        var f = NONE;
        var h = NONE;
        if (s == Section.FIELD) {
            h = hand;
        }
        else if (s == Section.ROW) {
            f = field;
            h = hand;
        }
        Move(s, i, h, f, hold);
    }
    public void Move(Section s, bool hold=true) {
        Move(s, DEFAULT_INDEX, hold);
    }
    public void Move(int i, bool hold=true) {
        Move(section, i, hand, field, hold);
    }
    public void Hold() {holding = true;}
    public void Release() {holding = false;}
}
