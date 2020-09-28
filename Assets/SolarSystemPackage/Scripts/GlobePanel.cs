using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//[ExecuteInEditMode]
public class GlobePanel : MonoBehaviour {
    public GameObject GlobeTarget = null;
    private GlobeCamera gc;
    // Use this for initialization
    void Start () {
		
	}
	// Update is called once per frame
	void Update () {
		
	}
    public void Init(GameObject target, Action<GameObject> selected )
    {
        GlobeTarget = target;
        _selected = selected;
        gc= GlobeTarget.GetComponent<GlobeCamera>();
        if(gc == null)
        {
            gc= GlobeTarget.AddComponent<GlobeCamera>();
        }
        gc.Init();
        gc.Active= true;
        GetComponentInChildren<Text>().text = GlobeTarget.name;
        GetComponentInChildren<RawImage>().texture = gc.RawImg;
    }

    Action<GameObject> _selected = null;

    public void Remove()
    {
        gc.Active = false;
    }

    public void SelectGlobe()
    {
        if (_selected != null) _selected(GlobeTarget);
    }
}
