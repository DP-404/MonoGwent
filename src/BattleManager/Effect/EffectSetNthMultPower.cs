

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
        foreach (var c in bm.Current.field) {
            if (c is not CardUnit u) continue;
            if (u.is_hero) continue;
            if (u.name == card.name) mult += 1;
        }

        if (mult <= 1) return;
        card.power = card.power * mult;
    }
}
