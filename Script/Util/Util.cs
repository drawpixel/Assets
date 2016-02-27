using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class Util
{
	public static GameObject NewGameObject(string name, GameObject parent)
	{
		GameObject obj = new GameObject(name);
		//obj.transform.parent = parent == null ? null : parent.transform;
        if (parent != null)
        {
            obj.transform.SetParent(parent.transform);
        }
		obj.transform.localPosition = Vector3.zero;
		obj.transform.localScale = Vector3.one;
		return obj;
	}
	public static GameObject CreateGameObject(GameObject prefab, GameObject parent)
	{
        GameObject obj = (GameObject)GameObject.Instantiate(prefab);
        if (parent != null)
        {
            obj.transform.SetParent(parent.transform, false);
            /*
            RectTransform rt = obj.GetComponent<RectTransform>();
            if (rt == null)
            {
                obj.transform.SetParent(parent.transform);
            }
            */
        }
        //obj.transform.localPosition = prefab.transform.localPosition;
		//obj.transform.localScale = prefab.transform.localScale;
		return obj;
	}
    

	public static string GetGameObjectPath(GameObject obj)
	{
		Transform curt = obj.transform;
		
		string ret = curt.name + "/";
		while (curt.parent != null)
		{
			ret = curt.parent.name + "/" + ret;
			curt = curt.parent;
		}
		return ret;
	}
	
	public static GameObject FindGameObjectByName(GameObject parent, string name)
	{
		foreach (Transform t in parent.GetComponentsInChildren<Transform>())
		{
			if (name.Equals(t.name))
				return t.gameObject;
		}

		return null;
	}

	public static T FindComponentFromParent<T>(GameObject obj) where T : Component
	{
        if (obj == null)
            return null;

		while (true)
		{
			T t = obj.GetComponent<T>();
			if (t != null)
			{
				return t;
			}
            if (obj.transform.parent == null)
            {
                break;
            }
			obj = obj.transform.parent.gameObject;
		}
		
		return null;
	}

    public static GameObject FindGameObject(GameObject root, string name)
    {
        foreach (Transform t in root.GetComponentsInChildren<Transform>())
        {
            if (t.name.Equals(name))
            {
                return t.gameObject;
            }
        }
        return null;
    }
    public static T FindComponent<T>(GameObject root, string name) where T : Component
    {
        foreach (T t in root.GetComponentsInChildren<T>())
        {
            if (t.name.Equals(name))
            {
                return t;
            }
        }
        return null;
    }

    public static string GetFileMd5(string fn)
	{
		byte[] datas = File.ReadAllBytes(fn);
		return GetMd5(datas);
	}
	public static string GetMd5(string datas)
	{
		return GetMd5(Encoding.UTF8.GetBytes(datas));
	}
	public static string GetMd5(byte[] datas)
	{
		MD5 md5 = new MD5CryptoServiceProvider();
		byte[] ret = md5.ComputeHash(datas);
		StringBuilder sb = new StringBuilder();
		foreach (byte b in ret)
		{
			sb.Append(b.ToString("X2"));
		}
		return sb.ToString();

	}



}

