using UnityEngine;
using System.Collections;

public class InfoSkill : InfoBase
{
    ProtoSkill m_proto_skill;
    public ProtoSkill Proto
    {
        get { return m_proto_skill; }
        
    }

    public double Rank;
        
    public InfoSkill(ProtoBase proto) : base(proto)
    {
        m_proto_skill = (proto as ProtoSkill);
    }
}

