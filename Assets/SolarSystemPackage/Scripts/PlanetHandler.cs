using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PlanetHandler : ObjectHandler {
    /// <summary>
    /// Globe categories
    /// </summary>
    public enum GlobeCategories { Star, Planet, Moon, Ring, AsteroidField, Asteroid, Clouds, Ship }
    /// <summary>
    /// Planet Name
    /// </summary>
    public string Name = "";
    /// <summary>
    /// Category
    /// </summary>
    public string Category = "planet";
    /// <summary>
    /// Planet radius (km)
    /// </summary>
    public float Radius = 0;
    public float CustomRadius = 0;
    public float CustomGroupRadius = 0;
    /// <summary>
    /// Planet axial tilt in euler angles
    /// </summary>
    public Vector3 AxialTilt = Vector3.zero;
    public Vector3 RotationAxial = Vector3.zero;
    /// <summary>
    /// round / day
    /// </summary>
    public float Rotation = 0;
    public GameObject Parent = null;
    public string ParentName = "";
    public Vector3 Direction = Vector3.right;
    public float Distant = 0;
    public float CustomDistant = 0;
    public float ScaleRate = 1;
    public float Period = 0;
    public Vector3 PeriodDirection = Vector3.up;

    private int subIndex = 0;
    public GameObject NextSubTarget
    {
        get {
            subIndex++;
            if (this.gameObject.GetComponentsInChildren<AsteroidHandler>().Length >= subIndex) subIndex = 0;
            return this.gameObject.GetComponentsInChildren<AsteroidHandler>()[subIndex].gameObject;
        }
    }
    public GameObject SubTarget
    {
        get
        {
            return this.gameObject.GetComponentsInChildren<AsteroidHandler>()[subIndex].gameObject;
        }
    }

    public PlanetMetric DistantMetric = PlanetMetric.AsSolarSystem;
    public float DistantCustomMultiplier = 1;
    /// <summary>
    /// value = km * DistantMultiplier
    /// uses for diameter and distant
    /// </summary>
    public float DistantMultiplier
    {
        get {
            if (DistantMetric == PlanetMetric.AsSolarSystem) return SolarSystemHandler.DistantMultiplier;
            if (DistantMetric == PlanetMetric.AsRoundAbout) return roundAbout.DistantMultiplier;
            return DistantCustomMultiplier;
        }
    }
    public PlanetMetric TimeMetric = PlanetMetric.AsSolarSystem;
    public float TimeCustomMultiplier = 1;
    /// <summary>
    /// value = second * TimeMultiplier
    /// uses for round and around
    /// </summary>
    public float TimeMultiplier
    {
        get
        {
            if (TimeMetric == PlanetMetric.AsSolarSystem) return SolarSystemHandler.TimeMultiplier;
            if (TimeMetric == PlanetMetric.AsRoundAbout) return roundAbout.TimeMultiplier;
            return TimeCustomMultiplier;
        }
    }
    /// <summary>
    /// The Planet to Rotating.
    /// </summary>
    //public GameObject PlanetObject;

    /// <summary>
    /// Working or not.
    /// </summary>
    public bool AnimationEnabled = true;

    /// <summary>
    /// Reset position (0,0,0),rotation (0,0,0), scale(1,1,1) relative by SolarSystem, then calculate them by attributes and go.
    /// </summary>
    public bool AnimationReset = false;

    /// <summary>
    /// AutoReset.
    /// </summary>
    public bool AutoReset = false;
    #region private variables
    private PlanetHandler _roundAbout;
    private PlanetHandler roundAbout
    {
        get
        {
            if (_roundAbout == null) if (Parent != null) _roundAbout = Parent.GetComponent<PlanetHandler>();
            return _roundAbout;
        }
        set
        {
            if (_roundAbout != null) _roundAbout = value;
        }
    }
    #endregion

    // Use this for initialization
    void Start () {
        lastResulutionType = DEFAULTResulutionType;
        ResulutionType = DEFAULTResulutionType;
        if (AutoReset) ResetPlanet();
    }
    public bool SaveToFile = false;
	void Update () {
        if (AnimationReset)
            ResetPlanet();
        if(AnimationEnabled && SolarSystemHandler.AnimationEnabled)
            UpdatePosition();
        if (lastResulutionType != ResulutionType)
            ChangeResolution();
        if (SaveToFile)
            System.IO.File.WriteAllText("Planet_" + this.Name + ".txt", GetPlanetData());
    }

    public string GetPlanetData()
    {
        SaveToFile = false;
        System.Text.StringBuilder b = new System.Text.StringBuilder();
        b.Append(GetStrData("Name", Name));
        b.Append(GetStrData("Category", Category));
        b.Append(GetStrData("Radius", Radius));
        b.Append(GetStrData("AxialTilt", AxialTilt));
        b.Append(GetStrData("Rotation", Rotation));
        b.Append(GetStrData("Parent", ParentName));
        b.Append(GetStrData("Distant", Distant));
        b.Append(GetStrData("Period", Period));
        b.Append(GetStrData("MaterialName", MaterialName));
        b.Append(GetStrData("TextureName", TextureName));
        b.Append(GetStrData("MeshName", MeshName));
        b.Append(GetStrData("Direction", Direction));
        b.Append(GetStrData("PeriodDirection", PeriodDirection));
        b.Append(GetStrData("ScaleRate", ScaleRate));
        
        return b.ToString();

    }
    //public static string DefaultName= "Name";
    //public static string DefaultCategory = "planet";
    //public static float DefaultRadius = 0;
    //public static Vector3 DefaultAxialTilt = Vector3.zero;
    //public static float DefaultRotation = 0;
    //public static string DefaultParentName = "";
    //public static Vector3 DefaultDirection = Vector3.right;
    //public static float DefaultDistant = 0;
    //public static float DefaultScaleRate = 1;
    //public static float DefaultPeriod = 0;
    //public static Vector3 DefaultPeriodDirection = Vector3.up;

    public int AsteroidaCount = 0;
    public int AsteroidaCountRnd = 0;
    public int AsteroidaMeshRnd = 0;
    public int AsteroidaNaterialRnd = 0;
    public int AsteroidaTextureRnd = 0;
    public int AsteroidaNormalRnd = 0;
    public float AsteroidaSizeMin = 0;
    public float AsteroidaSizeMax = 0;
    public float AsteroidaAngleMin = 0;
    public float AsteroidaAngleMax = 360;

    public void CreateDataByFile(List<string> data)
    {
        foreach (string a in data)
        {
            AddParameter(a);
        }
    }
    public void AddParameter(string a)
    {
        string[] s = a.Split(TagValue[0]);
        string n = s[0].Trim();
        if (n == "Name")
        {
            PeriodDirection = new Vector3(1, 0, 0);
            Direction = new Vector3(Random.Range(-40, 40), Random.Range(0, 0), Random.Range(-10, 10));
            SetValue(ref Name, s[1]);
        }
        else if (n == "ResulutionType") SetValueResulutionTypes(ref ResulutionType, s[1]);
        else if (n == "DefaultMobileMeshResulutionType") SetValueResulutionTypes(ref DefaultMobileMeshResulutionType, s[1]);
        else if (n == "DefaultMobileTextureResulutionType") SetValueResulutionTypes(ref DefaultMobileTextureResulutionType, s[1]);
        else if (n == "DefaultMobileResulutionType") SetValueResulutionTypes(ref DefaultMobileResulutionType, s[1]);
        else if (n == "MobileMeshResulutionType") SetValueResulutionTypes(ref MobileMeshResulutionType, s[1]);
        else if (n == "MobileTextureResulutionType") SetValueResulutionTypes(ref MobileTextureResulutionType, s[1]);
        else if (n == "MobileResulutionType") SetValueResulutionTypes(ref MobileResulutionType, s[1]);
        else if (n == "Category") SetValue(ref Category, s[1]);
        else if (n == "Radius") SetValue(ref Radius, s[1]);
        else if (n == "AxialTilt") SetValue(ref AxialTilt, s[1]);
        else if (n == "Rotation") SetValue(ref Rotation, s[1]);
        else if (n == "Parent") SetValue(ref ParentName, s[1]);
        else if (n == "Distant") SetValue(ref Distant, s[1]);
        else if (n == "Period") SetValue(ref Period, s[1]);
        else if (n == "DefaultMaterialName") SetValue(ref DefaultMaterialName, s[1]);
        else if (n == "DefaultTextureName") SetValue(ref DefaultTextureName, s[1]);
        else if (n == "DefaultMeshName") SetValue(ref DefaultMeshName, s[1]);
        else if (n == "MaterialName") SetValue(ref MaterialName, s[1]);
        else if (n == "TextureName") SetValue(ref TextureName, s[1]);
        else if (n == "MeshName") SetValue(ref MeshName, s[1]);
        else if (n == "Direction") SetValue(ref Direction, s[1]);
        else if (n == "PeriodDirection") SetValue(ref PeriodDirection, s[1]);
        else if (n == "ScaleRate") SetValue(ref ScaleRate, s[1]);
        else if (n == "AsteroidaCount") SetValue(ref AsteroidaCount, s[1]);
        else if (n == "AsteroidaCountRnd") SetValue(ref AsteroidaCountRnd, s[1]);
        else if (n == "AsteroidaMeshRnd") SetValue(ref AsteroidaMeshRnd, s[1]);
        else if (n == "AsteroidaNaterialRnd") SetValue(ref AsteroidaNaterialRnd, s[1]);
        else if (n == "AsteroidaTextureRnd") SetValue(ref AsteroidaTextureRnd, s[1]);
        else if (n == "AsteroidaNormalRnd") SetValue(ref AsteroidaNormalRnd, s[1]);
        else if (n == "AsteroidaSizeMin") SetValue(ref AsteroidaSizeMin, s[1]);
        else if (n == "AsteroidaSizeMax") SetValue(ref AsteroidaSizeMax, s[1]);
        else if (n == "AsteroidaAngleMin") SetValue(ref AsteroidaAngleMin, s[1]);
        else if (n == "AsteroidaAngleMax") SetValue(ref AsteroidaAngleMax, s[1]);
        else if (n == "CREATE") ResetPlanet(); 
    }
    private void ChangeResolution()
    {
        Mesh mm = GetMeshByName( MeshName.Equals("") ? Name : MeshName, CurrentMeshResulutionType);
        bool addCollider = !Category.Equals("Ring") && !Category.Equals("Clouds");
        SetMesh(this.gameObject, mm);
        SetCollider(this.gameObject, addCollider, addCollider);
        ChangeMaterial(TextureName.Equals("") ? Name : TextureName, gameObject, GetMaterialByName(MaterialName, CurrentResulutionType));
    }
    public float CurrentDistant
    {
        get
        {
            return GetCurrentDistant(SolarSystemHandler.SystemMode);
        }
    }
    public float GetCurrentDistant(SystemModes m)
    {
        if (m == SystemModes.Smart)
        {
            return CustomDistant * DistantMultiplier;
        }
        return Distant * DistantMultiplier;
    }
    public float CurrentRadius
    {
        get
        {
            return GetCurrentRadius(SolarSystemHandler.SystemMode);
        }
    }
    public float GetCurrentRadius(SystemModes m)
    {
        if (m == SystemModes.Smart)
        {
            return DistantMultiplier * CustomRadius * 2;
        }
        return DistantMultiplier * Radius * 2; ;
    }
    /// <summary>
    /// Reset the Planet object by attributes
    /// </summary>
    public void ResetPlanet()
    {
        AnimationReset = false;
        // Uncomment if yout installed Legacy Image Effect package
        //if (Category.Equals("Star")) SolarSystemHandler.MainCamera.gameObject.GetComponent<UnityStandardAssets.ImageEffects.SunShafts>().sunTransform = this.gameObject.transform;
        if (Name == "") Name = this.gameObject.name;
        else this.gameObject.name = Name;

        // Set parent, if no parent then SolarSystem will be
        this.transform.parent = SolarSystem.transform;
        if (Parent == null)
        {
            Parent = SolarSystem;
            if (!ParentName.Equals(""))
            {
                foreach (PlanetHandler ph in SolarSystem.GetComponentsInChildren<PlanetHandler>())
                {
                    if (ph.gameObject.name.Equals(ParentName))
                    {
                        Parent = ph.gameObject;
                        break;
                    }
                }
            }
        }
        if (Parent != null)
        {
            ParentName = Parent.gameObject.name;
        }
        if (Category == "AsteroidField")
        {
            gameObject.transform.localScale = Vector3.one;
            Vector3 globalScale = new Vector3(1, 1, 1) * ScaleRate;
            gameObject.transform.localScale = new Vector3(globalScale.x / gameObject.transform.lossyScale.x, globalScale.y / gameObject.transform.lossyScale.y, globalScale.z / gameObject.transform.lossyScale.z);
            CreateAsteroids(AsteroidaCount, MeshName, AsteroidaMeshRnd, MaterialName, AsteroidaNaterialRnd, TextureName, AsteroidaTextureRnd, TextureName, AsteroidaNormalRnd, 
                    AsteroidaSizeMin, AsteroidaSizeMax, AsteroidaAngleMin, AsteroidaAngleMax);
        }
        else
        {
            gameObject.transform.localScale = Vector3.one;
            Vector3 globalScale = new Vector3(CurrentRadius, CurrentRadius, CurrentRadius) * ScaleRate;
            gameObject.transform.localScale = new Vector3(globalScale.x / gameObject.transform.lossyScale.x, globalScale.y / gameObject.transform.lossyScale.y, globalScale.z / transform.lossyScale.z);
            if (TextureName.Equals("")) TextureName = Name;
            ChangeResolution();
        }
    }
    public float counter = 0;
    public float axialAngle = 180;
    private Vector3 _lastAxial = Vector3.zero;
    private Quaternion currentAxial = Quaternion.identity;
    private Quaternion currentRotation = Quaternion.identity;
    public bool axialBool = true;
    public bool rotBool = true;
    private float xROUND = 0;
    private float yROUND = 0;
    /// <summary>
    /// Set current position, rotation, around from Update.
    /// </summary>
    private void UpdatePosition()
    {
        counter++;
        float angle = 0;
        {
            if (Rotation == 0)
            {
                angle = xROUND = 0;
            }
            else
            {
                angle = (360.0f / (Mathf.Abs(Rotation * 24 * 60 * 60) / TimeMultiplier)) * Time.deltaTime;
                xROUND += angle * Mathf.Sign(Rotation);
                if (Mathf.Abs(xROUND) > 360) xROUND -= 360 * Mathf.Sign(xROUND);
            }
            if (_lastAxial != AxialTilt)
            {
                if (AxialTilt == Vector3.zero)
                {
                    RotationAxial = -Vector3.up;
                    currentAxial = Quaternion.identity;
                }
                else
                {
                    currentAxial = Quaternion.Euler(AxialTilt);
                    Vector3 d = AxialTilt.normalized == Vector3.zero ? Vector3.one : AxialTilt.normalized;
                    RotationAxial = - ((d == Vector3.right) ? Vector3.Cross(d, Vector3.up) : Vector3.Cross(d, Vector3.right));
                }
                _lastAxial = AxialTilt;
            }
            Vector3 rot = RotationAxial.normalized * xROUND;
            if(rot != Vector3.zero) currentRotation = Quaternion.Euler(rot);
            else currentRotation = Quaternion.identity;
            Quaternion r = Quaternion.identity;
            if (rotBool && axialBool)
            {
                if (currentRotation == Quaternion.identity) r = currentAxial;
                if (currentAxial == Quaternion.identity) r = currentRotation;
                if (currentRotation != Quaternion.identity && currentAxial != Quaternion.identity) r = currentAxial * currentRotation;
            }
            if (!rotBool && axialBool) r = currentAxial;
            if (!axialBool && rotBool) r = currentRotation;
            if (!rotBool && !axialBool) r = Quaternion.identity;
            if(Category == "AsteroidField")
                this.gameObject.transform.SetPositionAndRotation(Parent.transform.position, r);
            else
                this.gameObject.transform.SetPositionAndRotation(Parent.transform.position + Direction.normalized * CurrentDistant, r);
        }

        if (Category != "AsteroidField")
        {
            if (Parent == null || Period == 0 || TimeMultiplier < 1) return;
            angle = (360.0f / (Mathf.Abs(Period * 24 * 60 * 60) / TimeMultiplier)) * Time.deltaTime;
            yROUND += angle * Mathf.Sign(Period);
            if (Mathf.Abs(yROUND) > 360) yROUND -= 360 * Mathf.Sign(yROUND);
            Vector3 d2 = Direction.normalized;
            Vector3 rotateDirection = -((d2 != PeriodDirection.normalized) ? Vector3.Cross(d2, PeriodDirection.normalized) : (d2 != Vector3.up ? Vector3.Cross(d2, Vector3.up) : Vector3.Cross(d2, Vector3.right)));
            this.gameObject.transform.RotateAround(Parent.transform.position, rotateDirection, yROUND);
        }
    }
    private float sizeMul = 1;
    public void CreateAsteroids(int count, string meshName, int meshRnd, string materialName, int materialRnd, string textureName, int textureRnd, string normalName, int normalRnd,
    float sizeMin, float sizeMax, float angleMin, float angleMax)
    {
        if (this.gameObject.transform.lossyScale == Vector3.zero) return;
        sizeMul = CurrentRadius / Radius;
        bool genNew = true;
        // Delete all object
        if (this.gameObject.GetComponentsInChildren<AsteroidHandler>().Length != count)
        {
            foreach (AsteroidHandler r in this.gameObject.GetComponentsInChildren<AsteroidHandler>())
            {
                DestroyImmediate(r.gameObject);
            }
        }
        else
            genNew = false;
        for (int i=0;i<count;++i)
        {
            GameObject g = null;
            AsteroidHandler a = null;
            if (genNew)
            {
                g = new GameObject();
                a = g.AddComponent<AsteroidHandler>();
                a.Parent = this.gameObject;
                //g.tag = "Asteroid";
                g.transform.parent = this.transform;
                g.name = this.transform.name + "_" + i.ToString();
            }
            else
            {
                a = this.GetComponentsInChildren<AsteroidHandler>()[i];
                g = a.gameObject;
            }
            float r = CurrentRadius;
            a.mesh_Name = meshName.Replace("#", Random.Range(1, meshRnd).ToString());
            a.material_Name = materialName.Replace("#", Random.Range(1, materialRnd).ToString());
            a.texture_Name = textureName.Replace("#", Random.Range(1, textureRnd).ToString());
            a.normal_Name = normalName.Replace("#", Random.Range(1, normalRnd).ToString());
            a.size = (new Vector3(Random.Range(sizeMin, sizeMax), Random.Range(sizeMin, sizeMax), Random.Range(sizeMin, sizeMax)) * sizeMul);
            a.angle = Random.Range(angleMin, angleMax);
            a.dist = (new Vector3(Random.Range(-r, r), Random.Range(-r, r), Random.Range(-r, r)));
            Material mat = GetMaterialByName(a.material_Name, CurrentResulutionType);

            Mesh mm = GetMeshByName(a.mesh_Name, CurrentMeshResulutionType);
            SetMesh(g, mm);

            g.transform.localScale = Vector3.one;
            a.lossyScale = g.transform.lossyScale;
            a.globalScale = a.size;
            a.Radius = CurrentRadius;
            a.Direction = Direction;
            
            g.transform.localScale = new Vector3(a.globalScale.x / a.lossyScale.x, a.globalScale.y / a.lossyScale.y, a.globalScale.z / a.lossyScale.z);
            g.transform.position = this.gameObject.transform.position + a.dist + a.Direction.normalized * CurrentDistant;
            g.transform.RotateAround(this.gameObject.transform.position, Vector3.up, a.angle);

            SetCollider(g, true, true);
            ChangeMaterial(a.texture_Name, g, mat);
        }
    }
}
