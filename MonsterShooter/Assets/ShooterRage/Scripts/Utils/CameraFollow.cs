using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    public static CameraFollow instance;

    private Vector2 velocity;

    public float smoothTimeX = 0.05f;
    public float smoothTimeY = 0.05f;

    [Header("Camera Bounds")]
    public Vector2 minPos;
    public Vector2 maxPos;

    private GameObject player; //ref to the player object in the scene
    private Vector3 distance; //distance between camera and player
    private bool playerGot = false;   //check if player is available

    void Awake()
    {
        if (instance == null)
            instance = this;
        PlayerSettings();
    }

    //method which will be called by player spawns
    public void PlayerSettings()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        //gets the distance between player and camera , we need this distance so to maintain it in the game
        distance = (player.transform.position - transform.position);
        playerGot = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (player == null)
            return;

        Movement();
    }

    void Movement()
    {   //decide the x value
        float posX = Mathf.SmoothDamp(transform.position.x, player.transform.position.x, ref velocity.x, smoothTimeX);
        float posY = Mathf.SmoothDamp(transform.position.y, player.transform.position.y + 1f, ref velocity.y, smoothTimeY);
        //set the x value of camera

        posX = Mathf.Clamp(player.transform.position.x, minPos.x, maxPos.x);
        posY = Mathf.Clamp(player.transform.position.y + 1f, minPos.y, maxPos.y);

        transform.position = new Vector3(posX, posY, transform.position.z);

    }

}
