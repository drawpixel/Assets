using System.Collections;
using System.Collections.Generic;


public class ProtoSkill : ProtoBase
{
    public float MageSpend;
    public int[] Effects;

    ProtoEffect[] m_effects;
    public ProtoEffect[] ProtoEffects
    {
        get { return m_effects; }
    }

    public override void Create()
    {
        if (Effects.Length > 0)
        {
            m_effects = new ProtoEffect[Effects.Length];
            for (int i = 0; i < m_effects.Length; ++i)
            {
                ProtoEffect proto = ProtoMgr.Instance.GetByID<ProtoEffect>(Effects[i]);

                m_effects[i] = proto;
            }
        }
        
    }
}
