using System;
using System.Linq;

namespace MonoGwent;

public class EffectSetWeather : IEffect {
    public string Description {get => $"If there isn't a {Enum.GetName(row)} weather card set, searches one from the deck and sets it if found.";}
    public EffectType Type {get => EffectType.ON_USE;}
    public RowType row;

    public object Clone() {
        return new EffectSetWeather(row);
    }
    public EffectSetWeather(RowType row) {
        this.row = row;
    }
    public bool Eval(BattleManager bm) {
        if (bm.ExistsWeather(row)) return false;
        foreach (var c in bm.EffectPlayer.deck.Cards) {
            if (
                c is CardWeather
                && c.types.Contains(row)
            ) return true;
        }
        return false;
    }
    public void Use(BattleManager bm) {
        foreach (var c in bm.EffectPlayer.deck.Cards) {
            if (
                c is CardWeather
                && c.types.Contains(row)
            ) {
                bm.EffectPlayer.deck.Remove(c);
                c.PlayCard(bm);
                break;
            }
        }
    }
}
