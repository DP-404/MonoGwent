

namespace MonoGwent;

public class EffectSetNthMultPower : IEffect {
    public string Description {get => $"When placed, multiplies this card power by the amount of copies of it in your field.";}
    public EffectType Type {get => EffectType.ON_USE;}

    public bool Eval(BattleManager bm) {
        return true;
    }
    public void Use(BattleManager bm) {
        CardUnit card = (CardUnit)bm.HandCard;
        int mult = 0;
        foreach (var r in bm.Current.rows.Keys) {
            var row = bm.Current.rows[r];
            foreach (var c in row) {
                if (c.is_hero) continue;
                if (c.name == card.name) mult += 1;
            }
        }

        if (mult <= 1) return;
        card.modified_power = card.power * mult;
    }
}
