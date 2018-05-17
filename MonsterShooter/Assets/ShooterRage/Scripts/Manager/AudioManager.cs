using UnityEngine;

public class AudioManager : MonoBehaviour {

    public static AudioManager instance;

    [SerializeField]
    private AudioSource playerMusic, backgroundMusic;
    [SerializeField]
    private AudioClip gun, coin, button, star, zombieDie, playerDie, health, boom;
    [SerializeField]
    private AudioClip menuMusic, gameMusic;

    private void Awake()
    {
        if (instance != null) Destroy(gameObject);
        else if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void PlayGun()
    {
        playerMusic.PlayOneShot(gun);
    }

    public void PlayClick()
    {
        playerMusic.PlayOneShot(button);
    }

    public void PlayCoin()
    {
        playerMusic.PlayOneShot(coin);
    }

    public void PlayStar()
    {
        playerMusic.PlayOneShot(star);
    }

    public void PlayBoom()
    {
        playerMusic.PlayOneShot(boom);
    }

    public void PlayHealth()
    {
        playerMusic.PlayOneShot(health);
    }

    public void PlayZombieDie()
    {
        playerMusic.PlayOneShot(zombieDie);
    }

    public void PlayPlayerDie()
    {
        playerMusic.PlayOneShot(playerDie);
    }

    public void PlayMainMusic()
    {
        backgroundMusic.Stop();
        backgroundMusic.clip = menuMusic;
        backgroundMusic.Play();
    }

    public void PlayGameMusic()
    {
        backgroundMusic.Stop();
        backgroundMusic.clip = gameMusic;
        backgroundMusic.Play();
    }

    public void OffSound()
    {
        playerMusic.mute = true;
        backgroundMusic.mute = true;
    }

    public void OnSound()
    {
        playerMusic.mute = false;
        backgroundMusic.mute = false;
    }

}
