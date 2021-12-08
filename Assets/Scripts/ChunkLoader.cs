using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkLoader : MonoBehaviour
{

    public Transform Player;
    public GameObject Chunk;
    public SpawnFinder SpawnFinderController;
    public int ChunkLoadDistance = 2;

    private readonly Dictionary<Vector2, GameObject> loadedChunks = new Dictionary<Vector2, GameObject>();

    void Update()
    {
        StartCoroutine(FindChunksToLoad());
    }

    private IEnumerator FindChunksToLoad()
    {
        yield return null;
        Vector2 chunkWithPlayer = new Vector2(Mathf.Floor((Player.position.x) / 1000), Mathf.Floor((Player.position.z) / 1000));
        for (int i = (int)chunkWithPlayer.x - ChunkLoadDistance; i <= chunkWithPlayer.x + ChunkLoadDistance; i++)
            for (int j = (int)chunkWithPlayer.y - ChunkLoadDistance; j <= chunkWithPlayer.y + ChunkLoadDistance; j++)
                LoadChunk(new Vector2(i, j));
    }

    private void LoadChunk(Vector2 coords)
    {
        if (loadedChunks.ContainsKey(coords))
            return;
        GameObject chunk = Instantiate(Chunk);
        Chunk chunkController = chunk.GetComponent<Chunk>();

        chunkController.SpawnFinderController = SpawnFinderController;
        chunk.transform.position = new Vector3(coords.x * 1000, 0, coords.y * 1000);
        chunkController.ChunkPosition = coords;
        loadedChunks.Add(coords, chunk);
    }

}