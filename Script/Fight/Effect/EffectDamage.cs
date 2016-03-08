using System.Collections;
using System.Collections.Generic;


public class EffectDamage : EffectBase
{
    ProtoEffectDamage m_proto_damage;
    public new ProtoEffectDamage Proto
    {
        get { return m_proto_damage; }
    }

    public override void Create(ProtoEffect proto, Skill skill = null, int idx = 0)
	{
        base.Create(proto, skill, idx);

        m_proto_damage = proto as ProtoEffectDamage;
	}

    public override void Active(Creature target, int times = 0)
    {
        base.Active(target, times);

        target.TakeDamage(OwnerSkill.OwnerCreature, Proto.Damage / (float)Proto.Times);
    }
}
