using UnityEngine;

public class RunningEnemy : MonoBehaviour {

    [SerializeField] private float      speed     = 2f;                 //moving speed
    [SerializeField] private Transform  rayOrigin, targetRayOrigin;     //ray origin position to detect target and obstacle
    [SerializeField] private float      range     = 1f;                 //this is the range to detect object in front
    [SerializeField] private float      targetRange;                    //target range
    [SerializeField] private int        damAmount = 1;                  //damage amount
    [SerializeField] private Animator   anim;                           //ref to animator
    [SerializeField] private bool       move      = false;              //tell if moving or not
    [SerializeField] private GameObject coin, deathEffect;              //ref to prefabs
    [SerializeField] private LayerMask  targetLayer;                    //layer of player

    private GameObject   playerTarget;                                  //ref to player target
    private DamageScript damageScript;                                  //ref to damage script
    private float        scaleX;                                        //ref to scale

    // Use this for initialization
    void Start ()
    {
        playerTarget = GameObject.FindGameObjectWithTag("Player");      //get the player reference
        damageScript = GetComponent<DamageScript>();                    //get component
        scaleX       = transform.localScale.x;                          //set the scale
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

        PlayerInRange();                                                    //check if player is in range or not
        CheckObstacleAtFront();                                             //check if obstacle is in front or not
        if (move)                                                           //if move is true
            Move();                                                         //move
    }

    void CheckObstacleAtFront()
    {   //create a ray cast
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin.position, new Vector2(transform.localScale.x, 0), range);
        //if collider is not null and tag is Level
        if (hit.collider != null && hit.collider.CompareTag("Level"))
        {
            scaleX = -scaleX;   //inverse scale
            transform.localScale = new Vector3(scaleX, transform.localScale.y, transform.localScale.z); //set scale
        }
    }

    private void Move()
    {
        transform.position += new Vector3(speed * scaleX * Time.deltaTime, 0, 0);
        anim.SetTrigger("Run"); //set run animation to true
    }

    private void PlayerInRange()
    {
        if (move) return;                                           //if move is true we return
        //create a ray cast
        RaycastHit2D hit = Physics2D.Raycast(targetRayOrigin.position, new Vector2(transform.localScale.x, 0), targetRange, targetLayer);
        //if collider is not null and its player
        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            move = true;    //move is set to true
        }
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
        UnityEditor.Handles.color = Color.red;
        if (rayOrigin != null)
            UnityEditor.Handles.DrawLine(rayOrigin.position, rayOrigin.position + new Vector3(transform.localScale.x, 0, 0) * range);

        UnityEditor.Handles.color = Color.green;
        if (targetRayOrigin != null)
            UnityEditor.Handles.DrawLine(targetRayOrigin.position, targetRayOrigin.position + new Vector3(transform.localScale.x, 0, 0) * targetRange);
    }
#endif
}
