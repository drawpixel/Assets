using System.Collections;
using System.Collections.Generic;

public class ProtoCreature : ProtoBase
{
    public int HP;
    public int Armor;
    public int GrowthArmor;
    public int FireAP;
    public int EnergyAP;
    public int Crit;
    public int AntiCrit;

    public Int2D Dim;
    public int[] Occupies;

    public int[] Skills;

    ProtoSkill[] m_skills;
    public ProtoSkill[] ProtoSkills
    {
        get { return m_skills; }
    }

    public override void Create()
    {
        m_skills = new ProtoSkill[Skills.Length];
        for (int i = 0; i < Skills.Length; ++i)
        {
            m_skills[i] = ProtoMgr.Instance.GetByID<ProtoSkill>(Skills[i]);
        }
    }
}
