using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(CharacterController))]
public class EnemyMovement : MonoBehaviour
{
    private CharacterController character;
    public Vector3 target { get; private set; } = Vector3.zero;
    public Vector3 preTarget { get; private set; } = Vector3.zero;

    [Header("Controls")]
    [Min(0)] public float targetDistance = 0.0f;
    [SerializeField] [Min(0)] private float distanceTolerance = 0.1f;
    public bool doStrafe = true;
    public bool doZigzag = true;
    [SerializeField] private bool doEaseIn = true;
    [SerializeField] private bool doAdditionalCorner = false;
    public bool isStopped = false;
    
    [Header("Movement Controls")]
    [Min(0)] public float speed = 2.0f;
    [HideInInspector] public float speedScalar = 1.0f;
    [Min(0)] public float drag = 1.0f;
    [Min(0)] public float stoppingDrag = 2.0f;

    [Header("Stats")]
    [Tooltip("Smooths target distance for going into and out of strafe")]
    [SerializeField] [Min(0.001f)] private float distanceEase = 1.0f;
    [SerializeField] [Min(0)] private float strafeMagnitude = 1.0f;
    [SerializeField] [Min(0)] private float strafeFrequency = 1.0f;
    [SerializeField] [Min(0)] private float zigzagMagnitude = 1.0f;
    [SerializeField] [Min(0)] private float zigzagFrequency = 1.0f;

    [Header("Move Direction")]
    [Tooltip("This is designed to be used in line with a rotation functionality controlling the forward direction of movement. To prevent a slow rotation causing the enemy to walk sideways.")]
    [SerializeField] private Transform forwardDirection = null;

    private float timeOffset = 0.0f;
    public Vector3 velocity { get{ return _velocity * speed * speedScalar; } }
    private Vector3 _velocity = new Vector3();
    [HideInInspector] public float distance { get; private set; } = 0.0f;
    [HideInInspector] public NavMeshPath path { get; private set; } = null;

    private bool inQueue = false;

    // Events
    [HideInInspector] public UnityEvent onReachedTarget;
    private bool lastOnTargetDistance = false;

    private void Awake() 
    {
        character = GetComponent<CharacterController>();
        timeOffset = Random.value * 100;

        path = new NavMeshPath();
    }

    private void OnEnable() 
    {
        EnemyMovementQueue queue = EnemyMovementQueue.Instance;
        if (queue != null)
        {
            queue.AddToQueue(this);
            inQueue = true;
        }
    }

    private void OnDisable() 
    {
        EnemyMovementQueue queue = EnemyMovementQueue.Instance;
        if (queue != null)
        {
            queue.RemoveFromQueue(this);
        }
        inQueue = false;

        _velocity = Vector3.zero;
    }

    private void FixedUpdate()
    {
        if (character.enabled == true)
        {
            if (isStopped == false)
            {
                // Backup incase EnemyMovementQueue doesn't exist
                if (inQueue == false)
                    CalculatePath();

                // Set Values
                Vector3 moveVector = GetNextPathPoint() - transform.position;
                distance = Mathf.Clamp((moveVector.magnitude - targetDistance) * distanceEase, -1, 1);

                // Override move direction
                if (forwardDirection != null)
                {
                    moveVector = new Vector3(forwardDirection.forward.x, 0.0f, forwardDirection.forward.z).normalized;
                }

                // Ease In
                if (doEaseIn && path.corners.Length <= 2 && preTarget == Vector3.zero)
                {
                    if (Mathf.Abs(distance) < distanceTolerance)
                    {
                        distance = 0;
                    }
                    moveVector *= distance;
                }
                else
                {
                    moveVector.Normalize();
                }

                // Strafe
                bool nearTargetDistance = (path.corners.Length <= 2 && distance < 1 && preTarget == Vector3.zero);
                Vector3 strafeVector = new Vector3();
                if (nearTargetDistance)
                {
                    if (doStrafe)
                        strafeVector = CalculateStrafeVector(strafeMagnitude, strafeFrequency, moveVector);
                }
                else
                {
                    if (doZigzag)
                        strafeVector = CalculateStrafeVector(zigzagMagnitude, zigzagFrequency, moveVector);
                }
                moveVector += strafeVector;

                // Event
                bool onTargetDistance = nearTargetDistance && distance <= 0;
                if (lastOnTargetDistance == false && onTargetDistance == true)
                    onReachedTarget.Invoke();
                lastOnTargetDistance = onTargetDistance;

                // Actual Movement
                moveVector.y = 0;
                moveVector = Vector3.ClampMagnitude(moveVector, 1);

                _velocity += moveVector * Time.fixedDeltaTime;
            }

            // Move
            if (_velocity != Vector3.zero)
            {
                _velocity = _velocity * (1 - (isStopped ? stoppingDrag : drag) * Time.fixedDeltaTime);
                if (_velocity.sqrMagnitude <= 0.0001f)
                    _velocity = Vector3.zero; // Just round at a certain point
                character.SimpleMove(_velocity * speed * speedScalar);
            }
        }
    }

    public void SetTarget(Vector3 pTarget, bool pSafe = false, bool pUpdatePath = false)
    {
        // PreTarget
        if (doAdditionalCorner && (target - pTarget).sqrMagnitude > 4.0f) // Only recalcuate if target moved over 2 units
            RecalculatePreTarget(pTarget);

        // Switch
        target = pTarget;
        if (pSafe) // If there is a chance of it being placed where they already are (don't use safe if you are updating it to a transform on update)
            lastOnTargetDistance = false;

        if (pUpdatePath) // Incase a path needs to be calculated on the target switch
            CalculatePath();
    }

    private void RecalculatePreTarget(Vector3 pTarget)
    {
        Vector3 dir = pTarget - transform.position;
        float sqrDis = (dir.sqrMagnitude * 0.0015f) - 0.1f;
        if (Random.value > sqrDis)
        {
            preTarget = Vector3.zero;
        }
        else
        {
            // Vectors
            Vector3 mid = Vector3.Lerp(transform.position, pTarget, Random.Range(0.25f, 0.75f));
            Vector3 left = Vector3.Cross(dir, Vector3.up).normalized;

            // Randomize
            left *= Random.Range(2.5f, 8.0f);
            if (Random.value > 0.5f)
                left = -left;

            // SamplePosition
            if (NavMesh.SamplePosition(mid + left, out NavMeshHit hit, 3.0f, NavMesh.AllAreas))
            {
                preTarget = hit.position;
            }
            else
            {
                preTarget = Vector3.zero;
            }
        }
    }

    public void CalculatePath()
    {
        if (isStopped == false)
        {
            Vector3 t = target;
            if (preTarget != Vector3.zero)
            {
                Vector3 mDir = preTarget - transform.position;
                if ((new Vector3(mDir.x, mDir.y * 3, mDir.z)).sqrMagnitude < 3.0f) // 2.0f is 1.5units
                {
                    RecalculatePreTarget(target);
                }
                else
                {
                    t = preTarget;
                }
            }
            
            bool foundPath = NavMesh.CalculatePath(transform.position, t, NavMesh.AllAreas, path);

            // If preTarget was placed in an unreachable spot, clear preTarget and recalculate path
            if (foundPath == false && preTarget != Vector3.zero)
            {
                preTarget = Vector3.zero;
                NavMesh.CalculatePath(transform.position, target, NavMesh.AllAreas, path);
            }
        }
    }

    public Vector3 GetNextPathPoint(int pIndex = 1)
    {
        if (path.corners.Length > pIndex)
        {
            Vector3 moveDir = path.corners[pIndex] - transform.position;
            if ((new Vector3(moveDir.x, moveDir.y * 3, moveDir.z)).sqrMagnitude < 0.5f) // If already at next corner start going to the one after that)
            {
                return GetNextPathPoint(pIndex + 1);
            }
            else
            {
                return path.corners[pIndex];
            }
        }
        else
        {
            if (preTarget != Vector3.zero)
            {
                return preTarget;
            }
            else
            {
                return target;
            }
        }
    }

    private Vector3 CalculateStrafeVector(float pMag, float pFreq, Vector3 pMoveDir)
    {
        float strafeValue = pMag;
        if (pFreq != 0)
        {
            strafeValue = Mathf.Sin((Time.time + timeOffset) * pFreq) * pMag;
        }
        Vector3 rightVector = RotateVector2By(new Vector2(pMoveDir.x, pMoveDir.z), 90);
        return new Vector3(rightVector.x, 0, rightVector.y) * strafeValue;
    }

    private Vector2 RotateVector2By(Vector2 pVec, float pDeg)
    {
        pDeg *= 0.0174533f; // Deg to Rad
        float ca = Mathf.Cos(pDeg);
        float sa = Mathf.Sin(pDeg);
        float rx = pVec.x * ca - pVec.y * sa;
        return new Vector2(rx, pVec.x * sa + pVec.y * ca);
    }

    public void AddForce(Vector3 pForce)
    {
        _velocity += pForce;
    }

    public void SetVelocity(Vector3 pVelocity)
    {
        _velocity = pVelocity;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos() 
    {
        Gizmos.color = Color.cyan;
        UnityEditor.Handles.color = Color.cyan;

        UnityEditor.Handles.DrawWireDisc(target, Vector3.up, targetDistance - distanceTolerance);
        UnityEditor.Handles.DrawWireDisc(target, Vector3.up, targetDistance + distanceTolerance);

        Gizmos.DrawCube(target, Vector3.one * 0.1f);
        if (path != null)
        {
            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                if (i != 0)
                    Gizmos.DrawCube(path.corners[i], Vector3.one * 0.1f);

                Gizmos.DrawLine(path.corners[i], path.corners[i + 1]);
            }
        }

        Gizmos.DrawLine(transform.position, transform.position + (_velocity * 5));

        if (preTarget != Vector3.zero)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(preTarget, Vector3.one * 0.1f);
        }

        if (forwardDirection != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + new Vector3(forwardDirection.forward.x, 0.0f, forwardDirection.forward.z).normalized);
        }
    }
#endif
}