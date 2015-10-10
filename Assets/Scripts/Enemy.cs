using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : LivingEntity
{
    public enum State { Idle, Chasing, Attacking}
    private State currentState;
    private NavMeshAgent pathfinder; // caminho
    private Transform target; // player

    [SerializeField] private float refreshRate = 1.0f; // tempo para seguir
    [SerializeField] private float attackDistanceThereshold = 1.5f; // distancia para ataque
    [SerializeField] private float timeBetweenAttacks = 1f; //tempo entre um ataque e outro
    [SerializeField] private float timeNextAttack; // tempo para o proximo ataque
    [SerializeField] private float damage = 1f; // valor do dano para player

    private float myCollisionRadius;
    private float targetCollisionRadius;
    private Material skinMaterial;
    private Color originalColor;

    private LivingEntity targetEntity; //alvo vivo

    private bool hasTarget;

    private void Awake()
    {
    }

    protected override void Start()
    {
        base.Start();
        pathfinder = GetComponent<NavMeshAgent>();
        skinMaterial = GetComponent<Renderer>().material;
        originalColor = skinMaterial.color; // pega a cor original do object

        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            currentState = State.Chasing;
            hasTarget = true;
            target = GameObject.FindGameObjectWithTag("Player").transform;

            targetEntity = target.GetComponent<LivingEntity>();
            targetEntity.OnDeath += OnTargetDeath;

            myCollisionRadius = GetComponent<CapsuleCollider>().radius; //pega o raio do collider do enemy
            targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius; // pega o collider do player
            StartCoroutine(UpdatePath());
        }
    }
    	
	// Update is called once per frame
	void Update ()
    {
        if (hasTarget)
        {
            if (Time.time > timeNextAttack)
            {
                float sqrDstTarget = (target.position - transform.position).sqrMagnitude;
                if (sqrDstTarget < Mathf.Pow(attackDistanceThereshold + myCollisionRadius + targetCollisionRadius, 2))
                {
                    timeNextAttack = Time.time + timeBetweenAttacks;
                    StartCoroutine(Attack());
                }
            }
        }
    }

    private void OnTargetDeath()
    {
        hasTarget = false;
        currentState = State.Idle;
    }

    private IEnumerator Attack()
    {
        currentState = State.Attacking;
        pathfinder.enabled = false;

        Vector3 originalPosition = transform.position; // posicao do enemy
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        Vector3 attackPosition = target.position - dirToTarget * (myCollisionRadius); // posicao do play


        float percent = 0;
        float attackSpeed = 3;
        skinMaterial.color = Color.magenta;

        bool hasAppliedDamage = false;

        while (percent <= 1)
        {
            if (percent >= .5f && !hasAppliedDamage)
            {
                hasAppliedDamage = true;
                targetEntity.TakeDamage(damage);
            }
            percent += Time.deltaTime * attackSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);

            yield return null;
        }

        skinMaterial.color = originalColor;
        currentState = State.Chasing;
        pathfinder.enabled = true;
    }

    private IEnumerator UpdatePath()
    {
        float refreshRate = .25f;
        while (hasTarget)
        {
            if (currentState == State.Chasing)
            {
                Vector3 dirToTarget = (target.position - transform.position).normalized;
                Vector3 targetPosition = target.position - dirToTarget * (myCollisionRadius + targetCollisionRadius + attackDistanceThereshold/2);// quando o collider do enemy colidir com o do player ele não avança para cima do player 
                //new Vector3(target.position.x, 0, target.position.z);
                // seta a posicao que o enemy vai seguir
                if (!dead)
                {
                    pathfinder.SetDestination(targetPosition);
                }
            }
            yield return new WaitForSeconds(refreshRate);
        }
    }
}
