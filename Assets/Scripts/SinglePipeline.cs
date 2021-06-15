using Dummiesman;
using UnityEngine;
using UnityEngine.UI;

public class SinglePipeline : Pipeline
{
    private int pipelineType = PipelineType.SINGLE_PIPELINE;
    private const string default_room_path = "Assets/resources/default_room/";
    private const string default_model_path = "Assets/resources/default_model/model.obj";

    public SinglePipeline(RoomHelper roomHelper, ModelHelper modelHelper,CameraHelper cameraHelper)
    {
        base.roomHelper = roomHelper;
        base.modelHelper = modelHelper;
        base.cameraHelper = cameraHelper;
    }

    public override void init(string dataPath, string rootRoomPath, string rootMaterialPath, InputField categoriesInputField)
    {
        base.init(dataPath, default_room_path, rootMaterialPath, categoriesInputField);

        base.modelPaths.Add(default_model_path);
        base.modelMtlPaths.Add(" ");
        base.modelCategories.Add("chair");

    }
    public override int PipeLineType
    {
        get { return this.pipelineType; }
    }

    public override GameObject getRoomObject()
    {
        string objPath = base.roomPaths[0];
        string mtlPath = base.roomMtlPaths.Count > 0 ? base.roomMtlPaths[0] : " ";

        try
        {
            return new OBJLoader().Load(objPath, mtlPath);
        }
        catch
        {
            return new OBJLoader().Load(objPath);
        }
    }

    public override GameObject getModelObject()
    {
        string objPath = base.modelPaths[0];
        string mtlPath = base.modelMtlPaths[0];
        try
        {
            return new OBJLoader().Load(objPath, mtlPath);
        }
        catch
        {
            return new OBJLoader().Load(objPath);
        }
    }
}