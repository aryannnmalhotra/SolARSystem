using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//[ExecuteInEditMode]
public class ShipInterface : MonoBehaviour {
    public bool Opti = false;
    public float OptiDistant = 10000;
    public Vector3 TargetWordPos;
    public string TargetName;
    public float TargetMagni;

    public GameObject SolarSystem;
    private SolarSystemHandler solarHandler;

    public GameObject Target;
    public GameObject LookAtObject;
    public Vector3 LookAtPosition = Vector3.up;
    public Text TextObject;

    public float AnimatedAngle = 0;
    public float AngleX = 0;
    public float AngleY = 0;
    public float AngleZ = 0;

    public GameObject SpeedSlider;
    public GameObject AnimationSpeedSlider;

    public float DistantMin = 0.1f;
    public float DistantFarBoost = 200;
    public float _distant = 0.25f;
    public float Distant
    {
        get
        {
            return _distant;
        }
        set
        {
            if (value < 0) _distant = 0;
            else if (value > 1) _distant = 1;
            else _distant = value;
            if (Target != null && animationTime <= 0)
            {
                PlanetHandler p = Target.GetComponent<PlanetHandler>();
                GameObject t = Target;
                if (p!= null && p.Category == "AsteroidField") t = p.SubTarget;
                this.gameObject.transform.position = t.transform.position + (this.gameObject.transform.position - t.transform.position).normalized * DistantFrom;
            }
        }
    }
    /// <summary>
    /// Distant from target object
    /// </summary>
    public float DistantFrom
    {
        get
        {
            if (Target == null) return 0;
            float Xradius = Mathf.Max(targetPlanetObject.transform.lossyScale.x, targetPlanetObject.transform.lossyScale.y, targetPlanetObject.transform.lossyScale.z);
            return Xradius + DistantMin + Distant * Distant * Distant * Xradius * DistantFarBoost;
        }
    }

    private GameObject targetPlanetObject
    {
        get
        {
            return Target;
        }
    }

    private PlanetHandler planetHandler;

    public Camera myCamera;
    private bool rotationEnabled
    {
        get { return distantEnabled; }
    }
    private bool distantEnabled
    {
        get
        {
            bool ret = false;
            for (int i = 0; i < OnOffPanels.Length; ++i) ret = ret || OnOffPanels[i].activeSelf;
            return !ret;
        }
    }

    private InputHandler inp;
    // Use this for initialization
    void Start () {
        solarHandler= SolarSystem.GetComponent<SolarSystemHandler>();
        myCamera = GetComponentInChildren<Camera>();
        inp = GetComponent<InputHandler>();
        //inp.InputCamera = myCamera;
        InitActions();
        UIChangeSpeed();
        UIChangeAnimationSpeed();
    }
    void Update()
    {
        if (Target != null)
        {
            _mainObjectName = Target.name;
            // Float point protection
            Vector3 d = Target.transform.position;
            if (Mathf.Abs(d.magnitude) > OptiDistant || Mathf.Abs(d.x) > OptiDistant || Mathf.Abs(d.y) > OptiDistant || Mathf.Abs(d.z) > OptiDistant)
            {
                d = SolarSystem.transform.position - d;
                SolarSystem.transform.position = new Vector3(d.x, d.y, d.z);
            }
        }
        else if (!TargetName.Equals(""))
        {
            if (solarHandler.Targets.ContainsKey(TargetName))
            {
                ShowGlobe(solarHandler.Targets[TargetName]);
            }
        }
    }
    public float animationTime = 0;
    public bool freeMove = false;

    private void ClickSafe(GameObject g)
    {
        if (g == null) return;
        if (targets.ContainsKey(g.name))
        {
            lastSelected = inp.CurrentRayHitObject;
            animationTime = 1;
            UIViewTarget();
        }
        else
        {
            animationTime = 1;
            Target = g;
            LookAtObject = Target;
        }
    }
    private void Scroll(GameObject g)
    {
    }
    private void TargetChanged(GameObject g)
    {
        if (g != null)
        {
            PlanetHandler p = g.GetComponent<PlanetHandler>();
            if (p != null) TextObject.text = p.ParentName == "SolarSystem" ? g.name : p.ParentName + " : " + g.name;
            else TextObject.text = g.name;
        }
        else
            TextObject.text = "";
    }
    private void Zoom(GameObject g)
    {
        float d = inp.Zoom;
        if (freeMove)
            this.gameObject.transform.position += this.gameObject.transform.forward.normalized * d * 200;
        else
            Distant -= d;
    }
    private void InitActions()
    {
        inp.ClickSafeAction.Add(ClickSafe);
        inp.DoubleClickDownAction.Add(ClickSafe);
        inp.ChangeTargetAction.Add(TargetChanged);
        inp.ZoomAction.Add(Zoom);
    }

    void LateUpdate()
    {
        PlanetHandler p = null;
        GameObject ta = null;
        if (Target != null)
        {
            p = Target.GetComponent<PlanetHandler>();
            ta = Target;
            if (p!= null && p.Category == "AsteroidField") ta = p.NextSubTarget;
        }
        else
            if (!freeMove) return;

        if (AnimatedAngle > 0 && animationTime <= 0)
        {
            float spd = Mathf.Max(0.001f, boostPercent(AnimatedAngle));
            if (freeMove)
            {
                this.gameObject.transform.position += this.gameObject.transform.forward.normalized * spd * 20;
            }
            else
            {
                this.gameObject.transform.RotateAround(ta.transform.position, new Vector3(0, 1, 0), (360.0f * spd) * Time.deltaTime);
            }
        }

        Vector3 up = this.gameObject.transform.up;
        if (inp.IsScroll && animationTime == 0)
        {
            if (freeMove)
            {
                this.gameObject.transform.Rotate(this.gameObject.transform.up, inp.HorizontalScroll);
                Vector3 s = Vector3.Cross(this.gameObject.transform.forward, this.gameObject.transform.right).normalized;
                this.gameObject.transform.Rotate(-inp.VerticalScroll, inp.HorizontalScroll, 0);
            }
            else
            {
                this.gameObject.transform.RotateAround(ta.transform.position, this.gameObject.transform.right, -inp.VerticalScroll);
                this.gameObject.transform.RotateAround(ta.transform.position, this.gameObject.transform.up, inp.HorizontalScroll);
            }
        }
        if (ta == null) return;
        Vector3 pos = ta.transform.position + (this.gameObject.transform.position - ta.transform.position).normalized * DistantFrom;
        Vector3 tar = LookAtObject.transform.position;
        if (animationTime > 0)
        {
            float ff = animationTime / Time.deltaTime;
            animationTime -= Time.deltaTime;
            if (animationTime <= 0)
            {
                animationTime = 0;
                ff = 1;
            }
            pos = this.gameObject.transform.position + (pos - this.gameObject.transform.position) / ff;
            Vector3 lookFrom = this.gameObject.transform.position + this.gameObject.transform.forward.normalized * DistantFrom;
            tar = lookFrom + (tar - lookFrom) / ff;
            this.gameObject.transform.position = pos;
            this.gameObject.transform.LookAt(tar, up);
        }
        else if (!freeMove)
        {
            this.gameObject.transform.position = pos;
            this.gameObject.transform.LookAt(tar, up);
        }
    }

    private float boostPercent(float f)
    {
        return f * f ;
    }
    public Vector3 lookDir;
    private Dictionary<string, GameObject> targets
    {
        get { return solarHandler.Targets; }
        set { solarHandler.Targets= value; }
    }

    public GameObject UIPanel;
    public GameObject lastSelected;

    public void UIViewTarget()
    {
        Target = lastSelected;
        LookAtObject = Target;
        Distant = Distant;
        if (animationTime <= 0)
         this.gameObject.transform.LookAt(LookAtObject.transform);
    }

    public void UIChangeSpeed()
    {
        AnimatedAngle = SpeedSlider.GetComponent<Slider>().value;
    }
    public float MaxAnimationSpeed = 1000;
    public void UIChangeAnimationSpeed()
    {
        Slider s = AnimationSpeedSlider.GetComponent<Slider>();
        SolarSystem.GetComponent<SolarSystemHandler>().TimeCustomMultiplier = s.value == 0 ? 0 : 1 + boostPercent(s.value) * MaxAnimationSpeed;
    }

    public GameObject GlobePanelPrefab = null;
    public ScrollRect MainScrollRect;
    public ScrollRect ChildScrollRect;
    public ScrollRect SystemScrollRect;
    public GameObject[] OnOffPanels;
    private string _mainObjectName = "";
    public void ShowMainObjects()
    {
        if (MainScrollRect.gameObject.transform.parent.gameObject.activeSelf)
            RemoveListOfGlobe(MainScrollRect);
        else
        {
            if(ChildScrollRect.gameObject.transform.parent.gameObject.activeSelf) RemoveListOfGlobe(ChildScrollRect);
            ListOfGlobe(MainScrollRect, new List<string> { "Star", "Planet", "AsteroidField" }, new List<string> { "SolarSystem", "Sun", "AsteroidField" });
        }
    }
    public void ShowChildObjects()
    {
        if (ChildScrollRect.gameObject.transform.parent.gameObject.activeSelf)
            RemoveListOfGlobe(ChildScrollRect);
        else
        {
            if (MainScrollRect.gameObject.transform.parent.gameObject.activeSelf) RemoveListOfGlobe(MainScrollRect);
            if (!_mainObjectName.Equals("")) ListOfGlobe(ChildScrollRect, new List<string> { "Moon", "Ring", "Asteroida" }, new List<string> { _mainObjectName });
        }
    }
    public void ListOfGlobe(ScrollRect sr, List<string> tags, List<string> parents)
    {
        sr.gameObject.transform.parent.gameObject.SetActive(true);
        //sr.content.GetComponent<AspectRatioFitter>().aspectRatio = 1;
        float cc = 0;
        foreach (GameObject g in solarHandler.Targets.Values)
        {
            PlanetHandler h = g.GetComponent<PlanetHandler>();
            if(parents.Contains(h.ParentName) && tags.Contains(h.Category))
            {
                Instantiate(GlobePanelPrefab, sr.content).GetComponent<GlobePanel>().Init(g, ShowGlobe);
                cc++;
            }
        }
        if (cc == 0) sr.gameObject.transform.parent.gameObject.SetActive(false);
    }
    public void RemoveListOfGlobe(ScrollRect sr)
    {
        foreach (GlobePanel g in sr.content.GetComponentsInChildren<GlobePanel>())
        {
            g.Remove();
            DestroyImmediate(g.gameObject);
        }
        if (sr.content.childCount == 0) sr.gameObject.transform.parent.gameObject.SetActive(false);
    }
    public void ShowGlobe(GameObject gtarget)
    {
        GameObject g = gtarget;
        if (g == null) return;
        PlanetHandler p = g.GetComponent<PlanetHandler>();
        if(p != null && p.Category == "Planet")
            _mainObjectName = g.name;
        Target = g;
        LookAtObject = Target;
        Distant = Distant;
        this.gameObject.transform.LookAt(LookAtObject.transform);
    }
    public void SystemShow()
    {
        if (SystemScrollRect.gameObject.transform.parent.gameObject.activeSelf)
        {
            ClearSystemSettings();
            SystemScrollRect.gameObject.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            SystemScrollRect.gameObject.transform.parent.gameObject.SetActive(true);
            ClearSystemSettings();
            AddSystemSettings(
                new string[] { "Set resolution ALL object", "Set resolution CURRENT object", (solarHandler.SystemMode != SystemModes.Realistic ? "Realistic system" : "Smart system")  },
                new string[] { "SetResolutionAll", "SetResolutionCurrent", (solarHandler.SystemMode != SystemModes.Realistic ? "Realistic" : "Smart" )});
        }
    }
    private void ClearSystemSettings()
    {
        foreach (CustomButton g in SystemScrollRect.content.GetComponentsInChildren<CustomButton>())
        {
            DestroyImmediate(g.gameObject);
        }
    }
    public GameObject CustomButtonPrefab;
    private void AddSystemSettings(string[] name, string[] code)
    {
        for (int i = 0; i < name.Length; ++i)
        {
            Instantiate(CustomButtonPrefab, SystemScrollRect.content).GetComponent<CustomButton>().Init(name[i], code[i], SystemOptionClick);
        }
    }
    public void SystemOptionClick(GameObject sender, string code, string param)
    {
        if(code.Equals("Smart"))
        {
            if(solarHandler.SystemMode != SystemModes.Smart)
            {
                solarHandler.SystemMode = SystemModes.Smart;
                solarHandler.ResetAll = true;
            }
            SystemShow();
        }
        if (code.Equals("Realistic"))
        {
            if (solarHandler.SystemMode != SystemModes.Realistic)
            {
                solarHandler.SystemMode = SystemModes.Realistic;
                solarHandler.ResetAll = true;
            }
            SystemShow();
        }
        if (code.Equals("SetResolutionAll"))
        {
            ClearSystemSettings();
            AddSystemSettings(
                new string[] { "Low", "Medium", "High" },
                new string[] { "LowAll", "MediumAll", "HighAll" });
        }
        if (code.Equals("SetResolutionCurrent"))
        {
            ClearSystemSettings();
            AddSystemSettings(
                new string[] { "Low", "Medium", "High" },
                new string[] { "LowCurrent", "MediumCurrent", "HighCurrent" });
        }
        if (code.Equals("LowAll")) SetResulutionTypeAll(ObjectHandler.ResulutionTypes.LO);
        if (code.Equals("MediumAll")) SetResulutionTypeAll(ObjectHandler.ResulutionTypes.MID);
        if (code.Equals("HighAll")) SetResulutionTypeAll(ObjectHandler.ResulutionTypes.HI);
        if (code.Equals("LowCurrent"))
        {
            PlanetHandler p = Target.GetComponent<PlanetHandler>();
            if (p != null) p.ResulutionType = ObjectHandler.ResulutionTypes.LO;
        }
        if (code.Equals("MediumCurrent"))
        {
            PlanetHandler p = Target.GetComponent<PlanetHandler>();
            if (p != null) p.ResulutionType = ObjectHandler.ResulutionTypes.MID;
        }
        if (code.Equals("HighCurrent"))
        {
            PlanetHandler p = Target.GetComponent<PlanetHandler>();
            if(p != null) p.ResulutionType = ObjectHandler.ResulutionTypes.HI;
        }
    }
    private void SetResulutionTypeAll(ObjectHandler.ResulutionTypes r)
    {
        foreach (GameObject g in targets.Values)
        {
            PlanetHandler ph = g.GetComponent<PlanetHandler>();
            if (ph != null) ph.ResulutionType = r;
        }
    }
    public void UIOnOff(GameObject obj)
    {
        obj.SetActive(!obj.activeSelf);
        if(!obj.activeSelf)
            for (int i = 0; i < OnOffPanels.Length; ++i) OnOffPanels[i].SetActive(false);
    }
    public void PanelOnOff(GameObject g)
    {
        g.SetActive(!g.activeSelf);
    }

    public void FlightMode()
    {
        this.freeMove = !this.freeMove;
    }
}
