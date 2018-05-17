using UnityEngine;

public class SludgeScript : MonoBehaviour {

    [SerializeField] private float      coolDown;               //attack cool down time
    [SerializeField] private Transform  bulletPos;              //bullet spawn position
    [SerializeField] private float      bulletLifeSpan = 2f;    //bullet life span

    private float    timer;                                     //timer to track
    private Animator animator;                                  //ref to animator


	// Use this for initialization
	void Start ()
    {
        animator = GetComponent<Animator>();                    //get the component
        timer    = 0f;	                                        //set timer to zero
	}
	
	// Update is called once per frame
	void Update ()
    {
        timer += Time.deltaTime;                                //increase timer

        if (timer >= coolDown)                                  //if timer is more than coolDOwn 
        {
            timer = 0.0f;                                       //reset it to zero
            animator.SetTrigger("Attack");                      //attack
        }
	}

    void Attack()
    {
        GameObject bullet = ObjectPooling.instance.GetPoisonBullet();           //get bullet from object pooling
        bullet.transform.position = bulletPos.position;                         //set its transform
        bullet.SetActive(true);                                                 //activate it
        bullet.GetComponent<DeactivateObject>().BasicSettings(bulletLifeSpan);  //set its lifespan
    }
}
