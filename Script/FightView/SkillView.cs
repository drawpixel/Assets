using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillView : MonoBehaviour
{
    public string FxCaster;
    public float PrepareTime = 0;

    public enum StateType
    {
        Init,
        Prepare,
        Casting,
        Over,
    }
    StateType m_state = StateType.Init;
    public StateType State
    {
        get { return m_state; }
    }
    float m_state_counter = 0;

    Skill m_skill;
    public Skill Skill
    {
        get { return m_skill; }
    }
    public ProtoSkill Proto
    {
        get { return m_skill.Proto; }
    }

    CreatureView m_owner;
    public CreatureView Owner
    {
        get { return m_owner; }
    }


    bool m_is_casting = false;
    public bool IsCasting
    {
        get { return m_is_casting; }
    }

    EffectBaseView[] m_effects = null;
    public EffectBaseView[] Effects
    {
        get { return m_effects; }
    }

    public void Cache()
    {
        if (!string.IsNullOrEmpty(FxCaster))
        {
            FxPool.Instance.Cache(string.Format("Cast/{0}/{1}", FxCaster, FxCaster), 1);
        }
    }

    public void Create(Skill sk, CreatureView crt)
	{
        m_skill = sk;
        m_owner = crt;

        m_skill.OnCastPrepare += OnPrepareCast;
        m_skill.OnCast += OnCast;
        m_skill.OnCastOver += OnOver;

        m_effects = new EffectBaseView[sk.Effects.Length];
        for (int i = 0; i < m_effects.Length; ++i)
        {
            EffectBase eb = sk.Effects[i];
            
            string k = "";
            if (string.IsNullOrEmpty(eb.Proto.EffectViewOverride))
            {
                k = eb.Proto.Key;
            }
            else
            {
                k = eb.Proto.EffectViewOverride;
            }
            GameObject obj_e = ResMgr.Instance.CreateGameObject("Effect/" + k, gameObject);
            m_effects[i] = obj_e.GetComponent<EffectBaseView>();
            m_effects[i].Create(eb, this);
        }

        Cache();
	}
    void OnDestroy()
    {
        if (m_skill != null)
        {
            m_skill.OnCastPrepare -= OnPrepareCast;
            m_skill.OnCast -= OnCast;
            m_skill.OnCastOver -= OnOver;
        }
    }

    public void Update()
    {
        m_state_counter += Time.deltaTime;
        switch (State)
        {
            case StateType.Prepare:
                if (m_state_counter >= PrepareTime)
                {
                    Skill.IsBlock = false;
                    Skill.Cast();
                }
                break;
        }
    }
    

    public void Prepare()
    {
        if (m_state == StateType.Prepare)
        {
            return;
        }

        m_state = StateType.Prepare;
        m_state_counter = 0;

        if (!string.IsNullOrEmpty(FxCaster))
        {
            FxPool.Instance.Alloc(string.Format("Cast/{0}/{1}", FxCaster, FxCaster), Owner.gameObject);
        }

    }
    public void Cast()
    {
        m_state = StateType.Casting;
        m_state_counter = 0;
    }
    public void Over()
    {
        m_state = StateType.Over;
        m_state_counter = 0;
    }


    public void OnPrepareCast(Skill sk)
    {
        if (PrepareTime > 0 && State != StateType.Prepare)
        {
            sk.IsBlock = true;
            Prepare();
        }
    }
    public void OnCast(Skill sk)
    {
        Cast();
    }
    public void OnOver(Skill sk)
    {
        Over();
    }
}
