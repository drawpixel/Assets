using UnityEngine;
using System.Collections;

public class InfoCreature : InfoBase
{
    ProtoCreature m_proto_ac;
    public ProtoCreature Proto
    {
        get { return m_proto_ac; }
        
    }

    public double Rank;
    public int Star;
    public int Quality;
    
    public InfoCreature(ProtoBase proto) : base(proto)
    {
        m_proto_ac = (proto as ProtoCreature);
    }
}

