using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EffectBase
{
    public delegate void DgtActive(EffectBase effect, int idx);
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

    int m_idx_in_owner = 0;
    public int IndexInOwner
    {
        get { return m_idx_in_owner; }
    }

    public virtual void Create(ProtoEffect proto, Skill skill, int idx)
	{
        m_proto = proto;
        m_owner_skill = skill;
        m_idx_in_owner = idx;
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
        for (int t = 0; t < Proto.Times; ++ t)
        {
            for (int i = 0; i < CurtTargets.Count; ++i)
            {
                Creature crt = CurtTargets[i];
                Active(crt, t);
            }
        }
    }
    public virtual void Active(Creature target, int idx_times = 0)
    {
        if (State != StateType.Active)
        {
            m_state = StateType.Active;

            if (OnActive != null)
            {
                OnActive(this, idx_times);
            }    
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
                return FetchTargetsH(9);
            case EffectTargetSelect.FrontV:
                return FetchTargetsV(9);
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
        for (int y = 0; y < FightGrid.UnitCount.Y; ++y)
        {
            for (int i = 0; i < s_fetch_offset.Length; ++i)
            {
                int curt_x = s_fetch_offset[i] + c;
                if (curt_x < OwnerSkill.OwnerCreature.Index.X || curt_x >= OwnerSkill.OwnerCreature.Index.X + OwnerSkill.OwnerCreature.Proto.Dim.X)
                {
                    break;
                }
                
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
        for (int i = 0; i < s_fetch_offset.Length; ++i)
        {
            int curt_x = s_fetch_offset[i] + c;
            if (curt_x < 0 || curt_x >= FightGrid.UnitCount.X)
                continue;

            for (int y = 0; y < FightGrid.UnitCount.Y; ++y)
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

    public List<Creature> FetchTargetsH(int count)
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
                if (near != null && !targets.Contains(near) && near.State != Creature.StateType.Death)
                {
                    targets.Add(near);
                }
            }
            if (i >= count - 1)
                break;
        }
        
        return targets;
    }

    public List<Creature> FetchTargetsV(int count)
    {
        List<Creature> targets = FetchTargetsFront();
        if (targets == null)
            return null;

        Creature center = targets[0];

        for (int i = center.Index.Y; i < FightGrid.UnitCount.Y; ++i)
        {
            Creature near = center.FGrid.Units[i, center.CenterIndex.X].Creature;
            if (near != null && !targets.Contains(near) && near.State != Creature.StateType.Death)
            {
                targets.Add(near);
            }
        }

        return targets;
    }
}
