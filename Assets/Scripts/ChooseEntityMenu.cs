using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ChooseEntityMenu : MonoBehaviour
{
    public GameObject Venus;
    public GameObject Earth;
    public GameObject Mars;
    public GameObject Jupiter;
    public GameObject Saturn;
    public Camera FirstPersonCamera;
    public Interactivity InteractivityInstance;
    public AudioSource UIAudioPlayer;
    public AudioClip Button;
    public void ChooseVenus()
    {
        UIAudioPlayer.Stop();
        UIAudioPlayer.PlayOneShot(Button);
        InteractivityInstance.PositionToRenderAt = FirstPersonCamera.transform.position + (FirstPersonCamera.transform.forward * 0.5f);
        InteractivityInstance.RenderEntity(Venus);
    }
    public void ChooseEarth()
    {
        UIAudioPlayer.Stop();
        UIAudioPlayer.PlayOneShot(Button);
        InteractivityInstance.PositionToRenderAt = FirstPersonCamera.transform.position + (FirstPersonCamera.transform.forward * 0.5f);
        InteractivityInstance.RenderEntity(Earth);
    }
    public void ChooseMars()
    {
        UIAudioPlayer.Stop();
        UIAudioPlayer.PlayOneShot(Button);
        InteractivityInstance.PositionToRenderAt = FirstPersonCamera.transform.position + (FirstPersonCamera.transform.forward * 0.5f);
        InteractivityInstance.RenderEntity(Mars);
    }
    public void ChooseJupiter()
    {
        UIAudioPlayer.Stop();
        UIAudioPlayer.PlayOneShot(Button);
        InteractivityInstance.PositionToRenderAt = FirstPersonCamera.transform.position + (FirstPersonCamera.transform.forward * 2.5f);
        InteractivityInstance.RenderEntity(Jupiter);
    }
    public void ChooseSaturn()
    {
        UIAudioPlayer.Stop();
        UIAudioPlayer.PlayOneShot(Button);
        InteractivityInstance.PositionToRenderAt = FirstPersonCamera.transform.position + (FirstPersonCamera.transform.forward * 2.5f);
        InteractivityInstance.RenderEntity(Saturn);
    }
    public void Reset()
    {
        InteractivityInstance.Reset();
    }
}