using UnityEngine;

public class Spikes : MonoBehaviour {

    [SerializeField]
    private int damAmount = 1;  //damage amount

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))     //it tag is player
        {
            other.GetComponent<DamageScript>().ReduceHealth(damAmount); //reduce health
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player")) //if on stay
        {
            other.GetComponent<DamageScript>().ReduceHealth(damAmount);//reduce health
        }
    }
}
