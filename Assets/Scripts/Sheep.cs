using UnityEngine;

public class Sheep : MonoBehaviour
{

    public float PlayerDetectionDistance;
    public float PlayerMinDistanceTamed;
    public float PlayerMinDistanceUntamed;
    public float PlayerFollowScale;
    public float PlayerFleeScale;
    public float SheepDetectionDistance;
    public float SheepMinDistance;
    public float CenterScale;
    public float DistanceScale;
    public float DirectionScale;
    public bool Tamed;
    public float OutsideScale;
    public float OutsideBarnScale;
    public float InsideBarnScale;
    public float DoorScale;
    public float BarnDoorDistance;
    public float BarnDistance;
    public int TooSteepChecks;
    public float Speed;
    public float AutoSpeed;
    public float MaxSpeed;
    public float MinStepDistance;
    public float MinSize;
    public float MaxSize;
    public float RandomSpeedScale;
    public float MinUpdateDelay;
    public float UpdateDelayFalloff;
    public float MaxUpdateDistance;
    public float HungerTimeOutside;
    public float HungerTimeInside;
    public float HungerStartEating;
    public float HungerStopEating;
    public Transform Body;
    public GameObject Collar;
    public LayerMask GroundLayer;
    public LayerMask SheepLayer;
    public LayerMask SheepInsideLayer;
    public LayerMask BarnLayer;
    public Transform PlayerTransform;

    private Barn barn;

    private float randomSpeedSeed;
    private float randomDistanceSeed;

    private Vector2 movementDelta;
    private float nextMovementUpdate;

    private float tooSteepChecksSteps;

    private float hunger;
    private bool eating;
    public bool inside;

    private void Start()
    {
        SetBarn(barn);
        randomSpeedSeed = Random.Range(0f, 128f);
        randomDistanceSeed = Random.Range(0.8f, 1.2f);
        transform.localScale = Vector3.one * Random.Range(MinSize, MaxSize);
        tooSteepChecksSteps = Mathf.PI / TooSteepChecks;
        PlayerTransform = Player.Instance.transform;
        eating = false;
    }

    private void Update()
    {
        if (Pause.isPaused || To(PlayerTransform).magnitude > MaxUpdateDistance)
            return;
        ApplyHunger();
        transform.position = NextPosition();
    }

    private void ApplyHunger()
    {
        hunger = Mathf.Clamp(hunger + 1f / (inside && eating ? -HungerTimeInside : HungerTimeOutside) * Time.deltaTime, 0f, 1f);
        if (hunger >= HungerStartEating)
            eating = true;
        else if (hunger <= HungerStopEating)
            eating = false;
    }

    private Vector3 NextPosition()
    {
        if (Time.time > nextMovementUpdate)
            UpdateMovement();
        Vector2 position = AsVector2(transform.position);
        Vector2 nextPosition2 = position + movementDelta;
        Vector3 nextPositionGround = SpawnFinder.CastToGround(nextPosition2);
        if (TooSteep(transform.position, movementDelta))
        {
            movementDelta = AdjustedDelta(nextPositionGround);
            nextPosition2 = position + movementDelta;
            nextPositionGround = SpawnFinder.CastToGround(nextPosition2);
        }
        if (movementDelta.magnitude > MinStepDistance)
        {
            Vector3 lookAtPosition = AsVector3(nextPosition2);
            lookAtPosition.y = transform.position.y;
            transform.LookAt(lookAtPosition);
            Body.LookAt(SpawnFinder.CastToGround(transform.position + transform.forward));
        }
        return nextPositionGround;
    }

    private Vector2 AdjustedDelta(Vector3 position)
    {
        for (int i = 0; i < TooSteepChecks; i++)
        {
            float sin = Mathf.Sin(i * tooSteepChecksSteps);
            float cos = Mathf.Cos(i * tooSteepChecksSteps);
            Vector2 adjusted = new Vector2(cos * movementDelta.x - sin * movementDelta.y, sin * movementDelta.x + cos * movementDelta.y);
            if (!TooSteep(position, adjusted))
                return adjusted;
            sin = Mathf.Sin(-i * tooSteepChecksSteps);
            cos = Mathf.Cos(-i * tooSteepChecksSteps);
            adjusted = new Vector2(cos * movementDelta.x - sin * movementDelta.y, sin * movementDelta.x + cos * movementDelta.y);
            if (!TooSteep(position, adjusted))
                return adjusted;
        }
        return Vector2.zero;
    }

    private bool TooSteep(Vector3 position, Vector2 delta)
    {
        return Physics.Raycast(position + Vector3.up, AsVector3(delta), 1f, GroundLayer | BarnLayer);
    }

    private void UpdateMovement()
    {
        nextMovementUpdate = Time.time + UpdateDelay() + MinUpdateDelay;
        movementDelta = GetSheepVector();
        if (barn != null)
            movementDelta += GetBarnVector();
        movementDelta += GetPlayerVector();
        movementDelta *= Speed * Time.deltaTime;
    }

    private float UpdateDelay()
    {
        return 1f - UpdateDelayFalloff / To(PlayerTransform).magnitude;
    }

    private float RandomSpeedMultiplier()
    {
        return Mathf.PerlinNoise(Time.time / RandomSpeedScale, randomSpeedSeed) / 2f;
    }

    private Vector2 CapSpeed(Vector2 delta)
    {
        return delta.magnitude > MaxSpeed ? delta.normalized * MaxSpeed : delta;
    }

    private Vector2 GetSheepVector()
    {
        Vector2 center = Vector2.zero;
        Vector2 distance = Vector2.zero;
        Vector2 direction = Vector2.zero;
        Collider[] sheepColliders = Physics.OverlapSphere(transform.position, SheepDetectionDistance, inside ? SheepInsideLayer : SheepLayer);
        foreach (Collider collider in sheepColliders)
            if (collider.gameObject.Equals(gameObject))
            {
                if (!inside)
                    direction += AsVector2(transform.forward) * AutoSpeed;
            }
            else
            {
                distance += GetSheepDistanceVector(collider);
                if (!inside)
                {
                    center += GetCenterVector(collider);
                    direction += GetDirectionVector(collider);
                }
            }
        if (sheepColliders.Length > 1)
            center /= sheepColliders.Length - 1f;
        return RandomSpeedMultiplier() * (CenterScale * center - DistanceScale * randomDistanceSeed * distance + direction * DirectionScale);
    }

    private Vector2 GetSheepDistanceVector(Collider collider)
    {
        Vector2 vectorToSheep = To(collider);
        return -vectorToSheep.normalized * Mathf.Min(0f, -1f / (Mathf.Max(1.51f, vectorToSheep.magnitude) - 1.5f) + 1f / SheepMinDistance);
    }

    private Vector2 GetCenterVector(Collider collider)
    {
        Vector2 vectorToCenter = To(collider);
        return vectorToCenter.normalized * Mathf.Max(0f, -Mathf.Pow((vectorToCenter.magnitude - PlayerDetectionDistance * 2f / 3f) / (PlayerDetectionDistance / 3f), 2f) + 1f);
    }

    private Vector2 GetDirectionVector(Collider collider)
    {
        return collider.GetComponent<Sheep>().movementDelta * (1f - To(collider).magnitude / SheepDetectionDistance);
    }

    private Vector2 AsVector2(Vector3 vector3)
    {
        return new Vector2(vector3.x, vector3.z);
    }

    private Vector3 AsVector3(Vector2 vector2)
    {
        return new Vector3(vector2.x, 0f, vector2.y);
    }

    private Vector2 GetPlayerVector()
    {
        if (!InDetectionDistance(PlayerTransform, PlayerDetectionDistance))
            return Vector2.zero;
        Vector2 toPlayer = To(PlayerTransform);
        float multiplyer;
        if (Tamed)
            multiplyer = Mathf.Min(0f, -1f / (Mathf.Max(1.51f, toPlayer.magnitude) - 1.5f) + 1f / PlayerMinDistanceTamed) + Mathf.Max(0f, -Mathf.Pow((toPlayer.magnitude - PlayerDetectionDistance * 2f / 3f) / (PlayerDetectionDistance / 3f), 2f) + 1f) * PlayerFollowScale;
        else
            multiplyer = Mathf.Min(0f, -1f / (Mathf.Max(1.51f, toPlayer.magnitude) - 1.5f) + 1f / PlayerMinDistanceUntamed) * PlayerFleeScale;
        return toPlayer.normalized * multiplyer;
    }

    private bool InDetectionDistance(Transform transform, float detectionDistance)
    {
        return To(transform).magnitude <= detectionDistance;
    }

    private Vector2 GetBarnVector()
    {
        Vector2 toDoor = To(barn.DoorCenter);
        if (!inside && eating && toDoor.magnitude < BarnDoorDistance)
        {
            inside = true;
            gameObject.layer = (int)Mathf.Log(SheepInsideLayer.value, 2f);
        }
        else if (inside && !eating && To(barn.OutsideCenter).magnitude < BarnDoorDistance)
        {
            inside = false;
            gameObject.layer = (int)Mathf.Log(SheepLayer.value, 2f);
        }
        if (inside && eating)
            return To(barn).normalized * InsideBarnScale;
        if (inside && !eating)
            return To(barn.OutsideCenter).normalized * OutsideScale;
        if (!inside && eating)
            return toDoor.normalized * DoorScale;
        Vector2 toBarn = To(barn);
        return hunger * Mathf.Pow(toBarn.magnitude / BarnDistance, 2f) * OutsideBarnScale * toBarn.normalized;
    }

    private Vector2 To(Barn barn)
    {
        return To(barn.BarnCenter.transform);
    }

    private Vector2 To(Collider collider)
    {
        return To(collider.transform);
    }

    private Vector2 To(Transform transform)
    {
        return ToV2(transform.position);
    }

    private Vector2 ToV2(Vector3 position)
    {
        return AsVector2(ToV3(position));
    }

    private Vector3 ToV3(Vector3 position)
    {
        return position - transform.position;
    }

    public void SetBarn(Barn barn)
    {
        this.barn = barn;
        Tamed = barn != null;
        Collar.SetActive(Tamed);
        if (Tamed)
            Collar.GetComponent<MeshRenderer>().material.color = barn.Color;
    }

}
