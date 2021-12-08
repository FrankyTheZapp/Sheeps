using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barn : MonoBehaviour
{

    public int MinSheep = 20;
    public int MaxSheep = 40;
    public Color Color;
    public Transform BarnCenter;
    public Transform DoorCenter;
    public Transform OutsideCenter;
    public Transform SheepSpawn;

    public SkinnedMeshRenderer SkinnedMeshRenderer;

    private void Start()
    {
        InitializeColor();
        SpawnSheeps();
    }

    private void InitializeColor()
    {
        float i = transform.position.x / 100f;
        float j = transform.position.y / 100f;
        Color color = new Color(Mathf.PerlinNoise(i / 10f, j / 10f), Mathf.PerlinNoise(i / 20f, j / 20f), Mathf.PerlinNoise(i / 30f, j / 30f));
        SkinnedMeshRenderer.material.SetColor("_BaseColor", color);
        SkinnedMeshRenderer.material.SetColor("_EmissiveColor", color);
        Color = color;
    }

    private void SpawnSheeps()
    {
        for (int i = 0; i < Random.Range(MinSheep, MaxSheep); i++)
        {
            float v = i / 10f;
            Vector3 offset = new Vector3(Mathf.Sin(v), 0f, Mathf.Cos(v));
            SheepFactory.NewSheep(SheepSpawn.position + offset * i, this);
            UI.IncreaseSheepCount(1);
        }
    }

}
