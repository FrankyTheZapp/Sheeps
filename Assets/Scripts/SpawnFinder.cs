using UnityEngine;

public class SpawnFinder : MonoBehaviour
{


    public Player PlayerController;
    public Transform PlayerSpawn;
    public UI UIController;
    public LayerMask Ground;
    public int MaxAttempts = 1000;

    static public SpawnFinder instance;

    private CharacterController characterController;

    private void Start()
    {
        characterController = PlayerController.GetComponent<CharacterController>();
        instance = this;
    }

    public void SpawnPlayer()
    {
        Vector3 position = CastDown(PlayerSpawn.position);
        position.y += 1;
        characterController.transform.position = position;
        PlayerController.enabled = true;
        characterController.enabled = true;
    }

    public static Vector3 RandomSpawnLocation(Chunk chunk, float radius, float maxYDelta)
    {
        for (int i = 0; i < instance.MaxAttempts; i++)
        {
            Vector3 position = CastToGround(Random2(chunk));
            if (Spawnable(position, radius, maxYDelta))
                return position;
        }
        Debug.Log("Spawn not found");
        return Vector3.zero;
    }

    private static Vector2 Random2(Chunk chunk)
    {
        return new Vector2(chunk.ChunkPosition.x * chunk.TerrainData.size.x + RandomFloat(chunk), chunk.ChunkPosition.y * chunk.TerrainData.size.x + RandomFloat(chunk));
    }

    private static float RandomFloat(Chunk chunk)
    {
        return Random.Range(0, chunk.TerrainData.size.x);
    }

    public static bool Spawnable(Vector3 position, float radius, float maxYDelta)
    {
        float t = 0f;
        Vector3 offset = Vector3.zero;
        while (offset.magnitude < radius)
        {
            offset.x = t * Mathf.Cos(t);
            offset.z = t * Mathf.Sin(t);
            offset = offset.normalized * t / Mathf.PI / 2f;
            t += Mathf.PI / ((offset.magnitude + 1f) * 3f);
            Vector3 positionDown = CastDown(position + offset * 2f);
            if (positionDown.magnitude == 0f || Mathf.Abs(position.y - positionDown.y) > maxYDelta)
                return false;
        }
        return true;
    }

    public static Vector3 CastToGround(Vector2 position)
    {
        return CastToGround(new Vector3(position.x, 0f, position.y));
    }

    public static Vector3 CastToGround(Vector3 position)
    {
        return CastDown(position, instance.Ground);
    }

    public static Vector3 CastDown(Vector2 position)
    {
        return CastDown(new Vector3(position.x, 0, position.y));
    }

    public static Vector3 CastDown(Vector3 position)
    {
        return CastDown(position, -1);
    }

    public static Vector3 CastDown(Vector3 position, LayerMask layer)
    {
        position.y = 2000f;
        RaycastHit raycastHit;
        if (layer >= 0)
            Physics.Raycast(new Ray(position, Vector3.down), out raycastHit, Mathf.Infinity, layer);
        else
            Physics.Raycast(new Ray(position, Vector3.down), out raycastHit);
        return raycastHit.point;
    }

}
