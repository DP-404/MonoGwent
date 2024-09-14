

using System;

namespace MonoGwent;

public enum EffectType {
    ON_USE,
    ON_PHASE_END
}

public interface IEffect : ICloneable {
    public string Description {get;}
    public EffectType Type {get;}
    public abstract bool Eval(BattleManager bm);
    public abstract void Use(BattleManager bm);
}
