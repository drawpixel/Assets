using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class CreatureViewProg : MonoBehaviour
{
    
    CreatureView m_crt_view;
    public CreatureView CrtView
    {
        get { return m_crt_view; }
    }

    public Creature Crt
    {
        get { return m_crt_view.Crt; }
    }

    public InfoCreature Info
    {
        get { return m_crt_view.Info; }
    }

    public GameObject RootLevel;
    public GameObject RootMode;

    public Image ImgValueHP;
    public Image ImgValueEng;
    public Image ImgMode;
    public Text TxtLevel;

    public void Create(CreatureView cv)
	{
        m_crt_view = cv;
        Crt.OnDead += OnDead;
        Crt.OnTakeDamage += OnTakeDamage;
    }
    void OnDestroy()
    {
        Crt.OnDead -= OnDead;
        Crt.OnTakeDamage -= OnTakeDamage;
    }


    void Update()
    {
        
    }

    void Refresh()
    {
        ImgValueHP.fillAmount = Crt.RemainHP / Crt.MaxHP;
        gameObject.SetActive(Crt.State != Creature.StateType.Death);
    }

    void OnTakeDamage(Creature killer, float damage)
    {
        Refresh();
    }
    void OnDead(Creature killer)
    {
        Refresh();
    }

}
