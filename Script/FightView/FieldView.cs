using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class FieldView : MonoBehaviour
{
    GameObject m_obj = null;

    public void Create(string k)
	{
        m_obj = ResMgr.Instance.CreateGameObject(string.Format("Field/{0}", k), gameObject);
    }
    
    void Update()
    {
        
    }

}
