using System.Collections;
using System.Collections.Generic;
using Dummiesman;
using UnityEngine;
using System.IO;
using System;
using System.Linq;
using UnityEngine.UI;
using System.Threading;

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

    public Text startTime;
    public Text endTime;
    public Text modelCount;

    public Button nextModelButton;
    public Button randomiseTextureButton;
    public Button randomiseLightButton;
    public Button randomiseCameraButton;

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
    public string[] exceptionObjects = { "wall","floor"};


    // Start is called before the first frame update
    void Start()
    {

        Logger.WriteLine("Start");

        roomHelper = new RoomHelper(shader);
        modelHelper = new ModelHelper(shader);
        lightHelper = new LightManager(lightSources);
        cameraHelper = new CameraHelper(MainCamera, cameraMinInputField.text, cameraMaxInputField.text, cameraHeightInputField.text);
        currentPipeline = new PipelineFactory(roomHelper, modelHelper, cameraHelper,lightHelper).getPipeline(PipelineType.SINGLE_PIPELINE); //Defualt

        //===============================TODO: remove
        categoriesInputField.text = "chair,bed,desk,wardrobe,table,bookcase,sofa";
        modelCountInputField.text = "10";
        categoryCountInputField.text = "2";

        modelsPathField.text = "/Users/apple/OVGU/Thesis/Dataset/pix3d/model/";
        roomPathField.text = "/Users/apple/OVGU/Thesis/scenenet/robotvault-downloadscenenet-cfe5ab85ddcc/3d-scene/";
        materialPathField.text = "/Users/apple/OVGU/Thesis/scenenet/robotvault-downloadscenenet-cfe5ab85ddcc/texture_library";
        outputPathField.text = "/Users/apple/OVGU/Thesis/s2r3dfree_v3/";
        modelCountInputField.text = "10";

        cameraMinInputField.text = "0.75";
        cameraMaxInputField.text = "1.5";
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
        //Pipeline currentPipeline = PipelineFactory.GetInstance(roomHelper, modelHelper, cameraHelper, lightHelper).getPipeline(PipelineType.SINGLE_PIPELINE);
        //currentPipeline.init(modelsPathField.text, outputPathField.text, roomPathField.text, materialPathField.text, categoriesInputField.text, categoryCountInputField.text);

        //clearEnvironment();

        //room = currentPipeline.getRoomObject();
        //currentPipeline.setupRoom(room);

        //model = currentPipeline.getModelObject();
        //currentPipeline.setupModel(model);

        //roomHelper.randomiseSkybox(skyBoxList);
        //lightSourceList = currentPipeline.setupLigtSources(model, room);

        //currentPipeline.setupCamera(model);
        //replaceModel(room, model, currentPipeline.getModelCategory(),MainCamera);


        // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // //
        //clearEnvironment();

        //Pipeline currentPipe = PipelineFactory.GetInstance(roomHelper, modelHelper, cameraHelper, lightHelper).getPipeline(PipelineType.ROOM_PIPELINE);
        //currentPipe.init(modelsPathField.text, outputPathField.text, roomPathField.text, materialPathField.text, categoriesInputField.text, categoryCountInputField.text, new Vector3(0, 0, 0));

        //GameObject room = currentPipe.getRoomObject();
        //currentPipe.setupRoom(room);

        //GameObject model = currentPipe.getModelObject();
        //currentPipe.setupModel(model);

        //Vector3 origin = model.transform.position;
        //bool cameraNotSet = true;
        //do
        //{
        //    Debug.Log("camera setting");
        //    currentPipeline.replaceModel(this,origin);
        //    cameraNotSet = !currentPipe.setupCamera(model);
        //} while (cameraNotSet);

        //lightSourceList = currentPipe.setupLigtSources(model, room);
        //currentPipe.execute(this, skyBoxList);

        // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // // //

        multiThread();
    }

    public void clearGameObject(GameObject gameObject)
    {
        try
        {
            Transform[] allChildren = gameObject.GetComponentsInChildren<Transform>();
            foreach(Transform child in allChildren)//Skip first child as parent gets returned
            {
                child.gameObject.SetActive(false);
                DestroyImmediate(child.gameObject);
            }
        }
        catch (Exception e)

        {
            Debug.Log("No model object to destroy");
            Debug.Log(e);

        }
    }

    public void clearEnvironment()
    {
        clearEnvironment(room, model);
    }

    public void clearEnvironment(GameObject room, GameObject model)
    {
        clearGameObject(room);
        clearGameObject(model);

        foreach (GameObject gameObject in lightSourceList)
        {
            clearGameObject(gameObject);
        }
    }

    public void snap()
    {
        currentPipeline.execute(this, skyBoxList);
    }

    public void singleEnvironment()
    {
        Pipeline currentPipe = new PipelineFactory(roomHelper, modelHelper,cameraHelper,lightHelper).getPipeline(PipelineType.SINGLE_PIPELINE);
        currentPipe.init(modelsPathField.text, outputPathField.text, roomPathField.text, materialPathField.text, categoriesInputField.text, categoryCountInputField.text, new Vector3(0, 0, 0));
        StartCoroutine(buildEnv(currentPipe));
    }

    public void randomizeEnvironment()
    {
        nextModelButton.gameObject.SetActive(true);
        randomiseCameraButton.gameObject.SetActive(true);
        randomiseLightButton.gameObject.SetActive(true);
        randomiseTextureButton.gameObject.SetActive(true);

        currentPipeline = new PipelineFactory(roomHelper, modelHelper,cameraHelper, lightHelper).getPipeline(PipelineType.ROOM_PIPELINE);
        currentPipeline.init(modelsPathField.text, outputPathField.text, roomPathField.text, materialPathField.text, categoriesInputField.text, categoryCountInputField.text, new Vector3(0, 0, 0));

        setEnv();
       //StartCoroutine(buildEnv(currentPipe));
    }



    public void setEnv()
    {
        model = currentPipeline.getModelObject();

        bool roomDoesNotContainsCategory = true;
        do
        {
            room = currentPipeline.getRoomObject();
            Transform[] allChildren = room.GetComponentsInChildren<Transform>(true);
            foreach (Transform child in allChildren)
            {
                if (currentPipeline.getModelCategory().ToLower() == child.name.ToLower())
                {
                    roomDoesNotContainsCategory = false;
                    break;
                }
            }

            if (roomDoesNotContainsCategory)
            {
                clearGameObject(room);
            }
        } while (roomDoesNotContainsCategory);


        currentPipeline.setupRoom(room);
        currentPipeline.setupModel(model);

        Vector3 origin = model.transform.position;
        bool cameraNotSet = true;
        do
        {
            Debug.Log("camera setting");
            //replaceModel(room, model, currentPipe.getModelCategory(), currentPipe.getMainCamera(), origin);
            if (currentPipeline.replaceModel(this, origin))
            {
                model.transform.eulerAngles = new Vector3(0, 90, 0);
            }
            cameraNotSet = !currentPipeline.setupCamera(model);
        } while (cameraNotSet);

        lightSourceList = currentPipeline.setupLigtSources(model, room);

    }

    public void onRandomiseTexture()
    {
        currentPipeline.setupRoom(room);

    }

    public void onRandomiseCamera()
    {

        bool cameraNotSet = true;
        do
        {
            cameraNotSet = !currentPipeline.setupCamera(model);
        } while (cameraNotSet);

    }

    public void onRandomiseLight()
    {
        foreach (GameObject gameObject in lightSourceList)
        {
            clearGameObject(gameObject);
        }

        lightSourceList = currentPipeline.setupLigtSources(model, room);

    }

    public void onNextModel()
    {
        StartCoroutine(onNextModelTrig());
    }

    IEnumerator onNextModelTrig()
    {

        currentPipeline.execute(this, skyBoxList);

        //Wait till last save is complete
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        //clearEnvironment(room, model);
        clearGameObject(model);

        foreach (GameObject gameObject in lightSourceList)
        {
            clearGameObject(gameObject);
        }

        setEnv();
    }


    public void multiThread()
    {
        string[] categories = categoriesInputField.text.Split(',');

        int i = 0;

        foreach(string category in categories)
        {
            i += 50;
            Vector3 origin = new Vector3(0, 0, i);

            //maincamera.tag = category;
            cameraHelper = new CameraHelper(GameObject.Instantiate(MainCamera), cameraMinInputField.text, cameraMaxInputField.text, cameraHeightInputField.text);
            roomHelper = new RoomHelper(shader);
            modelHelper = new ModelHelper(shader);
            lightHelper = new LightManager(GameObject.Instantiate(lightSources));
            Pipeline currentPipe = new PipelineFactory(roomHelper, modelHelper, cameraHelper, lightHelper).getPipeline(PipelineType.SINGLE_PIPELINE);
            currentPipe.init(modelsPathField.text, outputPathField.text, roomPathField.text, materialPathField.text, category, categoryCountInputField.text, origin);

            Debug.Log("multiThread");

            //Thread thread = new Thread(() => Run(currentPipe));
            //thread.Start();

            StartCoroutine(buildEnv(currentPipe));
        }
    }


    IEnumerator buildEnv(Pipeline currentPipe)
    {
        Debug.Log("buildEnv");
        startTime.text = getCurrentTime();
        endTime.text = "";
        for (int i = 0; i < currentPipe.getModelCount(); i++) {
            //clearEnvironment();

            GameObject room = currentPipe.getRoomObject();
            currentPipe.setupRoom(room);

            GameObject model = currentPipe.getModelObject();
            currentPipe.setupModel(model);

            Vector3 origin = model.transform.position;
            bool cameraNotSet = true;
            do
            {
                Debug.Log("camera setting");
                //replaceModel(room, model, currentPipe.getModelCategory(), currentPipe.getMainCamera(), origin);
                currentPipe.replaceModel(this,origin);
                cameraNotSet = !currentPipe.setupCamera(model);
            } while (cameraNotSet);

            lightSourceList = currentPipe.setupLigtSources(model, room);
            currentPipe.execute(this,skyBoxList);

            modelCount.text = i.ToString();

            //Wait till last save is complete
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            //yield return new WaitForEndOfFrame();
            //yield return new WaitForEndOfFrame();

            Resources.UnloadUnusedAssets();

            yield return room;

            clearEnvironment(room, model);
        }

        endTime.text = getCurrentTime();

    }


    public void createDataset()
    {
        clearEnvironment();
        //StartCoroutine(LoadObjects());

        Pipeline currentPipe = new PipelineFactory(roomHelper, modelHelper, cameraHelper, lightHelper).getPipeline(PipelineType.ROOM_PIPELINE);
        currentPipe.init(modelsPathField.text, outputPathField.text, roomPathField.text, materialPathField.text, categoriesInputField.text, categoryCountInputField.text, new Vector3(0, 0, 0));

        StartCoroutine(buildEnv2(currentPipe));
    }

    IEnumerator buildEnv2(Pipeline currentPipe)
    {
        Logger.WriteLine("buildEnv2");

        startTime.text = getCurrentTime();
        endTime.text = "";
        Debug.Log("buildEnv2");
        int failSafeAttempts = 10;
        for (int i = 0; i < currentPipe.getModelCount(); i++)
        {
            Logger.WriteLine("Model Number: " + i.ToString());
            //clearEnvironment();
            GameObject model = currentPipe.getModelObject();
            modelCount.text = i.ToString();
            bool roomDoesNotContainsCategory = true;

            do
            {
                int attempts = 0;
                GameObject room = currentPipe.getRoomObject();
                Transform[] allChildren = room.GetComponentsInChildren<Transform>(true);
                foreach (Transform child in allChildren)
                {
                    if (child.name.ToLower().Contains(currentPipe.getModelCategory().ToLower()))
                    {
                        roomDoesNotContainsCategory = false;
                        break;
                    }
                }

                if (roomDoesNotContainsCategory)
                {
                    Logger.WriteLine("roomDoesNotContainsCategory");
                    //clearGameObject(room);
                    //room.SetActive(false);
                    continue;
                }

                currentPipe.setupRoom(room);
                currentPipe.setupModel(model);

                Vector3 origin = model.transform.position;
                bool cameraNotSet = true;
                do
                {
                    attempts++;
                    if (attempts >= failSafeAttempts)
                    {
                        Logger.WriteLine("failSafeAttempts reached");
                        break;
                    }
                    Debug.Log("camera setting");
                    //replaceModel(room, model, currentPipe.getModelCategory(), currentPipe.getMainCamera(), origin);
                    if (replaceModel(room, model, currentPipe.getModelCategory(), currentPipe.getMainCamera(), origin))
                    {
                        model.transform.eulerAngles = new Vector3(0, 90, 0);
                    }
                    cameraNotSet = !currentPipeline.setupCamera(model);
                } while (cameraNotSet);

                if (attempts >= failSafeAttempts)
                {
                    clearGameObject(model);
                    //clearEnvironment(room, model);
                    foreach (GameObject gameObject in lightSourceList)
                    {
                        clearGameObject(gameObject);
                    }
                    break;
                }

                lightSourceList = currentPipe.setupLigtSources(model, room);

                currentPipe.execute(this, skyBoxList);

                //Wait till last save is complete
                yield return new WaitForEndOfFrame();
                yield return new WaitForEndOfFrame();
                yield return new WaitForEndOfFrame();
                //yield return new WaitForEndOfFrame();
                //yield return new WaitForEndOfFrame();

                Resources.UnloadUnusedAssets();

                yield return room;

                clearGameObject(model);
                //clearEnvironment(room, model);
                foreach (GameObject gameObject in lightSourceList)
                {
                    clearGameObject(gameObject);
                }

            } while (roomDoesNotContainsCategory);
        }
        endTime.text = getCurrentTime();
    }

    // Invoked when the value of the text field changes.
    public void cameraParameterChanged()
    {
        Debug.Log("cameraParameterChanged");
        cameraHelper = null;
        cameraHelper = new CameraHelper(MainCamera, cameraMinInputField.text, cameraMaxInputField.text, cameraHeightInputField.text);
    }

    private bool replaceModel(GameObject room, GameObject model, string category, Camera maincamera, Vector3 origin)
    {
        Debug.Log("replaceModel");
        List<GameObject> matchingObjects = new List<GameObject>();
        Transform[] allChildren = room.GetComponentsInChildren<Transform>(true);
        foreach (Transform child in allChildren)
        {
            if (child.name.ToLower().Contains(category.ToLower()))
            {
                matchingObjects.Add(child.gameObject);
            }
        }

        Debug.Log("matchingObjects.Count:" + matchingObjects.Count.ToString());
        if (matchingObjects.Count < 1)
        {
            checkIntersection(room, model, origin);
            return false;
        }

        GameObject reference = matchingObjects[random.Next(matchingObjects.Count)];
        bool rotate = modelHelper.modifyScale(model, reference);
        //model.transform.parent = reference.transform.parent;

        Vector3 referencePosition = reference.gameObject.GetComponent<MeshRenderer>().bounds.center;
        model.transform.position = new Vector3(referencePosition.x, modelHelper.GetMeshHierarchyBounds(model).size.y / 2, referencePosition.z);
        model.transform.rotation = reference.transform.rotation;

        reference.SetActive(false);
        //DestroyImmediate(reference);

        checkIntersection(room, model, origin);

        maincamera.transform.LookAt(model.transform);
        return rotate;
    }

    private void checkIntersection(GameObject room, GameObject model, Vector3 origin)
    {
        Debug.Log("checkIntersection");
        Transform[] allChildren = room.GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            if (child != null && model.GetComponent<Collider>().bounds.Intersects(child.gameObject.GetComponent<Collider>().bounds))//If target object is intersecting with gameobject
            {
                int pos = Array.IndexOf(exceptionObjects, child.name);
                if (pos == -1) //If exceptionObjects string array doesnt have the name of the child, delete the child
                {
                    Debug.Log("Bounds intersecting");
                    if (child.name == "bed")
                    {
                        string[] objectsToDestory = { "duvet", "pillow" };
                        foreach (string objectToDestroy in objectsToDestory)
                        {
                            FindAndDestroy(room, objectToDestroy);
                        }
                    }

                    child.gameObject.SetActive(false);
                    //DestroyImmediate(child.gameObject);
                }
                //else if (pos == 0)//if wall is intersecting then go back to origin
                //{
                //    Debug.Log("Bounds intersecting");
                //    model.transform.position = origin;
                //    checkIntersection(room,model, origin);
                //}
            }
        }
        return;
    }

    private void FindAndDestroy(GameObject room, string objectName)
    {
        try
        {
            Transform[] allChildren = room.GetComponentsInChildren<Transform>();
            foreach (Transform child in allChildren)
            {
                if (child.name == objectName)
                {
                    GameObject objectToDestroy = child.gameObject;
                    if (objectToDestroy != null)
                    {
                        //DestroyImmediate(objectToDestroy);
                        objectToDestroy.SetActive(false);

                    }
                }
            }
        }
        catch
        {

        }
    }

    private string getCurrentTime()
    {
        DateTime time = DateTime.Now;
        string hour = LeadingZero(time.Hour);
        string minute = LeadingZero(time.Minute);
        string second = LeadingZero(time.Second);
        return hour + ":" + minute + ":" + second;
    }

    private string LeadingZero(int n)
    {
        return n.ToString().PadLeft(2, '0');
    }

}
