

namespace MonoGwent;

public class CardBoost : Card {

    public override string type_name {get => "Boost";}
    public int bonus;

    public override bool PlayCard(BattleManager bm)
    {
        if (bm.Current.boosts[(RowType)bm.Cursor.index] is not null) return false;

        bm.Current.boosts[(RowType)bm.Cursor.index] = this;
        bm.Current.hand.Remove(this);
        return true;
    }
}
