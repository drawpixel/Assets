using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletView : MonoBehaviour
{
    public delegate void DgtReachTarget(BulletView bv, int idx);
    public DgtReachTarget OnReachTarget;
    
    public AnimationCurve CurveFlySide = new AnimationCurve(new Keyframe(0, 0, 0, 1), new Keyframe(1, 1, 1, 0));
    public AnimationCurve CurveFlyForward = new AnimationCurve(new Keyframe(0, 0, 0, 1), new Keyframe(1, 1, 1, 0));
    
    public float Speed = 100;

    public enum AlignType
    {
        Center,
        Grid,
        FirstCenterX,
        FirstCenterY,
        FitstGridX,
        FitstGridY,
    }
    public AlignType Align = AlignType.Center;

    public enum FlyModeType
    {
        Linear,
        Laser,
    }
    public FlyModeType FlyMode = FlyModeType.Linear; 

    float m_fly_time = 0;
    public float FlyTime
    {
        get { return m_fly_time; }
    }
    float m_curt_fly_time = 0;
    public float CurtFlyTime
    {
        get { return m_curt_fly_time; }
    }

    //public string FxFly;
    public GameObject PrefabFxFly;
    public GameObject PrefabFxEnd;

    float m_time_counter = 0;
    float m_totol_time_counter = 0;

    Vector3 m_start;
    Vector3[] m_end;
    int m_end_index = 0;

    FxBase m_fb = null;

    public Vector3 CurtEnd
    {
        get
        {
            return m_end[m_end_index];
        }
    }
    public Vector3 CurtStart
    {
        get
        {
            return m_end_index == 0 ? m_start : m_end[m_end_index - 1];
        }
    }
    

    bool m_active_in_pool = true;
    public bool ActiveInPool
    {
        get { return m_active_in_pool; }
    }


    public void Create()
    {
        
    }
    public void Active(Vector3 s, Creature target)
    {
        Active(s, new Creature[1] { target });
    }
    public void Active(Vector3 s, Creature[] targets)
    {
        m_active_in_pool = true;
        gameObject.SetActive(true);
        
        m_start = s;
        m_end = GetTargetPos(targets);
        m_end_index = 0;
        
        m_time_counter = 0;
        m_totol_time_counter = 0;
        
        

        m_fly_time = 0;
        Vector3 prev = s;
        for (int i = 0; i < m_end.Length; ++i)
        {
            m_fly_time += Vector3.Distance(prev, m_end[i]) / Speed;
            prev = m_end[i];
        }
        m_curt_fly_time = Vector3.Distance(s, m_end[0]) / Speed;

        transform.localPosition = s;
        
        if (FlyMode == FlyModeType.Laser)
        {
            UpdateRotation(CurtEnd);
            if (PrefabFxFly != null)
            {
                m_fb = FxPool.Instance.Alloc(PrefabFxFly, null);
                m_fb.transform.localPosition = transform.localPosition;
                m_fb.transform.localRotation = transform.localRotation;
                m_fb.ScaleBody(Vector3.Distance(CurtStart, CurtEnd));
            }
            
        }
        else if (FlyMode == FlyModeType.Linear)
        {
            Vector3 curt = CalcPos(0.02f);
            UpdateRotation(curt);
            if (PrefabFxFly != null)
            {
                m_fb = FxPool.Instance.Alloc(PrefabFxFly, gameObject);
            }
            
        }

        OnReachTarget = null;
    }
    public void Inactive()
    {
        if (m_fb != null)
        {
            FxPool.Instance.Free(m_fb);
            m_fb = null;
        }
        m_active_in_pool = false;
        gameObject.SetActive(false);
    }

    public void Update()
    {
        m_time_counter += Time.deltaTime;
        m_totol_time_counter += Time.deltaTime;
                
        if (FlyMode == FlyModeType.Linear)
        {
            Vector3 curt = CalcPos(m_time_counter);
            UpdateRotation(curt);
            transform.localPosition = curt;
        }
        
        if (m_time_counter >= m_curt_fly_time)
        {
            if (PrefabFxEnd != null)
            {
                FxBase fb = FxPool.Instance.Alloc(PrefabFxEnd, null);
                fb.transform.localPosition = CurtEnd;
            }
            
        }

        if (m_totol_time_counter >= FlyTime)
        {
            m_active_in_pool = false;
            BulletViewPool.Instance.Free(this);
        }
        else if (m_time_counter >= m_curt_fly_time)
        {
            ++ m_end_index;
            m_time_counter = 0;
            m_curt_fly_time = Vector3.Distance(m_end[m_end_index], m_end[m_end_index - 1]) / Speed;

            if (FlyMode == FlyModeType.Laser)
            {
                transform.localPosition = CurtStart;
                UpdateRotation(CurtEnd);

                if (m_fb != null)
                {
                    FxPool.Instance.Free(m_fb);
                    m_fb = null;
                }
                if (PrefabFxFly != null)
                {
                    m_fb = FxPool.Instance.Alloc(PrefabFxFly, null);
                    m_fb.transform.localPosition = CurtStart;
                    m_fb.transform.localRotation = transform.localRotation;
                    m_fb.ScaleBody(Vector3.Distance(CurtStart, CurtEnd));
                }
            }

            if (OnReachTarget != null)
            {
                OnReachTarget(this, m_end_index);
            }
        }
    }



    void UpdateRotation(Vector3 to_pos)
    {
        Vector3 dir = (to_pos - transform.localPosition).normalized;
        float rot_z = Vector3.Angle(dir, Vector3.up);
        if (dir.x < 0)
        {
            rot_z = 360 - rot_z;
        }
        transform.localRotation = Quaternion.Euler(0, 0, -rot_z);
    }
    Vector3 CalcPos(float time)
    {
        if (FlyMode == FlyModeType.Linear)
        {
            float rate = Mathf.Clamp01(time / m_curt_fly_time);
            float side = CurveFlySide.Evaluate(rate);
            float forward = CurveFlyForward.Evaluate(rate);

            Vector3 curt = Vector3.up * forward * (CurtEnd - CurtStart).magnitude;
            curt.x += side;

            Vector3 dir = (CurtEnd - CurtStart).normalized;
            float rot_z = Vector3.Angle(Vector3.up, dir);
            if (dir.x < 0)
            {
                rot_z = 360 - rot_z;
            }
            Quaternion quat_dir = Quaternion.Euler(0, 0, -rot_z);

            curt = quat_dir * curt;
            curt = CurtStart + curt;

            return curt;
        }
        else
        {
            return CurtStart;
        }
    }


    Vector3[] GetTargetPos(Creature[] crts)
    {
        Vector3[] rets = new Vector3[crts.Length];
        switch (Align)
        {
            case AlignType.Center:
                for (int i = 0; i < crts.Length; ++ i)
                {
                    rets[i] = crts[i].Center;
                }
                break;
            case AlignType.Grid:
                for (int i = 0; i < crts.Length; ++i)
                {
                    rets[i] = crts[i].CenterAlign;
                }
                break;
            case AlignType.FirstCenterX:
                for (int i = 0; i < crts.Length; ++i)
                {
                    rets[i] = crts[i].Center;
                    rets[i].x = crts[0].Center.x;
                }
                break;
            case AlignType.FitstGridX:
                for (int i = 0; i < crts.Length; ++i)
                {
                    rets[i] = crts[i].CenterAlign;
                    rets[i].x = crts[0].CenterAlign.x;
                }
                break;
        }
        return rets;
    }
}






public class BulletViewPool
{
    public static BulletViewPool Instance;
    public static void CreasteInstance()
    {
        Instance = new BulletViewPool();
    }
    public static void DestroyInstance()
    {
        Instance = null;
    }

    GameObject m_bullet_root;

    Dictionary<string, List<BulletView>> m_pool = new Dictionary<string, List<BulletView>>();

    public void Init()
    {
        m_bullet_root = Util.NewGameObject("BulletRoot", Launcher.Instance.CanvasUI.gameObject);
    }
    public BulletView Alloc(string key, Vector3 start, Creature[] targets)
    {
        if (!m_pool.ContainsKey(key))
        {
            m_pool[key] = new List<BulletView>();
        }

        BulletView ret = null;

        foreach (BulletView m in m_pool[key])
        {
            if (!m.ActiveInPool)
            {
                ret = m;
                break;
            }
        }

        if (ret == null)
        {
            BulletView new_m = ResMgr.Instance.CreateGameObject(string.Format("Bullet/{0}/{1}", key, key), m_bullet_root).GetComponent<BulletView>();
            new_m.Create();
            ret = new_m;
            m_pool[key].Add(new_m);
        }

        if (targets != null)
        {
            ret.Active(start, targets);
        }
                
        return ret;
    }
    public void Free(BulletView bv)
    {
        bv.Inactive();
    }

    public void Cache(string k, int count)
    {
        List<BulletView> bvs = new List<BulletView>();
        for (int i = 0; i < count; ++i)
        {
            BulletView bv = Alloc(k, Vector3.zero, null);
            bvs.Add(bv);
        }
        for (int i = 0; i < count; ++i)
        {
            Free(bvs[i]);
        }
    }
}