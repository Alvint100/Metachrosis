using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{


    public string nextLevel;

    public AudioClip winSFX;
    public AudioClip loseSFX;

     public TMP_Text messageText;

    AudioSource audioSource;

    public GameObject player;

    public GameObject nextButton;



    private bool gameWon = false;

    private bool gameOver = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        nextButton.SetActive(false);
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {


        if (gameOver)
        {
            return;
        }
        bool isPlayerAlive = false;

        if (player)
        {
                isPlayerAlive = player.GetComponent<PlayerHealth>().GetAlive;
        } 
        
       if (gameWon)
        {
            DisplayGameMessage("You Won!");
            PlaySoundClip(winSFX);
            nextButton.SetActive(true);
               Destroy(player);
          gameOver = true;
            return;
        }



        if (!isPlayerAlive)
        {
             DisplayGameMessage("YOU DIED");
            PlaySoundClip(loseSFX);
            nextButton.SetActive(true);
            gameOver = true;
            return;
        }

       

        nextButton.SetActive(false);

        messageText.enabled = false;

     
        
        
        
        
    }

      void DisplayGameMessage(string message)
    {

        if(messageText){
        messageText.text = message;
        messageText.enabled = true;
     
        }
          
      
    }


      void PlaySoundClip(AudioClip clip)
    {
       audioSource.clip = clip;
       audioSource.Play();
    }


      public void ReloadSameScene()
    {
        Debug.Log("Testing testing");

        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }


     void OnTriggerEnter(Collider other)
    {
       // Debug.Log("Erm what the " + other.tag);
       if (other.CompareTag("Player"))
        {

            GameObject[] enemies =  GameObject.FindGameObjectsWithTag("Enemy");
            
            if(enemies.Length ==0)
            {
                     gameWon = true;
            }

      
        }

        
    }
}
