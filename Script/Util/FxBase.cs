using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FxBase : MonoBehaviour
{
	public float Life = 2;
	public float FreeDelay = 0;

	float m_life_counter = 0;
	public float LifeCounter
	{
		get {return m_life_counter;}
	}

	float m_curt_life = 0;
	public float CurtLife
	{
		get {return m_curt_life;}
		set
		{
			m_curt_life = value;
		}
	}

	Animator[] m_cached_animator;
	ParticleSystem[] m_cached_particle;

	public void Create()
	{
		m_cached_animator = GetComponentsInChildren<Animator> ();
		m_cached_particle = GetComponentsInChildren<ParticleSystem> ();
	}

	public void Reset()
	{
		m_life_counter = 0;
		m_curt_life = Life;

		if (m_cached_animator != null) 
        {
			foreach (Animator m in m_cached_animator) 
            {
				m.Play (0, 0, 0);
			}
		}

		if (m_cached_particle != null) 
        {
			foreach (ParticleSystem m in m_cached_particle)
			{
                //m.time = 0;
                //m.Clear();
                //m.Stop();
				//m.Play();
                //Debug.Log(string.Format("{0} {1} {2} {3}", m.IsAlive(), m.isPaused, m.isPlaying, m.isStopped));
			}
		}
	}

	void DisableFx()
	{
		if (m_cached_particle != null) {
			foreach (ParticleSystem m in m_cached_particle)
			{
				m.Stop();
			}
		}
	}

	void Update()
	{
		m_life_counter += Time.deltaTime;
		if (CurtLife >= 0 && m_life_counter > CurtLife) 
		{
			FxPool.Instance.Free(this);
		}
	}

	public void CountDown()
	{
		CurtLife = LifeCounter + FreeDelay;
	}

	bool m_active_in_pool = true;
	public bool ActiveInPool
	{
		get {return m_active_in_pool;}
		set
		{
			if (value)
			{
				gameObject.SetActive(true);
				Reset();
			}
			else
			{
				gameObject.SetActive(false);
				DisableFx();
			}
			m_active_in_pool = value;
		}
	}
}

public class FxPool
{
	public static FxPool Instance;
	public static void CreasteInstance()
	{
		Instance = new FxPool();
	}
	public static void DestroyInstance()
	{
		Instance = null;
	}

	GameObject m_fx_root;

	Dictionary<string, List<FxBase>> m_pool = new Dictionary<string, List<FxBase>>();

	public void Init()
	{
		m_fx_root = Util.NewGameObject ("FxRoot", Launcher.Instance.CanvasUI.gameObject);
	}
	public FxBase Alloc(string key, GameObject parent = null)
	{
        if (!m_pool.ContainsKey (key)) 
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
			new_m.Create ();
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
