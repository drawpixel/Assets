using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class CtlBG : MonoBehaviour
{
    [Serializable]
    public class LayerInfo
    {
        public float Speed = 1;
        public float Interval = 0;
        public Sprite TexBG;
        public int Order = 0;
    }
    public LayerInfo[] LayerInfos;


    public class Layer
    {
        public LayerInfo Info;
        public SpriteRenderer[] SRdr;
        public float Height = 0;
    }
    Layer[] m_layers = null;

    float m_counter = 0;



	public void Start()
	{
        m_layers = new Layer[LayerInfos.Length];
        for (int l = 0; l < m_layers.Length; ++ l)
        {
            Layer layer = new Layer();
            layer.Info = LayerInfos[l];
            layer.Height = (layer.Info.TexBG.rect.height + layer.Info.Interval) * Launcher.SpriteScale;
            layer.SRdr = new SpriteRenderer[Mathf.FloorToInt((Launcher.LogicDim.Y * Launcher.SpriteScale) / layer.Height) + 1];
            for (int i = 0; i < layer.SRdr.Length; ++ i)
            {
                SpriteRenderer rdr = Util.NewGameObject(layer.Info.TexBG.name + i.ToString(), gameObject).AddComponent<SpriteRenderer>();
                rdr.sprite = layer.Info.TexBG;
                rdr.sortingOrder = layer.Info.Order;
                rdr.transform.localPosition = new Vector3(0, layer.Height * i, 0);
                layer.SRdr[i] = rdr;
            }
            
            m_layers[l] = layer;
        }
        //ImageBG[0].transform.localPosition = Vector3.zero;
        //ImageBG[1].transform.localPosition = new Vector3(0, ImageBG[0].rectTransform.rect.height, 0);
	}
    
    void Update()
    {
        m_counter -= Time.deltaTime * 100;

        for (int l = 0; l < m_layers.Length; ++l)
        {
            Layer layer = m_layers[l];
            
            for (int i = 0; i < layer.SRdr.Length; ++i)
            {
                layer.SRdr[i].transform.localPosition -= new Vector3(0, Time.deltaTime * layer.Info.Speed, 0);
                if (layer.SRdr[i].transform.localPosition.y <= -layer.Height)
                {
                    layer.SRdr[i].transform.localPosition += Vector3.up * layer.Height * 2;
                }
            }
                
            
        }
        
    }
}
