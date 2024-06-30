

namespace MonoGwent;

public enum EffectType {
    ON_USE,
    ON_PHASE_END
}

public interface IEffect
{
    public string Description {get;}
    public EffectType Type {get;}
    public abstract bool Eval(BattleManager bm);
    public abstract void Use(BattleManager bm);
}

public class EffectNone : IEffect {
    public string Description {get => "";}
    public EffectType Type {get => EffectType.ON_USE;}

    public bool Eval(BattleManager bm) {
        return false;
    }
    public void Use(BattleManager bm) {
        return;
    }
}

public class EffectDrawExtraCard : IEffect {
    public string Description {get => "Draws an extra card.";}
    public EffectType Type {get => EffectType.ON_USE;}

    public bool Eval(BattleManager bm) {
        return bm.EffectPlayer.deck.Count > 0;
    }
    public void Use(BattleManager bm) {
        bm.EffectPlayer.ReceiveCard();
    }
}

public class EffectRecoverLastDiscardedCard : IEffect {
    public string Description {get => "Takes card on top of graveyard to the hand.";}
    public EffectType Type {get => EffectType.ON_USE;}


    public bool Eval(BattleManager bm) {
        if (
            bm.EffectPlayer.graveyard.Count == 0
            || bm.EffectPlayer.hand.Count >= Player.HAND_CARDS_LIMIT
            || (
                bm.EffectPlayer.graveyard[^1] is CardUnit
                && ((CardUnit)bm.EffectPlayer.graveyard[^1]).is_hero
                )
        ) return false;
        return true;
    }
    public void Use(BattleManager bm) {
        bm.EffectPlayer.hand.Add(bm.EffectPlayer.graveyard[^1]);
        bm.EffectPlayer.graveyard.RemoveAt(bm.EffectPlayer.graveyard.Count-1);
    }
}

public class EffectLeaderWinOnMatch : IEffect {
    public string Description {get => "Wins a phase in case of match.";}
    public EffectType Type {get => EffectType.ON_PHASE_END;}

    public bool Eval(BattleManager bm) {
        return bm.Victor is null;
    }
    public void Use(BattleManager bm) {
        bm.AddVictor(bm.EffectPlayer);
    }
}

