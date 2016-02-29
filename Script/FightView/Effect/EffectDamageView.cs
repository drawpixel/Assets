using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectDamageView : EffectBaseView
{
    public string FxTarget;
    
    EffectDamage m_effect_damage;
    public new EffectDamage Effect
    {
        get { return m_effect_damage; }
    }

    public override void Create(EffectBase effect, SkillView sv)
	{
        base.Create(effect, sv);

        m_effect_damage = effect as EffectDamage;
	}

    public override void Prepare()
    {
        base.Prepare();
    }
    public override void Active()
    {
        base.Active();

        foreach (Creature crt in Effect.CurtTargets)
        {
            CreatureView cv = Owner.Owner.FCtrllerView.GetCreatureView(crt);

            if (!string.IsNullOrEmpty(FxTarget))
            {
                FxBase fb = FxPool.Instance.Alloc(FxTarget);
                fb.transform.localPosition = cv.transform.localPosition;
            }
        }
    }

    
}
