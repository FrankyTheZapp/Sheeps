using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class Day : MonoBehaviour
{

    public float DayLengthMinutes;
    public float DayNightRatio;
    public Volume Volume;
    public float FogBaseHeightScale;

    public Transform Sun;

    private Vector3 lookAtPosition;

    void Update()
    {
        if (Pause.isPaused)
            return;
        RotateSun();
    }

    private void RotateSun()
    {
        float rotationPerSecond = 1f / (DayLengthMinutes * 60f);
        float timeInDay = (Time.time * rotationPerSecond + Mathf.Sin(2f * Mathf.PI * Time.time * rotationPerSecond) / (DayNightRatio * Mathf.PI)) * 2f * Mathf.PI;
        float sunTimeInDay = UI.midday ? Mathf.PI : timeInDay;
        Vector3 lookAtDirektion = transform.forward * Mathf.Cos(sunTimeInDay) + transform.right * Mathf.Sin(sunTimeInDay);
        lookAtPosition = transform.position + lookAtDirektion;
        Sun.LookAt(lookAtPosition);
        Volume.profile.TryGet(out Fog fog);
        fog.baseHeight.value = (Mathf.Sin(timeInDay) + 1f) * FogBaseHeightScale;
        fog.maximumHeight.value = fog.baseHeight.value;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(lookAtPosition, 0.2f);
    }
}
