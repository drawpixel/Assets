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


    static int[] s_fetch_offset = new int[] { 0, 1, -1, 2, -2, 3, -3, 4, -4 };
    public List<Creature> FetchTargets()
    {
        switch (Proto.TargetSelect)
        {
            case EffectTargetSelect.Front:
                return FetchTargetsFront();
            case EffectTargetSelect.FrontH:
                return FetchTargetsHorizontal(9);
        }

        return null;
    }
    public List<Creature> FetchTargetsFront()
    {
        List<Creature> targets = new List<Creature>();

        FightGrid grid = m_proto.TargetGridFace ? 
                            OwnerSkill.OwnerCreature.FGrid.Face : 
                            OwnerSkill.OwnerCreature.FGrid;

        int c = OwnerSkill.OwnerCreature.Index.X + (OwnerSkill.OwnerCreature.Proto.Dim.X - 1) / 2;
        for (int i = 0; i < s_fetch_offset.Length; ++ i)
        {
            int curt_x = s_fetch_offset[i] + c;
            if (curt_x < 0 || curt_x >= FightGrid.UnitCount.X)
                continue;
            
            for (int y = 0; y < FightGrid.UnitCount.Y; ++ y )
            {
                FightGrid.Unit u = grid.Units[y, curt_x];
                if (u.Creature == null || u.Creature.State == Creature.StateType.Death)
                {
                    continue;
                }
                else
                {
                    targets.Add(u.Creature);
                    return targets;
                }
            }
        }
        
        return null;
    }

    public List<Creature> FetchTargetsHorizontal(int count)
    {
        List<Creature> targets = FetchTargetsFront();
        if (targets == null)
            return null;

        Creature center = targets[0];

        for (int i = 1; i < s_fetch_offset.Length; ++i)
        {
            int offset = s_fetch_offset[i];
            if (center.Index.X + offset >= 0 && center.Index.X + offset < FightGrid.UnitCount.X)
            {
                Creature near = center.FGrid.Units[center.Index.Y, center.Index.X + offset].Creature;
                if (near != null && !targets.Contains(near))
                {
                    targets.Add(near);
                }
            }
            if (i >= count - 1)
                break;
        }
        
        return targets;
    }
}
