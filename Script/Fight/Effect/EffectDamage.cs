using System.Collections;
using System.Collections.Generic;


public class EffectDamage : EffectBase
{
    ProtoEffectDamage m_proto_damage;
    public new ProtoEffectDamage Proto
    {
        get { return m_proto_damage; }
    }

    public override void Create(ProtoEffect proto, Skill skill = null)
	{
        base.Create(proto, skill);

        m_proto_damage = proto as ProtoEffectDamage;
	}

    public override void Active(Creature target)
    {
        base.Active(target);

        target.TakeDamage(OwnerSkill.OwnerCreature, Proto.Damage);
    }
}
