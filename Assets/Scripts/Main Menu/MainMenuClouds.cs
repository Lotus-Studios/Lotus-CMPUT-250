using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuClouds : MonoBehaviour
{
    [SerializeField] private RectTransform rectTrans;
    [SerializeField] private float centreOffset = 100f;
    [SerializeField] private float tweenSpeed = 12f;

    void Update()
    {
        float newOffset = Mathf.Sin(Time.time * tweenSpeed) * centreOffset;
        
        rectTrans.anchoredPosition = new Vector2(newOffset, 0);
    }
}
