using System.Collections;
using System.Collections.Generic;


public class EffectBase
{
    public delegate void DgtActive(EffectBase effect, Creature[] targets);
    public DgtActive OnActive;

    public delegate bool DgtActivePrepare(EffectBase effect, Creature[] targets);
    public DgtActivePrepare OnActivePrepare;
    
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

    public virtual void Create(ProtoEffect proto, Skill skill = null)
	{
        m_proto = proto;
        m_owner_skill = skill;
	}
    
    public virtual void Active()
    {
        Creature[] targets = FetchTargets();
        if (OnActivePrepare != null)
        {
            bool is_ready = OnActivePrepare(this, targets);
            if (!is_ready)
            {
                return;
            }
        }

        DoActive(targets);
    }

    public virtual void DoActive(Creature[] targets)
    {
        if (OnActive != null)
        {
            OnActive(this, targets);
        }
    }

    public Creature[] FetchTargets()
    {
        switch (Proto.TargetSelect)
        {
            case EffectTargetSelect.Front:
                return FetchTargetsFront();
        }

        return null;
    }
    public Creature[] FetchTargetsFront()
    {
        Creature[] targets = null;

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
                    targets = new Creature[1];
                    targets[0] = u.Creature;
                    return targets;
                }
            }
        }
        
        return null;
    }

    protected void Active(Creature[] targets)
    {
        if (OnActive != null)
        {
            OnActive(this, targets);
        }
    }
    
}
