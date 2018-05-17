using UnityEngine;

public class FlyingEnemy : MonoBehaviour {

    [SerializeField] private float      range = 8, speed = 3;   //speed and range
    [SerializeField] private int        damAmount = 1;          //damage amount
    [SerializeField] private GameObject coin, deathEffect;      //ref to prefabs

    private DamageScript damageScript;                          //ref to damage script
    private Transform    target;                                //ref to target toward gameobject will move

	// Use this for initialization
	void Start ()
    {
        damageScript = GetComponent<DamageScript>();                    //get the component
        target = GameObject.FindGameObjectWithTag("Player").transform;  //get the player
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

        if (target != null)                                                 //if target is not null
        {   //check distance between target and gameobject is less than range
            if (Vector3.Distance(transform.position, target.position) <= range) 
            {                                                               //move towards target
                transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
                LookAtTarget();                                             //look at target
            }
        }
	}

    void LookAtTarget()
    {
        //target is at left side
        if (transform.position.x >= target.position.x)
            transform.localScale = new Vector3(1, 1, 1);
        else
            transform.localScale = new Vector3(-1, 1, 1);
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
        UnityEditor.Handles.color = Color.red;
        UnityEditor.Handles.DrawWireDisc(transform.position, new Vector3(0, 0, -1), range);
    }
#endif
}
