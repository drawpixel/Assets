using System.Collections;
using System.Collections.Generic;


public class BuffBase
{
    EffectBase m_owner_effect;
    public EffectBase OwnerEffect
    {
        get { return m_owner_effect; }
        set
        {
            m_owner_effect = value;
        }
    }

    Creature m_owner_crt;
    public Creature OwnerCreature
    {
        get { return m_owner_crt; }
    }

    public void Create(Creature crt)
	{
        m_owner_crt = crt;
	}
    
}
