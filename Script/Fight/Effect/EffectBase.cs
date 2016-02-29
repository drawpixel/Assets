using System.Collections;
using System.Collections.Generic;


public class EffectBase
{
    public delegate void DgtActive(EffectBase effect);
    public DgtActive OnActive;

    public delegate void DgtActivePrepare(EffectBase effect);
    public DgtActivePrepare OnActivePrepare;

    public delegate void DgtActiveOver(EffectBase effect);
    public DgtActiveOver OnActiveOver;

    public enum StateType
    {
        Idle,
        Prepare,
        Block,
        Active,
        Over,
    }
    StateType m_state = StateType.Idle;
    public StateType State
    {
        get { return m_state; }
    }

    ProtoEffect m_proto;
    public ProtoEffect Proto
    {
        get { return m_proto; }
    }

    Skill m_owner_skill;
    public Skill OwnerSkill
    {
        get { return m_owner_skill; }
    }

    List<Creature> m_targets = null;
    public List<Creature> CurtTargets
    {
        get { return m_targets; }
    }

    public virtual void Create(ProtoEffect proto, Skill skill)
	{
        m_proto = proto;
        m_owner_skill = skill;
	}

    public virtual void Idle()
    {
        m_state = StateType.Idle;
    }

    public virtual void Prepare()
    {
        m_state = StateType.Prepare;

        m_targets = FetchTargets();

        if (OnActivePrepare != null)
        {
            OnActivePrepare(this);
        }
    }

    public virtual void Active()
    {
        m_state = StateType.Active;

        if (OnActive != null)
        {
            OnActive(this);
        }
    }

    public virtual void Over()
    {
        m_state = StateType.Over;

        if (OnActiveOver != null)
        {
            OnActiveOver(this);
        }
    }

    public virtual void Block()
    {
        m_state = StateType.Block;
    }



    public List<Creature> FetchTargets()
    {
        switch (Proto.TargetSelect)
        {
            case EffectTargetSelect.Front:
                return FetchTargetsFront();
        }

        return null;
    }
    public List<Creature> FetchTargetsFront()
    {
        List<Creature> targets = new List<Creature>();

        FightGrid grid = m_proto.TargetGridFace ? 
                            OwnerSkill.OwnerCreature.FGrid.Face : 
                            OwnerSkill.OwnerCreature.FGrid;

        for (int x = OwnerSkill.OwnerCreature.Index.X; x < FightGrid.UnitCount.X + OwnerSkill.OwnerCreature.Index.X; ++x)
        {
            int curt_x = x % FightGrid.UnitCount.X;

            for (int y = 0; y < FightGrid.UnitCount.Y; ++ y )
            {
                FightGrid.Unit u = grid.Units[y, curt_x];
                if (u.Creature == null || u.Creature.State == Creature.StateType.Death)
                    continue;
                else
                {
                    targets.Add(u.Creature);
                    return targets;
                }
            }
        }
        
        return null;
    }

}
