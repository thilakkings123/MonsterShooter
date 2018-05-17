using UnityEngine;

public enum EnemyType { Petrol, Shoot } //enemy type

public class EnemyMovement : MonoBehaviour {

    [SerializeField] private EnemyType  enemyType = EnemyType.Petrol;   //enemy type
    [SerializeField] private Transform  leftEnd, rightEnd;              //ref to ends
    [SerializeField] private float      speed = 2f;                     //moving speed
    [SerializeField] [Header("For shooter enemy")]
    private float range = 4f;                                           //this is the range on shooter enemy
    [SerializeField] private Transform  rayOrigin;                      //origin of raycast
    [SerializeField] private float      bulletShootDelay;               //time between 2 bullets
    [SerializeField] private Transform  bulletSpawnPos;                 //bullet spawn position
    [SerializeField] private int        damAmount = 1;                  //damage amount
    [SerializeField] private float      bulletLifeSpan = 2f;            //bullet life span
    [SerializeField] private GameObject coin, deathEffect;              //ref to prefabs

    private float        gunCoolDown;                                   //time tracking
    private DamageScript damageScript;                                  //ref to damage script
    private bool         movingRight = true;                            //tell the direction
    private Vector3      target;                                        //target to move towards
    private float        scaleX;                                        //scale x ref
    private Animator     anim;                                          //ref to animator
    private GameObject   playerTarget;                                  //player refrence
    private bool         playerInRange = false;                         //check if player is in range or not

	// Use this for initialization
	void Start ()
    {
        gunCoolDown                     = 0;                                            //set it to zero
        playerTarget                    = GameObject.FindGameObjectWithTag("Player");   //get the player
        anim                            = GetComponent<Animator>();                     //get the component
        damageScript                    = GetComponent<DamageScript>();                 //get the component
        scaleX                          = transform.localScale.x;                       //set the scale x value
        if (movingRight) target         = rightEnd.position;                            //depending on direction set the target
        else if (!movingRight) target   = leftEnd.position;
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

        if (enemyType == EnemyType.Petrol) PetrolMovement();                //if its petrol enemy set movement to petrol
        if (enemyType == EnemyType.Shoot) ShootMovement();                  //if its shoot enemy set movement to shoot  
    }

    void PetrolMovement()
    {
        //if difference between target x and gameobject x value us lss than 0.1f
        if (Mathf.Abs(transform.position.x - target.x) <= 0.1f)             
        {
            movingRight = !movingRight;                                     //inverse direction
            if (movingRight) target = rightEnd.position;                    //depending on direction set next target
            else if (!movingRight) target = leftEnd.position;

            scaleX = -scaleX;                                               //inverse scale
            transform.localScale = new Vector3(scaleX, transform.localScale.y, transform.localScale.z); //set the scale
        }
        Move();
    }

    void ShootMovement()                                                    
    {   
        gunCoolDown -= Time.deltaTime;                                      //reduce cool down
        TrackPlayerTarget();                                                //track player

        if (!playerInRange)                                                 //if player is not in range
        {
            anim.SetBool("walk", true);                                     //walk animation is true
            anim.SetBool("enemyFound", false);                              //enemyfound animation is false
            PetrolMovement();                                               //petrol movement
        }
        else if (playerInRange)                                             //if player is in range
        {
            anim.SetBool("walk", false);                                    //walk animation is false
            if (gunCoolDown <= 0)                                           //if cool down is less or equal to zero
            {
                gunCoolDown = bulletShootDelay;                             //reset cool down
                anim.SetBool("enemyFound", true);                           //enemyfound animation is true
            }
            else if (gunCoolDown > 0)                                       //if cool down is more than zero
                anim.SetBool("enemyFound", false);                          //enemyfound animation is false
        }
    }

    public void ShootBullet()
    {
        GameObject bulletObj = ObjectPooling.instance.GetBullet();                      //get bullet from object pooling
        bulletObj.transform.position = bulletSpawnPos.position;                         //set its position
        if (movingRight)                                                                //moving right
            bulletObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        else if (!movingRight)                                                          //moving left
            bulletObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));

        bulletObj.SetActive(true);
        bulletObj.GetComponent<BulletScript>().DestroyPlayer = true;                    //tell whom to destroy "player or enemy"
        bulletObj.GetComponent<DeactivateObject>().BasicSettings(bulletLifeSpan);       //called its basic settings
    }

    void TrackPlayerTarget()
    {   //call raycast hit
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin.position, new Vector2(transform.localScale.x, 0), range);

        //if collider is null and its tag is Player
        if (hit.collider != null && hit.collider.CompareTag("Player"))  
        {
            playerInRange = true;               //player is in range
        }
        else if (hit.collider == null)          //if collider is null
        {       
            playerInRange = false;              //player is not in range
        }
    }

    private void Move()
    {
        transform.position += new Vector3(speed * scaleX * Time.deltaTime, 0, 0);
        float x = Mathf.Clamp(transform.position.x, leftEnd.position.x, rightEnd.position.x);
        transform.position = new Vector3(x, transform.position.y, transform.position.z);
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

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<DamageScript>().ReduceHealth(damAmount);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (leftEnd != null && rightEnd != null)
        {
            UnityEditor.Handles.color = Color.red;
            UnityEditor.Handles.DrawLine(leftEnd.position, rightEnd.position);
            UnityEditor.Handles.color = Color.yellow;

            if (rayOrigin != null)
                UnityEditor.Handles.DrawLine(rayOrigin.position, rayOrigin.position + new Vector3(transform.localScale.x, 0, 0) * range);

            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(leftEnd.position, 0.1f);
            Gizmos.DrawSphere(rightEnd.position, 0.1f);

        }
    }
#endif
}
