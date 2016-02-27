using UnityEngine;
using UnityEditor;
using System.Collections;
using System;
using System.IO;

public class MenuEditorPattern
{
    [MenuItem("Kit/PatternEditor", false, 0)]
    static public void EditorPattern()
    {
        WinEditorPattern win = EditorWindow.GetWindow<WinEditorPattern>();
        win.wantsMouseMove = true;
    }
}

public class WinEditorPattern : EditorWindow {

    // Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    static int WidthUnit = 128;
    static int WidthDetail = 8;

    Int2D m_dim = new Int2D(0, 0);
    Texture2D m_tex;

    bool[,] m_units = null;
    bool[,] m_details = null;

    void OnGUI()
    {
        m_tex = (Texture2D)EditorGUI.ObjectField(new Rect(0, 0, 40, 40), m_tex, typeof(Texture2D), false);
        if (m_tex == null)
            return;
        if (m_tex.width % WidthUnit != 0 || m_tex.height % WidthUnit != 0)
        {
            ShowNotification(new GUIContent("Invalid texture size"));
            return;
        }

        if (m_dim.X != m_tex.width / WidthUnit || m_dim.Y != m_tex.height / WidthUnit)
        {
            m_dim.X = m_tex.width / WidthUnit;
            m_dim.Y = m_tex.height / WidthUnit;

            m_units = new bool[m_tex.height / WidthUnit, m_tex.width / WidthUnit];
            for (int y = 0; y < m_units.GetLength(0); ++y)
            {
                for (int x = 0; x < m_units.GetLength(1); ++x)
                {
                    m_units[y, x] = false;
                }
            }

            m_details = new bool[m_tex.height / WidthDetail, m_tex.width / WidthDetail];
            for (int y = 0; y < m_details.GetLength(0); ++y)
            {
                for (int x = 0; x < m_details.GetLength(1); ++x)
                {
                    m_details[y, x] = false;
                }
            }

            for (int y = 0; y < m_tex.height; ++y)
            {
                for (int x = 0; x < m_tex.width; ++x)
                {
                    Color clr = m_tex.GetPixel(x, m_tex.height - y - 1);
                    if (clr.a > 0.5f)
                    {
                        m_units[y / WidthUnit, x / WidthUnit] |= true;
                        m_details[y / WidthDetail, x / WidthDetail] |= true;
                    }
                }
            }
        }

        /*
        EditorGUI.DrawRect(new Rect(0, 40, m_dim.X * WidthUnit, m_dim.Y * WidthUnit), Color.black);
        for (int i = 0; i <= m_dim.X; ++i)
        {
            EditorGUI.DrawRect(new Rect(WidthUnit * i, 40, 4, m_dim.Y * WidthUnit + 4), Color.gray);
        }
        for (int i = 0; i <= m_dim.Y; ++i)
        {
            EditorGUI.DrawRect(new Rect(0, 40 + WidthUnit * i, m_dim.X * WidthUnit + 4, 4), Color.gray);            
        }
        */
        for (int y = 0; y < m_details.GetLength(0); ++y)
        {
            for (int x = 0; x < m_details.GetLength(1); ++x)
            {
                EditorGUI.DrawRect(new Rect(x * WidthDetail, y * WidthDetail + 40, WidthDetail, WidthDetail), m_details[y, x] ? Color.green : Color.grey);
            }
        }
        

        GUI.DrawTexture(new Rect(0, 40, m_tex.width, m_tex.height), m_tex);
        

        if (GUI.Button(new Rect(0, 40 + m_tex.height, 200, 20), "Save"))
        {
            string text = "";
            text += string.Format("{0},{1}\n", m_dim.X, m_dim.Y);
            for (int y = 0; y < m_details.GetLength(0); ++y)
            {
                for (int x = 0; x < m_details.GetLength(1); ++x)
                {
                    if (m_details[y, x])
                    {
                        text += (y * m_details.GetLength(1) + x).ToString();
                        text += ",";
                    }
                }
            }
            File.WriteAllText(Application.dataPath + "/Pattern/" + m_tex.name + ".txt", text);
            AssetDatabase.Refresh();
        }
    }
}
