﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class FPSCounter : MonoBehaviour
{
    [SerializeField] private Text _fpsText = default;
    [SerializeField] private float _hudRefreshRate = 1f;
 
    private float _timer;

    void Awake()
    {
        QualitySettings.vSyncCount = 1;
    }
 
    private void Update()
    {
        if (Time.unscaledTime > _timer)
        {
            int fps = (int)(1f / Time.unscaledDeltaTime);
            _fpsText.text = "FPS: " + fps;
            _timer = Time.unscaledTime + _hudRefreshRate;
        }
    }
}