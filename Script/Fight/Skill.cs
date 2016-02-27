using System.Collections;
using System.Collections.Generic;


public class Skill
{
    public delegate void DgtCast(Creature caster);
    public DgtCast OnCast;
    

    InfoSkill m_info;
    public InfoSkill Info
    {
        get { return m_info; }
    }
    public ProtoSkill Proto
    {
        get { return m_info.Proto; }
    }

    Creature m_owner_crt;
    public Creature OwnerCreature
    {
        get { return m_owner_crt; }
    }


    bool m_is_casting = false;
    public bool IsCasting
    {
        get { return m_is_casting; }
    }

    EffectBase[] m_effects = null;
    public EffectBase[] Effects
    {
        get { return m_effects; }
    }


    public void Create(InfoSkill info, Creature crt)
	{
        m_info = info;
        m_owner_crt = crt;

        m_effects = new EffectBase[Proto.ProtoEffects.Length];
        for (int i = 0; i < m_effects.Length; ++i)
        {
            ProtoEffect proto = Proto.ProtoEffects[i];
            m_effects[i] = proto.NewInstance(); ;
        }
	}
    
    public void Update(float interval)
    {
    }

    public void Cast(Creature caster)
    {
        if (OnCast != null)
        {
            OnCast(caster);
        }
    }

    
}
