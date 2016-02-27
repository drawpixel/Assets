using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightCtllerView : MonoBehaviour
{

    FightCtller m_ctller;
    public FightCtller Ctller
    {
        get { return m_ctller; }
    }

    FightGridView[] m_fgv = new FightGridView[2];
    public FightGridView[] FGridViews
    {
        get
        {
            return m_fgv;
        }
    }

    GameObject m_field = null;
    public GameObject Field
    {
        get { return m_field; }
    }

    

    public void Create(FightCtller ctller)
	{
        m_ctller = ctller;
        gameObject.layer = (int)UnityLayer.UI;

        for (int i = 0; i < m_fgv.Length; ++i)
        {
            m_fgv[i] = Util.NewGameObject("Grid_" + ctller.FGrids[i].Dir.ToString(), gameObject).AddComponent<FightGridView>();
            m_fgv[i].Create(this, m_ctller.FGrids[i]);
        }

        m_field = ResMgr.Instance.CreateGameObject(string.Format("Field/{0}", "F01"), gameObject);
	}
    
    void Update()
    {
        
    }

    

    
}
