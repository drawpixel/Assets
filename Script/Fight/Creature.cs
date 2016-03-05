using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Creature
{
    public delegate void DgtIdle();
    public DgtIdle OnIdle;

    public delegate void DgtCast(int idx);
    public DgtCast OnCast;

    public delegate void DgtDead(Creature killer);
    public DgtDead OnDead;

    public delegate void DgtFreeze(Creature killer);
    public DgtFreeze OnFreeze;


    public delegate void DgtTakeDamage(Creature killer, float curt);
    public DgtTakeDamage OnTakeDamage;

    public delegate void DgtMentalityChange(Creature caster, MentalityType prev, MentalityType curt);
    public DgtMentalityChange OnMentalityChange;


    InfoCreature m_info;
    public InfoCreature Info
    {
        get { return m_info; }
    }
    public ProtoCreature Proto
    {
        get { return m_info.Proto; }
    }

    FightGrid m_fg = null;
    public FightGrid FGrid
    {
        get { return m_fg; }
        set
        {
            m_fg = value;
        }
    }
    Int2D m_idx;
    public Int2D Index
    {
        get { return m_idx; }
        set
        {
            m_idx = value;
        }
    }
    public Int2D CenterIndex
    {
        get
        { return Index + new Int2D((Proto.Dim.X - 1) / 2, (Proto.Dim.Y - 1) / 2); }
    }
    public Vector3 Center
    {
        get
        {
            Vector3 v = FGrid.Units[Index.Y, Index.X].Position;
            Vector3 offset = new Vector3(((Proto.Dim.X - 1) / 2.0f) * FightGrid.UnitSize, ((Proto.Dim.Y - 1) / 2.0f) * FightGrid.UnitSize, 0);
            if (FGrid.Dir == FightGrid.DirType.Up)
                v += offset;
            else
                v += new Vector3(offset.x, -offset.y, 0);
            return v;
        }
    }
    public Vector3 CenterAlign
    {
        get
        {
            Vector3 v = FGrid.Units[Index.Y, Index.X].Position;
            Vector3 offset = new Vector3(((Proto.Dim.X - 1) / 2) * FightGrid.UnitSize, ((Proto.Dim.Y - 1) / 2) * FightGrid.UnitSize, 0);
            if (FGrid.Dir == FightGrid.DirType.Up)
                v += offset;
            else
                v += new Vector3(offset.x, -offset.y, 0);
            return v;
        }
    }

    float m_max_hp = 0;
    public float MaxHP
    {
        get { return m_max_hp; }
    }
    float m_remain_hp = 0;
    public float RemainHP
    {
        get { return m_remain_hp; }
    }


    public enum StateType
    {
        Idle,
        Death,
        Casting,
    }
    StateType m_state = StateType.Idle;
    public StateType State
    {
        get { return m_state; }
    }
    float m_state_counter = 0;


    public enum MentalityType
    {
        Normal,
        Freeze,
        Blind,
        Sleep,
    }
    MentalityType m_mentality = MentalityType.Normal;
    public MentalityType Mentality
    {
        get { return m_mentality; }
    }


    List<Skill> m_skills = new List<Skill>();
    public List<Skill> Skills
    {
        get { return m_skills; }
    }

    public void Create(InfoCreature info)
	{
        m_info = info;

        m_max_hp = m_remain_hp = info.Proto.HP;

        for (int i = 0; Info.Skills != null && i < Info.Skills.Length; ++ i)
        {
            Skill sk = new Skill();
            sk.Create(Info.Skills[i], this);
            m_skills.Add(sk);
        }
        
        Idle();
	}
    
    public void Update(float interval)
    {
        m_state_counter += interval;
        
        switch (State)
        {
            case StateType.Idle:
                break;
            case StateType.Death:
                break;
        }
    }

    public void TakeDamage(Creature killer, float damage)
    {
        if (State == StateType.Death)
            return;

        m_remain_hp -= damage;
        if (m_remain_hp <= 0)
        {
            m_remain_hp = 0;
        }
        if (OnTakeDamage != null)
        {
            OnTakeDamage(killer, damage);
        }

        if (m_remain_hp <= 0)
        {
            Dead(killer);
        }
    }
    public void TakeMentality(Creature caster, MentalityType mt)
    {
        MentalityType prev = m_mentality;
        m_mentality = mt;

        if (OnMentalityChange != null)
        {
            OnMentalityChange(caster, prev, mt);
        }
    }


    public void Idle()
    {
        if (State == StateType.Death)
            return;
        m_state = StateType.Idle;
        m_state_counter = 0;
    }
    public void Cast(int idx)
    {
        if (idx >= m_skills.Count)
            return;

        m_state = StateType.Casting;
        m_state_counter = 0;

        m_skills[idx].Cast();

        if (OnCast != null)
        {
            OnCast(idx);
        }
    }
    public void Dead(Creature killer)
    {
        m_state = StateType.Death;
        m_state_counter = 0;

        if (OnDead != null)
        {
            OnDead(killer);
        }
    }
}
