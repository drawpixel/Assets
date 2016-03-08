using System.Collections;
using System.Collections.Generic;


public class Skill
{
    public delegate void DgtCastPrepare(Skill sk);
    public DgtCastPrepare OnCastPrepare;
    public delegate void DgtCast(Skill sk);
    public DgtCast OnCast;
    public delegate void DgtCastOver(Skill sk);
    public DgtCastOver OnCastOver;

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
    bool m_is_block = false;
    public bool IsBlock
    {
        get { return m_is_block; }
        set
        {
            m_is_block = value;
        }
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
            EffectBase eb = proto.NewInstance(); 
            eb.Create(proto, this, i);
            eb.OnActiveOver += OnEffectActiveOver;
            m_effects[i] = eb;
        }
	}
    
    public void Update(float interval)
    {
    }

    public void Cast()
    {
        if (OnCastPrepare != null)
        {
            OnCastPrepare(this);
        }

        if (IsBlock)
        {
            return;
        }

        m_is_casting = true;

        foreach (EffectBase eb in m_effects)
        {
            if (eb.State != EffectBase.StateType.Block)
            {
                eb.Prepare();
            }
        }

        foreach (EffectBase eb in m_effects)
        {
            if (eb.State != EffectBase.StateType.Block)
            {
                eb.Active();
            }
        }

        foreach (EffectBase eb in m_effects)
        {
            if (eb.State != EffectBase.StateType.Block)
            {
                eb.Over();
            }
        }

        if (OnCast != null)
        {
            OnCast(this);
        }
    }
    void CastOver()
    {
        m_is_casting = false;

        foreach (EffectBase eb in m_effects)
        {
            eb.Idle();
        }

        if (OnCastOver != null)
        {
            OnCastOver(this);
        }
    }

    void OnEffectActiveOver(EffectBase effect)
    {
        bool all_over = true;
        foreach (EffectBase eb in m_effects)
        {
            if (eb.State != EffectBase.StateType.Over)
            {
                all_over = false;
                break;
            }
        }
        if (all_over)
        {
            CastOver();
        }
    }
    
}
