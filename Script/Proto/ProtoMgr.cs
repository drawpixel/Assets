using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;


public class ProtoMgr
{
    public static ProtoMgr Instance;
    public static void CreasteInstance()
    {
        Instance = new ProtoMgr();
    }
    public static void DestroyInstance()
    {
        Instance = null;
    }

    class Member
    {
        public Dictionary<int, System.Object> IDMap = new Dictionary<int, object>();
        public Dictionary<string, System.Object> KeyMap = new Dictionary<string, object>();
    }
    Dictionary<Type, Member> m_members = new Dictionary<Type, Member>();

    Dictionary<string, ProtoCreature> m_Creatures = new Dictionary<string, ProtoCreature>();

    public delegate string DgtLoad(string path);
    DgtLoad m_loader = null;
    
    public void Init(DgtLoad loader)
    {
        m_loader = loader;

        LoadProto<ProtoEffectDamage, ProtoEffect>("Proto/EffectDamage");

        LoadProto<ProtoSkill>("Proto/Skill");

        LoadProto<ProtoCreature>("Proto/Creature");
        
    }

    string[,] LoadCSV(string file)
    {
        string txt = m_loader(file);
        
        string[] lines = txt.Split(new string[] { "\r\n" }, System.StringSplitOptions.None);
        if (lines == null || lines.Length == 0)
            return null;
        string[] secs = lines[0].Split(new string[] { "," }, System.StringSplitOptions.None);
        if (secs == null || secs.Length == 0)
            return null;

        string[,] rets = new string[lines.Length, secs.Length];
        for (int l = 0; l < lines.Length; ++l)
        {
            secs = lines[l].Split(new string[] { "," }, System.StringSplitOptions.None);
            for (int s = 0; s < secs.Length; ++s)
            {
                rets[l, s] = secs[s];
            }
        }

        return rets;
    }
    void LoadProto<T>(string path)
        where T : ProtoBase, new()
    {
        LoadProto<T, T>(path);
    }
    void LoadProto<T, TB>(string path) 
        where T : ProtoBase, new() 
        where TB : ProtoBase
    {
        Member m = new Member();

        string[,] datas = LoadCSV(path);

        for (int r = 1; r < datas.GetLength(0); ++r)
        {
            if (string.IsNullOrEmpty(datas[r, 0]))
                continue;

            T new_m = new T();

            for (int c = 0; c < datas.GetLength(1); ++c)
            {
                if (TrySetValue(datas[0, c], new_m, datas[r, c]) == null)
                {
                    // the column can't is parsed.
                }

            }

            new_m.Create();

            m.IDMap[new_m.ID] = new_m;
            m.KeyMap[new_m.Key] = new_m;
        }

        m_members.Add(typeof(TB), m);
    }
    object TryGetValue(Type t, string v)
    {
        object final = null;

        try
        {
            Type vt = v.GetType();

            if (t == typeof(int))
            {
                final = int.Parse(v);
            }
            else if (t == typeof(float))
            {
                final = float.Parse(v);
            }
            else if (t == typeof(double))
            {
                final = double.Parse(v);
            }
            else if (t == typeof(string))
            {
                final = (string)v;
            }
            else if (t == typeof(Int2D))
            {
                string[] xy = v.Split(new char[] { '_' });
                final = new Int2D(int.Parse(xy[0]), int.Parse(xy[1]));
            }
            else if (t.IsEnum)
            {
                string s = (string)v;
                final = Enum.Parse(t, s);
            }
            else if (t.IsArray)
            {
                string s = (string)v;
                string[] elems = s.Split(';');

                if (t == typeof(int[]))
                {
                    int[] arr = new int[elems.Length];
                    for (int i = 0; i < elems.Length; ++i)
                    {
                        if (!int.TryParse(elems[i], out arr[i]))
                            return null;
                    }
                    final = arr;
                }
                else if (t == typeof(float[]))
                {
                    float[] arr = new float[elems.Length];
                    for (int i = 0; i < elems.Length; ++i)
                    {
                        if (!float.TryParse(elems[i], out arr[i]))
                            return null;
                    }
                    final = arr;
                }
                else if (t == typeof(double[]))
                {
                    double[] arr = new double[elems.Length];
                    for (int i = 0; i < elems.Length; ++i)
                    {
                        if (!double.TryParse(elems[i], out arr[i]))
                            return null;
                    }
                    final = arr;
                }
                else if (t == typeof(string[]))
                {
                    final = elems;
                }
            }
        }
        catch (Exception e)
        {
            throw new Exception(e.ToString() + "  " + t.ToString() + "  " + v);
        }

        return final;
    }
    object TrySetValue(string name, System.Object o, string v)
    {
        object final = null;

        try
        {
            var prop = o.GetType().GetProperty(name);
            var field = o.GetType().GetField(name);

            if (v != null && (prop != null || field != null))
            {
                Type t = prop != null ? prop.PropertyType : field.FieldType;

                final = TryGetValue(t, v);

                if (prop != null)
                {
                    prop.SetValue(o, final, null);
                }
                else if (field != null)
                {
                    field.SetValue(o, final);
                }
            }
        }
        catch (Exception e)
        {
            throw new Exception(e.ToString() + "  " + name.ToString() + "  " + v);
        }
        return final;
    }


    public T GetByID<T>(int id) where T : ProtoBase
    {
        return m_members[typeof(T)].IDMap[id] as T;
    }
    public T GetByKey<T>(string key) where T : ProtoBase
    {
        return m_members[typeof(T)].KeyMap[key] as T;
    }
}

