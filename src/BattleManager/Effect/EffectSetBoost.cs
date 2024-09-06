using System;
using System.Linq;

namespace MonoGwent;

public class EffectSetBoost : IEffect {
    public string Description {get => $"If there isn't a {Enum.GetName(row)} boost card set, searches one from the deck and sets it if found.";}
    public EffectType Type {get => EffectType.ON_USE;}
    public RowType row;

    public EffectSetBoost(RowType row) {
        this.row = row;
    }
    public bool Eval(BattleManager bm) {
        if (bm.EffectPlayer.HasBoost(row)) return false;
        foreach (var c in bm.EffectPlayer.deck.Cards) {
            if (
                c is CardBoost
                && c.types.Contains(row)
            ) return true;
        }
        return false;
    }
    public void Use(BattleManager bm) {
        foreach (var c in bm.EffectPlayer.deck.Cards) {
            if (
                c is CardBoost
                && c.types.Contains(row)
            ) {
                bm.EffectPlayer.deck.Remove(c);
                c.PlayCard(bm);
                break;
            }
        }
    }
}
