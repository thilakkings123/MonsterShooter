using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Elements               //element class
{   
    public string elementName;      //element name
    public Text costText;           //cost text
    public Image[] levels;          //levels images
}

public class ShopManager : MonoBehaviour {

    public static ShopManager instance;

    [SerializeField] private GameObject playerPanel, gunPanel;          //ref to playerPanel and gun panel
    [SerializeField] private Button     removeAdsBtn;                   //ref to remove ads btn
    [SerializeField] private Text       shopCoinText, coinShopCoinText; //ref to shop coin text and coinshop coin text

    [SerializeField] private Elements[] elements;                       //elements array

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    // Use this for initialization
    void Start ()
    {
        gunPanel.SetActive(false);                                      //set gun panel deactive
        playerPanel.SetActive(true);                                    //set player panel deactive
        SetElementsLevels();                                            //set the element levels

        UpdateCoins();                                                  //update coin
        UpdateElementCost();

        if (!GameManager.instance.canShowAds)                           //if we cannot show ads (means we have purchase remove ads)
            removeAdsBtn.interactable = false;                          //make remove ads btn interactable false
    }

    public void UpgradeBtn(string element)                              //upgrade btn
    {
        AudioManager.instance.PlayClick();
        if (element == "Health")                                        //if element is health      
        {
            if (GameManager.instance.coins >= (100 * GameManager.instance.health))  //if we have enough coins
            {
                if (GameManager.instance.health < 5)                    //if health is less than 5
                {
                    GameManager.instance.coins -= 100 * GameManager.instance.health;                //we reduce the coins
                    //GameUI.instance.LevelClearedCoinText.text = "" + GameManager.instance.coins;  //update coin text
                    GameManager.instance.health++;                                                  //increase health
                    PlayerController.instance.UpdateHealth();                                       //update health
                    GameManager.instance.Save();                                                    //save
                    UpdateCoins();                                                                  //update coins
                }
            }
            else
            {
                //animate get coin btn
            }
        }
        else if (element == "Speed")                                    //if its speed
        {
            if (GameManager.instance.coins >= (100 * GameManager.instance.speed))   //if we have enough coins
            {
                if (GameManager.instance.speed < 5)                     //if speed is less than 5
                {
                    GameManager.instance.coins -= 100 * GameManager.instance.speed;                 //we reduce the coins
                    //GameUI.instance.LevelClearedCoinText.text = "" + GameManager.instance.coins;
                    GameManager.instance.speed++;                                                   //increase speed
                    PlayerController.instance.Speed += 0.5f;                                        //increase player speed
                    GameManager.instance.Save();                                                    //save
                    UpdateCoins();                                                                   //update coins
                }
            }
            else
            {
                //animate get coin btn
            }
        }
        else if (element == "Damage")
        {
            if (GameManager.instance.coins >= (100 * GameManager.instance.gunDamage))
            {
                if (GameManager.instance.gunDamage < 5)
                {
                    GameManager.instance.coins -= 100 * GameManager.instance.gunDamage;
                    //GameUI.instance.LevelClearedCoinText.text = "" + GameManager.instance.coins;
                    GameManager.instance.gunDamage++;
                    PlayerController.instance.GunDamage = GameManager.instance.gunDamage;
                    GameManager.instance.Save();
                    UpdateCoins();
                }
            }
            else
            {
                //animate get coin btn
            }
        }
        else if (element == "FireRate")
        {
            if (GameManager.instance.coins >= (100 * GameManager.instance.gunFireRate))
            {
                if (GameManager.instance.gunFireRate < 5)
                {
                    GameManager.instance.coins -= 100 * GameManager.instance.gunFireRate;
                    //GameUI.instance.LevelClearedCoinText.text = "" + GameManager.instance.coins;
                    GameManager.instance.gunFireRate++;
                    PlayerController.instance.GunFireRate -= 0.2f;
                    if (PlayerController.instance.GunFireRate < 0.2f)
                        PlayerController.instance.GunFireRate = 0.2f;
                    GameManager.instance.Save();
                    UpdateCoins();
                }
            }
            else
            {
                //animate get coin btn
            }
        }

        SetElementsLevels();
        UpdateElementCost();
    }

    private void UpdateElementCost()
    {
        elements[0].costText.text = "" + 100 * GameManager.instance.health;
        elements[1].costText.text = "" + 100 * GameManager.instance.speed;
        elements[2].costText.text = "" + 100 * GameManager.instance.gunDamage;
        elements[3].costText.text = "" + 100 * GameManager.instance.gunFireRate;
    }

    private void SetElementsLevels()
    {
        for (int i = 0; i < GameManager.instance.health; i++)   //we loop through number of health
        {
            elements[0].levels[i].enabled = true;               //adn set the number of element active
        }

        for (int i = 0; i < GameManager.instance.speed; i++)
        {
            elements[1].levels[i].enabled = true;
        }

        for (int i = 0; i < GameManager.instance.gunDamage; i++)
        {
            elements[2].levels[i].enabled = true;
        }

        for (int i = 0; i < GameManager.instance.gunFireRate; i++)
        {
            elements[3].levels[i].enabled = true;
        }
    }

    public void UpdateCoins()
    {
        coinShopCoinText.text = "" + GameManager.instance.coins;        //update coin shop coin text
        shopCoinText.text = "" + GameManager.instance.coins;            //update shop coin text
    }

    public void ActivatePlayerPanel()                                   //button to activate player panel
    {
        AudioManager.instance.PlayClick();
        gunPanel.SetActive(false);                                      //deactivate gun panel
        playerPanel.SetActive(true);                                    //activate player panel
    }

    public void ActivateGunPanel()                                      //button to activate gun panel
    {
        AudioManager.instance.PlayClick();
        gunPanel.SetActive(true);                                       //activate gun panel
        playerPanel.SetActive(false);                                   //deactivate player panel
    }

    #region Coin Shop                                                   //coin shop buttons

    public void Buy250Coins()
    {
        AudioManager.instance.PlayClick();
        Purchaser.instance.Buy250Coins();
    }

    public void Buy600Coins()
    {
        AudioManager.instance.PlayClick();
        Purchaser.instance.Buy600Coins();
    }

    public void Buy1500Coins()
    {
        AudioManager.instance.PlayClick();
        Purchaser.instance.Buy1500Coins();
    }

    public void RemoveAds()
    {
        AudioManager.instance.PlayClick();
        Purchaser.instance.BuyNoAds();
    }

    #endregion

}
