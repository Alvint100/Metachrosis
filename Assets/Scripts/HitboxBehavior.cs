using UnityEngine;

public class HitboxBehavior : MonoBehaviour
{


    public int damage = 5;
     void OnTriggerEnter(Collider other)
    {
       // Debug.Log("Erm what the " + other.tag);
       if (other.CompareTag("Player"))
        {

            Debug.Log("Nice!");
           var playerHealth = other.transform.GetComponent<PlayerHealth>();

        
            if (playerHealth)
            {
                playerHealth.TakeDamage(damage);
            } 
        }

        
    }

}
