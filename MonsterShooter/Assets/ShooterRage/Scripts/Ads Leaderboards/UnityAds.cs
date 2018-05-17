using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

#if UNITY_ADS
using UnityEngine.Advertisements;
#endif

public class UnityAds : MonoBehaviour
{

    public static UnityAds instance;

    private int i = 0;
    private bool rewardAdReady = false;

    private bool doubleCoins = false;

    public bool RewardAdReady
    {
        get { return rewardAdReady; }
    }

    [HideInInspector]
    public managerVars vars;

    void OnEnable()
    {
        vars = Resources.Load<managerVars>("managerVarsContainer");
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    // Use this for initialization
    void Start()
    {
        i = 0;
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
#if UNITY_ADS
        if (Advertisement.IsReady("rewardedVideo"))
        {
            rewardAdReady = true;
        }
        else if (!Advertisement.IsReady("rewardedVideo"))
        {
            rewardAdReady = false;
        }
#endif
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance == null)
            return;

        if (GameManager.instance.playerDead == true || GameManager.instance.levelComplete)
        {
            //we want only one ad to be shown so we put condition that when i is 0 we show ad.
            if (i == 0)
            {
                i++;
                GameManager.instance.gamesPlayed++;

                if (GameManager.instance.gamesPlayed >= vars.showInterstitialAfter)
                {
                    GameManager.instance.gamesPlayed = 0;
                    //use any one of them
                    //admob ads
#if AdmobDef
                    AdsManager.instance.ShowInterstitial();

#elif UNITY_ADS     //unity ads     
                    if(!vars.admobActive)
                        ShowUnityAd();
#endif
                }
            }
        }
    }

    public void ShowAds()
    {
#if AdmobDef
                    AdsManager.instance.ShowInterstitial();

#elif UNITY_ADS     //unity ads     
                    if(!vars.admobActive)
                        ShowUnityAd();
#endif
    }

    public void ShowUnityAd()
    {
#if UNITY_ADS
        if (Advertisement.IsReady())
        {
            Advertisement.Show();
        }
#endif
    }

    //use this function for showing reward ads
    public void ShowRewardedAd()
    {
#if UNITY_ADS
        if (Advertisement.IsReady("rewardedVideo"))
        {
            var options = new ShowOptions { resultCallback = HandleShowResult };
            Advertisement.Show("rewardedVideo", options);
        }
        else
        {
            Debug.Log("Ads not ready");
        }
#endif
    }

#if UNITY_ADS
    private void HandleShowResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                Debug.Log("The ad was successfully shown.");
                GameManager.instance.points += GameManager.instance.currentPoints;
                GameManager.instance.currentPoints *= 2; /*here we give double points as reward*/
                GameUI.instance.gameOverUI.coinText.text = "" + GameManager.instance.currentPoints;
                GameManager.instance.Save();
                break;
            case ShowResult.Skipped:
                Debug.Log("The ad was skipped before reaching the end.");
                break;
            case ShowResult.Failed:
                Debug.LogError("The ad failed to be shown.");

                break;
        }
    }
#endif

}
