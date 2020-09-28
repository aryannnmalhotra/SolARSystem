using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public enum PlanetMetric { AsSolarSystem, AsRoundAbout, Custom }
public enum SystemModes { Realistic, Smart }

[ExecuteInEditMode]
public class SolarSystemHandler : ObjectHandler {

    public bool AnimationEnabled = false;

    public Camera MainCamera = null;
    public float DistantCustomMultiplier = 1;

    public float DistantMultiplier
    {
        get { return DistantCustomMultiplier; }
    }

    public float TimeCustomMultiplier = 1;

    public float TimeMultiplier
    {
        get { return TimeCustomMultiplier; }
    }

    public SystemModes SystemMode = SystemModes.Realistic;
    public float DistantRange = 1000;
    public float DiameterRange = 100;
    public float SqrtMulDistant = 2;
    public float SqrtMulRadius = 1;

    public float MaxDistant = -1;
    public float MinDistant = -1;
    public float MaxDiameter = -1;
    public float MinDiameter = -1;

    /// <summary>
    /// Reset all object
    /// </summary>
    public bool ResetAll = false;

    /// <summary>
    /// GeneratePercent
    /// </summary>
    public Text TextObject;
    // Use this for initialization
    void Start () {
        DEFAULTResulutionType = this.ResulutionType;
        RefreshTargetObjects();
        if(Targets.Count == 0)
            GenerateByFile= AutoGenerateByFile;
    }
    public bool RecalcMinMax = false;
    void Update()
    {

        if (GenerateByFile || GenerateByFileAdditive)
        {
            if(!GenerateByFileAdditive)
            {
                Targets = new Dictionary<string, GameObject>();
                ObjectHandler.SharedMeshes = new Dictionary<string, MeshClass>();
                GenerateByFileAdditive = false;
                GenerateByFile = false;
                StartCoroutine(DestroyAllObject(this.gameObject));
            }
            else
                StartCoroutine(GeneratePlanets(FileName));
        }
        if (GenerateToFile) GenerateFile(FileName);
        if(RecalcMinMax || ResetAll)
        {
            RefreshTargetObjects();
            RecalcMinMax = false;
            RecalculateCustomData();
        }
        if (ResetAll)
        {
            ResetAll = false;
            ResetAllObject();
        }
    }
    private IEnumerator DestroyAllObject(GameObject parent)
    {
        //foreach (Transform t in parent.GetComponentsInChildren<Transform>())
        //{
        //    yield return DestroyAllObject(t.gameObject);
        //}
        //if (this.gameObject != parent) DestroyImmediate(parent);
        //else 
        yield return GeneratePlanets(FileName);
        yield return 1;
    }
    private IEnumerator ResetAllObject()
    {
        int cnto = 0;
        int maxo = Targets.Count;
        int comm = 10;
        LogOut("Refresh system... " + maxo + " / " + cnto);
        foreach (GameObject g in Targets.Values)
        {
            g.GetComponent<PlanetHandler>().ResetPlanet();
            cnto++;
            if (--comm <= 0)
            {
                comm = 10;
                LogOut("Refresh system... " + maxo + " / " + cnto);
                yield return 0.01f;
            }
        }
        LogOut("Wellcome passengers!");
    }
    private void RecalculateCustomData()
    {
        PlanetHandler p=null;
        if (SystemMode == SystemModes.Smart)
        {
            MaxDistant = MinDistant = MaxDiameter = MinDiameter = -1;
            foreach (GameObject g in Targets.Values)
            {
                string gtag = "";
                p = g.GetComponent<PlanetHandler>();
                if (p != null) gtag = p.Category;
                if (gtag == "Star" || gtag == "Planet" || gtag == "Moon" || gtag == "AsteroidField")
                {
                    float f = p.Radius * DistantMultiplier;
                    if (f > 0)
                    {
                        if (f > MaxDiameter) MaxDiameter = f;
                        if (f < MinDiameter || MinDiameter == -1) MinDiameter = f;
                    }
                    f = p.Distant * DistantMultiplier;
                    if (p.Parent != null && f > 0)
                    {
                        if (f > MaxDistant) MaxDistant = f;
                        if (f < MinDistant || MinDistant == -1) MinDistant = f;
                    }
                }
            }
            // Set CustomRadius
            foreach (GameObject g in Targets.Values)
            {
                p = g.GetComponent<PlanetHandler>();
                float curr = sqrt3radius(p.DistantMultiplier * p.Radius * 2);
                float rang = sqrt3radius(SolarSystemHandler.MaxDiameter);
                float r = (curr / rang) * SolarSystemHandler.DiameterRange;
                p.CustomRadius = r / (DistantMultiplier * 2);
                p.CustomGroupRadius = p.CustomRadius;
                p.CustomDistant = -1;
            }
            foreach (string pn in Parents.Keys)
            {
                RecursGetDistant(pn);
            }
            StartCoroutine(ResetAllObject());
        }
    }
    private float RecursGetDistant(string name)
    {
        PlanetHandler p = Targets[name].GetComponent<PlanetHandler>();
        float back = p.CustomRadius;
        float lastDistant = 0;
        if (Parents.ContainsKey(name))
        {
            foreach (GameObject g in Parents[name])
            {
                PlanetHandler c = g.GetComponent<PlanetHandler>();
                if (c.Distant != 0)
                {
                    float cRadius = RecursGetDistant(g.name);
                    //back += (c.Distant - lastDistant) * (cRadius / c.Radius) + cRadius;
                    float currDistant = c.Distant - lastDistant;
                    float curr = sqrt3radius(c.DistantMultiplier * currDistant);
                    float rang = sqrt3radius(SolarSystemHandler.MaxDistant);
                    float r = (curr / rang) * SolarSystemHandler.DistantRange;
                    back += (r / c.DistantMultiplier) + cRadius;
                    c.CustomDistant = back;
                    back += cRadius;
                    lastDistant = c.Distant;
                }
            }
        }
        p.CustomGroupRadius = back;
        return back;
    }
    public enum GetCustomDataType { Distant, Radius }
    public float GetCustomData(string name, GetCustomDataType d, SystemModes m)
    {
        float back = 0;
        if (!Targets.ContainsKey(name)) return 0;
        PlanetHandler p = Targets[name].GetComponent<PlanetHandler>();
        if (d == GetCustomDataType.Distant) back = p.CurrentDistant;
        if (d == GetCustomDataType.Radius) back = p.CurrentRadius;
        foreach (PlanetHandler g in p.gameObject.GetComponentsInChildren<PlanetHandler>())
        {
            if (g.ParentName.Equals(name)) back += GetCustomData(g.gameObject.name, d, m);
        }
        return back;
    }
    private float sqrt3radius(float f)
    {
        float back = f;
        for (int i = 0; i < SolarSystemHandler.SqrtMulRadius; ++i) back = Mathf.Sqrt(back);
        return back;
    }
    private float sqrt3distant(float f)
    {
        float back = f;
        for (int i = 0; i < SolarSystemHandler.SqrtMulDistant; ++i) back = Mathf.Sqrt(back);
        return back;
    }

    public bool AutoGenerateByFile = false;
    public bool GenerateByFile = false;
    public bool GenerateByFileAdditive = false;
    public bool GenerateToFile = false;
    public string FileName = "SolarSystemData.txt";
    private void LogOut(string text)
    {
        if (TextObject != null) TextObject.text = text;
    }
    private int yieldCount = 2;
    private int yieldCountStart = 20;
    private IEnumerator GeneratePlanets(string filename)
    {
        yieldCountStart = Random.Range(30,80);
        LogOut("Loading system data...");
        GenerateByFile = false;
        yieldCountStart = 500;
        yieldCount = yieldCountStart;
        TextAsset textAsset = (TextAsset)Resources.Load("SolarSystemData", typeof(TextAsset));
        yield return null;
        int cnto = 1;
        yieldCountStart = 10;
        yieldCount = yieldCountStart;
        yield return null;
        LogOut("Generate objects... " + cnto);
        GameObject g = new GameObject();
        PlanetHandler p = g.AddComponent<PlanetHandler>();
        g.name = "Creating";
        using (TextReader reader = new StringReader(textAsset.text))
        {
            string s = reader.ReadLine();
            while(s != "END")
            {
                //Debug.Log(s);
                if (s == "CREATE")
                {
                    g.transform.parent = this.transform;
                    if (s.Trim() != "") p.AddParameter(s);
                    RefreshTargetAdd(g);
                    if (--yieldCount <= 00)
                    {
                        LogOut("Generate objects... " + cnto);
                        yieldCount = yieldCountStart;
                        yield return 0.01f;
                    }
                    g = new GameObject();
                    g.name = "Creating";
                    p = g.AddComponent<PlanetHandler>();
                    cnto++;
                }
                else if (s.Trim() != "") p.AddParameter(s);
                s = reader.ReadLine();
            }
        }
        if (g.name == "Creating") DestroyImmediate(g);
        yield return null;
        LogOut("Refresh system...");
        RefreshTargetFinish();
        yield return 5;
        ResetAll = true;
    }
    public int TargetsChanged = -1;
    public Dictionary<string, GameObject> Targets = new Dictionary<string, GameObject>();
    public void RefreshTargetObjects()
    {
        TargetsChanged++;
        Targets = new Dictionary<string, GameObject>();
        Parents = new Dictionary<string, List<GameObject>>();
        foreach (PlanetHandler g in this.GetComponentsInChildren<PlanetHandler>())
        {
            Targets.Add(g.gameObject.name, g.gameObject);
            if (g.Parent != this.gameObject)
            {
                if (!Parents.ContainsKey(g.Parent.name)) Parents.Add(g.Parent.name, new List<GameObject>());
                Parents[g.Parent.name].Add(g.gameObject);
            }
        }
        RefreshParentsObjects();
    }
    public Dictionary<string, List<GameObject>> Parents = new Dictionary<string, List<GameObject>>();
    private void RefreshParentsObjects()
    {
        // Sort by Distant
        foreach (string k in Parents.Keys)
            Parents[k].Sort(delegate (GameObject x, GameObject y)
            {
                return x.GetComponent<PlanetHandler>().Distant.CompareTo(y.GetComponent<PlanetHandler>().Distant); ;
            });
    }
    private void RefreshTargetInit()
    {
        TargetsChanged++;
        Targets = new Dictionary<string, GameObject>();
        Parents = new Dictionary<string, List<GameObject>>();
    }
    private void RefreshTargetAdd(GameObject g)
    {
        PlanetHandler p = g.GetComponent<PlanetHandler>();
        Targets.Add(g.name, g);
        if (p.Parent != this.gameObject)
        {
            if (!Parents.ContainsKey(p.Parent.name)) Parents.Add(p.Parent.name, new List<GameObject>());
            Parents[p.Parent.name].Add(g);
        }
    }
    private void RefreshTargetFinish()
    {
        foreach (string k in Parents.Keys)
            Parents[k].Sort(delegate (GameObject x, GameObject y)
            {
                return x.GetComponent<PlanetHandler>().Distant.CompareTo(y.GetComponent<PlanetHandler>().Distant); ;
            });
    }
    public void AddTargetObject(GameObject g)
    {
        Targets.Add(g.name, g);
        TargetsChanged++;
    }
    private void GenerateFile(string filename)
    {
        GenerateToFile = false;
        using (System.IO.StreamWriter file = new System.IO.StreamWriter(filename, true))
        {
            foreach (PlanetHandler p in this.GetComponentsInChildren<PlanetHandler>())
            {
                file.WriteLine(p.GetPlanetData());
            }
        }
    }
}
