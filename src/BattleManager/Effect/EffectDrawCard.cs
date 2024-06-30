

namespace MonoGwent;

public class EffectDrawCard : IEffect {
    public string Description {get => "Draws an extra card.";}
    public EffectType Type {get => EffectType.ON_USE;}

    public bool Eval(BattleManager bm) {
        return bm.EffectPlayer.deck.Count > 0;
    }
    public void Use(BattleManager bm) {
        bm.EffectPlayer.ReceiveCard();
    }
}