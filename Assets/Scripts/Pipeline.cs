
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Dummiesman;
using UnityEngine;
using UnityEngine.UI;

public abstract class AbstractPipeline
{
    public abstract int PipeLineType { get; set; }
    public abstract GameObject getRoomObject();
    public abstract void skipRoomPath();
    public abstract GameObject getModelObject();
    public abstract int getModelCount();
    public abstract void setModelCount(int modelCount);
    public abstract string getModelCategory();
    public abstract string getModelName();
    public abstract Camera getMainCamera();

    public abstract void init(string dataPath, string outputPath, string rootRoomPath, string rootMaterialPath, string categoriesInput, string imagesPerCategory, Vector3 origin);
    public abstract void setupRoom(GameObject room);
    public abstract void setupModel(GameObject model);
    public abstract bool setupCamera(GameObject model);
    public abstract List<GameObject> setupLigtSources(GameObject model, GameObject room);

    public abstract void execute(MonoBehaviour mono, List<Material> skyBoxList);
    public abstract bool replaceModel(MonoBehaviour mono, Vector3 origin);
}


public class Pipeline : AbstractPipeline
{
    protected RoomHelper roomHelper;
    protected ModelHelper modelHelper;
    protected CameraHelper cameraHelper;
    protected LightManager lightHelper;
    protected CustomImageSynthesis customImageSynthesis;
    protected Vector3 origin;

    protected int totalModelCount = 0;
    protected int imagesPerCategory = 0;
    protected int currentModelNumber = -1;
    protected bool incrementCategoryCounter = false;

    protected List<string> roomPaths = new List<string>();
    protected List<string> roomMtlPaths = new List<string>();

    protected List<string> modelPaths = new List<string>();
    protected List<string> modelMtlPaths = new List<string>();
    protected List<string> modelCategories = new List<string>();
    protected List<string> modelNames = new List<string>();

    protected string[] categories;
    protected string rootRoomPath, rootMaterialPath, dataPath,outputPath, currentRoomPath;
    protected GameObject currentRoom, currentModel;
    protected System.Random random = new System.Random();

    protected const string default_room_path = "Assets/resources/default_room/";
    protected const string default_model_path = "Assets/resources/default_model/model.obj";

    protected Dictionary<string, string> roomCacheDictionary = new Dictionary<string, string>();

    public override int PipeLineType { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    
    public override GameObject getRoomObject()
    {
        throw new System.NotImplementedException();
    }

    public override GameObject getModelObject()
    {
        throw new System.NotImplementedException();
    }

    void updateRoomPathList(string rootRoomPath)
    {
        //Debug.Log("updateRoomPathList");

        string[] dirList = Directory.GetDirectories(rootRoomPath);
        foreach (string dir in dirList)
        {
            roomPaths.AddRange(Directory.GetFiles(dir, "*.obj"));
            roomMtlPaths.AddRange(Directory.GetFiles(dir, "*.mtl"));
        }

        roomPaths.AddRange(Directory.GetFiles(rootRoomPath, "*.obj"));
        roomMtlPaths.AddRange(Directory.GetFiles(rootRoomPath, "*.mtl"));

        //Debug.Log(roomPaths.Count);

        for (int i=0; i < roomPaths.Count; i++)
        {
            GameObject room;
            try
            {
                room = new OBJLoader().Load(roomPaths[i], roomMtlPaths[i]);
            }
            catch
            {
                room = new OBJLoader().Load(roomPaths[i]);
            }

            room.name = "room" + roomCacheDictionary.Count.ToString();

            roomCacheDictionary.Add(roomPaths[i],room.name);

            Transform[] allChildren = room.GetComponentsInChildren<Transform>();
            foreach (Transform child in allChildren)
            {
                if (child.name != room.name)
                {
                    child.gameObject.SetActive(false);
                }
            }
        }

        Logger.WriteLine("updateRoomPathList completed");

    }

    public override void init(string dataPath, string outputPath, string rootRoomPath, string rootMaterialPath, string categoriesInput, string imagesPerCategory, Vector3 origin)
    {
        //Debug.Log("init1");
        this.dataPath = dataPath;
        this.outputPath = outputPath;
        this.rootMaterialPath = rootMaterialPath;
        this.rootRoomPath = rootRoomPath;
        this.categories = categoriesInput.Split(',');
        this.origin = origin;

        updateRoomPathList(rootRoomPath);

        if (!String.IsNullOrEmpty(imagesPerCategory))
        {
            this.imagesPerCategory = Int32.Parse(imagesPerCategory);
            this.incrementCategoryCounter = true;
        }

        this.customImageSynthesis = new CustomImageSynthesis(cameraHelper.getMainCamera());

        foreach (string category in this.categories)
        {
            int catagoryCount = 0;
            string categoryPath = this.dataPath + Path.DirectorySeparatorChar + category;
            string[] folders = Directory.GetDirectories(categoryPath, "*", System.IO.SearchOption.TopDirectoryOnly);

            while (!this.incrementCategoryCounter || catagoryCount != this.imagesPerCategory)
            {
                foreach (string folderPath in folders)
                {
                    //Debug.Log("category: " + category+" /// folderPath: " + folderPath);
                    //Debug.Log("modelName: " + new DirectoryInfo(folderPath).Name);
                    //Debug.Log(modelCategories.Count);

                    catagoryCount++;

                    string objPath = Directory.GetFiles(folderPath, "*.obj")[0];
                    string mtlPath = "";
                    try
                    {
                        mtlPath = Directory.GetFiles(folderPath, "*.mtl")[0];
                    }
                    catch
                    {
                    }

                    modelPaths.Add(objPath);
                    modelMtlPaths.Add(mtlPath);
                    modelCategories.Add(category);
                    modelNames.Add(new DirectoryInfo(folderPath).Name);

                    this.totalModelCount += 1;

                    if (this.incrementCategoryCounter && catagoryCount == this.imagesPerCategory)
                    {
                        break;
                    }

                    //Debug.Log("catagoryCount");
                    //Debug.Log(catagoryCount);
                    //Debug.Log("modelCount");
                    //Debug.Log(modelCount);
                }

                if (!this.incrementCategoryCounter)
                {
                    break;
                }

            }

        }
    }

    public override void execute(MonoBehaviour mono, List<Material> skyBoxList)
    {
        roomHelper.randomiseSkybox(skyBoxList);

        string destinationPath = outputPath + Path.DirectorySeparatorChar + getModelCategory() + Path.DirectorySeparatorChar + getModelName();

        //Debug.Log(getModelCategory()+ " "+ getModelName());
        //Debug.Log(destinationPath);
        //Debug.Log(getModelCategory());

        Directory.CreateDirectory(destinationPath);
        var files = Directory.GetFiles(destinationPath, "*.*", SearchOption.AllDirectories);
        int targetName = -1;
        string str2 = string.Empty;
        foreach (string filePath in files)
        {
            string fileName = Path.GetFileName(filePath);
            string number = new String(fileName.Where(Char.IsDigit).ToArray());
            //Debug.Log(number);
            targetName = (targetName <= Int32.Parse(number)) ? Int32.Parse(number) : targetName;
        }

        targetName = (targetName == -1) ? 0 : targetName + 1;

        this.customImageSynthesis.OnSceneChange();
        mono.StartCoroutine(this.customImageSynthesis.Save(targetName.ToString(), currentRoom, currentModel, -1, -1, destinationPath));

    }

    public override void setupRoom(GameObject room)
    {
        roomHelper.changeShader(room);
        roomHelper.preprocessRoom(room);
        roomHelper.applyTextures(room, rootMaterialPath);
    }

    public override void setupModel(GameObject model)
    {
        modelHelper.setupModel(model,origin);
    }

    public override int getModelCount()
    {
        return totalModelCount;
    }

    public override void setModelCount(int modelCount)
    {
        this.totalModelCount = modelCount;
    }

    public override bool setupCamera(GameObject model)
    {
        return cameraHelper.randomizeCamera(currentRoom, model, true);
    }

    public override List<GameObject> setupLigtSources(GameObject model, GameObject room)
    { 
        return lightHelper.randomizeLight(model,room);
    }

    public override string getModelCategory()
    {
        return modelCategories[currentModelNumber];
    }

    public override string getModelName()
    {
        return modelNames[currentModelNumber];
    }

    public override Camera getMainCamera()
    {
        return cameraHelper.getMainCamera();
    }

    public override bool replaceModel(MonoBehaviour mono, Vector3 origin)
    {
        throw new NotImplementedException();
    }

    public override void skipRoomPath()
    {
        throw new NotImplementedException();
    }
}

public static class PipelineType
{
    public const int SINGLE_PIPELINE = 1;
    public const int ROOM_PIPELINE = 2;
}