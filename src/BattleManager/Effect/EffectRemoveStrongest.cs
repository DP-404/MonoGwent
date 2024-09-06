using System;

namespace MonoGwent;

public class EffectRemoveStrongest : IEffect {
    public string Description {get => $"Removes the card with the highest power from the field.";}
    public EffectType Type {get => EffectType.ON_USE;}

    public bool Eval(BattleManager bm) {
        return true;
    }
    public void Use(BattleManager bm) {
        Player player = null;
        CardUnit card = null;
        foreach (var p in bm.Players) {
            foreach (var c in p.field) {
                if (c is not CardUnit u) continue;
                if (u.is_hero) continue;
                if (
                    card is null
                    || c.power > card.power
                ) {
                    player = p;
                    card = u;
                }
            }
        }

        if (card is null) return;
        if (card == bm.HandCard) return;
        player.field.Remove(card);
        player.DisposeOf(card);
    }
}
