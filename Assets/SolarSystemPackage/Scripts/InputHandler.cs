using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//[ExecuteInEditMode]
public class InputHandler : MonoBehaviour {
    /// <summary>
    /// Perform RayCast modes. 
    /// Disabled = no raycast, Enabled = always raycast, ClickUp = when released the click, stc.
    /// </summary>
    public enum RayCastModes { Disabled, Enabled, ClickUp, Touched, ClickDown, DoubleClickDown, DoubleClickUp}
    /// <summary>
    /// Current raycast mode
    /// </summary>
    public RayCastModes RayCastMode = RayCastModes.Enabled;
    /// <summary>
    /// GameObject was raycasted on last frame
    /// </summary>
    public GameObject LastRayHitObject = null;
    /// <summary>
    /// GameObject was raycasted on this frame
    /// </summary>
    public GameObject CurrentRayHitObject = null;
    /// <summary>
    /// If the GraphicRaycaster is on another gameobject it have to give
    /// </summary>
    public GameObject GraphicRaycasterObject;
    /// <summary>
    /// The doubleclick time on secound
    /// </summary>
    public float DoubleClickTime = 0.5f;
    /// <summary>
    /// Touch or mouse horizontally moving current frame value (* boost)
    /// </summary>
    public float HorizontalMove = 0;
    /// <summary>
    /// Touch or mouse vertically moving current frame value (* boost)
    /// </summary>
    public float VerticalMove = 0;
    /// <summary>
    /// Mouse horizontally moving current frame value click on (* boost)
    /// </summary>
    public float HorizontalScroll = 0;
    /// <summary>
    /// Mouse vertically moving current frame value when click on (* boost)
    /// </summary>
    public float VerticalScroll = 0;
    /// <summary>
    /// Touch (by first and second finger) or mouse wheel zoom current frame value (* boost)
    /// </summary>
    public float Zoom = 0;
    /// <summary>
    /// WASD key enabled
    /// </summary>
    public bool WASDMove = true;
    /// <summary>
    /// WASD as scroll
    /// </summary>
    public bool WASDScroll = true;
    /// <summary>
    /// Zoom as Up & Down arrow enabled.
    /// </summary>
    public bool UpDownZoomEnabled = true;
    /// <summary>
    /// If UI is hitted then Click & Zoom is disabled
    /// </summary>
    public bool UISafeMode = true;
    /// <summary>
    /// Have to give for ScreenPointToRay.
    /// </summary>
    public Camera InputCamera = null;
    /// <summary>
    /// If it is true the Update event run in LastUpdate, otherwise Update.
    /// </summary>
    public bool LastUpdateMode = false;
    /// <summary>
    /// Touch (one & first finger) or mouse down click event
    /// </summary>
    public List<Action<GameObject>> ClickDownAction = new List<Action<GameObject>>();
    /// <summary>
    /// Touch (one & first finger) or mouse up click event
    /// </summary>
    public List<Action<GameObject>> ClickUpAction = new List<Action<GameObject>>();
    /// <summary>
    /// Touch (one & first finger) or mouse up click event if no moving / scrolling / raycast any target
    /// </summary>
    public List<Action<GameObject>> ClickSafeAction = new List<Action<GameObject>>();
    /// <summary>
    /// Touch (one & first finger) or mouse down doubleclick event
    /// </summary>
    public List<Action<GameObject>> DoubleClickDownAction = new List<Action<GameObject>>();
    /// <summary>
    /// Touch (one & first finger) or mouse up doubleclick event
    /// </summary>
    public List<Action<GameObject>> DoubleClickUpAction = new List<Action<GameObject>>();
    /// <summary>
    /// Touch (first & second finger) or mouse (clicked) move event
    /// </summary>
    public List<Action<GameObject>> ZoomAction = new List<Action<GameObject>>();
    /// <summary>
    /// When raycast target changed event
    /// </summary>
    public List<Action<GameObject>> ChangeTargetAction = new List<Action<GameObject>>();
    /// <summary>
    /// When HorizontalMove or VerticalMove changed event
    /// </summary>
    public List<Action<GameObject>> MoveAction = new List<Action<GameObject>>();
    /// <summary>
    /// When HorizontalScroll or VerticalScroll changed event
    /// </summary>
    public List<Action<GameObject>> ScrollAction = new List<Action<GameObject>>();
    /// <summary>
    /// Move (horizontally & vertically) move / scroll multiplier for not mobile.
    /// </summary>
    public float MoveBoost = 1.0f;
    /// <summary>
    /// Move (horizontally & vertically) move / scroll multiplier for mobile.
    /// </summary>
    public float MoveAndroidBoost = 0.4f;
    /// <summary>
    /// WASD move/scroll multiplier.
    /// </summary>
    public float WASDMoveBoost = 1.5f;
    /// <summary>
    /// Zoom multiplier for not mobile.
    /// </summary>
    public float ZoomBoost = 1.0f;
    /// <summary>
    /// Zoom multiplier for mobile.
    /// </summary>
    public float ZoomAndroidBoost = 0.4f;
    /// <summary>
    /// Current move boost value
    /// </summary>
    public float MoveBoostValue
    {
        get { return IsMobil ? MoveAndroidBoost : MoveBoost; }
    }
    /// <summary>
    /// Current zoom boost value
    /// </summary>
    public float ZoomBoostValue
    {
        get { return IsMobil ? ZoomAndroidBoost : ZoomBoost; }
    }
    private float lastClickTimeUp = 0;
    private float lastClickTimeDown = 0;
    private bool touched = false;
    private bool fingerStart = false;
    private bool fingerMoved = false;
    private List<RaycastResult> uiRaycastResult = new List<RaycastResult>();
    public bool uiHit = false;
    /// <summary>
    /// UIRaycastResult
    /// </summary>
    public List<RaycastResult> UIRaycastResult
    {
        get { return uiRaycastResult; }
    }
    /// <summary>
    /// True if UI hitted
    /// </summary>
    public bool IsUIHit
    {
        get { return uiHit; }
    }

    void Start () {
        //InputCamera = Camera.current;
    }

    void Update()
    {
        if (!LastUpdateMode) InputUpdate();
    }

    void LateUpdate()
    {
        if (LastUpdateMode) InputUpdate();
    }

    private bool rayCastProceed = false;
    private Ray ray;
    private RaycastHit hit;
    private void InputUpdate()
    {
        rayCastProceed = false;
        CurrentRayHitObject = null;
        HorizontalMove = VerticalMove = HorizontalScroll = VerticalScroll = Zoom = 0;
        lastClickTimeUp -= lastClickTimeUp <= 0 ? 0 : Time.deltaTime;
        lastClickTimeDown -= lastClickTimeDown <= 0 ? 0 : Time.deltaTime;
        RayCastProcess(RayCastModes.Enabled);
        if (Input.GetMouseButtonDown(0))
        {
            touched = true;
            fingerMoved = false;
            RayCastProcess(RayCastModes.ClickDown);
            CallAction(ClickDownAction);
            if (lastClickTimeDown > 0)
            {
                RayCastProcess(RayCastModes.DoubleClickDown);
                CallAction(DoubleClickDownAction);
            }
            lastClickTimeDown = DoubleClickTime;
        }
        if (Input.GetMouseButtonUp(0))
        {
            touched = false;
            RayCastProcess(RayCastModes.ClickUp);
            CallAction(ClickUpAction);
            if (lastClickTimeUp > 0)
            {
                RayCastProcess(RayCastModes.DoubleClickUp);
                CallAction(DoubleClickUpAction);
            }
            lastClickTimeUp = DoubleClickTime;
            if(!fingerMoved)
                CallAction(ClickSafeAction);
        }

        if (touched) RayCastProcess(RayCastModes.Touched);

        if (IsMobil)
        {
            if (Input.touchCount > 0)
            {
                if (Input.touchCount == 2)
                {
                    Touch touchZero = Input.GetTouch(0);
                    Touch touchOne = Input.GetTouch(1);
                    Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                    Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
                    float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                    float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;
                    float deltaMagnitudeDiff = (prevTouchDeltaMag - touchDeltaMag) / Screen.width;

                    if (deltaMagnitudeDiff != 0)
                    {
                        Zoom = -deltaMagnitudeDiff * ZoomBoostValue;
                    }
                }
                if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    RayCastProcess(RayCastModes.ClickUp);
                    CallAction(ClickUpAction);
                    if (lastClickTimeUp > 0)
                    {
                        RayCastProcess(RayCastModes.DoubleClickUp);
                        CallAction(DoubleClickUpAction);
                    }
                    lastClickTimeUp = DoubleClickTime;
                    if (!fingerMoved)
                        CallAction(ClickSafeAction);
                }
                if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    RayCastProcess(RayCastModes.ClickDown);
                    CallAction(ClickDownAction);
                    if (lastClickTimeDown > 0)
                    {
                        RayCastProcess(RayCastModes.DoubleClickDown);
                        CallAction(DoubleClickDownAction);
                    }
                    lastClickTimeDown = DoubleClickTime;
                }
                if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved && fingerStart) // || Input.GetTouch(0).phase == TouchPhase.Stationary))
                {
                    Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
                    if (touchDeltaPosition.x != 0.0f)
                    {
                        HorizontalMove = touchDeltaPosition.x * MoveBoostValue;
                    }
                    if (touchDeltaPosition.y != 0.0f)
                    {
                        VerticalMove = touchDeltaPosition.y * MoveBoostValue;
                    }
                }
                if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    fingerStart = false;
                    fingerMoved = false;
                }
                if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    fingerStart = true;
                    fingerMoved = false;
                }
            }
        }
        else
        {
            HorizontalMove = Input.GetAxis("Mouse X") * MoveBoostValue;
            VerticalMove = Input.GetAxis("Mouse Y") * MoveBoostValue;
            Zoom = Input.GetAxis("Mouse ScrollWheel") * ZoomBoostValue;
        }
        if(UpDownZoomEnabled && Zoom == 0)
        {
            if (Input.GetKey(KeyCode.UpArrow)) Zoom += 0.1f * ZoomBoostValue;
            if (Input.GetKey(KeyCode.DownArrow)) Zoom -= 0.1f * ZoomBoostValue;
        }
        bool WASDEnabledTouched = false;
        if (WASDScroll || WASDMove)
        {
            if (Input.GetKey(KeyCode.W))
            {
                VerticalMove += WASDMoveBoost * MoveBoostValue;
                WASDEnabledTouched = WASDScroll;
            }
            if (Input.GetKey(KeyCode.S))
            {
                VerticalMove -= WASDMoveBoost * MoveBoostValue;
                WASDEnabledTouched = WASDScroll;
            }
            if (Input.GetKey(KeyCode.A))
            {
                HorizontalMove -= WASDMoveBoost * MoveBoostValue;
                WASDEnabledTouched = WASDScroll;
            }
            if (Input.GetKey(KeyCode.D))
            {
                HorizontalMove += WASDMoveBoost * MoveBoostValue;
                WASDEnabledTouched = WASDScroll;
            }
        }
        if (HorizontalMove != 0 || VerticalMove != 0)
        {
            fingerMoved = true;
            CallAction(MoveAction);
            if (touched || WASDEnabledTouched)
            {
                HorizontalScroll += HorizontalMove;
                VerticalScroll += VerticalMove;
                if (HorizontalScroll != 0 || VerticalScroll != 0)
                    CallAction(ScrollAction);
            }
        }
        if(Zoom != 0) CallAction(ZoomAction);
        if(LastRayHitObject != CurrentRayHitObject) CallAction(ChangeTargetAction);
        if(UISafeMode && IsUIHit)
        {
            HorizontalMove = VerticalMove = HorizontalScroll = VerticalScroll = Zoom = 0;
        }
        LastRayHitObject = CurrentRayHitObject;
    }
    /// <summary>
    /// True if was scroll in the current frame.
    /// </summary>
    public bool IsScroll
    {
        get
        {
            return (HorizontalScroll != 0 || VerticalScroll != 0);
        }
    }
    /// <summary>
    /// True if was move in the current frame.
    /// </summary>
    public bool IsMove
    {
        get
        {
            return (HorizontalMove != 0 || VerticalMove != 0);
        }
    }
    /// <summary>
    /// True if was zoom in the current frame.
    /// </summary>
    public bool IsZoom
    {
        get
        {
            return (Zoom != 0);
        }
    }
    /// <summary>
    /// True if this is a mobile platform.
    /// </summary>
    public bool IsMobil
    {
        get
        {
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
                return true;
            return false;
        }
    }
    private void CallAction(List<Action<GameObject>> ac)
    {
        if (UISafeMode && IsUIHit) return;
        foreach (Action<GameObject> a in ac)
        {
            a(CurrentRayHitObject);
        }
    }
    private void RayCastProcess(RayCastModes m)
    {
        if (rayCastProceed || m != RayCastMode) return;
        rayCastProceed = true;
        if (IsMobil)
        {
            if (Input.touchCount == 0) return;
            ray = InputCamera.ScreenPointToRay(Input.GetTouch(0).position);
        }
        else ray = InputCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            CurrentRayHitObject = hit.transform.gameObject;
        }
        uiRaycastResult = UIRaycast();
        uiHit = false;
        foreach (RaycastResult r in uiRaycastResult)
        {
            uiHit = true;
        }
    }
    /// <summary>
    /// UI raycasting
    /// </summary>
    /// <returns></returns>
    public List<RaycastResult> UIRaycast()
    {
        GraphicRaycaster gr = GraphicRaycasterObject.GetComponent<GraphicRaycaster>();
        if(gr == null) gr = GraphicRaycasterObject.AddComponent<GraphicRaycaster>();
        PointerEventData ped = new PointerEventData(null);
        ped.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        gr.Raycast(ped, results);
        //foreach (RaycastResult hit in results)
        //{
        //}
        return results;
    }
}
