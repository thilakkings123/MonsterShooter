using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    public static PlayerController instance;

    private Rigidbody2D myBody;

    [SerializeField] private Image      healthBar;                          //ref to health bar ui
    private bool       usingKeyboard = true;               //to change player input
    [SerializeField] private float      jumpForce = 12;                     //jump force of player
    [SerializeField] private float      groundRadius = 0.25f;               //collider radius which detects ground
    [SerializeField] private Transform  ground;                             //position of collider which detects ground
    [SerializeField] private LayerMask  whatIsGround, whatIsOneWayPlatform; //layers to identify
    [SerializeField] private Transform  bulletSpawnPos;                     //bullet spawn position
    [SerializeField] private float      bulletLifeSpan = 2f;                //life span of bullet
    [SerializeField] private float      immuneTime = 0.25f;                 //we need this to protect player from gatting damage every frame

    private float        gunFireRate;                                       //time between 2 bullets
    private float        speed;                                             //movement speed of player
    private int          gunDamage;                                         //damage done by gun / bullet
    private float        currentImmuneTime;                                 //current player immunity time from damage
    private DamageScript damageScript;                                      //ref to damage script attached                       
    private float        gunCoolDown;                                       //time tracking of gun cool down
    private bool         isGrounded, jump;                                  //to check if player is on ground , and is jumping
    private bool         lookingRight       = true;                         //to tell which direction player is looking
    private bool         jumpPressed        = false;                        //this is for special jump function
    private bool         onOneWayPlatform   = false;                        //tells if we are on one way platform
    private float        layerChangeTime    = 0.2f;                         //we change player layer
    private float        currentlayerChangeTime;                            //time tracker
    private Animator     anim;                                              //ref to animator
    private float        x;
    private float        h;                                                 //horizontal value for movement

    //for touch inputs
    private bool  move = false, firing = false, playerImmune = false;       //player moving or not
    private float btnHorizontal;                                            //ref to gui button value
    private float dirX;                                                     //ref to direction player is moving

    //getter and setter , so we can access private variables from other scripts
    public bool UsingKeyboard       {                                    get { return usingKeyboard;     } }
    public float Speed              { set { speed             = value; } get { return speed;             } }
    public float GunFireRate        { set { gunFireRate       = value; } get { return gunFireRate;       } }
    public int GunDamage            { set { gunDamage         = value; } get { return gunDamage;         } }
    public Image HealthBar          { set { healthBar         = value; } get { return healthBar;         } }
    public bool PlayerImmune        { set { playerImmune      = value; } get { return playerImmune;      } }
    public float CurrentImmuneTime  { set { currentImmuneTime = value; } get { return currentImmuneTime; } }
    public float ImmuneTime         {                                    get { return immuneTime;        } }

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    // Use this for initialization
    void Start ()
    {
        gunCoolDown                        = 0;                             //at start we want it to zero so player can immideatly fire
        currentImmuneTime                  = 0;                             //at start we want it to zero so player can get hurt
        currentlayerChangeTime             = layerChangeTime;               
        myBody                             = GetComponent<Rigidbody2D>();   //get the component
        anim                               = GetComponent<Animator>();      //get the component
        damageScript                       = GetComponent<DamageScript>();  //get the component
        GameManager.instance.playerDead    = false;                         //set dead to false
        GameManager.instance.levelComplete = false;                         //set level complete to false

        speed = 3 + 0.5f * GameManager.instance.speed;                      //set the speed
        if (speed > 8) speed = 8;                                           //if speed is more than 8 we set it to 8

        gunFireRate = 1f - 0.2f * GameManager.instance.gunFireRate;         //set gunFireRate
        if (gunFireRate < 0.2f) gunFireRate = 0.2f;                         //if gunFireRate is less than 0.2f , we set it to 0.2f

        gunDamage = GameManager.instance.gunDamage;                         //set gun damage

#if MOBILE_INPUT
        usingKeyboard = false;
#else
        usingKeyboard = true;
#endif
    }

    // Update is called once per frame
    void Update ()
    {
        if (GameManager.instance.levelComplete)                             //if level is complete
        {
            myBody.velocity = Vector2.zero;                                 //set velocity to zero
            anim.SetFloat("speed", Mathf.Abs(myBody.velocity.x));           //set the animation
            return;                                                         //and return
        }

        if (damageScript.CurrentHealth <= 0)                                //if current health is less or equal to 0
        {
            if (!GameManager.instance.playerDead)                           //and player is not dead
            {
                AudioManager.instance.PlayPlayerDie();
                GameManager.instance.playerDead = true;                     //we set dead to true
                GameUI.instance.LevelFailed();                              //call level failed ui
            }
            myBody.velocity = Vector3.zero;                                 //set velocity to zero
            anim.SetBool("isDead", true);                                   //set the animation
            return;                                                         //return
        }

        GroundCheck();                                                      //keep checking ground
        OneWayPlatformCheck();                                              //keep checking OneWayPlatfrom

        if (jumpPressed)                                                    //if jumpPressed is true
        {
            if (onOneWayPlatform)                                           //and on OneWayPlatfrom
            {
                currentlayerChangeTime -= Time.deltaTime;                   //we reduce time
                if (currentlayerChangeTime <= 0)                            //if its less or equal to zero
                    gameObject.layer = LayerMask.NameToLayer("AvoidOneWayPlatform");    //change the layer
            }
            else
                currentlayerChangeTime = layerChangeTime;                   //else reset the time
        }

        gunCoolDown -= Time.deltaTime;                                      //keep reducing gunCoolDown

        if (usingKeyboard == false)                                         //if usingKeyboard is false
        {
            if (move)                                                       //move is true
            {
                btnHorizontal = Mathf.Lerp(btnHorizontal, dirX, (speed + 2));   //set btnHorizontal value
            }

            if(firing && gunCoolDown <= 0)                                  //if firing is true and gunCoolDown is less or equal to zero
                Shoot();                                                    //we shoot

            if (jump)                                                       //if jump is true
            {
                jump = false;                                               //set it to false
                Jump();                                                     //call jump
            }

        }

        else if (usingKeyboard == true)                                     //if usingKeyboard is true
        {
            h = Input.GetAxisRaw("Horizontal");                             //get horizontal input

            if (Input.GetKeyDown(KeyCode.Space))                            //if we press down space
            {
                jump = true;                                                //set jump to true
                jumpPressed = true;                                         //jumpPressed to true
            }
            else if (Input.GetKeyUp(KeyCode.Space))                         //if we lift space
            {
                jumpPressed = false;                                        //jumpPressed to false
            }

            if (Input.GetMouseButtonDown(0) && gunCoolDown <= 0)            //if mouse is clicked and gunCoolDown is less or equal to zero
            {
                Shoot();                                                    //we shoot
            }
        }

        anim.SetFloat("speed", Mathf.Abs(myBody.velocity.x));               //set the animation
    }

    public void UpdateHealth()                                              //update health
    {
        damageScript.UpdateHealth();
    }

    private void FixedUpdate()
    {
        if (damageScript.CurrentHealth <= 0)                                //if current health is less or equal to zero
            return;                                                         //return

        if (usingKeyboard == false)                                         //if keyboard input is false
        {          
            if (move)                                                       //move is true
            {
                HandleMovement(btnHorizontal);                              //handle movement
                HandleFlip(dirX);                                           //handle direction
            }
            else if (!move)                                                 //if move is false
            {
                if (Mathf.Abs(btnHorizontal) > 0)                           //if btnHorizontal is move than 0
                {
                    btnHorizontal = 0;                                      //set it to zero
                    HandleMovement(btnHorizontal);                          //call handleMovement
                }
            }

            //if (jump)                                                       //if jump is true
            //{
            //    jump = false;                                               //set it to false
            //    Jump();                                                     //call jump
            //}

        }

        else if (usingKeyboard == true)                                     //if we are using keyboard
            Movement();                                                     //call movement method
    }

    void Movement()                                                         
    {
        HandleMovement(h);                                  
        HandleFlip(h);

        if (jump)
        {
            jump = false;
            Jump();
        }
    }

    private void Shoot()                                                    //shoot method
    {   
        gunCoolDown = gunFireRate;                                          //reset gunCoolDOwn value
        anim.SetTrigger("shoot");                                           //play shoot animation
    }

    private void ShootBullet()                                              //shoot the bullet
    {
        AudioManager.instance.PlayGun();
        GameObject bulletObj = ObjectPooling.instance.GetBullet();          //get bullet from object pool
        bulletObj.transform.position = bulletSpawnPos.position;             //set its tranform position to bullet position
        if (lookingRight)                                                   //moving right
            bulletObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));  //set bullet rotation
        else if (!lookingRight)                                             //moving left
            bulletObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));

        bulletObj.SetActive(true);
        bulletObj.GetComponent<BulletScript>().DestroyPlayer = false;       //tell whom to destroy "player or enemy"
        bulletObj.GetComponent<BulletScript>().DamageAmount = gunDamage;
        bulletObj.GetComponent<DeactivateObject>().BasicSettings(bulletLifeSpan);       //called its basic settings
    }

    private void Jump()                                                     //jump method
    {
        if (isGrounded)                                                     //if grounded
        {
            transform.parent = null;                                        //set parent to null
            isGrounded = false;                                             //grounded to false
            myBody.velocity = new Vector2(myBody.velocity.x, 0f);           //set y velocity to zero
            myBody.velocity = new Vector2(myBody.velocity.x, jumpForce);    //set y velocity to jump force
        }
    }

    void HandleMovement(float horizontal)                                   //handle movement
    {
        myBody.velocity = new Vector3(horizontal * speed, myBody.velocity.y);   //set the velocity
        if (CameraFollow.instance != null)                                  //if camerafollow is not null
        {   //clamp the x value
            x = Mathf.Clamp(transform.position.x, CameraFollow.instance.minPos.x - 6f, CameraFollow.instance.maxPos.x + 6f);
            transform.position = new Vector3(x, transform.position.y, transform.position.z);    //set the player x value
        }
    }

    void HandleFlip(float horizontal)                                       //handle the flip                    
    {
        if (horizontal > 0.1f)                                              //if horizontal value is more than 0.1f
        {
            lookingRight = true;                                            //we look right
            transform.localScale = new Vector3(1, transform.localScale.y);  //set the scale
        }
        else if (horizontal < -0.1f)                                        //if horizontal value is less than -0.1f
        {
            lookingRight = false;                                           //we look left
            transform.localScale = new Vector3(-1, transform.localScale.y); //set the scale
        }
    }

    #region Touch Btn

    //used by left and right buttons
    public void BtnMove(float direction)                                    //method for touch inputs
    {
        dirX = direction;                                                   //set the direction
        move = true;                                                        //move to true
    }

    //used by left and right buttons
    public void StopMove()                                                      
    {   
        move = false;
    }

    public void JumpBtn()                                                   //method for touch inputs
    {
        jump = true;                                                        //set jump to true
        jumpPressed = true;
    }

    public void StopJump()
    {
        jumpPressed = false;
    }

    public void Firing(bool _firing)                                        //method for touch inputs
    {
        firing = _firing;                                                   //set the firing value
    }

    #endregion

    void GroundCheck()                                                      //ground check method
    {
        isGrounded = Physics2D.OverlapCircle(ground.transform.position, groundRadius, whatIsGround | whatIsOneWayPlatform);    //we create overlap circle

        if (myBody.velocity.y > 0.1f)                                      //if y velocity is less than 0.1f
        {
            isGrounded = Physics2D.OverlapCircle(ground.transform.position, groundRadius, whatIsGround);    //we create overlap circle
        }

        anim.SetBool("isGrounded", isGrounded);                             //set the animations
    }

    void OneWayPlatformCheck()                                              //OneWayPlatform check method
    {   //create a ray cast
        RaycastHit2D hit = Physics2D.Raycast(ground.transform.position, new Vector2(0, -1), 0.25f, whatIsOneWayPlatform);

        if (hit.collider != null && hit.collider.CompareTag("StaticPlatform"))  //check if it hit collider and its tag is StaticPlatform
        {
            onOneWayPlatform = true;                                        //on onewayplatform is true
        }
        if (hit.collider == null)                                           //if the collider is null
        {
            onOneWayPlatform = false;                                       //on onewayplatform is false

            if (gameObject.layer != LayerMask.NameToLayer("Player"))        //if gameobject layer is not player layer
                StartCoroutine(ChangeLayer());                              //start a Coroutine
        }
    }

    IEnumerator ChangeLayer()                                               //Coroutine which changes layer
    {
        yield return new WaitForSeconds(0.1f);                              //wait for 0.1f sec
        gameObject.layer = LayerMask.NameToLayer("Player");                 //change the layer to player
    }

    private void OnCollisionEnter2D(Collision2D other)                      
    {
        if (other.collider.CompareTag("MovingPlatform"))                    //if colliding with moving platfrom
        {
            transform.parent = other.gameObject.transform;                  //set the parent
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.collider.CompareTag("MovingPlatform"))                   //if colliding with moving platfrom
        {
            transform.parent = null;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (ground != null)
        {
            UnityEditor.Handles.color = Color.green;
            UnityEditor.Handles.DrawWireDisc(ground.position, new Vector3(0, 0, -1), groundRadius);

            UnityEditor.Handles.DrawLine(ground.position, ground.position + new Vector3(0, -1, 0) * 0.25f);
        }
    }
#endif
}
