using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FxBase : MonoBehaviour
{
	public float Life = 2;
	public float FreeDelay = 0;
    public RectTransform[] RectTransLaserBodies;
    public Transform[] LaserEnd;

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

    public void ScaleBody(float length)
    {
        foreach (RectTransform rt in RectTransLaserBodies)
        {
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, length);
        }
        foreach (Transform t in LaserEnd)
        {
            t.localPosition = Vector3.up * length;
        }
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

	public void FxUpdate()
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
