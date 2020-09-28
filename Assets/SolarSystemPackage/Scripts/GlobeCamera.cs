using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class GlobeCamera : MonoBehaviour {

    public RenderTexture RawImg = null;
    private float _distant = 0;
    private GameObject cameraObject;
    private Camera cam;

    public bool Active
    {
        get { return cameraObject.activeSelf; }
        set { cameraObject.SetActive(value);  }
    }

    void Start()
    {
    }

    public void Init()
    {
        if (RawImg != null) return;
        RawImg = new RenderTexture(256, 256, 16, RenderTextureFormat.ARGB32);
        RawImg.Create();
        cameraObject = new GameObject();
        cameraObject.transform.parent = this.transform;
        cameraObject.name = this.gameObject.name + "-Camera";
        cam = cameraObject.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.targetTexture = RawImg;
        cam.nearClipPlane = 0.01f;
        cam.farClipPlane = 100000.0f;
    }
    public Vector3 CameraViewAxis = new Vector3(0, 10, 180);
    public bool StillPic = false;
    public bool _stillPic = false;
    void Update () {
        CameraViewAxis.x = (CameraViewAxis.x + Time.deltaTime) % 360;
        _distant = Mathf.Max(this.transform.lossyScale.x, this.transform.lossyScale.y, this.transform.lossyScale.z) * 2f;
        cameraObject.transform.position = this.transform.position + CameraViewAxis.normalized * _distant;
        cameraObject.transform.LookAt(this.transform, Vector3.up);
        cam.nearClipPlane = (_distant / 2) * 1.5f;
        cam.farClipPlane = cam.nearClipPlane + (_distant / 2);
    }
}
