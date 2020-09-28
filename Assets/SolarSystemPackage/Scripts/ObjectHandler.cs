using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ObjectHandler : MonoBehaviour {
    public enum ResulutionTypes { Default, LO, MID, HI }
    public ResulutionTypes ResulutionType = ResulutionTypes.HI;
    protected ResulutionTypes lastResulutionType = ResulutionTypes.HI;
    protected static ResulutionTypes DEFAULTResulutionType = ResulutionTypes.HI;
    public ResulutionTypes MobileMeshResulutionType = ResulutionTypes.Default;
    public ResulutionTypes MobileTextureResulutionType = ResulutionTypes.Default;
    public ResulutionTypes MobileResulutionType = ResulutionTypes.Default;
    public static ResulutionTypes DefaultMobileMeshResulutionType = ResulutionTypes.Default;
    public static ResulutionTypes DefaultMobileTextureResulutionType = ResulutionTypes.Default;
    public static ResulutionTypes DefaultMobileResulutionType = ResulutionTypes.Default;
    public static string DefaultMaterialName = "MaterialDefault";
    public string MaterialName = "";
    protected string lastMaterialName = "";
    public static string DefaultTextureName = "Default";
    public string TextureName = "";
    protected string lastTextureName = "";
    public static string DefaultMeshName = "Sphere";
    public string MeshName = "";
    protected string lastMeshName = "";


    protected static Hashtable textures = new Hashtable();
    /// <summary>
    /// Textures in the resuurce folder
    /// </summary>
    public static Hashtable Textures
    {
        get
        {
            if(textures.Count == 0)
            {
                Object[] tobj = Resources.LoadAll("Texture", typeof(Texture));

                foreach (var t in tobj)
                {
                    textures.Add(t.name,t);
                }
            }
            return textures;
        }
    }
    protected static Hashtable meshes = new Hashtable();
    /// <summary>
    /// Textures in the resuurce folder
    /// </summary>
    public static Hashtable Meshes
    {
        get
        {
            if (meshes.Count == 0)
            {
                Object[] tobj = Resources.LoadAll("Mesh", typeof(Mesh));

                foreach (var t in tobj)
                {
                    meshes.Add(t.name, t);
                }
            }
            return meshes;
        }
    }
    public class MeshClass
    {
        public Mesh mesh = null;
        public List<GameObject> children = new List<GameObject>();
        public MeshClass(GameObject g, Mesh m)
        {
            mesh = m;
            children.Add(g);
        }
    }
    public static Dictionary<string, MeshClass> SharedMeshes = new Dictionary<string, MeshClass>();
    public void SetMesh(GameObject g, Mesh m)
    {
        MeshFilter mf = g.GetComponent<MeshFilter>();
        string key = "";
        if (mf == null)
            mf = g.AddComponent<MeshFilter>();
        else
        {
            key = mf.sharedMesh.name;
            if (SharedMeshes.ContainsKey(key) && mf.sharedMesh.name != m.name) SharedMeshes[key].children.Remove(g);
        }
        //if (sharedMeshes.ContainsKey(mf.mesh.name) && mf.mesh.name != m.name) sharedMeshes[mf.mesh.name].children.Remove(g);
        if (m != null && key != m.name)
        {
            MeshClass mc = null;
            if (!SharedMeshes.ContainsKey(m.name))
            {
                mc = new MeshClass(g, Instantiate(Instantiate(m)));
                mf.sharedMesh = mc.mesh;
                SharedMeshes.Add(m.name, mc);
            }
            else
            {
                mc = SharedMeshes[m.name];
                mc.children.Add(g);
                mf.sharedMesh = mc.mesh;
            }
        }
        //Debug.Log("sharedMeshes.Count:" + sharedMeshes.Count + "  Mesh:"+m.name);
        MeshRenderer mr = g.GetComponent<MeshRenderer>();
        if (mr == null) mr = g.AddComponent<MeshRenderer>();
    }
    public void SetCollider(GameObject g, bool addCollider, bool addRigibody)
    {
        if (addCollider)
        {
            SphereCollider sc = g.GetComponent<SphereCollider>();
            if (sc == null) sc = g.AddComponent<SphereCollider>();
            sc.center = Vector3.zero;
            sc.radius = 0.5f;
        }
        if (addRigibody)
        { 
            Rigidbody rb = g.GetComponent<Rigidbody>();
            if (rb == null) rb = g.AddComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeAll;
            rb.useGravity = false;
        }
    }

    protected static Hashtable materials = new Hashtable();
    /// <summary>
    /// Textures in the resuurce folder
    /// </summary>
    public static Hashtable Materials
    {
        get
        {
            if (materials.Count == 0)
            {
                Object[] tobj = Resources.LoadAll("Material", typeof(Material));

                foreach (var t in tobj)
                {
                    materials.Add(t.name, t);
                }
            }
            return materials;
        }
    }

    protected static Hashtable prefabs = new Hashtable();
    /// <summary>
    /// Textures in the resuurce folder
    /// </summary>
    public static Hashtable Prefabs
    {
        get
        {
            if (prefabs.Count == 0)
            {
                Object[] tobj = Resources.LoadAll("Prefab", typeof(GameObject));

                foreach (var t in tobj)
                {
                    prefabs.Add(t.name, t);
                }
            }
            return prefabs;
        }
    }

    public static Texture GetTextureByName(string name, ResulutionTypes resulutionType)
    {
        string key = name + ResolutionDecode(resulutionType);
        if (Textures.ContainsKey(key)) return (Texture)Textures[key];
        else if (Textures.ContainsKey(name)) return (Texture)Textures[name];
        else if (Textures.ContainsKey(DefaultTextureName + ResolutionDecode(resulutionType))) return (Texture)Textures[DefaultTextureName + ResolutionDecode(resulutionType)];
        else if (Textures.ContainsKey(DefaultTextureName)) return (Texture)Textures[DefaultTextureName];
        return null;
    }
    public static Mesh GetMeshByName(string name, ResulutionTypes resulutionType)
    {
        string key = name + ResolutionDecode(resulutionType);
        if (Meshes.ContainsKey(key)) return (Mesh)Meshes[key];
        if (Meshes.ContainsKey(name)) return (Mesh)Meshes[name];
        if (Meshes.ContainsKey(DefaultMeshName + ResolutionDecode(resulutionType))) return (Mesh)Meshes[DefaultMeshName + ResolutionDecode(resulutionType)];
        if (Meshes.ContainsKey(DefaultMeshName)) return (Mesh)Meshes[DefaultMeshName];
        return null;
    }
    public static Material GetMaterialByName(string name, ResulutionTypes ResulutionType)
    {
        string key = "";
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            key = name + "-Mobile" + ResolutionDecode(ResulutionType);
            if (Materials.ContainsKey(key)) return (Material)Materials[key];
            else if (Materials.ContainsKey(name + "-Mobile")) return (Material)Materials[name + "-Mobile"];
        }
        key = name + ResolutionDecode(ResulutionType);
        if (Materials.ContainsKey(key)) return (Material)Materials[key];
        else if (Materials.ContainsKey(name)) return (Material)Materials[name];
        else if (Materials.ContainsKey(DefaultMaterialName)) return (Material)Materials[DefaultMaterialName];
        return null;
    }
    public static string DefaultPrefab = "DefaultPrefab";
    public static GameObject GetPrefabByName(string name, ResulutionTypes ResulutionType)
    {
        string key = name + ResolutionDecode(ResulutionType);
        if (Prefabs.ContainsKey(key)) return (GameObject)Prefabs[key];
        else if (Prefabs.ContainsKey(name)) return (GameObject)Prefabs[name];
        else if (Prefabs.ContainsKey(DefaultPrefab)) return (GameObject)Prefabs[DefaultPrefab];
        return null;
    }
    public static string ResolutionDecode(ResulutionTypes r)
    {
        if (r == ResulutionTypes.LO) return "-Lo";
        if (r == ResulutionTypes.MID) return "-Mi";
        if (r == ResulutionTypes.HI) return "-Hi";
        return "";
    }
    public ResulutionTypes CurrentResulutionType
    {
        get
        {
            if (IsMobil && MobileResulutionType != ResulutionTypes.Default) return MobileResulutionType;
            if (IsMobil && DefaultMobileResulutionType != ResulutionTypes.Default) return DefaultMobileResulutionType;
            return ResulutionType;
        }
    }
    public ResulutionTypes CurrentMeshResulutionType
    {
        get
        {
            if (IsMobil && MobileMeshResulutionType != ResulutionTypes.Default) return MobileMeshResulutionType;
            if (IsMobil && DefaultMobileMeshResulutionType != ResulutionTypes.Default) return DefaultMobileMeshResulutionType;
            return ResulutionType;
        }
    }
    public ResulutionTypes CurrentTextureResulutionType
    {
        get
        {
            if (IsMobil && MobileTextureResulutionType != ResulutionTypes.Default) return MobileTextureResulutionType;
            if (IsMobil && DefaultMobileTextureResulutionType != ResulutionTypes.Default) return DefaultMobileTextureResulutionType;
            return ResulutionType;
        }
    }
    public bool IsMobil
    {
        get
        {
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
                return true;
            return false;
        }
    }
    private GameObject _SolarSystem;
    /// <summary>
    /// The whole Solar System object.
    /// </summary>
    protected GameObject SolarSystem
    {
        get
        {
            if (_SolarSystem == null)
            {
                _SolarSystemHandler = this.gameObject.GetComponentInParent<SolarSystemHandler>();
                _SolarSystem = _SolarSystemHandler.gameObject;
            }
            return _SolarSystem;
        }
        set { _SolarSystem = value; }
    }
    private SolarSystemHandler _SolarSystemHandler;
    public SolarSystemHandler SolarSystemHandler
    {
        get
        {
            if (SolarSystem == null) _SolarSystemHandler = null;
            return _SolarSystemHandler;
        }
        set
        {
            if (_SolarSystemHandler != null) _SolarSystemHandler = value;
        }
    }

    public void ChangeMaterial(string name, GameObject go, Material newMat)
    {
        Texture _diffuse = GetTextureByName(name, CurrentTextureResulutionType);
        Texture _normal = GetTextureByName(name + "-Normal", CurrentTextureResulutionType);
        Texture _bump = GetTextureByName(name + "-Bump", CurrentTextureResulutionType);
        ChangeMaterial(go, newMat, _diffuse, _normal, _bump);
        lastResulutionType = CurrentResulutionType;
    }
    public void ChangeMaterial(GameObject go, Material newMat, Texture diffuse, Texture normal, Texture bump)
    {
        MeshRenderer mRenderer = go.GetComponent<MeshRenderer>();
        Material mMaterial = new Material(newMat);
        Material[] materials = new Material[1];
        if (diffuse != null) mMaterial.SetTexture("_MainTex", diffuse);
        if (bump != null) mMaterial.SetTexture("_BumpMap", bump);
        if (normal != null) mMaterial.SetTexture("_BumpMap", normal);
        materials[0] = mMaterial;
        mRenderer.materials = materials;
        lastResulutionType = CurrentResulutionType;
    }
    static public System.Type GetDeclaredType<TSelf>(TSelf self)
    {
        return typeof(TSelf);
    }
    public object GetValueByString<T>(ref T param, string value)
    {
        System.Type typeParameterType = GetDeclaredType(param);
        if (param != null) typeParameterType = param.GetType();
        if (typeParameterType.ToString() == "UnityEngine.Vector3")
        {
            float x, y, z;
            float.TryParse(value.Split(TagValues[0])[0], out x);
            float.TryParse(value.Split(TagValues[0])[1], out y);
            float.TryParse(value.Split(TagValues[0])[2], out z);
            return new Vector3(x,y,z);
        }
        else if (typeParameterType.ToString() == "UnityEngine.Material")
        {
            return Resources.Load<Material>(@"Materials\"+value);
        }
        else if (typeParameterType.ToString() == "UnityEngine.Texture")
        {
            return Resources.Load<Texture>(@"Textures\" + value);
        }
        else if (typeParameterType.ToString() == "UnityEngine.GameObject")
        {
            GameObject g = Resources.Load<GameObject>(@"Prefabs\" + value);
            if (g == null)
            {
                if (SolarSystemHandler != null && SolarSystemHandler.Targets.ContainsKey(value)) g = SolarSystemHandler.Targets[value];
            }
            return g;
        }
        return (T)System.Convert.ChangeType(value, typeof(T));
    }
    public void SetValueArray<T>(ref T[] param, string[] value)
    {
        param = new T[value.Length];
        for (int i = 0; i < value.Length; ++i) param[i] = (T)GetValueByString(ref param[i], value[i]);
    }
    public void SetValue<T>(ref T param, string value)
    {
        if(!value.Equals(""))
            param = (T)GetValueByString(ref param, value);
    }
    public void SetValueResulutionTypes(ref ResulutionTypes param, string value)
    {
        if (value.Equals("")) param = ResulutionTypes.Default;
        if (value.Equals("Default")) param = ResulutionTypes.Default;
        if (value.Equals("LO")) param = ResulutionTypes.LO;
        if (value.Equals("MID")) param = ResulutionTypes.MID;
        if (value.Equals("HI")) param = ResulutionTypes.HI;
    }
    private string[] _tag = new string[] { "", "\r\n", "=", ";", "," };
    public string TagFile { get { return _tag[0]; } }
    public string TagAttr { get { return _tag[1]; } }
    public string TagValue { get { return _tag[2]; } }
    public string TagValueDatas { get { return _tag[3]; } }
    public string TagValues { get { return _tag[4]; } }
    public string GetStrData(string name, Vector3 data)
    {
        if (data == Vector3.zero) return "";
        return GetStrData(name, data.x.ToString()+TagValues+ data.y.ToString()+TagValues+ data.z.ToString());
    }
    public string GetStrData(string name, float data)
    {
        if (data == 0) return "";
        return GetStrData(name, data.ToString());
    }
    public string GetStrData(string name, bool data)
    {
        return GetStrData(name, data.ToString());
    }
    public string GetStrData(string name, GameObject data)
    {
        if (data == null) return "";
        return GetStrData(name, data.name);
    }
    public string GetStrData(string name, Material data)
    {
        if (data == null) return "";
        return GetStrData(name, data.name);
    }
    public string GetStrData(string name, Texture data)
    {
        if (data == null) return "";
        return GetStrData(name, data.name);
    }
    public string GetStrData(string name, PlanetMetric data)
    {
        if (data == PlanetMetric.AsSolarSystem) return "";
        return GetStrData(name, data.ToString());
    }
    public string GetStrData(string name, string data)
    {
        if (data == "") return "";
        return name + TagValue + data + TagAttr;
    }
    public string GetStrArrayData(string name, Material[] data)
    {
        string[] s = new string[data.Length];
        for (int i = 0; i < data.Length; ++i) s[i] = data[i].name;
        return GetStrArrayData(name, s);
    }
    public string GetStrArrayData(string name, Texture[] data)
    {
        string[] s = new string[data.Length];
        for (int i = 0; i < data.Length; ++i) s[i] = data[i].name;
        return GetStrArrayData(name, s);
    }
    public string GetStrArrayData(string name, GameObject[] data)
    {
        string[] s = new string[data.Length];
        for (int i = 0; i < data.Length; ++i) s[i] = data[i].name;
        return GetStrArrayData(name, s);
    }
    public string GetStrArrayData(string name, Vector3[] data)
    {
        string[] s = new string[data.Length];
        for (int i = 0; i < data.Length; ++i) s[i] = data[i].x.ToString() + TagValues + data[i].y.ToString() + TagValues + data[i].z.ToString();
        return GetStrArrayData(name, s);
    }
    public string GetStrArrayData(string name, float[] data)
    {
        string[] s = new string[data.Length];
        for (int i = 0; i < data.Length; ++i) s[i] = data[i].ToString();
        return GetStrArrayData(name, s);
    }
    public string GetStrArrayData(string name, string[] data)
    {
        if (data.Length == 0) return "";
        string back = "";
        if (data.Length > 0)
        {
            back = data[0];
            for (int i = 1; i < data.Length; ++i) { back += TagValueDatas + data[i]; }
        }
        return name + TagValue + back + TagAttr;
    }

}
