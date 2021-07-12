using System.Collections;
using System.Collections.Generic;
using Dummiesman;
using UnityEngine;
using System.IO;
using System;
using System.Linq;
using UnityEngine.UI;

public class ModelManager : MonoBehaviour
{
    public Camera MainCamera;
    public GameObject lightSources;
    public Light lightSource;

    public int lightIntensityLow;
    public int lightIntensityHigh;
    public int lightXRotationLow;
    public int lightXRotationHigh;
    public int lightYRotationLow;
    public int lightYRotationHigh;

    public InputField roomPathField;
    public InputField materialPathField;
    public InputField modelsPathField;
    public InputField outputPathField;
    public InputField categoriesInputField;
    public InputField modelCountInputField;
    public InputField categoryCountInputField;

    public InputField cameraMinInputField;
    public InputField cameraMaxInputField;
    public InputField cameraHeightInputField;

    public string[] categories;
    public GameObject[] categoryReference;
    public GameObject cameraPositions;
    private GameObject room;
    public GameObject walls;
    public GameObject floor;
    public List<Material> wallList;
    public List<Material> floorList;
    public List<Material> skyBoxList;

    public Shader shader;
    public string datapath;

    GameObject model;

    private System.Random random = new System.Random();

    private List<string> roomPaths = new List<string>();
    private List<string> roomMtlPaths = new List<string>();

    private RoomHelper roomHelper;
    private ModelHelper modelHelper;
    private CameraHelper cameraHelper;
    private LightManager lightHelper;

    private Pipeline currentPipeline;

    private List<GameObject> lightSourceList = new List<GameObject>();


    // Start is called before the first frame update
    void Start()
    {
        roomHelper = new RoomHelper(shader);
        modelHelper = new ModelHelper(shader);
        lightHelper = new LightManager(lightSources);
        cameraHelper = new CameraHelper(MainCamera, cameraMinInputField.text, cameraMaxInputField.text, cameraHeightInputField.text);
        currentPipeline = PipelineFactory.GetInstance(roomHelper, modelHelper, cameraHelper,lightHelper).getPipeline(PipelineType.SINGLE_PIPELINE); //Defualt

        //===============================TODO: remove
        categoriesInputField.text = "chair,bed,desk,wardrobe,table,bookcase,sofa";
        modelCountInputField.text = "10";
        categoryCountInputField.text = "2";

        modelsPathField.text = "/Users/apple/OVGU/Thesis/Dataset/pix3d/model/";
        roomPathField.text = "/Users/apple/OVGU/Thesis/scenenet/robotvault-downloadscenenet-cfe5ab85ddcc/3d-scene/";
        materialPathField.text = "/Users/apple/OVGU/Thesis/scenenet/robotvault-downloadscenenet-cfe5ab85ddcc/texture_library";
        outputPathField.text = "/Users/apple/OVGU/Thesis/s2r3dfree/";
        modelCountInputField.text = "10";

        cameraMinInputField.text = "1";
        cameraMaxInputField.text = "1.2";
        cameraHeightInputField.text = "0.25";

        //===============================TODO: remove

        cameraMinInputField.onValueChanged.AddListener(delegate { cameraParameterChanged(); });
        cameraMaxInputField.onValueChanged.AddListener(delegate { cameraParameterChanged(); });
        cameraHeightInputField.onValueChanged.AddListener(delegate { cameraParameterChanged(); });

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
        currentPipeline = PipelineFactory.GetInstance(roomHelper, modelHelper, cameraHelper, lightHelper).getPipeline(PipelineType.SINGLE_PIPELINE);
        currentPipeline.init(modelsPathField.text, outputPathField.text, roomPathField.text, materialPathField.text, categoriesInputField.text, categoryCountInputField.text);

        clearEnvironment();

        room = currentPipeline.getRoomObject();
        currentPipeline.setupRoom(room);

        model = currentPipeline.getModelObject();
        currentPipeline.setupModel(model);

        roomHelper.randomiseSkybox(skyBoxList);
        lightSourceList = currentPipeline.setupLigtSources(model, room);

        currentPipeline.setupCamera(model);
        replaceModel(room, model, currentPipeline.getModelCategory());

    }


    public void clearGameObject(GameObject gameObject)
    {
        try
        {
            gameObject.SetActive(false);
            DestroyImmediate(gameObject);
        }
        catch (Exception e)

        {
            Debug.Log("No model object to destroy");
            Debug.Log(e);

        }
    }

    public void clearEnvironment()
    {
        clearGameObject(room);
        clearGameObject(model);

        foreach(GameObject gameObject in lightSourceList)
        {
            clearGameObject(gameObject);
        }
    }



    public void singleEnvironment()
    {
        currentPipeline = PipelineFactory.GetInstance(roomHelper, modelHelper,cameraHelper,lightHelper).getPipeline(PipelineType.SINGLE_PIPELINE);
        StartCoroutine(buildEnv());
    }

    public void randomizeEnvironment()
    {
        currentPipeline = PipelineFactory.GetInstance(roomHelper, modelHelper,cameraHelper, lightHelper).getPipeline(PipelineType.ROOM_PIPELINE);
        StartCoroutine(buildEnv());
    }


    IEnumerator buildEnv()
    {
        Debug.Log("buildEnv");
        currentPipeline.init(modelsPathField.text, outputPathField.text, roomPathField.text, materialPathField.text, categoriesInputField.text, categoryCountInputField.text);

        for (int i = 0; i < currentPipeline.getModelCount(); i++) {
            clearEnvironment();

            room = currentPipeline.getRoomObject();
            currentPipeline.setupRoom(room);

            model = currentPipeline.getModelObject();
            currentPipeline.setupModel(model);

            roomHelper.randomiseSkybox(skyBoxList);
            lightSourceList = currentPipeline.setupLigtSources(model,room);

            replaceModel(room, model, currentPipeline.getModelCategory());
            currentPipeline.setupCamera(model);

            currentPipeline.execute(this);


            //Wait till last save is complete
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            //yield return new WaitForEndOfFrame();
            //yield return new WaitForEndOfFrame();

            yield return room;
        }
    }


    public void createDataset()
    {
        clearEnvironment();     
        StartCoroutine(LoadObjects());
    }

    // Invoked when the value of the text field changes.
    public void cameraParameterChanged()
    {
        Debug.Log("cameraParameterChanged");
        cameraHelper = null;
        cameraHelper = new CameraHelper(MainCamera, cameraMinInputField.text, cameraMaxInputField.text, cameraHeightInputField.text);
    }


    IEnumerator LoadObjects()
    {
        int categoryIndex = 0;
        foreach (string category in categories)
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


                //Vector3 sizeCalculated = model.GetComponent<Renderer>().bounds.size;
                //Debug.Log(sizeCalculated);

                //Vector3 sizeCalculated2= categoryReference[0].GetComponent<Renderer>().bounds.size;
                //Debug.Log(sizeCalculated2);


                //Adding mesh collider to all child objects of model
                Transform[] allChildren = model.GetComponentsInChildren<Transform>();
                foreach (Transform child in allChildren)
                {
                    modelHelper.attachMeshColliders(child.gameObject);
                }

                if (categoryReference.Length != 0 && categories.Length == categoryReference.Length)
                {
                    var currentBounds = modelHelper.GetMeshHierarchyBounds(model);
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

                    //string filePath = folderPath + Path.DirectorySeparatorChar + "img";
                    string filePath = outputPathField.text + Path.DirectorySeparatorChar + "img";

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


        modelHelper.modifyScale(model, reference);
        //model.transform.parent = reference.transform.parent;

        model.transform.position = reference.gameObject.GetComponent<MeshRenderer>().bounds.center;
        DestroyImmediate(reference);

        MainCamera.transform.LookAt(model.transform);
    }

}
