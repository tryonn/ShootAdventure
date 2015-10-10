using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : LivingEntity
{
    public enum State { Idle, Chasing, Attacking}
    State currentState;


    private NavMeshAgent pathfinder; // caminho
    private Transform target; // player

    [SerializeField] private float refreshRate = 1.0f; // tempo para seguir
    [SerializeField] private float attackDistanceThereshold = 1.5f; // distancia para ataque
    [SerializeField] private float timeBetweenAttacks = 1f; //tempo entre um ataque e outro
    [SerializeField] private float timeNextAttack; // tempo para o proximo ataque

    private float myCollisionRadius;
    private float targetCollisionRadius;
    private Material skinMaterial;
    private Color originalColor;
    private void Awake()
    {
        pathfinder = GetComponent<NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    protected override void Start()
    {
        base.Start();
        currentState = State.Chasing;

        myCollisionRadius = GetComponent<CapsuleCollider>().radius; //pega o raio do collider do enemy
        targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius; // pega o collider do player
        skinMaterial = GetComponent<Renderer>().material;
        originalColor = skinMaterial.color; // pega a cor original do object

        StartCoroutine(UpdatePath());
    }
    	
	// Update is called once per frame
	void Update ()
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
        while (percent <= 1)
        {
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
        while (target != null)
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
