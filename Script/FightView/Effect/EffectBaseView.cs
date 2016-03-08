using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectBaseView : MonoBehaviour
{
    public float PrepareTime = 1;
    public float PrepareMultiEffectIndex = 0;
    public float ReadyTime = 1;
    public float LastTime = 1;

    public enum StateType
    {
        Init,
        Prepare,
        Ready,
        Active,
        Over,
    }
    StateType m_state = StateType.Init;
    public StateType State
    {
        get { return m_state; }
    }
    float m_state_counter = 0;


    public enum ToTargetType
    {
        Once,
        Sequent,
        OneFirst,
        Penetrate,
    }
    
    public ToTargetType ToTarget = ToTargetType.Once;
    public float NextTargetTime = 0.5f;
    int m_target_idx = 0;
    float m_target_counter = 0;
    

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
        Effect.OnActiveOver += OnActiveOver;
	}
    void OnDestroy()
    {
        Effect.OnActive -= OnActive;
        Effect.OnActivePrepare -= OnActivePrepare;
        Effect.OnActiveOver -= OnActiveOver;
    }
    void Update()
    {
        m_state_counter += Time.deltaTime;

        switch (m_state)
        {
            case StateType.Prepare:
                {
                    if (m_state_counter >= PrepareMultiEffectIndex * Effect.IndexInOwner)
                    {
                        Ready();
                    }
                }
                break;
            case StateType.Ready:
                if (m_state_counter >= PrepareTime)
                {
                    switch (ToTarget)
                    {
                        case ToTargetType.Once:
                            Effect.Active();
                            m_target_idx = Effect.CurtTargets.Count;
                            break;
                        case ToTargetType.Sequent:
                            ActiveInternal(Effect.CurtTargets[m_target_idx++]);
                            break;
                        case ToTargetType.OneFirst:
                            ActiveInternal(Effect.CurtTargets[m_target_idx++]);
                            break;
                    }
                    
                }
                break;
            case StateType.Active:
                m_target_counter += Time.deltaTime;
                if (m_target_counter >= NextTargetTime && m_target_idx < Effect.CurtTargets.Count)
                {
                    m_target_counter = 0;
                    switch (ToTarget)
                    {
                        case ToTargetType.Sequent:
                            ActiveInternal(Effect.CurtTargets[m_target_idx++]);
                            break;
                        case ToTargetType.OneFirst:
                            for (int i = 1; i < Effect.CurtTargets.Count; ++i)
                            {
                                ActiveInternal(Effect.CurtTargets[m_target_idx++]);
                            }
                            break;
                    }
                }
                
                if (m_state_counter >= LastTime && m_target_idx >= Effect.CurtTargets.Count)
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

        m_target_idx = 0;
        m_target_counter = 0;
    }
    public virtual void Ready()
    {
        m_state_counter = 0;
        m_state = StateType.Ready;
    }
    public virtual void Active()
    {
        m_state_counter = 0;
        m_state = StateType.Active;
    }
    public virtual void Over()
    {
        m_state_counter = 0;
        m_state = StateType.Over;
    }


    protected virtual void ActiveInternal(Creature target)
    {
        Effect.Active(target);
    }
    

    public void OnActivePrepare(EffectBase effect)
    {
        Prepare();

        effect.Block();
    }
    public void OnActive(EffectBase effect, int times)
    {
        Active();

        effect.Block();
    }
    public void OnActiveOver(EffectBase effect)
    {
        Over();
    }
}
