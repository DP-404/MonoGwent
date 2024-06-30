using System;
using System.Linq;

namespace MonoGwent;

public class EffectRemoveRivalWeakest : IEffect {
    public string Description {get => $"Removes the rival card with the lowest power from the field.";}
    public EffectType Type {get => EffectType.ON_USE;}

    public bool Eval(BattleManager bm) {
        return true;
    }
    public void Use(BattleManager bm) {
        Tuple<RowType,CardUnit> card = null;
        foreach (var r in bm.Rival.rows.Keys) {
            var row = bm.Rival.rows[r];
            foreach (var c in row) {
                if (c.is_hero) continue;
                if (card is null || c.power < card.Item2.power) {
                    card = new (r, c);
                }
            }
        }

        if (card is null) return;
        bm.Rival.rows[card.Item1].Remove(card.Item2);
        bm.Rival.graveyard.Add(card.Item2);
    }
}
