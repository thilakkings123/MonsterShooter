using UnityEngine;

public class PickUp : MonoBehaviour {

    public enum PickUpType { heart, coin, star, chest } //types of pickups

    public PickUpType pickUpType = PickUpType.coin; //se set default as coin

    private void OnTriggerEnter2D(Collider2D col)   //on collision
    {
        if (col.CompareTag("Player"))   //if its player
        {
            switch (pickUpType)
            {
                case PickUpType.heart:  //if its heart
                    //we check if current health is less than max health
                    if (col.GetComponent<DamageScript>().CurrentHealth < col.GetComponent<DamageScript>().MaxHealth)
                    {
                        AudioManager.instance.PlayHealth();
                        col.GetComponent<DamageScript>().IncreaseHealth(5); //we increase it by 5 (5 is max player can have after all upgrades)
                        gameObject.SetActive(false); //deactivet the gameobject
                    }
                    break;

                case PickUpType.coin:                   //if its coin
                    AudioManager.instance.PlayCoin();
                    GameManager.instance.coins++;       //we increase coin by 1
                    GameManager.instance.Save();        //we save it
                    GameManager.instance.coinsEarned++; //we increase by 1
                    GameUI.instance.UpdateCoins();      //update the texts
                    gameObject.SetActive(false);        //deactive gameobject
                    break;

                case PickUpType.star:                   //if its star
                    AudioManager.instance.PlayStar();
                    GameUI.instance.StarEarned++;       //we increase it by 1
                    gameObject.SetActive(false);        //deactive it
                    break;

                case PickUpType.chest:                  //if its chest
                    AudioManager.instance.PlayCoin();
                    GameManager.instance.coins += 50;   //increase coin by 50
                    GameManager.instance.Save();        //save it
                    GameManager.instance.coinsEarned += 50; //update coins earned
                    GameUI.instance.UpdateCoins();
                    gameObject.SetActive(false);
                    break;
            }
            
        }
    }
}
