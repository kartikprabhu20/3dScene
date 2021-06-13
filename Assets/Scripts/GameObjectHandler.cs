
using UnityEngine;

internal class GameObjectHandler
{

    private static GameObjectHandler instance = null;
    private static readonly object padlock = new object();

    private GameObjectHandler()
    {
    }

    public static GameObjectHandler GetInstance()
    {
        lock (padlock)
        {
            if (instance == null)
            {
                instance = new GameObjectHandler();
            }
            return instance;
        }
    }

    public void clearEnvironment(GameObject room, GameObject model)
    {
        clearGameObject(room);
        clearGameObject(model);
    }

    public void clearGameObject(GameObject gameObject)
    {
        try
        {
            gameObject.SetActive(false);
            Object.DestroyImmediate(gameObject);
        }
        catch
        {
            Debug.Log("No model object to destroy");
        }
    }
}