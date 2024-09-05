

namespace MonoGwent;

public class CardLeader : Card {
    public override string type_name {get => "Leader";}
    public bool used = false;

    public override CardLeader Copy() {
        return (CardLeader)base.Copy();
    }
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
