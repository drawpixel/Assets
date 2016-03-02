using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletView : MonoBehaviour
{
    public AnimationCurve CurveFlySide = new AnimationCurve(new Keyframe(0, 0, 0, 1), new Keyframe(1, 1, 1, 0));
    public AnimationCurve CurveFlyForward = new AnimationCurve(new Keyframe(0, 0, 0, 1), new Keyframe(1, 1, 1, 0));
    
    public float Speed = 100;

    float m_fly_time = 0;
    public float FlyTime
    {
        get { return m_fly_time; }
    }

    float m_time_counter = 0;

    Vector3 m_start;
    Vector3 m_end;

    bool m_is_actived = false;

    public void Active(Vector3 s, Vector3 e)
    {
        m_start = s;
        m_end = e;
        m_fly_time = Vector3.Distance(s, e) / Speed;
        m_time_counter = 0;
        m_is_actived = true;

        transform.localPosition = s;

        Vector3 curt = CalcPos(0.02f);
        UpdateRotation(curt);
        
    }

    public void Update()
    {
        if (!m_is_actived)
            return;

        m_time_counter += Time.deltaTime;


        //Vector3 curt = Vector3.Lerp(m_start, m_end, Mathf.Clamp01(m_time_counter / m_fly_time)); //CalcPos(m_time_counter);
        Vector3 curt = CalcPos(m_time_counter);

        UpdateRotation(curt);

        transform.localPosition = curt;

        if (m_time_counter >= FlyTime)
        {
            m_is_actived = false;
            FxPool.Instance.Free(gameObject.GetComponent<FxBase>());
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
        float rate = Mathf.Clamp01(time / FlyTime);
        float side = CurveFlySide.Evaluate(rate);
        float forward = CurveFlyForward.Evaluate(rate);

        Vector3 curt = Vector3.up * forward * (m_end - m_start).magnitude;
        curt.x += side;

        Vector3 dir = (m_end - m_start).normalized;
        float rot_z = Vector3.Angle(Vector3.up, dir);
        if (dir.x < 0)
        {
            rot_z = 360 - rot_z;
        }
        Quaternion quat_dir = Quaternion.Euler(0, 0, -rot_z);

        curt =  quat_dir * curt;
        curt = m_start + curt;

        return curt;
    }
    
}
