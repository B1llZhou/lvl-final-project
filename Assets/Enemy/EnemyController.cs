using JetBrains.Annotations;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

[Serializable]
enum EnemyMovementAction
{
    DashToPlayer,
    DashFromPlayer,
    StayStill,
    StayAtRange
}

[Serializable]
struct EnemyBeat
{
    public Color beatColor;
    public bool attacking;
    public bool shielding;
    public bool shooting;
    public EnemyMovementAction movement;

    public EnemyBeat(Color beatColor, bool attacking, bool shielding, bool shooting, EnemyMovementAction movement)
    {
        this.beatColor = beatColor;
        this.attacking = attacking;
        this.shielding = shielding;
        this.shooting = shooting;
        this.movement = movement;
    }
}

public class EnemyController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField]
    GameObject attackZone;
    [SerializeField] 
    Collider attackCollider;
    [SerializeField]
    GameObject shield;
    SpriteRenderer spriteRenderer;
    CharacterController controller;
    [SerializeField]
    private GameObject projectilePrefab;
    [SerializeField] private GameObject indicator;
    private BeatPulse beatPulse;

    [Header("Basic")] 
    [SerializeField] private bool isActive = false;

    [Header("Rhythm Props")]
    [SerializeField]
    private float bpm = 76;
    private float beatLength;
    private float beatTimer = 0;
    private int timeSignature = 4;
    private int curBeatNum = 0;

    [Header("Dash Props")]
    [SerializeField]
    private float beatMoveSpeed = 15;
    [SerializeField]
    private float beatMoveDuration = .1f;

    [SerializeField]
    [Tooltip("When the enemy should try to get further")]
    private float closeRadius = 10;
    [SerializeField]
    [Tooltip("When the enemy should try to get closer")]
    private float farRadius = 20;

    [Header("Beat Props")]
    [SerializeField]
    private EnemyBeat beat1 = new EnemyBeat(Color.white, false, false, false, EnemyMovementAction.DashToPlayer);
    [SerializeField]
    private EnemyBeat beat2 = new EnemyBeat(new Color(1, 0, 0, .3f), false, true, true, EnemyMovementAction.StayStill);
    [SerializeField]
    private EnemyBeat beat3 = new EnemyBeat(new Color(1, 0, 0, .6f), false, false, false, EnemyMovementAction.DashToPlayer);
    [SerializeField]
    private EnemyBeat beat4 = new EnemyBeat(Color.red, true, true, true, EnemyMovementAction.StayStill);

    private EnemyBeat curBeat;

    private Vector3 attackPos = new Vector3(0, .9f, 0);
    private Vector3 restPos = Vector3.zero;

    [Header("General Movement Props")]
    [SerializeField]
    private bool followPlayer = false;
    [SerializeField]
    private float followDistance = 10;
    [SerializeField]
    private float enemySpeed = 2.0f;
    [SerializeField]
    private float gravityValue = -9.81f;
    private Vector3 enemyVelocity;
    private bool groundedPlayer;

    [SerializeField]
    private float knockbackDuration = .2f;
    private bool hasKnockback = false;
    
    [Header("Target player")]
    [SerializeField] public float rotationSpeed = 10f;


    void Start()
    {
        beatLength = 60 / bpm;
        attackZone.SetActive(true);

        beatPulse = GetComponent<BeatPulse>();
        
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer) spriteRenderer.color = new Color();
        
        controller = GetComponent<CharacterController>();
        curBeat = beat1;
    }

    void Update()
    {
        if (!isActive && Input.GetKeyDown(KeyCode.P)) isActive = true;
        if (isActive)
        {
            UpdateBeats();
            UpdateMovement();
            RotateToTarget();
        }
    }

    void UpdateBeats()
    {
        beatTimer += Time.deltaTime;
        if (beatTimer > beatLength)
        {
            //We'll need to be very precise with beat timing or we'll need a way to sync up with the music every measure
            beatTimer -= beatLength;

            IncrementBeat();

            switch (curBeatNum)
            {
                case 0:
                    ApplyEnemyBeat(beat1);
                    break;
                case 1:
                    ApplyEnemyBeat(beat2);
                    break;
                case 2:
                    ApplyEnemyBeat(beat3);
                    break;
                case 3:
                    ApplyEnemyBeat(beat4);
                    break;
            }
        }
    }

    void IncrementBeat()
    {
        curBeatNum++;
        if (curBeatNum >= timeSignature)
        {
            curBeatNum = 0;
        }
    }

    void ApplyEnemyBeat(EnemyBeat beat) 
    {
        beatPulse.StartBeatPulse();
        
        curBeat = beat;
        shield.SetActive(beat.shielding);
        GetComponent<Health>().canTakeDamage = !beat.shielding;
        // spriteRenderer.color = beat.beatColor;
        ChangeIndicatorColor(beat.beatColor);
        
        if (beat.attacking)
        {
            attackZone.transform.position = transform.position + attackPos;
            attackCollider.enabled = true;
        }
        else
        {
            attackZone.transform.position = transform.position + restPos;
            attackCollider.enabled = false;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Vector3 toPlayerVector = player.transform.position - transform.position;
        float toPlayerDistance = toPlayerVector.magnitude;
        Vector3 toPlayerDirection = toPlayerVector.normalized;

        if (beat.shooting)
        {
            Vector3 toPlayerXZ = new Vector3(toPlayerDirection.x, 0, toPlayerDirection.z);
            Instantiate(projectilePrefab, transform.position, Quaternion.LookRotation(toPlayerXZ, transform.up));
        }

        if (!hasKnockback)
        {
            if (beat.movement == EnemyMovementAction.DashToPlayer)
            {
                enemyVelocity += toPlayerDirection * beatMoveSpeed;
                StartCoroutine(BeatMoveTimer());
            }
            if (beat.movement == EnemyMovementAction.DashFromPlayer)
            {
                enemyVelocity += -toPlayerDirection * beatMoveSpeed;
                StartCoroutine(BeatMoveTimer());
            }
            if (beat.movement == EnemyMovementAction.StayAtRange)
            {
                if (toPlayerDistance < closeRadius)
                {
                    enemyVelocity += -toPlayerDirection * beatMoveSpeed;
                }
                else if (toPlayerDistance > farRadius)
                {
                    enemyVelocity += toPlayerDirection * beatMoveSpeed;
                }
                else
                {
                    int direction = Random.Range(0, 2);
                    Debug.Log(direction);
                    if (direction == 0)
                    {
                        enemyVelocity += -transform.right * beatMoveSpeed;
                    }
                    else 
                    {
                        enemyVelocity += transform.right * beatMoveSpeed;
                    }
                }

                StartCoroutine(BeatMoveTimer());
            }
        }
    }

    IEnumerator BeatMoveTimer()
    {
        yield return new WaitForSeconds(beatMoveDuration);
        enemyVelocity = Vector3.zero;
    }

    void UpdateMovement()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && enemyVelocity.y < 0)
        {
            enemyVelocity.y = 0f;
        }

        enemyVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(enemyVelocity * Time.deltaTime);

        if (followPlayer)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            Vector3 toPlayerDirection = player.transform.position - transform.position;
            float toPlayerDistance = toPlayerDirection.magnitude;
            toPlayerDirection = toPlayerDirection.normalized;
            if (toPlayerDistance < followDistance)
            {
                Vector3 move = new Vector3(toPlayerDirection.x, 0, toPlayerDirection.z);
                controller.Move(move * Time.deltaTime * enemySpeed);
        
                if (move != Vector3.zero)
                {
                    gameObject.transform.forward = move;
                }
            }
        }
    }

    public void ApplyKnockback(Vector3 direction, float power)
    {
        if (hasKnockback)
        {
            return;
        }
        hasKnockback = true;
        enemyVelocity += direction * power;
        StartCoroutine(KnockbackTimer());
    }

    IEnumerator KnockbackTimer()
    {
        yield return new WaitForSeconds(knockbackDuration);
        enemyVelocity = Vector3.zero;
        hasKnockback = false;
    }
    
    private void RotateToTarget() {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;
        
        Vector3 direction = (player.transform.position - transform.position).normalized;
        direction.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    private void ChangeIndicatorColor(Color color)
    {
        if (indicator == null) return;
        Renderer renderer = indicator.GetComponent<Renderer>();
        renderer.material.color = color;
    }
}
