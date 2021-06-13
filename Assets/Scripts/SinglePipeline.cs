using Dummiesman;
using UnityEngine;

public class SinglePipeline : Pipeline
{
    private int pipelineType = PipelineType.SINGLE_PIPELINE;
    private const string default_room_path = "Assets/resources/default_room/room.obj";
    private const string default_model_path = "Assets/resources/default_model/model.obj";

    public SinglePipeline(RoomHelper roomHelper, ModelHelper modelHelper)
    {
        base.roomHelper = roomHelper;
        base.modelHelper = modelHelper;
    }

    public override int PipeLineType
    {
        get { return this.pipelineType; }
    }

    public override GameObject getRoomObject()
    {
        return new OBJLoader().Load(default_room_path);
    }

    public override GameObject getModelObject()
    {
        return new OBJLoader().Load(default_model_path);
    }
}