using System.Collections;
using System.Collections.Generic;


public enum EffectTargetSelect
{
    All,
    Front,
    FrontV,
    FrontH,
    Back,
    BackV,
    BackH,
}

public enum EffectExistType
{
    Caster,
    Target,
}
public enum EffectExcludeType
{
    Add,
    Refuse,
    Replace,
}

public class ProtoEffect : ProtoBase
{
    public float Probability;
    public bool TargetGridFace = true;
    public EffectTargetSelect TargetSelect = EffectTargetSelect.Front;

    public string EffectViewOverride = "";

    public virtual EffectBase NewInstance()
    {
        return null;
    }
}

public class ProtoEffectDamage : ProtoEffect
{
    public float Damage;

    public override EffectBase NewInstance()
    {
        return new EffectDamage();
    }
}

public class ProtoEffectDamageLast : ProtoEffect
{
    public EffectExistType Exist = EffectExistType.Target;
    public EffectExcludeType Exclude = EffectExcludeType.Replace;
    public float LastTime;
    public float Interval;
    public float Damage;

    public override EffectBase NewInstance()
    {
        return new EffectDamageLast();
    }
}
