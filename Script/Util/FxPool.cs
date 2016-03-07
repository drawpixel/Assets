using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class FxPool : MonoBehaviour
{
	public static FxPool Instance;
	void Awake()
	{
		Instance = this;
	}
	public void OnDestroy()
	{
		Instance = null;
	}

	GameObject m_fx_root;

	Dictionary<string, List<FxBase>> m_pool = new Dictionary<string, List<FxBase>>();

	public void Init()
	{
		m_fx_root = Util.NewGameObject ("FxRoot", Launcher.Instance.CanvasUI.gameObject);
	}
    void Update()
    {
        foreach (List<FxBase> fbs in m_pool.Values)
        {
            foreach (FxBase fb in fbs)
            {
                if (fb.ActiveInPool)
                {
                    fb.FxUpdate();
                }
            }
        }
    }
    public FxBase Alloc(string key, GameObject parent = null)
    {
        if (!m_pool.ContainsKey(key))
        {
            m_pool[key] = new List<FxBase>();
        }

        FxBase ret = null;

        foreach (FxBase m in m_pool[key])
        {
            if (!m.ActiveInPool)
            {
                ret = m;
                break;
            }
        }

        if (ret == null)
        {
            FxBase new_m = ResMgr.Instance.CreateGameObject("Fx/" + key, null).GetComponent<FxBase>();
            new_m.Create();
            ret = new_m;
            m_pool[key].Add(new_m);
        }

        ret.ActiveInPool = true;

        if (parent != null)
        {
            ret.transform.SetParent(parent.transform);
        }
        else
        {
            ret.transform.SetParent(m_fx_root.transform);
        }
        ret.transform.localPosition = Vector3.zero;
        ret.transform.localRotation = Quaternion.identity;
        ret.transform.localScale = Vector3.one;

        return ret;
    }
    public FxBase Alloc(GameObject prefab, GameObject parent = null)
    {
        if (!m_pool.ContainsKey (prefab.name)) 
		{
			m_pool[prefab.name] = new List<FxBase>();
		}

		FxBase ret = null;

		foreach (FxBase m in m_pool[prefab.name]) 
		{
			if (!m.ActiveInPool)
			{
				ret = m;
				break;
			}
		}

		if (ret == null) 
		{
            FxBase new_m = Util.CreateGameObject(prefab, null).GetComponent<FxBase>();
			new_m.Create ();
			ret = new_m;
            m_pool[prefab.name].Add(new_m);
		}

		ret.ActiveInPool = true;

		if (parent != null) 
		{
			ret.transform.SetParent(parent.transform);
		} 
		else 
		{
			ret.transform.SetParent(m_fx_root.transform);
		}
		ret.transform.localPosition = Vector3.zero;
		ret.transform.localRotation = Quaternion.identity;
		ret.transform.localScale = Vector3.one;

		return ret;
	}
	public void Free(FxBase fb)
	{
		if (fb.FreeDelay > 0 && fb.CurtLife < 0)
        {
			fb.CountDown();
		}
        else
        {
			fb.ActiveInPool = false;
			fb.transform.SetParent(m_fx_root.transform);
		}
	}

    public void Cache(string k, int count)
    {
        List<FxBase> fbs = new List<FxBase>();
        for (int i = 0; i < count; ++i)
        {
            FxBase fb = Alloc(k, m_fx_root);
            fbs.Add(fb);
        }
        for (int i = 0; i < count; ++i)
        {
            Free(fbs[i]);
        }
    }
}
