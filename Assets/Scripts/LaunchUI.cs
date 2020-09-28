using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LaunchUI : MonoBehaviour
{
    public AudioSource UIAudioPlayer;
    public GameObject HelpScreen;
    public AudioClip Button;
    public AudioClip MenuPopUp;
    public void Help()
    {
        UIAudioPlayer.Stop();
        UIAudioPlayer.PlayOneShot(Button);
        UIAudioPlayer.PlayOneShot(MenuPopUp);
        HelpScreen.SetActive(true);
    }
    public void Home()
    {
        UIAudioPlayer.Stop();
        UIAudioPlayer.PlayOneShot(Button);
        HelpScreen.SetActive(false);
    }
    public void StartExperience()
    {
        UIAudioPlayer.Stop();
        UIAudioPlayer.PlayOneShot(Button);
        SceneManager.LoadScene("ARExperienceScene");
    }
}
