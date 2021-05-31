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

    private List<string> roomPaths = new List<string>();
    private List<string> roomMtlPaths = new List<string>();

    private RoomHelper roomHelper;
    private GameObjectManager gameObjectManager;

    // Start is called before the first frame update
    void Start()
    {
        gameObjectManager = new GameObjectManager();
        roomHelper = new RoomHelper(shader, gameObjectManager);

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

    
    public void test()
    {
        clearEnvironment();

        string rootRoomPath = "/Users/apple/OVGU/Thesis/scenenet/robotvault-downloadscenenet-cfe5ab85ddcc/rooms/"; //TODO: globalise
        updateRoomPathList(rootRoomPath);

        for (int i = 0; i < roomPaths.Count; i++)
        {
            string objPath = roomPaths[i];
            string mtlPath = roomMtlPaths[i];

            room = new OBJLoader().Load(objPath, mtlPath);

            //Debug.Log("==========================");

            //Transform[] allChildren = room.GetComponentsInChildren<Transform>();

            //List<string> objectList = new List<string>();
            //foreach (Transform child in allChildren)
            //{
            //    //Debug.Log(child.name);
            //    objectList.Add(child.name);
            //}
            //string result = string.Join(",", objectList.ToArray());
            //Debug.Log(result);

            //clearEnvironment();
            //Debug.Log("==========================");

            Debug.Log("==========================");

           

            var currentBounds = GetMeshHierarchyBounds(room);
            var currentSize = currentBounds.size;
            Debug.Log(currentSize.x);
            Debug.Log(currentSize.y);
            Debug.Log(currentSize.z);
            //clearEnvironment();

            //Debug.Log(RandomPointInBounds(currentBounds));
            MainCamera.transform.position = RandomPointInBounds(currentBounds);

            Debug.Log("==========================");
            break;

        }

    }

    public static Vector3 RandomPointTransform(Vector3 position, Vector3 distance)
    {
        return new Vector3(
            UnityEngine.Random.Range(position.x - distance.x, position.x + distance.x) + distance.x,
            UnityEngine.Random.Range(position.y , position.y + distance.y) + distance.y,
            UnityEngine.Random.Range(position.z - distance.z, position.z + distance.z) + distance.z
        );
    }

    private Vector3 RandomPoint(GameObject room,GameObject model, Vector3 distance)
    {
        Bounds roomBounds = GetMeshHierarchyBounds(room);
        var modelBounds = GetMeshHierarchyBounds(model);
        float diff = 1;
        bool withinBounds = true;
        Vector3 newPosition = Vector3.zero;
        do
        {
            newPosition = new Vector3(
                                UnityEngine.Random.Range(modelBounds.min.x - distance.x, modelBounds.max.x + distance.x),
                                UnityEngine.Random.Range(modelBounds.min.y, modelBounds.max.y + distance.y),
                                UnityEngine.Random.Range(modelBounds.min.z - distance.z, modelBounds.max.z + distance.z));

            diff = Vector3.Distance(model.transform.position, newPosition);

            withinBounds = roomBounds.Contains(newPosition);
        } while (!(diff == 1.5f && withinBounds));


        Debug.Log(diff);
        return newPosition; // otherwise return the new position
        
    }

    public static Vector3 RandomPointInBounds(Bounds bounds)
    {
        return new Vector3(
            UnityEngine.Random.Range(bounds.min.x, bounds.max.x),
            UnityEngine.Random.Range(bounds.min.y, bounds.max.y),
            UnityEngine.Random.Range(bounds.min.z, bounds.max.z)
        );
    }

    public void clearGameObject(GameObject gameObject)
    {
        try
        {
            gameObject.SetActive(false);
            DestroyImmediate(gameObject);
        }
        catch
        {
            Debug.Log("No model object to destroy");
        }
    }

    public void clearEnvironment()
    {
        clearGameObject(room);
        clearGameObject(model);
    }

    public void randomizeEnvironment()
    {

        StartCoroutine(randomiseEnv());
    }

    IEnumerator randomiseEnv()
    {
        string rootRoomPath = "/Users/apple/OVGU/Thesis/scenenet/robotvault-downloadscenenet-cfe5ab85ddcc/3d-scene/";
        //string rootRoomPath = "/Users/apple/OVGU/Thesis/scenenet/robotvault-downloadscenenet-cfe5ab85ddcc/rooms/"; //TODO: globalise
        updateRoomPathList(rootRoomPath);
        clearEnvironment();
        roomHelper.randomiseSkybox(skyBoxList);


        //string objPath = "/Users/apple/OVGU/Thesis/scenenet/robotvault-downloadscenenet-cfe5ab85ddcc/1Bedroom/77_labels.obj";
        //string mtlPath = "/Users/apple/OVGU/Thesis/scenenet/robotvault-downloadscenenet-cfe5ab85ddcc/1Bedroom/77_labels.mtl";

        int roomIndex = random.Next(roomPaths.Count);
        string objPath = roomPaths[roomIndex];
        string mtlPath = roomMtlPaths[roomIndex];

        room = new OBJLoader().Load(objPath, mtlPath);

        roomHelper.changeShader(room);
        roomHelper.preprocessRoom(room);
        roomHelper.applyTextures(room);

        string folderPath = "/Users/apple/OVGU/Thesis/Dataset/pix3d/model/chair/IKEA_EKENAS/";
        string modelPath = Directory.GetFiles(folderPath, "*.obj")[0];
        try
        {
            string modelmtlPath = Directory.GetFiles(folderPath, "*.mtl")[0];
            model = new OBJLoader().Load(modelPath, modelmtlPath);
        }
        catch
        {
            model = new OBJLoader().Load(modelPath);
        }


        model.name = "target_model";

        //Adding mesh collider to all child objects of model
        Transform[] allChildren = model.GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            attachMeshCollider(child.gameObject);
        }

        randomizeCamera(room,model, false);
        replaceModel(room, model, "chair");

        yield return room;
    }





    void randomizeCamera(GameObject room,GameObject targetObject, bool withBounds)
    {
        if (withBounds)
        {
            var currentBounds = GetMeshHierarchyBounds(targetObject);
            MainCamera.transform.position = RandomPointInBounds(currentBounds);
        }
        else
        {
            //MainCamera.transform.position = RandomPointTransform(targetObject.transform.position, new Vector3(1f,1f,1f)); //TODO: make distance userinput
            MainCamera.transform.position = RandomPoint(room,targetObject, new Vector3(1f, 1f, 1f)); //TODO: make distance userinput

        }
    }
    void updateRoomPathList(string rootRoomPath)
    {
        string[] dirList = Directory.GetDirectories(rootRoomPath);

        foreach (string dir in dirList)
        {
            roomPaths.AddRange(Directory.GetFiles(dir, "*.obj"));
            roomMtlPaths.AddRange(Directory.GetFiles(dir, "*.mtl"));

            //Directory.GetFiles(dir, "*.obj").CopyTo(roomPaths, roomPaths.Length);
            //Directory.GetFiles(dir, "*.mtl").CopyTo(roomMtlPaths, roomMtlPaths.Length);
        }

    }

    public void createDataset()
    {
        clearEnvironment();     
        StartCoroutine(LoadObjects());
    }

    public Bounds GetMeshHierarchyBounds(GameObject go)
    {
        Bounds bounds = new Bounds(); // Not used, but a struct needs to be instantiated

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
                randomizeEnvironment(); //change room

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

                replaceModel(room, model, category);

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

                    roomHelper.randomiseSkybox(skyBoxList);

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

                clearEnvironment();
                //break;
            }
            categoryIndex = categoryIndex + 1;
            //break;
        }
        yield return model;
    }

    private void replaceModel(GameObject room, GameObject model, string category)
    {
        List<GameObject> matchingObjects = new List<GameObject>();
        Transform[] allChildren = room.GetComponentsInChildren<Transform>();
        int i = 0;
        foreach (Transform child in allChildren)
        {
            if (child.name == category)
            {
                i += 1;
                matchingObjects.Add(child.gameObject);
            }
        }

        if(matchingObjects.Count == 0)
        {
            Debug.Log("No match found");
            return;
        }

        GameObject reference = matchingObjects[random.Next(matchingObjects.Count)];


        modifyScale(model, reference);
        //model.transform.parent = reference.transform.parent;

        model.transform.position = reference.gameObject.GetComponent<MeshRenderer>().bounds.center;
        DestroyImmediate(reference);

        MainCamera.transform.LookAt(model.transform);
    }

    private void modifyScale(GameObject target, GameObject reference)
    {
        var currentBounds = GetMeshHierarchyBounds(target);
        var currentSize = currentBounds.size;
        var categoryReferenceBounds = reference.GetComponent<Renderer>().bounds;
        var categoryReferenceSize = categoryReferenceBounds.size;
        float minimumNewSizeRatio = Math.Min(categoryReferenceSize.x / currentSize.x, Math.Min(categoryReferenceSize.y / currentSize.y, categoryReferenceSize.z / currentSize.z));
        //float minimumNewSizeRatio = (categoryReferenceSize.y / currentSize.y); //Only match height
        target.transform.localScale = target.transform.localScale * minimumNewSizeRatio;
    }

    void attachMeshCollider(GameObject child)
    {
        child.AddComponent<MeshCollider>();
        child.GetComponent<MeshCollider>().convex = true;
    }
}
