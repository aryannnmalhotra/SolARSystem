using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GoogleARCore;

public class Interactivity : MonoBehaviour
{
    private bool isReticleEnabled;
    private bool comparisonLoading;
    private float reticleFillAmount;
    private GameObject currentObject;
    private GameObject compareObject;
    private Touch touch;
    private Vector2 touchStart;
    private Vector2 touchStartOG;
    private Touch touch1;
    private Touch touch2;
    private Vector2 touchStart1;
    private Vector2 touchStart2;
    private Vector2 currentDistance;
    private Vector2 previousDistance;
    private Vector2 touchEnd1;
    private Vector2 touchEnd2;
    private Vector2 touchEnd;
    public Vector3 PositionToRenderAt;
    public int CurrentIndex;
    public Image ReticleImage;
    public GameObject MenuPanel;
    public GameObject CompareMenuPanel;
    public GameObject ResetButton;
    public Camera FirstPersonCamera;
    public AudioSource UIAudioPlayer;
    public AudioSource ZoomAudioPlayer;
    public AudioClip MenuPopUp;
    public AudioClip PlanetShifting;
    public AudioClip Button;
    void Start()
    {
        reticleFillAmount = 0;
    }
    public void RenderEntity(GameObject objectToSpawn)
    {
        MenuPanel.SetActive(false);
        ResetButton.SetActive(true);
        if (currentObject)
        {
            Destroy(currentObject);
            Destroy(compareObject);
        }
        currentObject = Instantiate(objectToSpawn) as GameObject;
        currentObject.transform.position = PositionToRenderAt;
        CurrentIndex = currentObject.GetComponent<Entity>().EntityIndex;
    }
    private void loadingReset()
    {
        comparisonLoading = false;
    }
    public void RenderCompareEntity(GameObject objectToCompare)
    {
        CompareMenuPanel.SetActive(false);
        ResetButton.SetActive(true);
        if (compareObject)
            Destroy(compareObject);
        compareObject = Instantiate(objectToCompare) as GameObject;
        compareObject.GetComponent<Entity>().IsCompareEntity = true;
        compareObject.GetComponent<Entity>().ComparisonPanel.SetActive(true);
        int compareIndex = compareObject.GetComponent<Entity>().EntityIndex;
        if (CurrentIndex == 1)
        {
            if (compareIndex == 2 || compareIndex == 3)
            {
                compareObject.transform.position = currentObject.transform.position + (FirstPersonCamera.transform.right * 0.1f);
                currentObject.transform.position -= (FirstPersonCamera.transform.right * 0.1f);
            }
            else if (compareIndex == 4 || compareIndex == 5)
            {
                compareObject.transform.position = currentObject.transform.position + (FirstPersonCamera.transform.forward * 0.9f);
            }
        }
        else if (CurrentIndex == 2)
        {
            if (compareIndex == 1 || compareIndex == 3)
            {
                compareObject.transform.position = currentObject.transform.position + (FirstPersonCamera.transform.right * 0.1f);
                currentObject.transform.position -= (FirstPersonCamera.transform.right * 0.1f);
            }
            else if (compareIndex == 4 || compareIndex == 5)
            {
                compareObject.transform.position = currentObject.transform.position + (FirstPersonCamera.transform.forward * 0.9f);
            }
        }
        else if (CurrentIndex == 3)
        {
            if (compareIndex == 1 || compareIndex == 2)
            {
                compareObject.transform.position = currentObject.transform.position + (FirstPersonCamera.transform.right * 0.1f);
                currentObject.transform.position -= (FirstPersonCamera.transform.right * 0.1f);
            }
            else if (compareIndex == 4 || compareIndex == 5)
            {
                compareObject.transform.position = currentObject.transform.position + (FirstPersonCamera.transform.forward * 0.9f);
            }
        }
        else if (CurrentIndex == 4)
        {
            if (compareIndex == 1 || compareIndex == 2 || compareIndex == 3)
            {
                compareObject.transform.position = currentObject.transform.position - (FirstPersonCamera.transform.forward * 0.9f);
            }
            else if (compareIndex == 5)
            {
                compareObject.transform.position = currentObject.transform.position + (FirstPersonCamera.transform.right * 0.8f);
                currentObject.transform.position -= (FirstPersonCamera.transform.right * 0.8f);
            }
        }
        else
        {
            if (compareIndex == 1 || compareIndex == 2 || compareIndex == 3)
            {
                compareObject.transform.position = currentObject.transform.position - (FirstPersonCamera.transform.forward * 0.7f);
            }
            else if (compareIndex == 4)
            {
                compareObject.transform.position = currentObject.transform.position + (FirstPersonCamera.transform.right * 0.8f);
                currentObject.transform.position -= (FirstPersonCamera.transform.right * 0.8f);
            }
        }
        Invoke("loadingReset", 0.1f);
    }
    public void Reset()
    {
        ZoomAudioPlayer.Stop();
        UIAudioPlayer.Stop();
        UIAudioPlayer.PlayOneShot(Button);
        ResetButton.SetActive(false);
        Destroy(currentObject);
        if (compareObject)
            Destroy(compareObject);
        if (isReticleEnabled)
        {
            ReticleImage.enabled = false;
            isReticleEnabled = false;
        }
        comparisonLoading = true;
        CompareMenuPanel.SetActive(false);
        MenuPanel.SetActive(false);
    }
    void Update()
    {
        if (Session.Status != SessionStatus.Tracking)
        {
            return;
        }
        else
        if (Input.touchCount == 1 && (Input.GetTouch(0)).phase == TouchPhase.Began && !currentObject)
        {
            MenuPanel.SetActive(true);
            UIAudioPlayer.PlayOneShot(MenuPopUp);
            /*TrackableHit hit;
            TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon;
            if (Frame.Raycast(touch.position.x, touch.position.y, raycastFilter, out hit))
            {
                if ((hit.Trackable is DetectedPlane))
                {
                    
                }
            }*/
        }
        if (currentObject)
        {
            if (Input.touchCount == 1)
            {
                touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    ZoomAudioPlayer.Stop();
                    UIAudioPlayer.Stop();
                    touchStart = touch.position;
                    touchStartOG = touchStart;
                    touchEnd = touch.position;
                }
                else if (touch.phase == TouchPhase.Moved)
                {
                    touchEnd = touch.position;
                    float verticalSwipeDistance = Mathf.Abs(touchStart.y - touchEnd.y);
                    float horizontalSwipeDistance = Mathf.Abs(touchStart.x - touchEnd.x);
                    if (verticalSwipeDistance >= 30)
                    {
                        UIAudioPlayer.Stop();
                        currentObject.transform.RotateAround(currentObject.transform.position, FirstPersonCamera.transform.right, 2 * Time.deltaTime * touch.deltaPosition.y);
                        UIAudioPlayer.PlayOneShot(PlanetShifting);
                        /*if (touchStart.y > touchEnd.y)
                        {
                            int targetRotation = 5;
                            currentObject.GetComponent<Entity>().RotateVertical(targetRotation);
                            touchStart = new Vector2(touchStart.x, touchEnd.y);
                        }
                        else
                        {
                            int targetRotation = 5;
                            currentObject.GetComponent<Entity>().RotateVertical(targetRotation);
                            touchStart = new Vector2(touchStart.x, touchEnd.y);
                        }*/
                    }
                    if (horizontalSwipeDistance >= 30)
                    {
                        UIAudioPlayer.Stop();
                        currentObject.transform.RotateAround(currentObject.transform.position, FirstPersonCamera.transform.up, -(2 * Time.deltaTime * touch.deltaPosition.x));
                        UIAudioPlayer.PlayOneShot(PlanetShifting);
                        /*if (touchStart.x > touchEnd.x)
                        {
                            int targetRotation = 5;
                            currentObject.GetComponent<Entity>().RotateVertical(targetRotation);
                            touchStart = new Vector2(touchEnd.x, touchStart.y);
                        }
                        else
                        {
                            int targetRotation = 5;
                            currentObject.GetComponent<Entity>().RotateVertical(targetRotation);
                            touchStart = new Vector2(touchEnd.x, touchStart.y);
                        }*/
                    }
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    touchEnd = touch.position;
                    float verticalSwipeDistance = Mathf.Abs(touchStartOG.y - touchEnd.y);
                    float horizontalSwipeDistance = Mathf.Abs(touchStartOG.x - touchEnd.x);
                    if (verticalSwipeDistance < 20 && horizontalSwipeDistance < 20 && !comparisonLoading)
                    {
                        UIAudioPlayer.Stop();
                        comparisonLoading = true;
                        ResetButton.SetActive(false);
                        CompareMenuPanel.SetActive(true);
                        UIAudioPlayer.PlayOneShot(MenuPopUp);
                    }
                }
            }
            else if (Input.touchCount == 2)
            {
                touch1 = Input.GetTouch(0);
                touch2 = Input.GetTouch(1);
                if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
                {
                    currentDistance = touch1.position - touch2.position;
                    previousDistance = (touch1.position - touch1.deltaPosition) - (touch2.position - touch2.deltaPosition);
                    float touchDelta = currentDistance.magnitude - previousDistance.magnitude;
                    if(touchDelta > 0)
                    {
                        ZoomAudioPlayer.Play();
                        currentObject.transform.position -= FirstPersonCamera.transform.forward * Time.deltaTime;
                        if (compareObject)
                            compareObject.transform.position -= FirstPersonCamera.transform.forward * Time.deltaTime;
                    }
                    else 
                    {
                        ZoomAudioPlayer.Play();
                        currentObject.transform.position += FirstPersonCamera.transform.forward * Time.deltaTime;
                        if (compareObject)
                            compareObject.transform.position += FirstPersonCamera.transform.forward * Time.deltaTime;
                    }
                }
                /*touchStart1 = touch1.position;
                touchStart2 = touch2.position;
                if (touch1.phase == TouchPhase.Began || touch2.phase == TouchPhase.Began)
                {
                    touchStart1 = touch1.position;
                    touchStart2 = touch2.position;
                    touchEnd1 = touch1.position;
                    touchEnd2 = touch2.position;
                }
                else if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
                {
                    touchEnd1 = touch1.position;
                    touchEnd2 = touch2.position;
                    if ((0.7f * Vector2.Distance(touchStart1, touchStart2)) > Vector2.Distance(touchEnd1, touchEnd2))
                    {
                        Destroy(currentObject);
                        if (compareObject)
                            Destroy(compareObject);
                        if (isReticleEnabled)
                        {
                            ReticleImage.enabled = false;
                            isReticleEnabled = false;
                        }
                        CompareMenuPanel.SetActive(false);
                        MenuPanel.SetActive(false);
                        Debugger.text = "RESET";
                    }
                }*/
            }
            if (Input.touchCount != 2)
            {
                ZoomAudioPlayer.Stop();
            }
            if (!currentObject.GetComponent<Entity>().DetailsPanel.activeSelf)
            {
                Ray ray = new Ray(FirstPersonCamera.transform.position, FirstPersonCamera.transform.forward);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if(hit.transform.gameObject == currentObject)
                    {
                        if (!isReticleEnabled)
                        {
                            reticleFillAmount = 0;
                            ReticleImage.fillAmount = reticleFillAmount;
                            ReticleImage.enabled = true;
                            isReticleEnabled = true;
                        }
                        else
                        {
                            reticleFillAmount = Mathf.Clamp(reticleFillAmount + Time.deltaTime / 3, 0, 1);
                            ReticleImage.fillAmount = reticleFillAmount;
                            if(reticleFillAmount >= 1)
                            {
                                ReticleImage.enabled = false;
                                isReticleEnabled = false;
                                currentObject.GetComponent<Entity>().DetailsPanel.SetActive(true);
                            }
                        }
                    }
                    else
                    {
                        ReticleImage.enabled = false;
                        isReticleEnabled = false;
                    }
                }
                else
                {
                    ReticleImage.enabled = false;
                    isReticleEnabled = false;
                }
            }
            if (compareObject && compareObject.GetComponent<Entity>().IsCompareEntity)
                compareObject.transform.Rotate(0, 30 * Time.deltaTime, 0);
        }
        if (!currentObject)
        {
            //CompareMenuPanel.SetActive(false);
            comparisonLoading = false;
        }
    }
}
