using UnityEngine;

public class BoomerEnemy : MonoBehaviour {

    [SerializeField] private    float       rangeLength  = 6;       //attack range
    [SerializeField] protected  float       coolDown     = 0.5f;    //attack cool down
    [SerializeField] private    Transform   bulletSpawnPos;         //bullet spawn position
    [SerializeField] private    int         damAmount    = 1;       //damage amount
    [SerializeField] private    int         boomLifeSpan = 1;       //life span of boom / bullet
    [SerializeField] protected  GameObject  coin, deathEffect;      //prefabs references

    protected   DamageScript damageScript;                          //ref to damage script
    private     float        scaleX;                                //to track local scale
    private     Transform    playerTarget;                          //track player target
    protected   float        currentTime;                           //time tracker
    private     bool         playerFound = false;                   //tell if player is in range or not

	// Use this for initialization
	protected virtual void Start ()
    {
        damageScript = GetComponent<DamageScript>();                //get the component
        scaleX = transform.localScale.x;                            //set the varible value
        currentTime = 0;                                            //set current time to zero
        playerTarget = GameObject.FindGameObjectWithTag("Player").transform;    //get player reference
    }

    // Update is called once per frame
    protected virtual void Update ()
    {
        if (damageScript.CurrentHealth <= 0)                                //if current health is zero
        {
            AudioManager.instance.PlayZombieDie();
            Instantiate(coin, transform.position, Quaternion.identity);    //spawn the coin gameobject
            GameObject death = Instantiate(deathEffect, transform.position, Quaternion.identity);   //spawn death effect
            death.GetComponent<DeactivateObject>().BasicSettings(2f);       //set death effect life span
            gameObject.SetActive(false);                                    //set gameobject deactive
        }

        currentTime -= Time.deltaTime;                                      //reduce time
        if (CheckPlayer())                                                  //if check player is true
        {
            LookAtPlayer();                                                 //look at player
            if (currentTime <= 0)                                           //if time is less or equal to zero
            {
                currentTime = coolDown;                                     //reset the time
                Shoot();                                                    //shoot
            }
        }
    }

    protected virtual bool CheckPlayer()
    {
        playerFound = false;                                                //set player found to false
        //if distance between gameobject and player is less than range
        if (Vector3.Distance(transform.position , playerTarget.transform.position) <= rangeLength)  
            playerFound = true;                                             //set player found to true

        return playerFound;                                                 //return the value
    }

    protected virtual void LookAtPlayer()
    {
        if (playerTarget == null)                                           //if player is null
        {
            playerTarget = GameObject.FindGameObjectWithTag("Player").transform;    //find player gameobject
            return;                                                         //return
        }

        //player is on right side
        if (playerTarget.position.x > transform.position.x)
            transform.localScale = new Vector3(-scaleX, transform.localScale.y, transform.localScale.z);
        else
            transform.localScale = new Vector3(scaleX, transform.localScale.y, transform.localScale.z);
    }

    protected virtual void Shoot()
    {
        GameObject boom = ObjectPooling.instance.GetBoom();                 //get the boom from object pooling
        boom.transform.position = bulletSpawnPos.position;                  //set its transform
        boom.SetActive(true);                                               //activate it
        boom.GetComponent<BoomScript>().Fire(playerTarget.position);        //set the boom target
        boom.GetComponent<DeactivateObject>().BasicSettings(boomLifeSpan);  //set the life span
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))                                     //if colliding gameobject is bullet
        {
            BulletScript script = other.GetComponent<BulletScript>();       //get the script component
            if (script.DestroyPlayer == false)                              //check if its to destroy player is false
            {
                damageScript.ReduceHealth(script.DamageAmount);             //do damage
            }
            other.gameObject.SetActive(false);                              //deactivate gameobject
        }

        if (other.CompareTag("Player"))                                     //if the gamobject is player
        {   
            other.GetComponent<DamageScript>().ReduceHealth(damAmount);     //do damage
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        UnityEditor.Handles.color = Color.red;
        UnityEditor.Handles.DrawWireDisc(transform.position, new Vector3(0, 0, -1), rangeLength);
    }
#endif

}
