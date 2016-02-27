using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class CreatureView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Creature m_crt;
    public Creature Crt
    {
        get { return m_crt; }
    }

    public InfoCreature Info
    {
        get { return m_crt.Info; }
    }

    FightGridView m_fgv = null;
    public FightGridView FGridView
    {
        get { return m_fgv; }
    }

    public Image ImgBG;
    public Text TxtName;

    CreatureViewProg m_prog;
    RectTransform m_rt;

    public void Create(Creature ac, FightGridView fgv)
	{
        m_crt = ac;
        m_crt.OnDead += OnDead;
        m_crt.OnTakeDamage += OnTakeDamage;

        m_fgv = fgv;

        m_rt = gameObject.GetComponent<RectTransform>();
        m_rt.sizeDelta = new Vector2(Info.Proto.Dim.X * FightGrid.UnitSize, Info.Proto.Dim.Y * FightGrid.UnitSize);

        //ImgBG.sprite = ResMgr.Instance.Load("Card/Common/BG_0") as Sprite;
        TxtName.text = Info.Proto.Key;
        
        GameObject prog_hp = ResMgr.Instance.CreateGameObject("UI/CrtProg", gameObject);
        m_prog = prog_hp.GetComponent<CreatureViewProg>();
        m_prog.Create(this);
        
        Idle();
    }
    void OnDestroy()
    {
        m_crt.OnDead -= OnDead;
        m_crt.OnTakeDamage -= OnTakeDamage;

        GameObject.DestroyObject(m_prog);
    }

    void Update()
    {
        
    }

    public void UpdateTransform()
    {
        UpdateTransform(Crt.Index);
    }
    public void UpdateTransform(Int2D pt)
    {
        Vector3 s = m_crt.FGrid.Units[pt.Y, pt.X].Position;
        s.x -= FightGrid.UnitSize * 0.5f;
        s.y += FightGrid.UnitSize * 0.5f;
        s.x += m_crt.Info.Proto.Dim.X * FightGrid.UnitSize * 0.5f;
        s.y -= m_crt.Info.Proto.Dim.Y * FightGrid.UnitSize * 0.5f;
        transform.localPosition = s;
    }


    void Idle()
    {
        
    }
    void OnTakeDamage(Creature killer, float damage)
    {
        //m_anim_ctl.CrossFade("BeHit", 0.05f, PlayMode.StopSameLayer);
    }
    void OnDead(Creature killer)
    {
        //m_anim_ctl.CrossFade("Die", 0.2f);
    }


    


    Vector2 m_drag_start;
    Vector2 m_drag_offset;
    public void OnBeginDrag(PointerEventData data)
    {
        m_drag_start = transform.localPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            transform.parent as RectTransform,
            data.position, data.pressEventCamera, out m_drag_offset);

        FGridView.SetVisible(true);
    }
    public void OnDrag(PointerEventData data)
    {
        //Debug.Log(data.pressPosition.ToString());
        

        Vector2 delta = data.position - data.pressPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            transform.parent as RectTransform,
            data.position, data.pressEventCamera, out delta);

        delta -= m_drag_offset;

        transform.localPosition = m_drag_start;
        transform.localPosition += new Vector3(delta.x, delta.y, 0);

        Vector2 in_world = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            transform.parent as RectTransform,
            data.position, data.pressEventCamera, out in_world);
        Int2D pt = FGridView.GetGridsUnderPoint(Crt, in_world);
        Debug.Log(pt.ToString());
        if (FGridView.FGrid.CanBeReplace(Crt, pt))
        {
            FGridView.SetUnitColor(pt, Crt.Proto.Dim, Color.green);
        }
        else
        {
            FGridView.SetUnitColor(pt, Crt.Proto.Dim, Color.red);
        }
    }
    public void OnEndDrag(PointerEventData data)
    {
        Vector2 in_world = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            transform.parent as RectTransform,
            data.position, data.pressEventCamera, out in_world);
        Int2D pt = FGridView.GetGridsUnderPoint(Crt, in_world);
        Debug.Log(pt.ToString());
        FGridView.FGrid.Replace(Crt, pt);

        foreach (CreatureView cv in FGridView.Creatures.Values)
        {
            cv.UpdateTransform();
        }    

        FGridView.SetVisible(false);
    }
}
