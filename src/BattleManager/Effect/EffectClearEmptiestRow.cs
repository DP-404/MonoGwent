using System;
using System.Collections.Generic;
using System.Linq;

namespace MonoGwent;

public class EffectClearEmptiestRow : IEffect {
    public string Description {get => $"Clears the row with fewer cards of either player.";}
    public EffectType Type {get => EffectType.ON_USE;}


    public bool Eval(BattleManager bm) {
        return true;
    }
    public void Use(BattleManager bm) {
        List<Tuple<Player,RowType>> fewest_rows = new();
        int fewest_count = 0;
        foreach (var p in bm.Players) {
            foreach (var rtype in Enum.GetValues(typeof(RowType)).Cast<RowType>()) {
                var r = p.GetRow(rtype);
                if (r.Count() == 0) continue;
                if (r.Count() < fewest_count || fewest_count == 0) {
                    fewest_rows.Clear();
                    fewest_count = r.Count();
                }
                if (r.Count() == fewest_count) {
                    fewest_rows.Add(new(p, rtype));
                }
            }
        }

        var index = Random.Shared.Next(fewest_rows.Count());
        var chosen_row = fewest_rows[index];
        var row = chosen_row.Item1.GetRow(chosen_row.Item2);

        for (int i = row.Count(); i != 0; i--) {
            var c = row[i-1];
            if (c is not CardUnit u) continue;
            if (u.is_hero) continue;
            chosen_row.Item1.DisposeOf(c);
            row.Remove(c);
        }
    }
}
