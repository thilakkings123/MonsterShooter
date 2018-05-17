using UnityEngine;

public class BulletScript : MonoBehaviour {

    public float speed = 10;        //speed of bullet
    [SerializeField]
    private int damageAmount = 1;   //damage done by bullet
    [SerializeField]
    private float effectLifeSpan = 0.52f;   //bullet collision effect life span

    private bool destroyPlayer = false; //to differentiate if bullet is to kill player or enemy
    public bool DestroyPlayer { get { return destroyPlayer; } set { destroyPlayer = value; } }  //getter and setter
    public int DamageAmount { get { return damageAmount; } set { damageAmount = value; } }  //getter and setter

    private Rigidbody2D rigi;   //ref to rigidbody2D

    private void Awake()
    {
        rigi = GetComponent<Rigidbody2D>(); //get the component
    }

    // Use this for initialization
    void OnEnable ()
    {
        rigi.velocity = transform.right.normalized * speed;   //we provide speed
    }

    private void OnTriggerEnter2D(Collider2D other)
    {   //on collision
        GameObject bulletEffect = ObjectPooling.instance.GetBulletEffect(); //we create effect
        bulletEffect.transform.position = transform.position;   //set its position
        bulletEffect.SetActive(true);   //activate it
        bulletEffect.GetComponent<DeactivateObject>().BasicSettings(effectLifeSpan);       //called its basic settings

        if (other.CompareTag("Player")) //if its player
        {
            other.GetComponent<DamageScript>().ReduceHealth(damageAmount);  //we make damage to player
        }

        gameObject.SetActive(false);    //and deactivet bullet gameobject
    }
}
