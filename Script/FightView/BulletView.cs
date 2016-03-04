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

    public enum FlyModeType
    {
        Jump,
        CenterAlign,
    }
    public FlyModeType FlyMode = FlyModeType.Jump;

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

    float m_time_counter = 0;
    float m_totol_time_counter = 0;

    Vector3 m_start;
    Vector3[] m_end;
    int m_end_index = 0;

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
    

    bool m_is_actived = false;

    public void Active(Vector3 s, Creature target)
    {
        Active(s, new Creature[1] { target });
    }
    public void Active(Vector3 s, Creature[] targets)
    {
        m_start = s;
        m_end = GetTargetPos(targets);
        m_end_index = 0;
        
        m_time_counter = 0;
        m_totol_time_counter = 0;
        m_is_actived = true;

        m_fly_time = 0;
        Vector3 prev = s;
        for (int i = 0; i < m_end.Length; ++i)
        {
            m_fly_time += Vector3.Distance(prev, m_end[i]) / Speed;
            prev = m_end[i];
        }
        m_curt_fly_time = Vector3.Distance(s, m_end[0]) / Speed;

        transform.localPosition = s;

        Vector3 curt = CalcPos(0.02f);
        UpdateRotation(curt);

        OnReachTarget = null;
    }

    public void Update()
    {
        if (!m_is_actived)
            return;

        m_time_counter += Time.deltaTime;
        m_totol_time_counter += Time.deltaTime;

        Vector3 curt = CalcPos(m_time_counter);

        UpdateRotation(curt);

        transform.localPosition = curt;

        if (m_totol_time_counter >= FlyTime)
        {
            m_is_actived = false;
            FxPool.Instance.Free(gameObject.GetComponent<FxBase>());
        }
        else if (m_time_counter >= m_curt_fly_time)
        {
            ++ m_end_index;
            m_time_counter = 0;
            m_curt_fly_time = Vector3.Distance(m_end[m_end_index], m_end[m_end_index - 1]) / Speed;

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

        curt =  quat_dir * curt;
        curt = CurtStart + curt;

        return curt;
    }


    Vector3[] GetTargetPos(Creature[] crts)
    {
        Vector3[] rets = new Vector3[crts.Length];
        switch (FlyMode)
        {
            case FlyModeType.Jump:
                for (int i = 0; i < crts.Length; ++ i)
                {
                    rets[i] = crts[i].Center;
                }
                break;
            case FlyModeType.CenterAlign:
                for (int i = 0; i < crts.Length; ++i)
                {
                    rets[i] = crts[i].CenterAlign;
                }
                break;
        }
        return rets;
    }
}
