﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillView : MonoBehaviour
{
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


    public void Create(Skill sk, CreatureView crt)
	{
        m_skill = sk;
        m_owner = crt;

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
	}
    
    public void Update()
    {
    }

}