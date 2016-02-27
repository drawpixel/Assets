using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class MsgBase
{

}

public class MsgCenter
{
    public delegate void DgtCreatureCreate(InfoCreature info, int inst_id);
    public static DgtCreatureCreate OnCreatureCreate = null;

    public delegate void DgtCreatureTakeDamage(int inst_id, float damage);
    public static DgtCreatureTakeDamage OnCreatureTakeDamage = null;

    public delegate void DgtCreatureDead(int inst_id);
    public static DgtCreatureDead OnCreatureDead = null;

    public delegate void DgtCreatureShoot(int inst_id, int idx_gun, Int2D target);
    public static DgtCreatureShoot OnCreatureShoot = null;

    
}

