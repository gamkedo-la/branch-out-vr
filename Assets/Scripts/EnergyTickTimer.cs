using System;
using UnityEngine;

public class EnergyTickTimer :MonoBehaviour
{
    public static event Action Tick;


    [SerializeField]
    private float realTimeSeconds = 1f;

    private float timer;
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= realTimeSeconds)
        {

            Tick?.Invoke();
            timer = 0f;
        }
    }
}
