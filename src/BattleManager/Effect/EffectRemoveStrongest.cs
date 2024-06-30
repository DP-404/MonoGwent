using System;
using System.Linq;

namespace MonoGwent;

public class EffectRemoveStrongest : IEffect {
    public string Description {get => $"Removes the card with the highest power from the field.";}
    public EffectType Type {get => EffectType.ON_USE;}

    public bool Eval(BattleManager bm) {
        return true;
    }
    public void Use(BattleManager bm) {
        Tuple<Player,RowType,CardUnit> card = null;
        foreach (var p in bm.Players) {
            foreach (var r in p.rows.Keys) {
                var row = p.rows[r];
                foreach (var c in row) {
                    if (c.is_hero) continue;
                    if (card is null || c.power > card.Item3.power) {
                        card = new (p, r, c);
                    }
                }
            }
        }

        if (card is null) return;
        if (card.Item3 == bm.HandCard) return;
        card.Item1.rows[card.Item2].Remove(card.Item3);
        card.Item1.DisposeOf(card.Item3);
    }
}
