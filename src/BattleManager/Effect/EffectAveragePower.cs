using System.Collections.Generic;
using System.Linq;

namespace MonoGwent;

public class EffectAveragePower : IEffect {
    public string Description {get => $"Sets the average power among all cards in the fields to all them.";}
    public EffectType Type {get => EffectType.ON_USE;}

    public bool Eval(BattleManager bm) {
        return true;
    }
    public void Use(BattleManager bm) {
        List<int> power = new();
        List<CardUnit> cards = new();

        foreach (var p in bm.Players) {
            foreach (var c in p.field) {
                if (c is not CardUnit unit) continue;
                if (unit.is_hero || unit.is_decoy) continue;
                power.Add(unit.ActualPower);
                cards.Add(unit);
            }
        }

        if (cards.Count() == 0) return;

        var average = (int)power.Average();
        foreach (var c in cards) {
            c.power = average;
        }
    }
}
