using UnityEngine;
using UnityEngine.UI;
public class PlayerHealth : MonoBehaviour
{

    public Slider healthSlider;
    public int startingHealth = 100;

    public static bool IsAlive{get; private set;}
 

    private int currentHealth;

    public GameObject player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = startingHealth;
        IsAlive = true;
        UpdateHealthSlider();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


      public void TakeDamage(int damage)
    {
        
        PlayerMovement movement = player.GetComponent<PlayerMovement>();

        if (movement.IsDashing)
        {
            return; //just don't take damage.
        }
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, startingHealth);

        UpdateHealthSlider();

        if(currentHealth <= 0 && IsAlive)
        {
            //player dies
            Die();
        }
    }


        void Die()
    {
        
        IsAlive = false;
        Destroy(gameObject);
        

    }


      void UpdateHealthSlider()
    {
        if (healthSlider)
        {
            healthSlider.value = currentHealth;
        }
    }

     public bool GetAlive
    {
        get { return IsAlive; }
    }

}
