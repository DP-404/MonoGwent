

namespace MonoGwent;

public class EffectLeaderRecoverLastDiscardedCard : IEffect {
    public string Description {get => "Takes card on top of graveyard to the hand.";}
    public EffectType Type {get => EffectType.ON_USE;}


    public object Clone() {
        return new EffectLeaderRecoverLastDiscardedCard();
    }
    public bool Eval(BattleManager bm) {
        if (
            bm.EffectPlayer.graveyard.Count == 0
            || bm.EffectPlayer.hand.Count >= Player.HAND_CARDS_LIMIT
            || (
                bm.EffectPlayer.graveyard[^1] is CardUnit unit
                && unit.is_hero
            )
        ) return false;
        return true;
    }
    public void Use(BattleManager bm) {
        bm.EffectPlayer.Retrieve(bm.EffectPlayer.graveyard[^1]);
        bm.EffectPlayer.graveyard.RemoveAt(bm.EffectPlayer.graveyard.Count-1);
    }
}
