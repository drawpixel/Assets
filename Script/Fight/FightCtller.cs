using System.Collections;
using System.Collections.Generic;

public class FightCtller
{
    public static Int2D Dim = new Int2D(640, 1136);

    FightGrid[] m_fg = new FightGrid[2];

    public FightGrid[] FGrids
    {
        get
        {
            return m_fg;
        }

        set
        {
            m_fg = value;
        }
    }

    public void Create()
	{
        for (int i = 0; i < m_fg.Length; ++i)
        {
            m_fg[i] = new FightGrid();
            m_fg[i].Create(this, (FightGrid.DirType)i);
        }
        
	}

    public void Update(float interval)
    {
        foreach (FightGrid fg in m_fg)
        {
            fg.Update(interval);
        }

    }

    public void Fight()
    {
        foreach (FightGrid fg in m_fg)
        {
            fg.Fight();
        }
    }


}
