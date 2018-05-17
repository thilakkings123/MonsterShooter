using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    private GameData data;

    public int totalLevel, totalCoins;

    [Header("Dont modify below variables")]
    public bool levelComplete;
    public int currentLevel = 0; //this is used to unlock levels because the array start from zero
    //this is for change from one level to another when player completes level , it start from level 1
    public int currentLevelNumber = 0;
    public bool playerDead = false;
    public int coinsEarned = 0, gamesPlayed;

    public bool isGameStartedFirstTime;
    public int coins;
    public int health, gunDamage, speed, gunFireRate;
    public bool canShowAds;
    public bool isMusicOn;
    public bool isSoundOn;
    public bool[] levels; // to keep track on levels

    void Awake()
    {
        MakeSingleton();
        InitializeGameVariables();
    }

    void MakeSingleton()
    {
        //this state that if the gameobject to which this script is attached , if it is present in scene then destroy the new one , and if its not present
        //then create new 
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void InitializeGameVariables()
    {
        Load();
        if (data != null)
        {
            isGameStartedFirstTime = data.getIsGameStartedFirstTime();
        }
        else
        {
            isGameStartedFirstTime = true;
        }

        if (isGameStartedFirstTime)
        {
            isGameStartedFirstTime = false;
            isMusicOn = true;
            isSoundOn = true;
            canShowAds = true;
            coins = totalCoins;

            health = 1;
            speed = 1;
            gunDamage = 1;
            gunFireRate = 1;

            levels = new bool[totalLevel];

            levels[0] = true;
            for (int i = 1; i < levels.Length; i++)
            {
                levels[i] = false;
            }

            data = new GameData();

            data.setIsGameStartedFirstTime(isGameStartedFirstTime);
            data.setLevels(levels);
            data.setMusicOn(isMusicOn);
            data.setSoundOn(isSoundOn);
            data.setCanShowAds(canShowAds);
            data.setCoins(coins);
            data.setHealth(health);
            data.setSpeed(speed);
            data.setGunDamage(gunDamage);
            data.setGunFireRate(gunFireRate);

            Save();

            Load();
        }
        else
        {
            isGameStartedFirstTime = data.getIsGameStartedFirstTime();
            isMusicOn = data.getMusicOn();
            isSoundOn = data.getSoundOn();
            levels = data.getLevels();
            coins = data.getCoins();
            canShowAds = data.getCanShowAds();
            health = data.getHealth();
            speed = data.getSpeed();
            gunDamage = data.getGunDamage();
            gunFireRate = data.getGunFireRate();
        }
    }


    //                              .........this function take care of all saving data like score , current player , current weapon , etc
    public void Save()
    {
        FileStream file = null;
        //whicle working with input and output we use try and catch
        try
        {
            BinaryFormatter bf = new BinaryFormatter();

            file = File.Create(Application.persistentDataPath + "/GameData.dat");

            if (data != null)
            {
                data.setIsGameStartedFirstTime(isGameStartedFirstTime);
                data.setLevels(levels);
                data.setMusicOn(isMusicOn);
                data.setSoundOn(isSoundOn);
                data.setCanShowAds(canShowAds);
                data.setCoins(coins);
                data.setHealth(health);
                data.setSpeed(speed);
                data.setGunDamage(gunDamage);
                data.setGunFireRate(gunFireRate);

                bf.Serialize(file, data);
            }
        }
        catch (Exception e)
        {
        }
        finally
        {
            if (file != null)
            {
                file.Close();
            }
        }


    }
    //                            .............here we get data from save
    public void Load()
    {
        FileStream file = null;
        try
        {
            BinaryFormatter bf = new BinaryFormatter();
            file = File.Open(Application.persistentDataPath + "/GameData.dat", FileMode.Open);
            data = (GameData)bf.Deserialize(file);

        }
        catch (Exception e)
        {
        }
        finally
        {
            if (file != null)
            {
                file.Close();
            }
        }
    }

    public void ResetGameManager()
    {
        isGameStartedFirstTime = true;
        isMusicOn = true;
        isSoundOn = true;
        canShowAds = true;
        coins = totalCoins;

        health = 1;
        speed = 1;
        gunDamage = 1;
        gunFireRate = 1;

        levels = new bool[totalLevel];

        levels[0] = true;
        for (int i = 1; i < levels.Length; i++)
        {
            levels[i] = false;
        }

        data = new GameData();

        data.setIsGameStartedFirstTime(isGameStartedFirstTime);
        data.setLevels(levels);
        data.setMusicOn(isMusicOn);
        data.setSoundOn(isSoundOn);
        data.setCanShowAds(canShowAds);
        data.setCoins(coins);
        data.setHealth(health);
        data.setSpeed(speed);
        data.setGunDamage(gunDamage);
        data.setGunFireRate(gunFireRate);

        Save();

        Load();
    }

}

[Serializable]
class GameData
{
    private bool isGameStartedFirstTime;

    private int coins;
    private int health, gunDamage, speed, gunFireRate;
    private bool isMusicOn;
    public bool isSoundOn;
    private bool canShowAds;
    private bool[] levels; //this keep track of which level is locked and which is not

    public void setCoins(int coins)
    {
        this.coins = coins;
    }

    public int getCoins()
    {
        return coins;
    }

    public void setHealth(int health)
    {
        this.health = health;
    }

    public int getHealth()
    {
        return health;
    }

    public void setSpeed(int speed)
    {
        this.speed = speed;
    }

    public int getSpeed()
    {
        return speed;
    }

    public void setGunDamage(int gunDamage)
    {
        this.gunDamage = gunDamage;
    }

    public int getGunDamage()
    {
        return gunDamage;
    }

    public void setGunFireRate(int gunFireRate)
    {
        this.gunFireRate = gunFireRate;
    }

    public int getGunFireRate()
    {
        return gunFireRate;
    }

    public void setCanShowAds(bool canShowAds)
    {
        this.canShowAds = canShowAds;
    }

    public bool getCanShowAds()
    {
        return this.canShowAds;
    }

    public void setIsGameStartedFirstTime(bool isGameStartedFirstTime)
    {
        this.isGameStartedFirstTime = isGameStartedFirstTime;

    }

    public bool getIsGameStartedFirstTime()
    {
        return this.isGameStartedFirstTime;

    }
    //                                                                    ...............music
    public void setMusicOn(bool isMusicOn)
    {
        this.isMusicOn = isMusicOn;

    }

    public bool getMusicOn()
    {
        return this.isMusicOn;

    }
    //                                                                      .......music

    //                                                                    ...............Sound
    public void setSoundOn(bool isSoundOn)
    {
        this.isSoundOn = isSoundOn;

    }

    public bool getSoundOn()
    {
        return this.isSoundOn;

    }
    //                                                                      .......Sound

    //                                                                       ..................Level locked/unlocked
    public void setLevels(bool[] levels)
    {
        this.levels = levels;

    }

    public bool[] getLevels()
    {
        return this.levels;

    }
    //                                                                       ..................Level locked/unlocked


}
