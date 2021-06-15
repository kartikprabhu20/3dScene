using UnityEngine;

public class CameraHelper: Helper
{
    private Camera mainCamera;

    public CameraHelper(Camera mainCamera)
    {
        this.mainCamera = mainCamera;
    }

    void randomizeCamera(GameObject room, GameObject targetObject, bool withBounds)
    {
        Renderer renderer = targetObject.gameObject.GetComponent<MeshRenderer>();
        bool randomized = false;
        do
        {
            if (withBounds)
            {
                var currentBounds = GetMeshHierarchyBounds(targetObject);
                mainCamera.transform.position = RandomPointInBounds(currentBounds);
            }
            else
            {
                //MainCamera.transform.position = RandomPointTransform(targetObject.transform.position, new Vector3(1f,1f,1f)); //TODO: make distance userinput
                mainCamera.transform.position = RandomPoint(room, targetObject, new Vector3(1f, 1f, 1f)); //TODO: make distance userinput

            }

            randomized = IsVisibleFrom(renderer, mainCamera);
        } while (!randomized);
    }

    private bool IsVisibleFrom(Renderer renderer, Camera camera)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
        return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
    }

    private static Vector3 RandomPointInBounds(Bounds bounds)
    {
        return new Vector3(
            UnityEngine.Random.Range(bounds.min.x, bounds.max.x),
            UnityEngine.Random.Range(bounds.min.y, bounds.max.y),
            UnityEngine.Random.Range(bounds.min.z, bounds.max.z)
        );
    }

    private Vector3 RandomPoint(GameObject room, GameObject model, Vector3 distance)
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
}