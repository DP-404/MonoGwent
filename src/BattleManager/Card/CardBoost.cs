

namespace MonoGwent;

public class CardBoost : Card {

    public override string type_name {get => "Boost";}
    public int bonus;

    public override CardBoost Copy() {
        return (CardBoost)base.Copy();
    }
    public override bool PlayCard(BattleManager bm)
    {
        var pos = (RowType)bm.Cursor.index;
        if (bm.Current.boosts[pos] is not null) return false;

        bm.Current.boosts[pos] = this;
        bm.Current.hand.Remove(this);
        position = pos;
        return true;
    }
}
