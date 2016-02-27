using System.Collections;
using System.Collections.Generic;


public class BuffDamage : BuffBase
{
    float m_damage = 0;

    public void Create(Creature crt, float damage)
	{
        base.Create(crt);

        m_damage = damage;
	}
    
}
