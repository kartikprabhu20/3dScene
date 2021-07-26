public class PipelineFactory
{
    private static PipelineFactory instance = null;
    private static readonly object padlock = new object();
    private RoomHelper roomHelper;
    private ModelHelper modelHelper;
    private CameraHelper cameraHelper;
    private LightManager lightHelper;

    private PipelineFactory(RoomHelper roomHelper, ModelHelper modelHelper, CameraHelper cameraHelper, LightManager lightHelper)
    {
        this.roomHelper = roomHelper;
        this.modelHelper = modelHelper;
        this.cameraHelper = cameraHelper;
        this.lightHelper = lightHelper;

    }

    public static PipelineFactory GetInstance(RoomHelper roomHelper, ModelHelper modelHelper, CameraHelper cameraHelper, LightManager lightHelper)
    {
        lock (padlock)
        {

            instance = new PipelineFactory(roomHelper, modelHelper,cameraHelper, lightHelper);
            return instance;
        }
    }


    public Pipeline getPipeline(int id, CameraHelper cameraHelper)
    {
        this.cameraHelper = cameraHelper;
        switch (id)
        {
            case PipelineType.SINGLE_PIPELINE:
                return new SinglePipeline(this.roomHelper, this.modelHelper, this.cameraHelper, this.lightHelper);
            case PipelineType.ROOM_PIPELINE:
                return new RoomPipeline(this.roomHelper, this.modelHelper, this.cameraHelper, this.lightHelper);
            default:
                return new SinglePipeline(this.roomHelper, this.modelHelper, this.cameraHelper, this.lightHelper);
        }
    }

    public Pipeline getPipeline(int id)
    {
        switch (id)
        {
            case PipelineType.SINGLE_PIPELINE:
                return new SinglePipeline(this.roomHelper, this.modelHelper,this.cameraHelper, this.lightHelper);
            case PipelineType.ROOM_PIPELINE:
                return new RoomPipeline(this.roomHelper, this.modelHelper, this.cameraHelper, this.lightHelper);
            default:
                return new SinglePipeline(this.roomHelper, this.modelHelper, this.cameraHelper, this.lightHelper);
        }
    }

}