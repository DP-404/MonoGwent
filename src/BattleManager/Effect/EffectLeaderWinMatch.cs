

namespace MonoGwent;

public class EffectLeaderWinMatch : IEffect {
    public string Description {get => "Wins a phase in case of match.";}
    public EffectType Type {get => EffectType.ON_PHASE_END;}

    public bool Eval(BattleManager bm) {
        return bm.Victor is null;
    }
    public void Use(BattleManager bm) {
        bm.AddVictor(bm.EffectPlayer);
    }
}
