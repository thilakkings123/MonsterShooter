using UnityEngine;

public class PoisonBullet : MonoBehaviour {

    [SerializeField]
    private float speed;                                                                    //moving speed
    [SerializeField]
    private float effectLifeSpan = 0.52f;                                                   //effect lifespan

    // Update is called once per frame
    void Update()
    {
        transform.Translate(speed * Vector3.down * Time.deltaTime);                         //move in down direction
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")                                                 //if colliding with player
        {
            col.GetComponent<DamageScript>().ReduceHealth(1);                               //reduce health by 1

            GameObject poisonEffect = ObjectPooling.instance.GetPoisonEffect();             //get poison effect
            poisonEffect.transform.position = transform.position;                           //set its transform
            poisonEffect.SetActive(true);                                                   //activate it
            poisonEffect.GetComponent<DeactivateObject>().BasicSettings(effectLifeSpan);    //set its life span
            gameObject.SetActive(false);                                                    //deactivate the gamobject
        }
        else if (LayerMask.LayerToName(col.gameObject.layer) == "Ground" || 
                 LayerMask.LayerToName(col.gameObject.layer) == "OneWayPlatforms")          //if colliding with ground or onewayplatform
        {
            GameObject poisonEffect = ObjectPooling.instance.GetPoisonEffect();             //get poison effect
            poisonEffect.transform.position = transform.position;                           //set its transform
            poisonEffect.SetActive(true);                                                   //activate it
            poisonEffect.GetComponent<DeactivateObject>().BasicSettings(effectLifeSpan);    //set its life span
            gameObject.SetActive(false);                                                    //deactivate the gamobject
        }


    }
}
