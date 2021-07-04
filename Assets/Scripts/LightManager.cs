using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : Helper
{
    private GameObject lightSources;
    private int lightIntensityLow = 1;
    private int lightIntensityHigh = 7;
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
        GameObject sunLight = lightSources.gameObject.transform.Find("Directional Light").gameObject;
        GameObject alternate = lightSources.gameObject.transform.Find("Point Light").gameObject;
        List<GameObject> lightSourceList = new List<GameObject>();


        if (Random.value < 0.5f)    
        {
            var currentBounds = GetMeshHierarchyBounds(room);
            int lightCount = Random.Range(0, 5);

            List<Vector3> lightPositions = getLightPositions(lightCount, currentBounds.min.x, currentBounds.max.x, currentBounds.min.z, currentBounds.max.z, currentBounds.max.y);
            float lightIntensity = Random.Range(indoorLightIntensityLow, indoorLightIntensityHigh);

            foreach (Vector3 position in lightPositions)
            {
                GameObject light = getLightSources().gameObject.transform.Find("Point Light").gameObject;
                GameObject lightSource = GameObject.Instantiate(light);
                lightSource.SetActive(true);
                lightSource.transform.position = position;
                lightSource.GetComponent<Light>().intensity = lightIntensity;

                lightSourceList.Add(lightSource);
            }
        }
        else
        {
            //sun light
            sunLight.GetComponent<Light>().intensity = Random.Range(lightIntensityLow, lightIntensityHigh);
            sunLight.gameObject.transform.eulerAngles = new Vector3(Random.Range(lightXRotationLow, lightXRotationHigh), Random.Range(lightYRotationLow, lightYRotationHigh), 0);

            GameObject lightSource = GameObject.Instantiate(sunLight);
            lightSource.SetActive(true);
            lightSourceList.Add(lightSource);
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
