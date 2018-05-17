using UnityEngine;

public class DeactivateObject : MonoBehaviour {

    private float currentTime;  //to track time

    public void BasicSettings(float _time)  //set the time , called by other scripts
    {
        currentTime = _time;
    }

    private void Update()   
    {
        if (currentTime > 0)    //if time is more than zero
            currentTime -= Time.deltaTime;  //reduce it
        else if (currentTime <= 0)  //if its less or equal to zero
            Deactivate();   //deactivate
    }

    void Deactivate()
    {
        gameObject.SetActive(false);    //set it to false
    }
}
