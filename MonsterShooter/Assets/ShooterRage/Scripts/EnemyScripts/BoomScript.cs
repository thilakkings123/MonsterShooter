using UnityEngine;

public class BoomScript : MonoBehaviour {

    [SerializeField] private int   damAmount       = 1;         //damage amount
    [SerializeField] private float speed           = 3;         //moving speed
    [SerializeField] private float speedOverTime   = 0.5f;      // Constant acceleration
    [SerializeField] private float hitDistance     = 0.2f;      // If target is close than this distance - it will be hitted
    [SerializeField] private float ballisticOffset = 0.5f;      // Ballistic trajectory offset (in distance to target)
    [SerializeField] private float effectLifeSpan  = 0.52f;     //life span
    [SerializeField] private Rigidbody2D myBody;

    private Vector2 originPoint;                                //point of spawn
    private Vector3 target;                                     //target point
    private Vector2 aimPoint;                                   // Last target's position
    private Vector2 myVirtualPosition;                          // Current position without ballistic offset
    private Vector2 myPreviousPosition;                         // Position on last frame
    private float   counter, currentSpeed;                      // Counter for acceleration calculation
    
    private bool    reachedTarget = false;

    Vector2 distanceToAim;

    // Update is called once per frame
    void FixedUpdate ()
    {
        counter += Time.fixedDeltaTime;
        //Add Acceleration
        currentSpeed += Time.fixedDeltaTime * speedOverTime;
        if (target != null)
            aimPoint = target;
        if (!reachedTarget)
        {
            // Calculate distance from firepoint to aim
            Vector2 originDistance = aimPoint - originPoint;
            // Calculate remaining distance
            distanceToAim = aimPoint - (Vector2)myVirtualPosition;
            // Move towards aim
            myVirtualPosition = Vector2.Lerp(originPoint, aimPoint, counter * currentSpeed / originDistance.magnitude);
            // Add ballistic offset to trajectory
            transform.position = AddBallisticOffset(originDistance.magnitude, distanceToAim.magnitude);
            myPreviousPosition = transform.position;
        }

        if (distanceToAim.magnitude <= hitDistance)
        {
            reachedTarget = true;
            myBody.gravityScale = 1;
        }
    }

    private void BasicSetting()
    {
        counter = 0;
        currentSpeed = speed;
        myBody.gravityScale = 0;
        reachedTarget = false;
    }

    /// <summary>
	/// Adds ballistic offset to trajectory.
	/// </summary>
	/// <returns>The ballistic offset.</returns>
	/// <param name="originDistance">Origin distance.</param>
	/// <param name="distanceToAim">Distance to aim.</param>
	private Vector2 AddBallisticOffset(float originDistance, float distanceToAim)
    {
        if (ballisticOffset > 0f)
        {
            // Calculate sinus offset
            float offset = Mathf.Sin(Mathf.PI * ((originDistance - distanceToAim) / originDistance));
            offset *= originDistance;
            // Add offset to trajectory
            return (Vector2)myVirtualPosition + (ballisticOffset * offset * Vector2.up);
        }
        else
        {
            return myVirtualPosition;
        }
    }

    public void Fire(Vector3 _target)
    {
        BasicSetting();
        originPoint = myVirtualPosition = myPreviousPosition = transform.position;
        target = _target;
        aimPoint = _target;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Effect();

        if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<DamageScript>().ReduceHealth(damAmount);
        }

        reachedTarget = false;
        myBody.gravityScale = 0;
        gameObject.SetActive(false);
    }

    void Effect()
    {
        AudioManager.instance.PlayBoom();
        GameObject bulletEffect = ObjectPooling.instance.GetBulletEffect();
        bulletEffect.transform.position = transform.position;
        bulletEffect.SetActive(true);
        bulletEffect.GetComponent<DeactivateObject>().BasicSettings(effectLifeSpan);       //called its basic settings
    }

}
