using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour {

    public static GameUI instance;

    [SerializeField]    //ref to text
    private Text gameCoinText, levelClearedCoinText, levelFailedIndexText, levelClearedIndexText, instructionsText;
    [SerializeField] private Image healthBar;                                                           //ref to health bar image
    [SerializeField] private Image[]    stars;                                                          //ref to star images
    [SerializeField] private Animator   pausePanel, shopPanel, levelFailedPanel, levelClearedPanel;     //ref to animators
    [SerializeField] private GameObject coinShopPanel, rewardAdsPanel;                                  //ref to gameobjects

    private GameObject       nextLevelBtn;                                                              //ref to nextLevelBtn
    private int              starEarned;                                                                //ref tp starEarned
    private PlayerController player;                                                                    //ref to player
    private float           screenShotTime = 10;
    private bool            screenShotTaken = false;
    private Button          shareBtn;
    private Image           screenShotImg;

    //getter and setter
    public int  StarEarned           { get { return starEarned; }           set { starEarned = value; } }
    public Text LevelClearedCoinText { get { return levelClearedCoinText; } set { levelClearedCoinText = value; } }
    public Image ScreenShotImg       { get { return screenShotImg; }        set { screenShotImg = value; } }

    private managerVars vars;

    private void OnEnable()
    {
        vars = Resources.Load<managerVars>("managerVarsContainer");
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    // Use this for initialization
    void Start ()
    {
#if AdmobDef
        if (GameManager.instance.canShowAds) AdsManager.instance.HideBannerAds();               //if we can show ads , show ads
#endif

        //sound button
        if (GameManager.instance.isMusicOn == true)
        {
            AudioListener.volume = 1;
        }
        else
        {
            AudioListener.volume = 0;
        }

        nextLevelBtn = GameObject.Find("NextButton");                                           //get the next btn
        screenShotImg = GameObject.Find("ScreenShotImage").GetComponent<Image>();
        shareBtn = GameObject.Find("ShareButton").GetComponent<Button>();
        if (shareBtn != null)
            shareBtn.onClick.AddListener(() => { ShareBtn(); });
        GameManager.instance.gamesPlayed++;                                                     //increase game player by 1
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();   //get reference to player

        GameManager.instance.coinsEarned = 0;                                                   //set coins earned to zero
        gameCoinText.text = "" + GameManager.instance.coinsEarned;                              //set the text
        AudioManager.instance.PlayGameMusic();
    }

    private void Update()
    {
        if (screenShotTaken == false)
        {
            screenShotTime -= Time.deltaTime;
            if (screenShotTime <= 0)
            {
                screenShotTaken = true;
                ShareScreenShot.instance.TakeScreenshot();
            }
        }
    }

    #region Game UI
    public void PauseBtn()                                                                      //pause btn
    {
        AudioManager.instance.PlayClick();
        pausePanel.SetBool("paused", true);                                                     //play pause panel animation
        StartCoroutine(Paused());                                                               
    }

    IEnumerator Paused()
    {
        yield return new WaitForSeconds(0.5f);                                                  //after 0.5f sec
        Time.timeScale = 0;                                                                     //set timescale to zero
#if AdmobDef
        if (GameManager.instance.canShowAds) AdsManager.instance.ShowBannerAds();               //if we can show ads , show ads
#endif
    }

    public void ShopBtn()                                                                       //shop button
    {
#if AdmobDef
        if (GameManager.instance.canShowAds) AdsManager.instance.HideBannerAds();               //hide banner ads
#endif
        AudioManager.instance.PlayClick();
        shopPanel.SetBool("shopActive", true);                                                  //play shop panel animation
        StartCoroutine(ShopActive());                               
    }

    IEnumerator ShopActive()
    {
        yield return new WaitForSeconds(0.5f);                                                  //after 0.5f sec
        Time.timeScale = 0;                                                                     //set timescale to zero
    }       

    public void ShopCloseBtn()                                                                  //shop close button
    {
#if AdmobDef
        if (GameManager.instance.canShowAds) AdsManager.instance.ShowBannerAds();               
        else if (!GameManager.instance.canShowAds) AdsManager.instance.DestroyBannerAds();
#endif
        AudioManager.instance.PlayClick();
        shopPanel.SetBool("shopActive", false);                     
        Time.timeScale = 1;                                                                     //set timescale to 1
    }

    public void CoinShopBtn()                                                                   //coin shop button
    {
        AudioManager.instance.PlayClick();
        coinShopPanel.SetActive(true);                                                          //active coin shop 
    }

    public void CoinShopCloseBtn()                                                              //coin shop close button
    {
        AudioManager.instance.PlayClick();
        coinShopPanel.SetActive(false);                                                         //deactive coin shop 
    }

    #endregion

    #region Pause UI
    public void ContinueBtn(bool inShop)                                                        //continue button
    {
#if AdmobDef
        if (GameManager.instance.canShowAds) AdsManager.instance.HideBannerAds();               //hide banner ads
#endif
        AudioManager.instance.PlayClick();
        Time.timeScale = 1;
        if (inShop)
            shopPanel.SetBool("shopActive", true);
        else
            pausePanel.SetBool("paused", false);
    }

    public void MainMenuBtn()                                                                   //main menu button
    {
        AudioManager.instance.PlayClick();
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1;
    }
    #endregion

    #region Level UI

    public void RetryBtn()                                                                      //retry button
    {
        AudioManager.instance.PlayClick();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);                       //load the active scene
    }

    public void NextLevelBtn()                                                                  //next level button
    {
        AudioManager.instance.PlayClick();
        int nextLevel = GameManager.instance.currentLevelNumber + 1;                            //get the next level number
        GameManager.instance.currentLevel++;
        GameManager.instance.currentLevelNumber++;
        SceneManager.LoadScene("Level_" + nextLevel);                                           //load the scene
    }

    public void CloseRewardAds()                                                                //cloase reward ads panel
    {
        AudioManager.instance.PlayClick();
        rewardAdsPanel.SetActive(false);    
    }

    void ShareBtn()
    {
        AudioManager.instance.PlayClick();
        ShareScreenShot.instance.ShareMethod();
    }

    public void RewardAds()
    {
        AudioManager.instance.PlayClick();
#if UNITY_ADS
        UnityAds.instance.ShowRewardedAd();
#endif
    }

#endregion

    public void LevelFailed()                                                               //level failed method
    {
        StartCoroutine(LevelFailedCoroutine());
    }

    IEnumerator LevelFailedCoroutine()              
    {
        yield return new WaitForSeconds(1f);                                                //wait for 1 sec
        levelFailedIndexText.text = "Level " + GameManager.instance.currentLevelNumber;     //set the text
        levelFailedPanel.SetBool("levelFailed", true);                                      //play level failed panel animation
#if AdmobDef
        if (GameManager.instance.canShowAds) AdsManager.instance.ShowBannerAds();               //if we can show ads , show ads
#endif
    }

    public void LevelCleared()
    {
        if (GameManager.instance.currentLevelNumber == GameManager.instance.levels.Length)  //if current level is last level
            nextLevelBtn.SetActive(false);                                                  //deactive next level btn

        StartCoroutine(LevelClearedCoroutine());
    }

    IEnumerator LevelClearedCoroutine()
    {
        yield return new WaitForSeconds(0.25f);                                             //wait for 0.25 sec
        if (UnityAds.instance.RewardAdReady) rewardAdsPanel.SetActive(true);                //if reward ads is ready , active reward ads panel
        GameManager.instance.levels[GameManager.instance.currentLevel + 1] = true;          //next level unlocked
        GameManager.instance.Save();                                                        //save
        levelClearedIndexText.text = "Level " + GameManager.instance.currentLevelNumber;    //set the text
        levelClearedPanel.SetBool("levelCleared", true);                                    //play level cleared panel animation     
        StartCoroutine(StarEarnedCoroutine());
        ShareScreenShot.instance.GetPhoto();
#if AdmobDef
        if (GameManager.instance.canShowAds) AdsManager.instance.ShowBannerAds();               //if we can show ads , show ads
#endif
    }

    IEnumerator StarEarnedCoroutine()
    {
        yield return new WaitForSeconds(0.55f);                                             //wait for 0.55 sec
        for (int i = 0; i < starEarned; i++)                                                //loop through total stars earned
        {
            yield return new WaitForSeconds(0.15f);                                         //wait for 0.15 sec
            stars[i].color = Color.white;                                                   //set its color to white
        }
    }

    public void UpdateCoins()                                                               //update coin
    {
        gameCoinText.text = "" + GameManager.instance.coinsEarned;                          //update text
    }

    public void ShowInstructions(string value)
    {
        instructionsText.gameObject.SetActive(true);
        instructionsText.text = value;
    }

    public void HideInstructions()
    {
        instructionsText.gameObject.SetActive(false);
    }
}
