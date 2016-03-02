using System.Collections;
using System.Collections.Generic;

public class FightCtller
{
    public static Int2D Dim = new Int2D(640, 1136);


    
    public enum StateType
    {
        Prepare,
        Idle,
        Casting,
        Over,
    }
    StateType m_state = StateType.Prepare;
    public StateType State
    {
        get { return m_state; }
    }

    FightGrid[] m_fg = new FightGrid[2];
    public FightGrid[] FGrids
    {
        get
        {
            return m_fg;
        }
    }

    public class QueueNode
    {
        public Creature Crt;

        public QueueNode(Creature crt)
        {
            Crt = crt;
        }
    }
    List<QueueNode> m_queue = new List<QueueNode>();


    public void Create()
	{
        for (int i = 0; i < m_fg.Length; ++i)
        {
            m_fg[i] = new FightGrid();
            m_fg[i].Create(this, (FightGrid.DirType)i);
        }
	}

    public void Idle()
    {
        m_state = StateType.Idle;

        foreach (FightGrid fg in m_fg)
        {
            fg.Idle();
        }
    }

    public void Over()
    {
        m_state = StateType.Over;

    }

    public void Next()
    {
        int ret = CheckResult();
        if (ret >= 0)
        {
            Over();
            return;
        }
        if (m_queue.Count == 0)
        {
            NextRound();
        }
        if (m_queue.Count == 0)
        {
            return;
        }
        if (m_queue[0].Crt.State == Creature.StateType.Death)
        {
            m_queue.RemoveAt(0);
            Next();
            return;
        }
        m_queue[0].Crt.Skills[0].OnCastOver += OnCastOver;
        m_queue[0].Crt.Cast(0);
        m_queue.RemoveAt(0);

        m_state = StateType.Casting;
    }

    int CheckResult()
    {
        for (int f = 0; f < m_fg.Length; ++f)
        {
            bool all_death = true;
            foreach (Creature crt in m_fg[f].Creatures.Keys)
            {
                if (crt.State != Creature.StateType.Death)
                {
                    all_death = false;
                    break;
                }
            }
            if (all_death)
            {
                return f;
            }
        }
        return -1;
    }

    void NextRound()
    {
        int toto = 0;
        List<Creature>[] crts = new List<Creature>[m_fg.Length];
        for (int f = 0; f < m_fg.Length; ++f)
        {
            crts[f] = new List<Creature>();
            for (int y = 0; y < FightGrid.UnitCount.Y; ++y)
            {
                for (int x = 0; x < FightGrid.UnitCount.X; ++x)
                {
                    Creature curt = m_fg[f].Units[y, x].Creature;
                    if (curt != null && !crts[f].Contains(curt) && curt.State != Creature.StateType.Death)
                    {
                        crts[f].Add(curt);
                        ++toto;
                    }
                }
            }
        }

        m_queue.Clear();

        int side = 0;
        while (true)
        {
            if (crts[side].Count > 0)
            {
                m_queue.Add(new QueueNode(crts[side][0]));
                crts[side].RemoveAt(0);
            }
            side = (side + 1) % m_fg.Length;

            if (m_queue.Count >= toto)
                break;
        }
    }

    void OnCastOver(Skill sk)
    {
        sk.OnCastOver -= OnCastOver;

        Idle();
    }
}
