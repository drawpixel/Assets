using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectDamageView : EffectBaseView
{
    public string Bullet;
    public float TimesInterval = 0.2f;
    //public string FxTarget;
    public GameObject PrefabFxTarget;

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

    public override void Ready()
    {
        base.Ready();

        if (!string.IsNullOrEmpty(Bullet))
        {
            CreateBullet();
        }
    }
    protected override void ActiveInternal(Creature target)
    {
        base.ActiveInternal(target);

        CreatureView cv = Owner.Owner.FCtrllerView.GetCreatureView(target);

        if (PrefabFxTarget != null)
        {
            FxBase fb = FxPool.Instance.Alloc(PrefabFxTarget);
            fb.transform.localPosition = cv.transform.localPosition;
        }    
    }

    void CreateBullet()
    {
        switch (ToTarget)
        {
            case ToTargetType.Once:
                PrepareTime = 0;
                foreach (Creature crt in Effect.CurtTargets)
                {
                    CreatureView cv = Owner.Owner.FCtrllerView.GetCreatureView(crt);
                    BulletView bv = CreateBullet(crt);
                    PrepareTime = Mathf.Max(PrepareTime, bv.FlyTime);
                }
                break;
            case ToTargetType.Sequent:
                {
                    BulletView bv = CreateBullet(Effect.CurtTargets.ToArray());
                    PrepareTime = bv.CurtFlyTime;
                    bv.OnReachTarget = (blt, idx) => { NextTargetTime = blt.CurtFlyTime; };
                }
                break;
            case ToTargetType.OneFirst:
                {
                    CreatureView cv = Owner.Owner.FCtrllerView.GetCreatureView(Effect.CurtTargets[0]);
                    CreateBullet(Effect.CurtTargets[0]);
                }
                break;
        }
    }
    BulletView CreateBullet(Creature target)
    {
        return CreateBullet(new Creature[1] { target });
    }
    BulletView CreateBullet(Creature[] targets)
    {
        BulletView bv = BulletViewPool.Instance.Alloc(Bullet, Owner.Owner.transform.localPosition, targets);
        return bv;
    }
}
