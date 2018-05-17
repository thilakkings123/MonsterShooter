using UnityEngine;

public class ExitDoor : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D collision) //detect collision
    {
        if (collision.CompareTag("Player"))             //if tag is player
        {
            if (GameManager.instance.levelComplete == false)    //if levelcomplete is false
            {
                GameManager.instance.levelComplete = true;      //if levelcomplete is true
                GameUI.instance.LevelCleared();                 //call level cleared method
            }
        }
    }
}
