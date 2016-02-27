using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightGridView : MonoBehaviour
{
    FightCtllerView m_ctller = null;
    public FightCtllerView FCtllerView
    {
        get { return m_ctller; }
    }

    FightGrid m_fight_grid;
    public FightGrid FGrid
    {
        get { return m_fight_grid; }
    }

    public FightGrid.DirType Dir
    {
        get { return m_fight_grid.Dir; }
    }
    
    public class Unit
    {
        public Int2D Index = new Int2D(0, 0);
        public FightGridUnitView UnitView = null;

        public Unit(Int2D idx, FightGridUnitView uv)
        {
            Index = idx;
            UnitView = uv;
        }
    }
    Unit[,] m_units = null;
    public Unit[,] Units
    {
        get
        {
            return m_units;
        }
    }

    Dictionary<Creature, CreatureView> m_crts = new Dictionary<Creature, CreatureView>();
    public Dictionary<Creature, CreatureView> Creatures
    {
        get { return m_crts; }
    }

    public void Create(FightCtllerView ctl, FightGrid grid)
	{
        m_ctller = ctl;
        m_fight_grid = grid;
        gameObject.layer = (int)UnityLayer.UI;

        Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
        Vector2 max = new Vector2(float.MinValue, float.MinValue);

        m_units = new Unit[FightGrid.UnitCount.Y, FightGrid.UnitCount.X];
        for (int y = 0; y < FightGrid.UnitCount.Y; ++y)
        {
            for (int x = 0; x < FightGrid.UnitCount.X; ++x)
            {
                GameObject obj_unit = ResMgr.Instance.CreateGameObject("Fight/GridUnit", gameObject);
                obj_unit.transform.localPosition = grid.Units[y, x].Position;
                FightGridUnitView uv = obj_unit.GetComponent<FightGridUnitView>();
                uv.Create(this, grid.Units[y, x]);
                m_units[y, x] = new Unit(new Int2D(x, y), uv);

                min.x = Mathf.Min(min.x, grid.Units[y, x].Position.x - FightGrid.UnitSize / 2);
                min.y = Mathf.Min(min.y, grid.Units[y, x].Position.y - FightGrid.UnitSize / 2);

                max.x = Mathf.Max(max.x, grid.Units[y, x].Position.x + FightGrid.UnitSize / 2);
                max.y = Mathf.Max(max.y, grid.Units[y, x].Position.y + FightGrid.UnitSize / 2);
            }
        }
        /*
        BoxCollider cld = gameObject.AddComponent<BoxCollider>();
        cld.center = min + (max - min) / 2;
        cld.size = (max - min);
        */
        SetVisible(false);

        m_fight_grid.OnAddCreature += OnAddCreature;
        m_fight_grid.OnRemoveCreature += OnRemoveCreature;
	}
    void OnDestroy()
    {
        m_fight_grid.OnAddCreature -= OnAddCreature;
        m_fight_grid.OnRemoveCreature -= OnRemoveCreature;
    }

    void Update()
    {
        
    }

    Unit GetUnitView(Vector2 pt)
    {
        foreach (Unit u in m_units)
        {
            if (u.UnitView.CheckPoint(pt))
                return u;
        }
        return null;
    }

    public void SetVisible(bool v)
    {
        foreach (Unit u in m_units)
        {
            u.UnitView.gameObject.SetActive(v);
        }
    }

    public void SetUnitColor(Int2D pt, Int2D dim, Color clr)
    {
        foreach (Unit u in m_units)
        {
            if (pt.X <= u.UnitView.Unit.Index.X && pt.Y <= u.UnitView.Unit.Index.Y &&
                pt.X + dim.X > u.UnitView.Unit.Index.X && pt.Y + dim.Y > u.UnitView.Unit.Index.Y)
            {
                u.UnitView.BodyColor = clr;
            }
            else
            {
                u.UnitView.BodyColor = Color.white;
            }
        }
    }

    void OnAddCreature(Creature crt, Int2D pt)
    {
        CreatureView crtv = ResMgr.Instance.CreateGameObject("UI/Crt", Launcher.Instance.CanvasUI.gameObject).GetComponent<CreatureView>();
        crtv.Create(crt, this);
        crtv.UpdateTransform(pt);
        m_crts[crt] = crtv;
    }
    void OnRemoveCreature(Creature crt)
    {
        GameObject.Destroy(m_crts[crt]);
        m_crts.Remove(crt);
    }

    

    public Int2D GetGridsUnderPoint(Creature crt, Vector2 pt)
    {
        Rect rc_check = new Rect(pt, new Vector2(crt.Proto.Dim.X * FightGrid.UnitSize, crt.Proto.Dim.Y * FightGrid.UnitSize));

        Int2D pt_ref = Int2D.zero;
        float min_dist = float.MaxValue;
        for (int x = 0; x <= (FightGrid.UnitCount.X - crt.Proto.Dim.X); ++x)
        {
            for (int y = 0; y <= (FightGrid.UnitCount.Y - crt.Proto.Dim.Y); ++y)
            {
                Vector2 orig = Vector2.zero;
                for (int dx = 0; dx < crt.Proto.Dim.X; ++dx)
                {
                    for (int dy = 0; dy < crt.Proto.Dim.Y; ++dy)
                    {
                        Vector2 curt = FGrid.Units[dy + y, dx + x].Position;
                        orig += curt;
                    }
                }
                orig /= (crt.Proto.Dim.X * crt.Proto.Dim.Y);

                if (Vector2.Distance(orig, pt) < min_dist)
                {
                    pt_ref = new Int2D(x, y);
                    min_dist = Vector2.Distance(orig, pt);
                }
            }
        }
        return pt_ref;
    }
    


    /*
    Unit m_unit_start = null;
    void OnMouseDown()
    {
        Vector3 pt = Launcher.Instance.MainCamera.ScreenToWorldPoint(InputWrap.TouchPosition);
        m_unit_start = GetUnitView(pt);

        foreach (Unit u in m_units)
        {
            u.UnitView.BodyColor = u == m_unit_start ? Color.white : Color.green;
        }
    }
    void OnMouseUp()
    {
        foreach (Unit u in m_units)
        {
            u.UnitView.BodyColor = new Color(1, 1, 1, 0);
        }
    }
    void OnMouseDrag()
    {
        //Vector3 pt = Launcher.Instance.MainCamera.ScreenToWorldPoint(InputWrap.TouchPosition);
        //Unit curt = GetUnitView(pt);
    }
    */
}
