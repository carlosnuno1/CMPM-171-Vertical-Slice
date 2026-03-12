using UnityEngine;

public class EnemyAIBehavior : MonoBehaviour
{
    public enum EnemyState { Patrol, Chase, Shoot, Reposition }

    [Header("Targeting")]
    public Transform player;
    public Transform[] waypoints;
    public LayerMask obstacleLayer;

    [Header("Flying Settings")]
    public float flyHeight = 4f;
    public float flySpeed = 5f;
    public float preferredDistanceInFront = 6f;
    public float bodyRotationSpeed = 5f;
    public float floatAmplitude = 0.3f;
    public float floatFrequency = 1.2f;

    [Header("Obstacle Avoidance")]
    public float avoidanceDistance = 4f;
    public float avoidanceForce = 7f;

    [Header("Shooting")]
    public GameObject pewpewPrefab;
    public Transform firePoint;
    public float timeBetweenShots = 1f;
    public int shotCount = 3;
    public float bulletForce = 40f;

    [Header("Reposition (Swoop)")]
    public float swoopRadius = 8f;
    public float swoopSpeedMultiplier = 1.8f;

    private EnemyState currentState = EnemyState.Patrol;
    private int shotsFired = 0;
    private float shotTimer = 0f;
    private int waypointIndex = 0;
    private Vector3 targetFlyPos;

    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        targetFlyPos = transform.position;
        changeState(EnemyState.Patrol);
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        switch (currentState)
        {
            case EnemyState.Patrol:
                UpdatePatrolState(distanceToPlayer);
                break;
            case EnemyState.Chase:
                UpdateChaseState(distanceToPlayer);
                break;
            case EnemyState.Shoot:
                UpdateShootingState(distanceToPlayer);
                break;
            case EnemyState.Reposition:
                UpdateRepositionState();
                break;
        }

        ApplyFlyingMovement();
        KeepUprightAndFacePlayer();
        AimFirePointAtPlayer();
    }

    void ApplyFlyingMovement()
    {
        Vector3 directionToTarget = (targetFlyPos - transform.position).normalized;
        Vector3 adjustedDirection = CalculateAvoidance(directionToTarget);
        Vector3 moveStep = transform.position + adjustedDirection;

        moveStep.y += Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;

        float currentSpeed = (currentState == EnemyState.Reposition) ? flySpeed * swoopSpeedMultiplier : flySpeed;
        transform.position = Vector3.MoveTowards(transform.position, moveStep, currentSpeed * Time.deltaTime);
    }

    Vector3 CalculateAvoidance(Vector3 currentDir)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, currentDir, out hit, avoidanceDistance, obstacleLayer))
        {
            Vector3 avoidDir = Vector3.Reflect(currentDir, hit.normal);
            avoidDir.y = 0;

            return (currentDir + avoidDir).normalized * avoidanceForce;
        }
        return currentDir;
    }

    void UpdatePatrolState(float dist)
    {
        if (dist <= 15f)
        {
            changeState(EnemyState.Chase);
            return;
        }

        if (waypoints != null && waypoints.Length > 0)
        {
            targetFlyPos = waypoints[waypointIndex].position;
            if (Vector3.Distance(transform.position, targetFlyPos) < 1f)
                waypointIndex = (waypointIndex + 1) % waypoints.Length;
        }
    }

    void UpdateChaseState(float dist)
    {
        if (dist <= 10f)
        {
            changeState(EnemyState.Shoot);
            return;
        }
        targetFlyPos = player.position + (player.forward * preferredDistanceInFront) + (Vector3.up * flyHeight);
    }

    void UpdateShootingState(float dist)
    {
        targetFlyPos = player.position + (player.forward * preferredDistanceInFront) + (Vector3.up * flyHeight);

        shotTimer -= Time.deltaTime;
        if (shotTimer <= 0f)
        {
            Shoot();
            shotTimer = timeBetweenShots;
            shotsFired++;
            if (shotsFired >= shotCount) changeState(EnemyState.Reposition);
        }
    }

    void UpdateRepositionState()
    {
        if (Vector3.Distance(transform.position, targetFlyPos) < 1.5f)
        {
            shotsFired = 0;
            changeState(EnemyState.Shoot);
        }
    }

    void changeState(EnemyState newState)
    {
        currentState = newState;
        if (newState == EnemyState.Reposition)
        {
            Vector2 randomCircle = Random.insideUnitCircle.normalized * swoopRadius;
            targetFlyPos = player.position + new Vector3(randomCircle.x, flyHeight, randomCircle.y);
        }
    }

    void KeepUprightAndFacePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            Quaternion lookRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * bodyRotationSpeed);
        }
    }

    void AimFirePointAtPlayer()
    {
        if (firePoint != null)
        {
            Vector3 direction = (player.position - firePoint.position).normalized;
            firePoint.rotation = Quaternion.LookRotation(direction);
        }
    }

    void Shoot()
    {
        if (pewpewPrefab && firePoint)
        {
            GameObject bullet = Instantiate(pewpewPrefab, firePoint.position, firePoint.rotation);
            Vector3 dir = (player.position - firePoint.position).normalized;
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb) rb.AddForce(dir * bulletForce, ForceMode.Impulse);
        }
    }
}