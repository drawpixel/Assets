using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightGrid
{
    public static int UnitSize = 128;
    public static int FightSize = 100;
    public static Int2D UnitCount = new Int2D(5, 3);


    public delegate void DgtAddCreature(Creature crt, Int2D pt);
    public DgtAddCreature OnAddCreature;
    public delegate void DgtRemoveCreature(Creature crt);
    public DgtRemoveCreature OnRemoveCreature;


    FightCtller m_ctller = null;
    public FightCtller FCtller
    {
        get { return m_ctller; }
    }

    public enum DirType
    {
        Up = 0,
        Down = 1,
    }
    DirType m_dir = DirType.Down;
    public DirType Dir
    {
        get { return m_dir; }
    }

    public class Unit
    {
        public Int2D Index = new Int2D(0, 0);
        public Vector3 Position = Vector3.zero;
        public Creature Creature = null;

        public Unit(Int2D idx, Creature crt)
        {
            Index = idx;
            Creature = crt;
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

    public FightGrid Face
    {
        get
        {
            if (Dir == DirType.Up)
                return m_ctller.FGrids[(int)DirType.Down];
            else if (Dir == DirType.Down)
                return m_ctller.FGrids[(int)DirType.Up];

            return null;
        }
    }


    public class Member
    {
        public Creature Crt;

        public Member(Creature crt)
        {
            Crt = crt;
        }
    }
    Dictionary<Creature, Member> m_crts = new Dictionary<Creature, Member>();
    public Dictionary<Creature, Member> Creatures
    {
        get { return m_crts; }
    }

    

    public void Create(FightCtller ctl, DirType dir)
	{
        m_dir = dir;
        m_ctller = ctl;

        m_units = new Unit[UnitCount.Y, UnitCount.X];
        for (int y = 0; y < Units.GetLength(0); ++y)
        {
            for (int x = 0; x < Units.GetLength(1); ++x)
            {
                m_units[y, x] = new Unit(new Int2D(x, y), null);
                m_units[y, x].Position = CalcUnitPosition(m_units[y, x].Index, dir);
            }
        }
	}
    
    public void Update(float interval)
    {
        foreach (Creature crt in m_crts.Keys)
        {
            crt.Update(interval);
        }
    }

    public void Idle()
    {
        foreach (Creature crt in m_crts.Keys)
        {
            crt.Idle();
        }
    }

    public bool CheckCreature(Creature crt, Int2D pt)
    {
        for (int y = 0; y < crt.Info.Proto.Dim.Y; ++y)
        {
            for (int x = 0; x < crt.Info.Proto.Dim.X; ++x)
            {
                Int2D curt = pt + new Int2D(x, y);
                if (curt.X < 0 || curt.Y < 0 || curt.X >= UnitCount.X || curt.Y >= UnitCount.Y)
                    return false;
                if (m_units[curt.Y, curt.X].Creature != null)
                    return false;
            }
        }
        return true;
    }
    public void AddCreature(Creature crt, Int2D pt)
    {
        if (!CheckCreature(crt, pt))
            return;

        crt.FGrid = this;
        m_crts.Add(crt, new Member(crt));

        EnterCreature(crt, pt);

        if (OnAddCreature != null)
        {
            OnAddCreature(crt, pt);
        }
    }
    public void RemoveCreature(Creature crt)
    {
        m_crts.Remove(crt);
        LeaveCreature(crt);
        if (OnRemoveCreature != null)
        {
            OnRemoveCreature(crt);
        }
    }
    void EnterCreature(Creature crt, Int2D pt)
    {
        crt.Index = pt;
        for (int x = 0; x < crt.Proto.Dim.X; ++x)
        {
            for (int y = 0; y < crt.Proto.Dim.Y; ++y)
            {
                Int2D dest = new Int2D(pt.X + x, pt.Y + y);
                Units[dest.Y, dest.X].Creature = crt;
            }
        }
    }
    void LeaveCreature(Creature crt)
    {
        for (int x = 0; x < crt.Proto.Dim.X; ++x)
        {
            for (int y = 0; y < crt.Proto.Dim.Y; ++y)
            {
                Int2D dest = new Int2D(crt.Index.X + x, crt.Index.Y + y);
                Units[dest.Y, dest.X].Creature = null;
            }
        }
        crt.Index = new Int2D(-1, -1);
    }

    Unit[,] SnapUnits()
    {
        Unit[,] ret = new Unit[UnitCount.Y, UnitCount.X];
        for (int y = 0; y < Units.GetLength(0); ++y)
        {
            for (int x = 0; x < Units.GetLength(1); ++x)
            {
                ret[y, x] = new Unit(new Int2D(x, y), Units[y, x].Creature);
            }
        }
        return ret;
    }
    public bool CanBeReplace(Creature crt, Int2D dest)
    {
        Int2D orig;
        return CanBeReplace(crt, dest, out orig);
    }
    public bool CanBeReplace(Creature crt, Int2D dest, out Int2D dest_offset)
    {
        dest_offset = new Int2D(-1, -1);

        // find all creature and calc bounder in area of dest
        Int2D dest_min = new Int2D(+int.MaxValue, +int.MaxValue);
        Int2D dest_max = new Int2D(-int.MaxValue, -int.MaxValue);
        HashSet<Creature> dest_crts = new HashSet<Creature>();
        HashSet<Int2D> will_occupied = new HashSet<Int2D>();
        for (int x = 0; x < crt.Proto.Dim.X; ++x)
        {
            for (int y = 0; y < crt.Proto.Dim.Y; ++y)
            {
                Int2D dest_curt = new Int2D(dest.X + x, dest.Y + y);
                will_occupied.Add(dest_curt);

                FightGrid.Unit dest_u = Units[dest_curt.Y, dest_curt.X];
                if (dest_u.Creature == null || dest_u.Creature == crt)
                    continue;

                dest_min.X = Mathf.Min(dest_min.X, dest_u.Creature.Index.X);
                dest_min.Y = Mathf.Min(dest_min.Y, dest_u.Creature.Index.Y);
                dest_max.X = Mathf.Max(dest_max.X, dest_u.Creature.Index.X + dest_u.Creature.Proto.Dim.X);
                dest_max.Y = Mathf.Max(dest_max.Y, dest_u.Creature.Index.Y + dest_u.Creature.Proto.Dim.Y);
                if (!dest_crts.Contains(dest_u.Creature))
                    dest_crts.Add(dest_u.Creature);
            }
        }
        
        List<Int2D> for_checks = new List<Int2D>();
        for_checks.Add(new Int2D(0, 0));
        for (int x = 0; x < crt.Proto.Dim.X; ++x)
        {
            for (int y = 0; y < crt.Proto.Dim.Y; ++y)
            {
                for_checks.Add(new Int2D(x, y));
            }
        }
        for (int x = dest_min.X; x < dest_max.X; ++x)
        {
            for (int y = dest_min.Y; y < dest_max.Y; ++y)
            {
                for_checks.Add(new Int2D(x - crt.Index.X, y - crt.Index.Y));
            }
        }


        Unit[,] temp_unit = SnapUnits();
        foreach (Unit u in temp_unit)
        {
            if (dest_crts.Contains(u.Creature) || u.Creature == crt)
                u.Creature = null;
        }
        
        for (int x = 0; x < crt.Proto.Dim.X; ++x)
        {
            for (int y = 0; y < crt.Proto.Dim.Y; ++y)
            {
                Int2D curt = dest + new Int2D(x, y);
                if (curt.X >= FightGrid.UnitCount.X || curt.Y >= FightGrid.UnitCount.Y)
                    return false;
                temp_unit[curt.Y, curt.X].Creature = crt;
            }
        }

        for (int i = 0; i < for_checks.Count; ++i)
        {
            // check possible in curt orig
            bool can_fit = true;
            Int2D curt_orig = new Int2D(crt.Index.X + for_checks[i].X, crt.Index.Y + for_checks[i].Y);
            foreach (Creature dest_crt in dest_crts)
            {
                for (int dx = 0; dx < dest_crt.Proto.Dim.X; ++dx)
                {
                    for (int dy = 0; dy < dest_crt.Proto.Dim.Y; ++dy)
                    {
                        Int2D check_pt = Int2D.zero;
                        if (i == 0)
                        {
                            Int2D offset = new Int2D(dest_crt.Index.X - dest.X, dest_crt.Index.Y - dest.Y);
                            check_pt = new Int2D(curt_orig.X + dx + offset.X, curt_orig.Y + dy + offset.Y);
                        }
                        else
                        {
                            Int2D offset = new Int2D(dest_crt.Index.X - dest_min.X, dest_crt.Index.Y - dest_min.Y);
                            check_pt = new Int2D(curt_orig.X + dx + offset.X, curt_orig.Y + dy + offset.Y);
                        }
                        if (check_pt.X < 0 || check_pt.Y < 0 || check_pt.X >= FightGrid.UnitCount.X || check_pt.Y >= FightGrid.UnitCount.Y)
                        {
                            can_fit = false;
                            break;
                        }
                        FightGrid.Unit check = temp_unit[check_pt.Y, check_pt.X];
                        //if (check.Creature == null || (check.Creature == crt && !will_occupied.Contains(check_pt)) ||
                        //    dest_crts.Contains(check.Creature))
                        if (check.Creature == null)
                        {
                            continue;
                        }
                        else
                        {
                            can_fit = false;
                            break;
                        }
                    }
                    if (!can_fit)
                        break;
                }
                if (!can_fit)
                    break;
            }
            if (can_fit)
            {
                dest_offset = (i == 0) ? dest - crt.Index : dest - ((crt.Index + for_checks[i]) + (dest - dest_min));
                return true;
            }
        }
        return false;
        
    }
    public bool Replace(Creature crt, Int2D dest_pt)
    {
        Int2D dest_offset;
        if (!CanBeReplace(crt, dest_pt, out dest_offset))
            return false;

        Debug.Log(dest_offset.ToString());

        Int2D src_pt = crt.Index;

        Dictionary<Creature, Int2D> dics = new Dictionary<Creature, Int2D>();
        for (int x = 0; x < crt.Proto.Dim.X; ++x)
        {
            for (int y = 0; y < crt.Proto.Dim.Y; ++y)
            {
                Int2D dest = new Int2D(dest_pt.X + x, dest_pt.Y + y);
                FightGrid.Unit u = Units[dest.Y, dest.X];
                if (u.Creature == null || u.Creature == crt)
                    continue;
                if (dics.ContainsKey(u.Creature))
                    continue;
                dics[u.Creature] = u.Creature.Index - dest_offset;
            }
        }

        LeaveCreature(crt);
        foreach (Creature c in dics.Keys)
        {
            LeaveCreature(c);
        }

        EnterCreature(crt, dest_pt);
        foreach (KeyValuePair<Creature, Int2D> pair in dics)
        {
            EnterCreature(pair.Key, pair.Value);
        }

        return true;
    }
    

    
    public Unit GetUnitByPos(Vector2 pt)
    {
        int x = Mathf.RoundToInt(pt.x / FightGrid.UnitSize) + UnitCount.X / 2;
        int y = Mathf.RoundToInt(pt.y / FightGrid.UnitSize);
        if (Dir == DirType.Down)
        {
            y += FightSize / UnitSize;
        }
        else
        {
            y -= FightSize / UnitSize;
        }
        if (x < 0 || x >= UnitCount.X || y < 0 || y >= UnitCount.Y)
        {
            return null;
        }
        return m_units[y, x];
    }

    public static Vector3 CalcUnitPosition(Int2D idx, DirType dir)
    {
        Vector3 pos = Vector3.zero;
        pos.x = -UnitCount.X * UnitSize / 2;
        pos.x += idx.X * UnitSize;
        pos.y = -FightSize;// -(FightCtller.Dim.Y / 2) + (UnitCount.Y * UnitSize) + 50;
        pos.y -= idx.Y * UnitSize;
        pos.x += UnitSize / 2;
        pos.y -= UnitSize / 2;
        if (dir == DirType.Up)
        {
            pos.y = -pos.y;
        }
        return pos;
    }

    
}
