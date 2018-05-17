using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour {

    int currentPage;                                        //ref to current page

    [SerializeField] private Color unlockColor, lockColor;  //ref to color
    [SerializeField] private int totalLevels;               //total levels (must be same as GameManager)

    [SerializeField][Header("11 levels each page")]
    private int maxPage;                                    //set it properly

    [SerializeField] private GameObject[] levelItems;       //ref to the buttons
    [SerializeField] private GameObject leftBtn, rightBtn;  //ref to left and right btn
    [SerializeField] private bool       unlockAllLevels = false;

	// Use this for initialization
	void Start ()
    {
        currentPage = 0;                                    //we start at zero page
        LoadPageInfo();                                     //load the page
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (currentPage == 0)                               //if current page is zero
        {
            leftBtn.SetActive(false);                       //deactive left btn
            rightBtn.SetActive(true);                       //active right btn
        }
        if (currentPage == maxPage - 1)                     //if we are on last page
        {
            rightBtn.SetActive(false);                      //active left btn
            leftBtn.SetActive(true);                        //deactive right btn
        }
        if (currentPage > 0 && currentPage < maxPage - 1)   //if we are in middle
        {
            leftBtn.SetActive(true);                        //active left btn
            rightBtn.SetActive(true);                       //active right btn
        }
    }

    void LoadPageInfo()
    {
        for (int i = 0; i < levelItems.Length; i++)         //we loop through all the level items length
        {
            levelItems[i].SetActive(false);                 //and deactivet them
        }

        for (int i = 0; i < levelItems.Length; i++)         //we loop through all the level items length
        {
            if (GameManager.instance.levels.Length > (i + currentPage * 11)) //if levels length of gamemanager is more
            {
                bool unlocked = GameManager.instance.levels[i + currentPage * 11];  //we check if level is unlocked
                levelItems[i].SetActive(true);              //set it active
                levelItems[i].transform.GetChild(0).GetComponent<Text>().text = (currentPage * 11 + i + 1).ToString();  //set the level number

                if (unlocked)                                                       //if unlocked we set its color to unlock color
                    levelItems[i].GetComponent<Image>().color = unlockColor;
                else if (!unlocked)                                                 //if locked we set its color to lock color
                    levelItems[i].GetComponent<Image>().color = lockColor;
            }
            else
                return;

        }
    }

    public void NextPage()              //next page button
    {
        if (currentPage < maxPage)      //if current page is less than max page
            currentPage++;              //we increase current page by 1
        LoadPageInfo();                 //load the page info
    }

    public void PreviosPage()           //Previous page button
    {
        if (currentPage > 0)            //if current page is more than 0
            currentPage--;              //we decrease current page by 1
        LoadPageInfo();                 //load the page info
    }

    public void GoToLevel(int _index)   //method called by level buttons
    {
        if (unlockAllLevels)
        {
            GameManager.instance.currentLevelNumber = _index + 1 + currentPage * 11;    //set current level number
            SceneManager.LoadScene("Level_" + GameManager.instance.currentLevelNumber); //load the level
            return;
        }

        bool unlocked = GameManager.instance.levels[_index + currentPage * 11]; //we check fro unlocked

        if (unlocked)                   //if unlocked
        {
            GameManager.instance.currentLevel = _index + currentPage * 11;              //we set current level
            GameManager.instance.currentLevelNumber = _index + 1 + currentPage * 11;    //set current level number
            SceneManager.LoadScene("Level_" + GameManager.instance.currentLevelNumber); //load the level
        }
    }
}
