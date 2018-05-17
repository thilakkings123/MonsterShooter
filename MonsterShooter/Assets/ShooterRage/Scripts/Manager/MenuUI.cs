using UnityEngine;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour {

    public GameObject mapPanel;
    [SerializeField]
    private Sprite soundOnSprites, soundOffSprites;
    [SerializeField]
    private Image soundImg;

    private managerVars vars;

    private void OnEnable()
    {
        vars = Resources.Load<managerVars>("managerVarsContainer");
    }

    // Use this for initialization
    void Start ()
    {
#if AdmobDef
        if (GameManager.instance.canShowAds) AdsManager.instance.ShowBannerAds();
        else if (!GameManager.instance.canShowAds) AdsManager.instance.DestroyBannerAds();
#endif

        //sound button
        if (GameManager.instance.isMusicOn == true)
        {
            AudioListener.volume = 1;
            soundImg.sprite = soundOffSprites;
        }
        else
        {
            AudioListener.volume = 0;
            soundImg.sprite = soundOnSprites;
        }

        AudioManager.instance.PlayMainMusic();
    }

    public void PlayBtn()
    {
        AudioManager.instance.PlayClick();
        mapPanel.SetActive(true);
    }

    public void MoreGamesBtn()
    {
        AudioManager.instance.PlayClick();
        Application.OpenURL(vars.moreGamesUrl);
    }

    public void RateBtn()
    {
        AudioManager.instance.PlayClick();
        Application.OpenURL(vars.rateButtonUrl);
    }

    public void FacebookBtn()
    {
        AudioManager.instance.PlayClick();
        Application.OpenURL(vars.facebookUrl);
    }

    public void TwitterBtn()
    {
        AudioManager.instance.PlayClick();
        Application.OpenURL(vars.twitterUrl);
    }

    public void HomeBtn()
    {
        AudioManager.instance.PlayClick();
        mapPanel.SetActive(false);
    }

    public void SoundBtn()
    {
        AudioManager.instance.PlayClick();
        if (GameManager.instance.isMusicOn == true)
        {
            GameManager.instance.isMusicOn = false;
            AudioListener.volume = 0;
            soundImg.sprite = soundOnSprites;
            GameManager.instance.Save();
        }
        else
        {
            GameManager.instance.isMusicOn = true;
            AudioListener.volume = 1;
            soundImg.sprite = soundOffSprites;
            GameManager.instance.Save();
        }
    }

}
