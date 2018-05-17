using UnityEngine;

public class ShieldAlien : MonoBehaviour {

    [SerializeField] private float      bulletShootDelay, attackTime, idleTime, range;  //few important variables
    [SerializeField] private Transform  bulletSpawnPos;                                 //bullet spawn position
    [SerializeField] private int        damAmount = 1;                                  //damage amount
    [SerializeField] private bool       lookingRight = true;                            //tell the direction
    [SerializeField] private float      bulletLifeSpan;                                 //bullet life span
    [SerializeField] private GameObject coin, deathEffect;                              //ref to prefabs

    private float        gunCoolDown, currentAttackTime, currentIdleTime;               //time trackers
    private DamageScript damageScript;                                                  //ref to damage script
    private bool         attacking = false;                                             //tell if attacking
    private Animator     anim;                                                          //animator reference
    private bool         playerInRange;                                                 //tell if player is in range
    private Transform    playerTarget;                                                  //ref to target

    // Use this for initialization
    void Start ()
    {
        playerTarget      = GameObject.FindGameObjectWithTag("Player").transform;   //get reference to the player
        currentAttackTime = attackTime;                                             //set the attack time
        currentIdleTime   = idleTime;                                               //set the idle time
        damageScript      = GetComponent<DamageScript>();                           //get component
        anim              = GetComponent<Animator>();                               //get component
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (damageScript.CurrentHealth <= 0)                                //if current health is zero
        {
            AudioManager.instance.PlayZombieDie();
            Instantiate(coin, transform.position, Quaternion.identity);    //spawn the chest gameobject
            GameObject death = Instantiate(deathEffect, transform.position, Quaternion.identity);   //spawn death effect
            death.GetComponent<DeactivateObject>().BasicSettings(2f);       //set death effect life span
            gameObject.SetActive(false);                                    //set gameobject deactive
        }

        CheckPlayer();                                                      //check fro the player

        if (playerInRange)                                                  //if player is in range
        {
            if (!attacking)                                                 //and attacking is false
            {
                currentIdleTime -= Time.deltaTime;                          //reduce the idle time
                if (currentIdleTime <= 0)                                   //if idle time is less than zero
                {
                    attacking = true;                                       //attacking is true
                    currentAttackTime = attackTime;                         //set attack time
                }
            }
            else if (attacking)                                             //if attacking is true
            {
                currentAttackTime -= Time.deltaTime;                        //reduce the attack time  
                if (currentAttackTime > 0)                                  //if attack time is more than zero
                {
                    gunCoolDown -= Time.deltaTime;                          //reduce cool down time

                    if (gunCoolDown <= 0)                                   //if cool down time is less than zero
                    {
                        gunCoolDown = bulletShootDelay;                     //reset cool down time
                        ShootBullet();                                      //shoot
                    }
                }
                else if (currentAttackTime <= 0)                            //if attack time is less than zero
                {
                    attacking = false;                                      //attacking is false
                    currentIdleTime = idleTime;                             //set idle time
                }
            }
        }
        else
            attacking = false;

        anim.SetBool("attack", attacking);                                  //set the animation
    }

    void CheckPlayer()
    {
        if (playerTarget == null)                                           //if player is null
        {
            playerTarget = GameObject.FindGameObjectWithTag("Player").transform;    //get player
            return;
        }

        //if distance between target and gameobject value is less than range
        if (Vector3.Distance(transform.position , playerTarget.position) <= range)
        {
            playerInRange = true;   //player is in range
        }
        else
            playerInRange = false;  //else player is not in range
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))                                     //if collider tag is bullet
        {
            BulletScript script = other.GetComponent<BulletScript>();       //get the script refrence
            if (script.DestroyPlayer == false)                              //if destroy player is false
            {
                damageScript.ReduceHealth(script.DamageAmount);             //do damage
            }
            other.gameObject.SetActive(false);                              //set the collided object deactive
        }

        if (other.CompareTag("Player"))                                     //if its player object
        {
            other.GetComponent<DamageScript>().ReduceHealth(damAmount);     //do damage to it
        }
    }

    private void ShootBullet()
    {
        GameObject bulletObj = ObjectPooling.instance.GetBullet();
        bulletObj.transform.position = bulletSpawnPos.position;
        if (lookingRight) //looking right
            bulletObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        else if (!lookingRight) //looking left
            bulletObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
        bulletObj.SetActive(true);
        bulletObj.GetComponent<BulletScript>().DestroyPlayer = true;  //tell whom to destroy "player or enemy"
        bulletObj.GetComponent<DeactivateObject>().BasicSettings(bulletLifeSpan);       //called its basic settings
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        UnityEditor.Handles.color = Color.red;
        UnityEditor.Handles.DrawWireDisc(transform.position, new Vector3(0, 0, -1), range);
    }
#endif

}
