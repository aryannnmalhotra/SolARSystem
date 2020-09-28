using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class AsteroidHandler : MonoBehaviour {

    public GameObject Parent = null;
    public string mesh_Name = "";
    public string material_Name = "";
    public string texture_Name = "";
    public string normal_Name = "";
    public float angle = 0;
    public Vector3 size = Vector3.one;
    public Vector3 dist = Vector3.one;
    public Vector3 globalScale;
    public Vector3 lossyScale;
    public float Radius = 0;
    public Vector3 Direction;
    public Vector3 MyRotate = Vector3.zero;

    // Use this for initialization
    void Start () {
        MyRotate = new Vector3(Random.Range(-100,100), Random.Range(-100, 100), Random.Range(-100, 100));
    }
	
	// Update is called once per frame
	void Update () {
        this.gameObject.transform.Rotate(MyRotate * Time.deltaTime);
	}
}
