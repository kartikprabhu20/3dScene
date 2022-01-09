using System;
using UnityEngine;

public class CameraHelper: Helper
{
    private Camera mainCamera;
    private float minDistance = 1.5f;
    private float maxDistance= 1.75f;
    private float minHeight = 0.25f;
    public string[] commonObjects = {};


    public CameraHelper(Camera mainCamera, string minDistance, string maxDistance, string minHeight)
    {
        this.mainCamera = mainCamera;
        if (!String.IsNullOrEmpty(minDistance))
        {
            this.minDistance = float.Parse(minDistance);
        }

        if (!String.IsNullOrEmpty(maxDistance))
        {
            this.maxDistance = float.Parse(maxDistance);
        }

        if (!String.IsNullOrEmpty(minHeight))
        {
            this.minHeight = float.Parse(minHeight);
        }

    }

    public Camera getMainCamera()
    {
        return this.mainCamera;
    }

    public bool randomizeCamera(GameObject room, GameObject targetObject, bool withBounds)
    {
        Renderer renderer = targetObject.gameObject.GetComponent<MeshRenderer>();
        bool randomized = false;
        int attempts = 10;
        bool cameraSet = true;
        do
        {
            ///////////////////////// below line works for single room pipeline
            //mainCamera.transform.position = RandomPointInAnnulus(targetObject.transform.position, this.minDistance, this.maxDistance);

            if (attempts == 0)
            {
                cameraSet = false;
                break;
            }

            Vector3[] positions = getCameraPositions(targetObject);
            mainCamera.transform.position = positions[UnityEngine.Random.Range(0, positions.Length)];
            //Debug.Log(mainCamera.transform.position);

            mainCamera.transform.LookAt(targetObject.transform);
            randomized = IsVisibleFrom(targetObject, mainCamera) & (mainCamera.transform.position.y > this.minHeight);
            attempts -= 1;
        } while (!randomized);

        return cameraSet;
    }

    public Vector3[] getCameraPositions(GameObject targetObject)
    {
        var randomDistance = UnityEngine.Random.Range(this.minDistance, this.maxDistance);
        //Vector3 position1 = targetObject.transform.position + new Vector3(0, UnityEngine.Random.Range(this.minHeight, this.minHeight + 0.5f), 0) + targetObject.transform.forward * randomDistance;
        //Vector3 position2 = targetObject.transform.position + new Vector3(0, UnityEngine.Random.Range(this.minHeight, this.minHeight + 0.5f), 0) + targetObject.transform.forward * randomDistance + targetObject.transform.right * 0.5f;
        //Vector3 position3 = targetObject.transform.position + new Vector3(0, UnityEngine.Random.Range(this.minHeight, this.minHeight + 0.5f), 0) + targetObject.transform.forward * randomDistance - targetObject.transform.right * 0.5f;
        //Vector3 position4 = targetObject.transform.position + new Vector3(0, UnityEngine.Random.Range(this.minHeight, this.minHeight + 0.5f), 0) + targetObject.transform.forward * randomDistance + targetObject.transform.right * 0.5f;

        //Vector3 position1 = targetObject.transform.position + new Vector3(0, UnityEngine.Random.Range(this.minHeight, this.minHeight + 0.5f), 0) - targetObject.transform.right * randomDistance;
        Vector3 position2 = targetObject.transform.position + new Vector3(0, UnityEngine.Random.Range(this.minHeight, this.minHeight + 0.5f), 0) - targetObject.transform.forward * randomDistance + targetObject.transform.right * UnityEngine.Random.Range(-0.5f,0.5f);

        Vector3[] positions = {position2};
        return positions;
    }
    public Vector3 RandomPointInAnnulus(Vector3 origin, float minRadius, float maxRadius)
    {

        var randomDirection = (UnityEngine.Random.insideUnitSphere + origin).normalized;

        var randomDistance = UnityEngine.Random.Range(minRadius, maxRadius);

        var point = origin + randomDirection * randomDistance;

        return point;
    }

    private bool IsVisibleFrom(GameObject targetModel, Camera camera)
    {
        RaycastHit hit;
        if (Physics.Linecast(camera.transform.position, targetModel.GetComponentInChildren<Renderer>().bounds.center, out hit))
        {
            if (hit.transform.name != targetModel.name)
            {
                Debug.Log(targetModel.name + " occluded by " + hit.transform.name);
                return false;
            }
        }
        return true;
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