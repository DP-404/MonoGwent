

namespace MonoGwent;

public class EffectNone : IEffect {
    public string Description {get => "";}
    public EffectType Type {get => EffectType.ON_USE;}

    public object Clone() {
        return new EffectNone();
    }
    public bool Eval(BattleManager bm) {
        return false;
    }
    public void Use(BattleManager bm) {
        return;
    }
}
