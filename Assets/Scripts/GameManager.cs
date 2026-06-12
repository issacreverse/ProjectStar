using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    bool flag = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Update()
    {
        if(flag)
        {
            WaveManager.Instance.StartWaveManager();
            flag = false;
        }
    }
}
