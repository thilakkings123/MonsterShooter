using System.Collections;
using UnityEngine;

public class DamageScript : MonoBehaviour {

    [SerializeField]
    private int maxHealth = 10;                                     //max health of gameobject
    [SerializeField] [Header("Damage Effect Color")]
    private Color targetColor;                                      //color when get damage
    [SerializeField]
    private float blinkSpeed = 0.1f, blinkDuration = 0.5f;          //blink time of color of damage
    [SerializeField]
    private Renderer rendererR;                                     //ref to sprite renderer

    private int currentHealth;                                      //current health
    private PlayerController playerController;                      //ref to player controller

    public int CurrentHealth { get { return currentHealth; } }      //getter
    public int MaxHealth { get { return maxHealth; } }              //setter

    // Use this for initialization
    void Start ()
    {
        if (gameObject.CompareTag("Player"))                        //if this gameobject tag is player
        {
            maxHealth = GameManager.instance.health * 3;            //set its max health
            playerController = GetComponent<PlayerController>();    //get player controller
        }

        currentHealth = maxHealth;                                  //set its current health to maxhealth
            
        UpdateHealthBar();                                          //health bar only for player
    }

    void UpdateHealthBar()
    {
        if (gameObject.CompareTag("Player"))
        {   //set value of fill
            gameObject.GetComponent<PlayerController>().HealthBar.fillAmount = (float)currentHealth / maxHealth;
        }
    }

    private void Update()
    {
        if (GameManager.instance.playerDead)
            return;

        if (gameObject.CompareTag("Player"))
        {
            if (playerController.PlayerImmune)                          //if player is immune
            {
                playerController.CurrentImmuneTime -= Time.deltaTime;   //decrease the immune time
                if (playerController.CurrentImmuneTime <= 0)            //when its less or equal to zero
                    playerController.PlayerImmune = false;              //set it false
            }
        }
    }

    public void ReduceHealth(int value)                                         //method called to reduce health
    {
        if (gameObject.CompareTag("Player") && playerController.PlayerImmune)   //if gameobject tag is player and its immune
            return;                                                             //then return

        currentHealth -= value;                                                 //reduce the health
        
        if (gameObject.CompareTag("Player"))                                    //if gameobject tag is player
        {
            AudioManager.instance.PlayPlayerDie();
            playerController.PlayerImmune = true;                               //set it immune
            playerController.CurrentImmuneTime = playerController.ImmuneTime;   //set the immune time
        }

        DamageEffect();                                                         //damage effect
        UpdateHealthBar();                                                      //update health bar only for player
    }

    public void IncreaseHealth(int value)                                       //to increase health
    {
        currentHealth += value;                                                 //increase health

        if (currentHealth > maxHealth) currentHealth = maxHealth;               //if health is more set it to max
        UpdateHealthBar();
    }

    public void UpdateHealth()  //this is for shop 
    {
        maxHealth += 3;
        currentHealth += 3;
        UpdateHealthBar();
    }

    void DamageEffect()
    {
        StartCoroutine(Blink());
    }

    IEnumerator Blink()
    {
        Color initialColor = Color.white;                   //set initial color to white
        float blinkStop = Time.time + blinkDuration;        //set blinkStop

        while (Time.time < blinkStop)                       //while time is less than blink shop
        {
            rendererR.material.color = initialColor;        //set color to initial color
            yield return new WaitForSeconds(blinkSpeed);    //after few sec
            rendererR.material.color = targetColor;         //set it to target color
            yield return new WaitForSeconds(blinkSpeed);    //wait for few sec
        }

        rendererR.material.color = initialColor;            //at last set back to initial color
    }
}
