using System.Collections;
using UnityEngine;

public class Chunk : MonoBehaviour
{

    public TerrainData TerrainDataPrefab;
    public TerrainData TerrainData;
    public Material TerrainMaterial;
    public Vector2 ChunkPosition;
    public int HeightmapResolution = 513;
    public int Layer = 6;
    public int EnergyCrystals = 100;
    public int Herds = 10;
    public int HerdMinSize = 100;
    public int HerdMaxSize = 200;
    public int Barns = 3;
    public int BarnHerdMinSize = 40;
    public int BarnHerdMaxSize = 60;
    public float DetailDensity = 0.2f;
    public float DetailDistance = 250f;
    public SpawnFinder SpawnFinderController;
    public bool GenerateTerrain = false;
    public GameObject Barn;

    private void Start()
    {
        InitiateTerrainData();
        if (GenerateTerrain)
        {
            StartCoroutine(ModelTerrain());
            StartCoroutine(FadeTextures());
        }
    }

    private void InitiateTerrainData()
    {
        if (TerrainData != null)
            return;
        TerrainData = Instantiate(TerrainDataPrefab);
        TerrainData.heightmapResolution = HeightmapResolution;
        TerrainData.size = new Vector3(1000f, 600f, 1000f);
        Terrain terrain = Terrain.CreateTerrainGameObject(TerrainData).GetComponent<Terrain>();
        terrain.materialTemplate = TerrainMaterial;
        terrain.transform.parent = transform;
        terrain.transform.localPosition = Vector3.zero;
        terrain.gameObject.layer = gameObject.layer;
        terrain.detailObjectDensity = DetailDensity;
        terrain.detailObjectDistance = DetailDistance;
    }

    public IEnumerator ModelTerrain()
    {
        float[,] row = new float[1, HeightmapResolution];
        for (int i = 0; i < HeightmapResolution; i++)
        {
            for (int j = 0; j < HeightmapResolution; j++)
                row[0, j] = GetLocalHeight(i, j);
            TerrainData.SetHeights(0, i, row);
            yield return null;
        }
        SpawnBarns();
        SpawnHerds();
    }

    private void SpawnBarns()
    {
        for (int i = 0; i < Barns; i++)
            SpawnBarn();
    }

    private void SpawnBarn()
    {
        Vector3 position = SpawnFinder.RandomSpawnLocation(this, 20f, 5f);
        if (position.magnitude == 0f)
            return;
        Terraform(position, 20f);
        Instantiate(Barn, position, Quaternion.identity);
    }

    private void Terraform(Vector3 position, float radius)
    {
        Vector2 heightmapPosition = ToHeightmapSpace(position);
        float heightmapRadius = ToHeightmapSpace(radius);
        int heightmapX = Mathf.RoundToInt(heightmapPosition.x - OffsetY() - heightmapRadius);
        int heightmapY = Mathf.RoundToInt(heightmapPosition.y - OffsetX() - heightmapRadius);
        int heightmapSize = Mathf.RoundToInt(heightmapRadius * 2f);
        float[,] heights = TerrainData.GetHeights(heightmapX, heightmapY, heightmapSize, heightmapSize);
        for (int i = 0; i < heights.GetLength(0); i++)
        {
            for (int j = 0; j < heights.GetLength(1); j++)
            {
                heights[i, j] = ToHeightmapSpaceHeight(position.y);
            }
        }
        TerrainData.SetHeights(heightmapX, heightmapY, heights);
    }

    private Vector2 ToHeightmapSpace(Vector3 position)
    {
        return new Vector2(ToHeightmapSpace(position.x), ToHeightmapSpace(position.z));
    }

    private float ToHeightmapSpace(float value)
    {
        return value * TerrainData.heightmapResolution / TerrainData.size.x;
    }

    private float ToHeightmapSpaceHeight(float height)
    {
        return height * 1f / TerrainData.size.y;
    }

    private void SpawnHerds()
    {
        for (int i = 0; i < Herds; i++)
            SpawnHerd();
    }

    private void SpawnHerd()
    {
        Vector3 position = SpawnFinder.RandomSpawnLocation(this, 30f, 10f);
        if (position.magnitude == 0f)
            return;
        int i;
        for (i = 0; i < Random.Range(HerdMinSize, HerdMaxSize); i++)
            SheepFactory.NewSheep(position + new Vector3(Mathf.Sin(i), 0f, Mathf.Cos(i)) * i / 5f);
        UI.IncreaseSheepCount(i);
    }

    private IEnumerator FadeTextures()
    {
        TerrainData.alphamapResolution = HeightmapResolution;
        float[,,] row = new float[1, HeightmapResolution, 2];
        for (int i = 0; i < HeightmapResolution; i++)
        {
            for (int j = 0; j < HeightmapResolution; j++)
            {
                row[0, j, 1] = (GetLocalHeight(i, j) - 0.1f) * 3f;
                row[0, j, 0] = 1 - row[0, j, 1];
            }
            TerrainData.SetAlphamaps(0, i, row);
            yield return null;
        }
    }

    private float GetLocalHeight(float i, float j)
    {
        return GetGlobalHeight(i + OffsetX(), j + OffsetY());
    }

    private float GetGlobalHeight(float i, float j)
    {
        float x = i * 0.002f;
        float y = j * 0.002f;
        float octave1 = Mathf.Pow(Mathf.PerlinNoise(x * 1.3f, y * 1.3f) + 0.1f, 1.9f) * 1f;
        float octave2 = Mathf.Pow(Mathf.PerlinNoise(x * 10f, y * 10f) + 0.1f, 0.5f) * 10f;
        float octave3 = Mathf.Pow(Mathf.PerlinNoise(x * 65f, y * 65f) + 0.1f, 1.1f) * 1.5f;
        return Mathf.Pow(octave1 * (octave2 + octave3), 1.8f) * 0.0075f;
    }

    public float OffsetX()
    {
        return ChunkPosition.y * (HeightmapResolution - 1);
    }

    public float OffsetY()
    {
        return ChunkPosition.x * (HeightmapResolution - 1);
    }

}
