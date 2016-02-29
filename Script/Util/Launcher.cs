using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Launcher : MonoBehaviour 
{
	public static Launcher Instance = null;
	void Awake()
	{
		Instance = this;
	}

    public static Int2D LogicDim = new Int2D(640, 1136);
    public static float SpriteScale = 0.01f;

	public Camera MainCamera;
    public GameObject RootSprite;

    public Canvas CanvasUI;


	// Use this for initialization
	void Start () 
	{
        //MainCamera.orthographicSize = 5 * (Screen.width / Screen.height) / (LogicDim.X / LogicDim.Y);

        RootSprite = Util.NewGameObject("RootSprite", null);
        //RootSprite.transform.localScale = Vector3.one * (MainCamera.orthographicSize / LogicDim.Y);

        Application.targetFrameRate = 50;

        ResMgr.CreateInstance ();
		ResMgr.Instance.Init ();
        
		FxPool.CreasteInstance ();
		FxPool.Instance.Init ();

        ProtoMgr.CreasteInstance();
        ProtoMgr.Instance.Init((string path) =>
            {
                TextAsset ta = ResMgr.Instance.Load(path) as TextAsset;
                return ta.text;
            });

		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;

        SetupFightCtller();
        
	}
	
	// Update is called once per frame
	void Update () 
	{
        
        
		/*
		if (Network.peerType == NetworkPeerType.Disconnected) 
		{
			NetworkConnectionError err = Network.InitializeServer (12, 7777, false);
		}
		*/
	}

	

	int m_fps = 0;
	int m_fps_cache = 0;
	float m_fps_counter = 0;
	void OnGUI()
	{
		++ m_fps_cache;
		m_fps_counter += Time.deltaTime;
		if (m_fps_counter > 1) 
		{
			m_fps_counter -= 1;
			m_fps = m_fps_cache;
			m_fps_cache = 0;
		}
		GUI.Label (new Rect(20, 20, 100, 40), "FPS: " + m_fps.ToString());
	}


    FightCtller m_fc;
    FightCtllerView m_fcv;
    void SetupFightCtller()
    {
        //ResMgr.Instance.CreateGameObject("BG/BG01", gameObject);

        m_fc = new FightCtller();
        m_fc.Create();

        GameObject ctl = Util.NewGameObject("FCtller", CanvasUI.gameObject);
        m_fcv = ctl.AddComponent<FightCtllerView>();
        m_fcv.Create(m_fc);

        string[] keys = null;
        Int2D[] pts = null;

        keys = new string[] { "Hadis", "Cretos", "Aflotiter", "Bosadon", "Bosadon", "Bosadon", "Bosadon", "Bosadon", "Bosadon" };
        pts = new Int2D[] { new Int2D(0, 0), new Int2D(2, 0), new Int2D(0, 1), new Int2D(2, 1), new Int2D(3, 1), new Int2D(4, 1), new Int2D(2, 2), new Int2D(3, 2), new Int2D(4, 2)};
        for (int p = 0; p < keys.Length; ++ p)
        {
            string k = keys[p];
            InfoCreature info = new InfoCreature(ProtoMgr.Instance.GetByKey<ProtoCreature>(k));
            info.Skills = new InfoSkill[1];
            info.Skills[0] = new InfoSkill(ProtoMgr.Instance.GetByKey<ProtoSkill>("Sk01"));

            Creature ac = new Creature();
            ac.Create(info);
            m_fc.FGrids[0].AddCreature(ac, pts[p]);
        }

        keys = new string[] { "Hadis", "Jos", "Bosadon", "Bosadon", "Bosadon", "Bosadon" };
        pts = new Int2D[] { new Int2D(0, 0), new Int2D(2, 0), new Int2D(0, 1), new Int2D(1, 1), new Int2D(0, 2), new Int2D(1, 2) };
        for (int p = 0; p < keys.Length; ++p)
        {
            string k = keys[p];
            InfoCreature info = new InfoCreature(ProtoMgr.Instance.GetByKey<ProtoCreature>(k));
            info.Skills = new InfoSkill[1];
            info.Skills[0] = new InfoSkill(ProtoMgr.Instance.GetByKey<ProtoSkill>("Sk01"));

            Creature ac = new Creature();
            ac.Create(info);
            m_fc.FGrids[1].AddCreature(ac, pts[p]);
        }

        m_fc.Idle();
    }
}
