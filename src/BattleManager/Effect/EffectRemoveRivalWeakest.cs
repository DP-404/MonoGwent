using System;

namespace MonoGwent;

public class EffectRemoveRivalWeakest : IEffect {
    public string Description {get => $"Removes the rival card with the lowest power from the field.";}
    public EffectType Type {get => EffectType.ON_USE;}

    public object Clone() {
        return new EffectRemoveRivalWeakest();
    }
    public bool Eval(BattleManager bm) {
        return true;
    }
    public void Use(BattleManager bm) {
        CardUnit card = null;
        foreach (var c in bm.Rival.field) {
            if (c is not CardUnit u) continue;
            if (u.is_hero) continue;
            if (
                card is null
                || c.power < card.power
            ) {
                card = u;
            }
        }

        if (card is null) return;
        bm.Rival.field.Remove(card);
        bm.Rival.DisposeOf(card);
    }
}
