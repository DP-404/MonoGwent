using System;
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
            foreach (var r in p.rows.Keys) {
                var row = p.rows[r];
                foreach (var c in row) {
                    if (c.is_hero || c.is_decoy) continue;
                    power.Add(c.ActualPower);
                    cards.Add(c);
                }
            }
        }

        var average = (int)power.Average();
        foreach (var c in cards) {
            c.modified_power = average;
        }
    }
}
