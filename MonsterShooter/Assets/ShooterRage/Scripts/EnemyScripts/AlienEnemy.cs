using UnityEngine;

public class AlienEnemy : MonoBehaviour {

    public enum AlienMode { idle, run, attack }                             //modes

    [SerializeField] private float        speed = 2f;                       //speed
    [SerializeField] private float        idleTime;                         //idle time
    [SerializeField] private Transform    leftEnd, rightEnd;                //ends
    [SerializeField] private Transform    beamSpawnPos;                     //beam spawn position
    [SerializeField] private float        beamMaxLength, attackSpeed;       //max beam length , attack speed
    [SerializeField] private float        attackTime;                       //attack time
    [SerializeField] private LineRenderer beam;                             //ref to line renderer
    [SerializeField] private AlienMode    alienMode = AlienMode.idle;       //mode
    [SerializeField] private int          damAmount = 1;                    //damage amount
    [SerializeField] private float        playerDamageCoolDown = 0.5f;      //damage after specific time
    [SerializeField] private LayerMask    whoIsPlayer;                      //detect player layer
    [SerializeField] private GameObject   chest, deathEffect;               //object prefabs ref

    private DamageScript damageScript;                                      //ref to damage script
    private float        currentIdleTime, currentAttackTime;                //time tracking
    private bool         movingRight = true;                                //decide direction
    private Vector3      target;                                            //target toward which object move
    private float        scaleX;                                            //to set locla scale
    private Animator     anim;                                              //ref to animator
    private bool         wasRuning = false;                                 //to decide the mode
    private float        currentCoolDown;                                   //time tracker
    private RaycastHit2D hit;                                               //raycast hit ref
    private float        x = 0;                                             //this is to increase the beam size with time

    // Use this for initialization
    void Start ()
    {
        anim                = GetComponent<Animator>();                     //get the component
        damageScript        = GetComponent<DamageScript>();                 //get the component
        scaleX              = transform.localScale.x;                       //set the scale
        currentIdleTime     = idleTime;                                     //set idle time
        currentAttackTime   = attackTime;                                   //set attack time
        alienMode           = AlienMode.idle;                               //set mode
        movingRight         = false;                                        //moving right is false at start
        target              = leftEnd.position;                             //at start target is left end
        currentCoolDown     = 0;                                            //cool down time is zero
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (damageScript.CurrentHealth <= 0)                                //if current health is zero
        {
            AudioManager.instance.PlayZombieDie();
            Instantiate(chest, transform.position, Quaternion.identity);    //spawn the chest gameobject
            GameObject death = Instantiate(deathEffect, transform.position, Quaternion.identity);   //spawn death effect
            death.GetComponent<DeactivateObject>().BasicSettings(2f);       //set death effect life span
            gameObject.SetActive(false);                                    //set gameobject deactive
        }

        if (alienMode == AlienMode.idle)                                    //if in idle mode
        {
            currentIdleTime -= Time.deltaTime;                              //reduce idle time

            if (currentIdleTime > 0)                                        //if idle time is more than zero
            {
                anim.SetBool("Run", false);                                 //set animation , run and attack to false
                anim.SetBool("Attack", false);
            }
            else if (currentIdleTime <= 0)                                  //if idle time is less or equal to zero
            {
                if (wasRuning == false)                                     //if alien was not running before it went to idle
                {
                    wasRuning = true;                                       //we set it to true
                    alienMode = AlienMode.run;                              //set the mode to run
                    anim.SetBool("Run", true);                              //set anim bool to true
                }
                else if (wasRuning == true)                                 //if alien was running before it went to idle
                {
                    wasRuning = false;                                      //we set it to false
                    currentCoolDown = 0;                                    //cool down to zero
                    currentAttackTime = attackTime;                         //set attack time
                    alienMode = AlienMode.attack;                           //set the mode to attack
                }
            }
        }
        else if (alienMode == AlienMode.run)                                //if alien is in run mode
        {   
            PetrolMovement();                                               //we call petrol movement method
        }
        else if (alienMode == AlienMode.attack)                             //if alien is in attack mode
        {
            currentAttackTime -= Time.deltaTime;                            //we reduce attack time

            if (currentAttackTime > 0)                                      //if its more than zero
            {
                Attack();                                                   //we attack
                anim.SetBool("Attack", true);                               //set animation to attack
            }
            else if (currentAttackTime <= 0)                                //if its less o equal to zero
            {
                ResetBeam();                                                //reset the beam
                alienMode = AlienMode.idle;                                 //set to idle mode
                anim.SetBool("Attack", false);                              //set attack animation to false
                currentIdleTime = idleTime;                                 //set idle time
            }
        }

    }

    void Attack()
    {
        if (Mathf.Abs(beam.GetPosition(1).x) < beamMaxLength)               //if beam length is less than max length
            x += attackSpeed * Time.deltaTime;                              //we increase x with time and attack speed

        hit = Physics2D.Raycast(beamSpawnPos.position, new Vector2(transform.localScale.x, 0), x, whoIsPlayer); //set the raycast
        beam.SetPosition(1, new Vector3(x, 0, 1));                          //set beam 2nd positions(line renderer position)

        if (currentCoolDown >= 0)                                           //if coold down is more or equal to zero
            currentCoolDown -= Time.deltaTime;                              //reduce it

        //if hit is colliding and the collider tag is "Player" and cool down time is less or equal to zero
        if (hit.collider != null && hit.collider.CompareTag("Player") && currentCoolDown <= 0)  
        {
            currentCoolDown = playerDamageCoolDown;                             //reset cool down time
            hit.collider.GetComponent<DamageScript>().ReduceHealth(damAmount);  //do damage to player
        }
    }

    void ResetBeam()
    {
        x = 0;                                                                  //x to zero
        beam.SetPosition(1, new Vector3(0, 0, 1));                              // 1st position of line renderer to default pos
    }

    void PetrolMovement()
    {
        //if difference between target x and gameobject x value is less than 0.1f
        if (Mathf.Abs(transform.position.x - target.x) <= 0.1f)                 
        {
            anim.SetBool("Run", false);                                         //set run animation to false
            alienMode           = AlienMode.idle;                               //set idle mode
            currentAttackTime   = attackTime;                                   //set attack time
            movingRight         = !movingRight;                                 //change direction
            if (movingRight)        target = rightEnd.position;                 //depending on direction set next target
            else if (!movingRight)  target = leftEnd.position;

            scaleX = -scaleX;                                                   //inverse scale
            transform.localScale = new Vector3(scaleX, transform.localScale.y, transform.localScale.z); //set the scale
            return;                                                             //return
        }
        Move();
    }

    private void Move()
    {
        transform.position += new Vector3(speed * scaleX * Time.deltaTime, 0, 0);   //move in given direction
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

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (leftEnd != null && rightEnd != null)
        {
            UnityEditor.Handles.color = Color.red;
            UnityEditor.Handles.DrawLine(leftEnd.position, rightEnd.position);

            UnityEditor.Handles.color = Color.yellow;
            if (beamSpawnPos != null)
                UnityEditor.Handles.DrawLine(beamSpawnPos.position, beamSpawnPos.position + new Vector3(transform.localScale.x, 0, 0) * x);

            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(leftEnd.position, 0.1f);
            Gizmos.DrawSphere(rightEnd.position, 0.1f);

        }
    }
#endif
}
