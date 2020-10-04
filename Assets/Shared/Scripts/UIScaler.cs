using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScaler : MonoBehaviour
{
    public CanvasScaler scaler;
    void Start()
    {
        scaler = gameObject.GetComponent<CanvasScaler>();
        if (Screen.dpi > (96 * 1.5)) {
            scaler.scaleFactor = 96 / Screen.dpi;
        } else {
            scaler.scaleFactor = 1;
        }
    }
}
