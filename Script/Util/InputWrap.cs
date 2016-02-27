using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class InputWrap
{
    public static bool HasTouch
    {
        get
        {
            return Input.touchCount > 0 || Input.GetMouseButton(0);
        }
    }
    public static Vector2 TouchPosition
    {
        get
        {
            if (Input.touchCount != 0)
            {
                return Input.touches[0].position;
            }
            else if (Input.GetMouseButton(0))
            {
                return new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            }
            return Vector2.zero;
        }
        
    }

}
