using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RegistMgr<T, TM> where T : class, new() where TM : class
{
	internal static T m_inst;

	public static T Instance
	{
		get {return m_inst;}
	}
	public static void CreateInstance()
	{
		m_inst = new T();
	}
	public static void DestroyInstance()
	{
		m_inst = null;
	}


	protected HashSet<TM> m_members = new HashSet<TM>();
	public HashSet<TM> Memebers
	{
		get {return m_members;}
	}

	public bool Regist(TM m)
	{
		if (m_members.Contains (m)) 
		{
			return false;
		}
		m_members.Add (m);
		return true;
	}
	public bool UnRegist(TM m)
	{
		if (!m_members.Contains (m)) 
		{
			return false;
		}
		m_members.Remove (m);
		return true;
	}
}

