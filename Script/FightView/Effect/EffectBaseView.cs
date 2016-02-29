using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectBaseView : MonoBehaviour
{
    public float PrepareTime = 1;
    public float LastTime = 1;

    public enum StateType
    {
        Init,
        Prepare,
        Active,
    }
    StateType m_state = StateType.Init;
    public StateType State
    {
        get { return m_state; }
    }
    float m_state_counter = 0;


    EffectBase m_effect;
    public EffectBase Effect
    {
        get { return m_effect; }
    }

    public ProtoEffect Proto
    {
        get { return m_effect.Proto; }
    }

    SkillView m_owner;
    public SkillView Owner
    {
        get
        {
            return m_owner;
        }
    }

    

    public virtual void Create(EffectBase effect, SkillView sv)
	{
        m_effect = effect;
        m_owner = sv;

        Effect.OnActive += OnActive;
        Effect.OnActivePrepare += OnActivePrepare;
	}
    void OnDestroy()
    {
        Effect.OnActive -= OnActive;
        Effect.OnActivePrepare -= OnActivePrepare;
    }
    void Update()
    {
        m_state_counter += Time.deltaTime;

        switch (m_state)
        {
            case StateType.Prepare:
                if (m_state_counter >= PrepareTime)
                {
                    Effect.Active();
                }
                break;
            case StateType.Active:
                if (m_state_counter >= LastTime)
                {
                    Effect.Over();
                }
                break;
        }
    }
    public virtual void Prepare()
    {
        m_state_counter = 0;
        m_state = StateType.Prepare;
    }
    public virtual void Active()
    {
        m_state_counter = 0;
        m_state = StateType.Active;
    }




    public void OnActivePrepare(EffectBase effect)
    {
        Prepare();

        if (PrepareTime > 0)
        {
            effect.Block();
        }
    }
    public void OnActive(EffectBase effect)
    {
        Active();

        if (LastTime > 0)
        {
            effect.Block();
        }
    }
}
