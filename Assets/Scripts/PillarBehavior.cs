using UnityEngine;

public class PillarBehavior : MonoBehaviour
{


    bool destroyed = false;

    void OnTriggerEnter(Collider other)
    {

        if (destroyed)
        {
            return;
        }
       // Debug.Log("Erm what the " + other.tag);
       if (other.CompareTag("EnemyHit"))
        {

            // Debug.Log("CHECKING CHECKING");
           var enemyHealth = other.transform.parent.transform.GetComponent<GolemBehavior>();

        
            if (enemyHealth)
            {
                enemyHealth.TakeDamage();
                destroyed = true;
            } 
        }


        
        
    }
}
