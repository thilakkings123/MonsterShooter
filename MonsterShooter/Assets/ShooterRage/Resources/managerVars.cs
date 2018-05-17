using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class managerVars : ScriptableObject {

    // Standart Vars
    [SerializeField]
    public string adMobInterstitialID, adMobBannerID, admobAppID, rateButtonUrl, leaderBoardID, moreGamesUrl, facebookUrl, twitterUrl;
	[SerializeField]
	public int showInterstitialAfter, bannerAdPoisiton;
    [SerializeField]
    public bool admobActive, googlePlayActive, unityIAP;
}
