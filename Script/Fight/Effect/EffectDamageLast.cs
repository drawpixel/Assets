﻿using System.Collections;
using System.Collections.Generic;


public class EffectDamageLast : EffectBase
{
    ProtoEffectDamageLast m_proto_damage;
    public new ProtoEffectDamageLast Proto
    {
        get { return m_proto_damage; }
    }

    public override void Create(ProtoEffect proto, Skill skill = null)
	{
        base.Create(proto, skill);

        m_proto_damage = proto as ProtoEffectDamageLast;
	}

    public override void Active(int times = 0)
    {
        for (int i = 0; i < CurtTargets.Count; ++i)
        {
            Creature crt = CurtTargets[i];
            crt.TakeDamage(OwnerSkill.OwnerCreature, Proto.Damage);
        }

        base.Active();
    }

    
}
