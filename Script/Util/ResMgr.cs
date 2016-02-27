using UnityEngine;
using System.Collections;

public class ResMgr
{
	public static ResMgr Instance = null;
	public static void CreateInstance()
	{
		Instance = new ResMgr();
	}
	public static void DestroyInstance()
	{
		Instance = null;
	}


	public void Init()
	{
	}

    public Object Load(string path)
    {
        return Resources.Load(path);
    }
	public GameObject CreateGameObject(string path, GameObject parent)
	{
		GameObject prefab = Resources.Load(path) as GameObject;
        if (prefab == null)
        {
            throw new System.Exception("Resources.Load is NULL. " + path);
        }
		return Util.CreateGameObject (prefab, parent);
	}
    
}
