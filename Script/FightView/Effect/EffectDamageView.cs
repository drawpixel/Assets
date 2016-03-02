using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectDamageView : EffectBaseView
{

    public string FxFly;
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

        if (!string.IsNullOrEmpty(FxFly))
        {
            switch (ToTarget)
            {
                case ToTargetType.Once:
                    PrepareTime = 0;
                    foreach (Creature crt in Effect.CurtTargets)
                    {
                        CreatureView cv = Owner.Owner.FCtrllerView.GetCreatureView(crt);
                        BulletView bv = CreateBullet(cv.transform.localPosition, FxFly);
                        PrepareTime = Mathf.Max(PrepareTime, bv.FlyTime);
                    }
                    break;
                case ToTargetType.Sequent:
                    {
                        CreatureView cv = Owner.Owner.FCtrllerView.GetCreatureView(Effect.CurtTargets[0]);
                        CreateBullet(cv.transform.localPosition, FxFly);
                    }
                    break;
                case ToTargetType.OneFirst:
                    {
                        CreatureView cv = Owner.Owner.FCtrllerView.GetCreatureView(Effect.CurtTargets[0]);
                        CreateBullet(cv.transform.localPosition, FxFly);
                    }
                    break;
            }
        }
    }
    protected override void ActiveInternal(Creature target)
    {
        base.ActiveInternal(target);

        CreatureView cv = Owner.Owner.FCtrllerView.GetCreatureView(target);

        if (!string.IsNullOrEmpty(FxTarget))
        {
            FxBase fb = FxPool.Instance.Alloc(FxTarget);
            fb.transform.localPosition = cv.transform.localPosition;
        }    
    }

    BulletView CreateBullet(Vector3 target_pos, string fx)
    {
        FxBase fb = FxPool.Instance.Alloc(string.Format("Bullet/{0}/{1}", fx, fx));
        BulletView bv = fb.gameObject.GetComponent<BulletView>();
        bv.Active(Owner.Owner.transform.localPosition, target_pos);
        return bv;
    }
}
