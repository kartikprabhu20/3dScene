using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : Helper
{
    private GameObject lightSources;
    private float lightIntensityLow = 0;
    private float lightIntensityHigh = 1;
    private float indoorLightIntensityLow = 0f;
    private float indoorLightIntensityHigh = 1.5f;
    private int lightXRotationLow= 0;
    private int lightXRotationHigh = 75;
    private int lightYRotationLow = -170;
    private int lightYRotationHigh = -10;

    public LightManager(GameObject lightSources)
    {
        this.lightSources = lightSources;

    }

    public GameObject getLightSources()
    {
        return lightSources;
    }

    public List<GameObject> randomizeLight(GameObject curretModel,GameObject room)
    {
        List<GameObject> lightSourceList = new List<GameObject>();


        string[] allChildren = {"PointLight","SunLight","SpotLight"};
        int randomChild = Random.Range(0, allChildren.Length);

        switch (allChildren[randomChild])
        {
            case "PointLight":
                var currentBounds = GetMeshHierarchyBounds(room);
                int lightCount = Random.Range(0, 5);

                List<Vector3> lightPositions = getLightPositions(lightCount, currentBounds.min.x, currentBounds.max.x, currentBounds.min.z, currentBounds.max.z, currentBounds.max.y);
                float lightIntensity = Random.Range(indoorLightIntensityLow, indoorLightIntensityHigh);

                foreach (Vector3 position in lightPositions)
                {
                    GameObject light = getLightSources().gameObject.transform.Find("PointLight").gameObject;
                    light = GameObject.Instantiate(light);
                    light.SetActive(true);
                    light.transform.position = position;
                    light.GetComponent<Light>().intensity = lightIntensity;

                    lightSourceList.Add(light);
                }
                break;

            case "SunLight":
                //sun light
                GameObject sunLight = lightSources.gameObject.transform.Find("SunLight").gameObject;
                sunLight.GetComponent<Light>().intensity = Random.Range(lightIntensityLow, lightIntensityHigh);
                sunLight.gameObject.transform.eulerAngles = new Vector3(Random.Range(lightXRotationLow, lightXRotationHigh), Random.Range(lightYRotationLow, lightYRotationHigh), 0);

                GameObject lightSource = GameObject.Instantiate(sunLight);
                lightSource.SetActive(true);
                lightSourceList.Add(lightSource);
                break;

            case "SpotLight":
                Debug.Log("SpotLight");
                GameObject spotLight = lightSources.gameObject.transform.Find("SpotLight").gameObject;
                GameObject spotSource = GameObject.Instantiate(spotLight);
                spotSource.SetActive(true);
                spotSource.GetComponent<Light>().intensity = Random.Range(indoorLightIntensityLow, indoorLightIntensityHigh);

                var modelBounds = GetMeshHierarchyBounds(curretModel);
                spotSource.transform.position = new Vector3(curretModel.transform.position.x, modelBounds.max.y + 1.5f, curretModel.transform.position.z);

                Debug.Log(spotLight.transform.position);
                
                lightSourceList.Add(spotSource);
                break;

            case "SpotLightCamera":
                //TODO: add light from camera
                break;

        }

        return lightSourceList;
    }

    public List<Vector3> getLightPositions(int numOfLights,float xMin, float xMax, float zMin, float zMax, float yMax)
    {
        List<Vector3> lightPositions = new List<Vector3>();
        float x = 0.0f;
        float z = 0.0f;
        float partitionX = 0.0f;
        float partitionZ = 0.0f;
        float yDelta = 0.75f;
        switch (numOfLights)
        {
            case 1:
                lightPositions.Add(new Vector3(xMin + (xMax - xMin) / 2, yMax - yDelta, zMin + (zMax - zMin) / 2));
                break;

            case 2:
                x = (xMax - xMin) / 4;
                z = (zMax - zMin) / 4;
                lightPositions.Add(new Vector3(xMin + x, yMax - yDelta, zMin + z));
                lightPositions.Add(new Vector3(xMin + x + (xMax - xMin) / 2, yMax - 0.3f, zMin+ z + (zMax - zMin) / 2));
                break;
            case 3:
                x = (xMax - xMin) / 2;
                z = (zMax - zMin) / 2;
                partitionX = (xMax - xMin) / 5;
                partitionZ = (xMax - xMin) / 5;
                lightPositions.Add(new Vector3(xMin + x, yMax - yDelta, zMin + z + partitionZ));
                lightPositions.Add(new Vector3(xMin + x - partitionX, yMax - yDelta, zMin + z - partitionZ));
                lightPositions.Add(new Vector3(xMin + x + partitionX, yMax - yDelta, zMin + z - partitionZ));
                break;
            case 4:
                x = (xMax - xMin) / 4;
                z = (zMax - zMin) / 4;
                lightPositions.Add(new Vector3(xMin + x, yMax - yDelta, z));
                lightPositions.Add(new Vector3(xMin + x, yMax - yDelta, zMin + z + (zMax - zMin) / 2));
                lightPositions.Add(new Vector3(xMin + x + (xMax - xMin) / 2, yMax - yDelta, zMin + z));
                lightPositions.Add(new Vector3(xMin + x + (xMax - xMin) / 2, yMax - yDelta, zMin + z + (zMax - zMin) / 2));
                break;
            case 5:
                x = (xMax - xMin) / 2;
                z = (zMax - zMin) / 2;
                partitionX = (xMax - xMin) / 5;
                partitionZ = (xMax - xMin) / 5;
                lightPositions.Add(new Vector3(xMin + x, yMax - yDelta, zMin + z));
                lightPositions.Add(new Vector3(xMin + x - partitionX, yMax - yDelta, zMin + z - partitionZ));
                lightPositions.Add(new Vector3(xMin + x - partitionX, yMax - yDelta, zMin + z + partitionZ));
                lightPositions.Add(new Vector3(xMin + x + partitionX, yMax - yDelta, zMin + z - partitionZ));
                lightPositions.Add(new Vector3(xMin + x + partitionX, yMax - yDelta, zMin + z + partitionZ));
                break;
            case 6:

                break;
            case 7:
                break;
            default:
                lightPositions.Add(new Vector3(xMin + (xMax - xMin) / 2, yMax - yDelta, zMin + (zMax - zMin) / 2));
                break;

        }
        return lightPositions;
    }
}
