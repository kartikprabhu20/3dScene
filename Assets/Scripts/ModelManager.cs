using System.Collections;
using System.Collections.Generic;
using Dummiesman;
using UnityEngine;
using System.IO;
using System;
using System.Linq;

public class ModelManager : MonoBehaviour
{
    public Camera MainCamera;
    public Light lightSource;
    public int lightIntensityLow;
    public int lightIntensityHigh;
    public int lightXRotationLow;
    public int lightXRotationHigh;
    public int lightYRotationLow;
    public int lightYRotationHigh;


    public string[] categories;
    public GameObject[] categoryReference;
    public GameObject cameraPositions;
    public GameObject room;
    public GameObject walls;
    public GameObject floor;
    public List<Material> wallList;
    public List<Material> floorList;
    public List<Material> skyBoxList;

    public Shader shader;
    public string datapath;

    GameObject model;

    public GameObject testmodel;
    private System.Random random = new System.Random();

    private string[] roomPaths;
    private string[] roomMtlPaths;

    // Start is called before the first frame update
    void Start()
    {


        //Test for testmodel
        //Transform[] allChildren = testmodel.GetComponentsInChildren<Transform>();
        //foreach (Transform child in allChildren)
        //{
        //    attachMeshCollider(child.gameObject);
        //}
        //var currentBounds = GetMeshHierarchyBounds(testmodel);
        //var currentSize = currentBounds.size;
        //var categoryReferenceBounds = categoryReference[0].GetComponent<Renderer>().bounds;
        //var categoryReferenceSize = categoryReferenceBounds.size;
        ////float minimumNewSizeRatio = Math.Min(categoryReferenceSize.x / currentSize.x, Math.Min(categoryReferenceSize.y / currentSize.y, categoryReferenceSize.z / currentSize.z));
        //testmodel.transform.localScale = testmodel.transform.localScale * (categoryReferenceSize.y / currentSize.y);
    }

    

    public void randomizeEnvironment()
    {
        string rootRoomPath = "/Users/apple/OVGU/Thesis/scenenet/robotvault-downloadscenenet-cfe5ab85ddcc/rooms/";
        updateRoomPathList(rootRoomPath);

        try
        {
            model.SetActive(false);
            DestroyImmediate(model);
        }
        catch
        {
            Debug.Log("No model object to destroy");
        }

        randomiseSkybox();


        string objPath = "/Users/apple/OVGU/Thesis/scenenet/robotvault-downloadscenenet-cfe5ab85ddcc/1Bedroom/77_labels.obj";
        string mtlPath = "/Users/apple/OVGU/Thesis/scenenet/robotvault-downloadscenenet-cfe5ab85ddcc/1Bedroom/77_labels.mtl";

        int roomIndex = random.Next(roomPaths.Length);
        //string objPath = roomPaths[roomIndex];
        //string mtlPath = roomMtlPaths[roomIndex];

        model = new OBJLoader().Load(objPath, mtlPath);

        changeShader(model);
        applyTextures(model);
    }

    void updateRoomPathList(string rootRoomPath)
    {
        string[] dirList = Directory.GetDirectories(rootRoomPath);

        foreach (string dir in dirList)
        {
            roomPaths = Directory.GetFiles(dir, "*.obj");
            roomMtlPaths = Directory.GetFiles(dir, "*.mtl");

        }

    }

    void applyTextures(GameObject gameObject)
    {
        string rootTexturePath = "/Users/apple/OVGU/Thesis/scenenet/robotvault-downloadscenenet-cfe5ab85ddcc/texture_library";
        string[] dirList = Directory.GetDirectories(rootTexturePath);


        Transform[] allChildren = gameObject.GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            string textureFolder = rootTexturePath + Path.DirectorySeparatorChar + child.name.Split('.')[0];
            if (dirList.Contains(textureFolder))
            {
                string[] textureList = Directory.GetFiles(textureFolder);
                Renderer rend = child.GetComponent<Renderer>();
                if (rend != null)
                {
                    Debug.Log("applying texture");
                    rend.material.mainTexture = loadTexture(textureList[random.Next(textureList.Length)]);
                }
                //break;
            }
        }
    }

    Texture2D loadTexture(string filePath)
    {
        Texture2D texture = null;
        if (File.Exists(filePath))     {
            byte[] fileData = File.ReadAllBytes(filePath);
            texture = new Texture2D(2, 2);
            texture.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        }

        return texture;
    }

    public void createDataset()
    {
        try
        {

            model.SetActive(false);
            DestroyImmediate(model);
        }
        catch
        {
            Debug.Log("No model object to destroy");
        }
            
        StartCoroutine(LoadObjects());
    }

    float componentMax(Vector3 a)
    {
        return Mathf.Max(Mathf.Max(a.x, a.y), a.z);
    }

    float componentMin(Vector3 a)
    {
        return Mathf.Min(Mathf.Min(a.x, a.y), a.z);
    }

    public Bounds GetMeshHierarchyBounds(GameObject go)
    {
        var bounds = new Bounds(); // Not used, but a struct needs to be instantiated

        if (go.GetComponent<Renderer>() != null)
        {
            bounds = go.GetComponent<Renderer>().bounds;
            // Make sure the parent is included
            Debug.Log("Found parent bounds: " + bounds);
            //bounds.Encapsulate(go.renderer.bounds);
        }
        foreach (var c in go.GetComponentsInChildren<MeshRenderer>())
        {
            //Debug.Log("Found {0} bounds are {1}", c.name, c.bounds);
            if (bounds.size == Vector3.zero)
            {
                bounds = c.bounds;
            }
            else
            {
                bounds.Encapsulate(c.bounds);
            }
        }
        return bounds;
    }

    IEnumerator LoadObjects()
    {
        int categoryIndex = 0;
        foreach(string category in categories)
        {
            //Debug.Log(category);
            string categoryPath = datapath + Path.DirectorySeparatorChar + category;
            //Debug.Log(categoryPath);

            string[] folders = Directory.GetDirectories(categoryPath, "*", System.IO.SearchOption.TopDirectoryOnly);

            //To teest single model
            //string[] folders = { "/Users/apple/OVGU/Thesis/Dataset/pix3d/model/chair/IKEA_EKENAS/" };

            foreach (string folderPath in folders)
            {
                Debug.Log(folderPath);
                
                string objPath = Directory.GetFiles(folderPath, "*.obj")[0];
                try
                {
                    string mtlPath = Directory.GetFiles(folderPath, "*.mtl")[0];
                    model = new OBJLoader().Load(objPath, mtlPath);
                }
                catch
                {
                    model = new OBJLoader().Load(objPath);

                }
                model.SetActive(true);

                model.transform.position = new Vector3(0, 0, 0);
                model.AddComponent<MeshRenderer>();
                Rigidbody gameObjectsRigidBody = model.AddComponent<Rigidbody>();
                model.GetComponent<Rigidbody>().drag = 10;
                model.GetComponent<Rigidbody>().mass = 10;

                Vector3 sizeCalculated = model.GetComponent<Renderer>().bounds.size;
                Debug.Log(sizeCalculated);

                Vector3 sizeCalculated2= categoryReference[0].GetComponent<Renderer>().bounds.size;
                Debug.Log(sizeCalculated2);


                //Adding mesh collider to all child objects of model
                Transform[] allChildren = model.GetComponentsInChildren<Transform>();
                foreach (Transform child in allChildren)
                {
                    attachMeshCollider(child.gameObject);
                }

                if (categoryReference.Length != 0 && categories.Length == categoryReference.Length)
                {
                    var currentBounds = GetMeshHierarchyBounds(model);
                    var currentSize = currentBounds.size;
                    var categoryReferenceBounds = categoryReference[categoryIndex].GetComponent<Renderer>().bounds;
                    var categoryReferenceSize = categoryReferenceBounds.size;
                    //float minimumNewSizeRatio = Math.Min(categoryReferenceSize.x / currentSize.x, Math.Min(categoryReferenceSize.y / currentSize.y, categoryReferenceSize.z / currentSize.z));
                    float minimumNewSizeRatio = (categoryReferenceSize.y / currentSize.y); //Only match height
                    model.transform.localScale = model.transform.localScale * minimumNewSizeRatio;
                }

                Transform[] cameraChildrens = cameraPositions.GetComponentsInChildren<Transform>();
                foreach (Transform childCam in cameraChildrens)
                {
                    //Debug.Log(childCam.name);
                    //Debug.Log(model.transform.position);
                    yield return new WaitForEndOfFrame();
                    yield return new WaitForEndOfFrame();

                    MainCamera.transform.position = childCam.gameObject.transform.position;
                    MainCamera.transform.rotation = childCam.gameObject.transform.rotation;

                    randomiseSkybox();

                    lightSource.GetComponent<Light>().intensity = random.Next(lightIntensityLow, lightIntensityHigh);
                    lightSource.gameObject.transform.eulerAngles = new Vector3(random.Next(lightXRotationLow, lightXRotationHigh), random.Next(lightYRotationLow, lightYRotationHigh), 0);

                    Material floorMaterial = floorList[random.Next(floorList.Count)];
                    floor.gameObject.GetComponent<MeshRenderer>().material = floorMaterial;

                    Material wallMaterial = wallList[random.Next(wallList.Count)];

                    foreach (Transform wall in walls.GetComponentsInChildren<Transform>().Where(go => go.gameObject != walls.gameObject))
                    {
                        //Debug.Log(wall.name);
                        wall.gameObject.GetComponent<MeshRenderer>().material = wallMaterial;
                    }


                    CustomImageSynthesis customImageSynthesis = new CustomImageSynthesis(MainCamera);

                    string filePath = folderPath + Path.DirectorySeparatorChar + "img";
                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
                    }

                    StartCoroutine(customImageSynthesis.Save(childCam.name, room, model, -1, -1, filePath));
                }

                //Wait till last save is complete
                yield return new WaitForEndOfFrame();
                yield return new WaitForEndOfFrame();
                yield return new WaitForEndOfFrame();
                model.SetActive(false);
                DestroyImmediate(model);
                //break;
            }
            categoryIndex = categoryIndex + 1;
            //break;
        }
        yield return model;
    }

    void randomiseSkybox()
    {
        RenderSettings.skybox = skyBoxList[random.Next(skyBoxList.Count)];
    }

    void changeShader(GameObject gameObject)
    {
        //Adding mesh collider to all child objects of model
        Transform[] allChildren = gameObject.GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            Debug.Log(child.name);
            Renderer rend = child.GetComponent<Renderer>();
            if (rend != null)
            {
                rend.material.shader = shader;
            }
        }
    }

    void attachMeshCollider(GameObject child)
    {
        child.AddComponent<MeshCollider>();
        child.GetComponent<MeshCollider>().convex = true;
    }
}
