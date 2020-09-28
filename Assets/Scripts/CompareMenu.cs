using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompareMenu : MonoBehaviour
{
    public GameObject Venus;
    public GameObject Earth;
    public GameObject Mars;
    public GameObject Jupiter;
    public GameObject Saturn;
    public Interactivity InteractivityInstance;
    public AudioSource UIAudioPlayer;
    public AudioClip Button;
    public void ChooseVenus()
    {
        UIAudioPlayer.Stop();
        UIAudioPlayer.PlayOneShot(Button);
        InteractivityInstance.RenderCompareEntity(Venus);
    }
    public void ChooseEarth()
    {
        UIAudioPlayer.Stop();
        UIAudioPlayer.PlayOneShot(Button);
        InteractivityInstance.RenderCompareEntity(Earth);
    }
    public void ChooseMars()
    {
        UIAudioPlayer.Stop();
        UIAudioPlayer.PlayOneShot(Button);
        InteractivityInstance.RenderCompareEntity(Mars);
    }
    public void ChooseJupiter()
    {
        UIAudioPlayer.Stop();
        UIAudioPlayer.PlayOneShot(Button);
        InteractivityInstance.RenderCompareEntity(Jupiter);
    }
    public void ChooseSaturn()
    {
        UIAudioPlayer.Stop();
        UIAudioPlayer.PlayOneShot(Button);
        InteractivityInstance.RenderCompareEntity(Saturn);
    }
}
