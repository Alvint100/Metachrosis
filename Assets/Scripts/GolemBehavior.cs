using UnityEngine;
using UnityEngine.AI;






public class GolemBehavior : MonoBehaviour
{


    public enum EnemyState{Chase, Attack, Die}

    public EnemyState currentState = EnemyState.Chase;

     [Header("Chase Settings")]
     public Transform player;

     public float stopDistance = 2;

     public float detectionRange = 100;
     NavMeshAgent agent;


     [Header("Attack Settings")]

     public GameObject hitbox;

    public float startingHealth = 100;



    private float currentHealth;

     
        private Animator animator;
        private readonly int AttackStateHash = Animator.StringToHash("Base Layer.Attack");

     //[Header("Die Settings")]

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        if (hitbox)
        {
            hitbox.SetActive(false);
        }
        
        currentHealth = startingHealth;


        agent.isStopped = true; //this will resume if the player is far enough away


    }

    // Update is called once per frame
    void Update()
    {
        
          switch (currentState)
        {
            case EnemyState.Chase:

             Chase();
             break;
            case EnemyState.Attack:
             Attack();
             break;
            case EnemyState.Die:
                Die();
                break;
        }


    }



    void Chase()
    {

        //Debug.Log("Hello?");
        if (player)
        {
            agent.SetDestination(player.position);
            float distance = Vector3.Distance(transform.position, player.position);


            if(distance > detectionRange)
            {
                agent.isStopped = true;
                return;
            }

            if(distance < stopDistance)
            {
                //Debug.Log("Erm..");
                currentState = EnemyState.Attack;
                agent.isStopped = true;
                return;
            }
            
            animator.SetInteger("animState",1);
            agent.isStopped = false;
            Debug.Log("Hi?");

            
        }
        else
        {
           animator.SetInteger("animState",0);
        }
   
        
    }


    void Attack()
    {


        if (!player)
        {
            currentState = EnemyState.Chase;
            return;
        }


    


        if (!hitbox)
        {
            Debug.LogWarning("No hitbox attached!");
            return;
        }

        animator.SetInteger("animState",2);
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);


        
        if(stateInfo.normalizedTime > 1f){

        float distance = Vector3.Distance(transform.position, player.position);
            if(distance > stopDistance){
                hitbox.SetActive(false);
                currentState = EnemyState.Chase;
            }

        }


        if (stateInfo.IsName("Base Layer.Attack"))
        {

                float percentThrough = (stateInfo.normalizedTime * 10) % 10; 


            if(percentThrough > 4 && percentThrough < 7)
        {
            hitbox.SetActive(true);
        }
        else
        {
            hitbox.SetActive(false);
        }
        }

    


        

        

        

    }

    void Die()
    {
        animator.SetInteger("animState",3);
        Destroy(gameObject, 3);
        

    }



    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, startingHealth);


        if(currentHealth <= 0)
        {
            currentState = EnemyState.Die;
        }
    }


    public void TakeDamage()
    {
        if(gameObject.tag == "Boss")
        {
            TakeDamage(startingHealth/3);
        }
        else
        {
            TakeDamage(startingHealth);
        }

    }
}
