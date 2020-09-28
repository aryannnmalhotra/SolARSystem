using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

//[ExecuteInEditMode]
public class CustomButton : MonoBehaviour {
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private string _code = "";
    public void Init(string text, string code, Action<GameObject, string,string> callBack)
    {
        _code = code;
        _callBack = callBack;
        this.gameObject.GetComponentInChildren<Text>().text = text;
    }

    Action<GameObject, string, string> _callBack = null;

    public void Click(string param)
    {
        if (_callBack != null) _callBack(this.gameObject, _code, param);
    }
}
