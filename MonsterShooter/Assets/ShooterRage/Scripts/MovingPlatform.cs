using UnityEngine;

public class MovingPlatform : MonoBehaviour {

    public enum MoveAxis { X, Y }   //axis of movement

    [SerializeField]
    private MoveAxis moveAxis = MoveAxis.X; //by default we set it to x
    [SerializeField]
    private float speed;                    //speed of movement
    [SerializeField]
    private Transform bigPoint, smallPoint; //point between which gameobject moves
    [SerializeField]
    private bool ascendingDirection = true; //direction of movement
    private Transform target;               //target towards which gameobject is moving
    private bool changeDirection = false;   //tell if direction is changed

	// Use this for initialization
	void Start ()
    {
        if (ascendingDirection) target = bigPoint;          //if ascending we move towards big point
        else if (!ascendingDirection) target = smallPoint;  //if ascending is false we move towards small point
    }

    private void Update()
    {
        ChangeDirection();
    }

    // Update is called once per frame
    void FixedUpdate ()
    {
        switch (moveAxis)       //depending on moveAxis
        {   
            case MoveAxis.X:    //if axis is X
                AxisX();        //we call AxisX method
                break;
            case MoveAxis.Y:
                AxisY();
                break;
        }
    }

    void AxisX()
    {   //if ascending we move right
        if (ascendingDirection) transform.Translate(Vector3.right * speed * Time.deltaTime);
        else if (!ascendingDirection) transform.Translate(-Vector3.right * speed * Time.deltaTime); //if not ascending we move left
    }

    void AxisY()
    {   //if ascending we move up
        if (ascendingDirection) transform.Translate(Vector3.up * speed * Time.deltaTime);
        else if (!ascendingDirection) transform.Translate(-Vector3.up * speed * Time.deltaTime);    //if not ascending we move down
    }

    void ChangeDirection()
    {
        switch (moveAxis)
        {
            case MoveAxis.X:    //if its x axis
                if (Mathf.Abs(transform.position.x - target.position.x) <= 0.1f)    //if distance between gameobject and target is less than 0.1f
                    changeDirection = true; //change direction
                break;
            case MoveAxis.Y: //if its y axis
                if (Mathf.Abs(transform.position.y - target.position.y) <= 0.1f)    //if distance between gameobject and target is less than 0.1f
                    changeDirection = true; //change direction
                break;
        }

        if (changeDirection) //if change direction is true
        {
            changeDirection = false;    //set it to false
            ascendingDirection = !ascendingDirection;   //change

            if (ascendingDirection) target = bigPoint;  //set the target
            else if (!ascendingDirection) target = smallPoint;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (smallPoint != null && bigPoint != null)
        {
            UnityEditor.Handles.color = Color.red;
            UnityEditor.Handles.DrawLine(smallPoint.position, bigPoint.position);
            UnityEditor.Handles.color = Color.yellow;

            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(smallPoint.position, 0.1f);
            Gizmos.DrawSphere(bigPoint.position, 0.1f);

        }
    }
#endif
}
