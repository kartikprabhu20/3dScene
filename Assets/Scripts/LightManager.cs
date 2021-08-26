using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager : Helper
{
    private GameObject lightSources;
    private float lightIntensityLow = 0;
    private float lightIntensityHigh = 0.75f;
    private float indoorLightIntensityLow = 0f;
    private float indoorLightIntensityHigh = 1.25f;
    private float rangeLow = 10f;
    private float rangeHigh = 20f;
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

        Transform[] roomChildren = room.GetComponentsInChildren<Transform>();
        List<GameObject> ceilingList = new List<GameObject>();
        foreach(Transform child in roomChildren)
        {
            if (child.name.Contains("ceil"))
            {
                ceilingList.Add(child.gameObject);
            }
        }

        string[] allChildren = {"PointLight","SunLight","SpotLight","Ceiling"};
        int randomChild = Random.Range(0, allChildren.Length);
        System.Random rnd = new System.Random();

        switch (allChildren[randomChild])
        {
            case "PointLight":
                var currentBounds = GetMeshHierarchyBounds(room);
                int lightCount = Random.Range(0, 5);

                List<Vector3> lightPositions = getLightPositions(lightCount, currentBounds.min.x, currentBounds.max.x, currentBounds.min.z, currentBounds.max.z, currentBounds.max.y);
                float lightIntensity = Random.Range(indoorLightIntensityLow, indoorLightIntensityHigh);
                float lightRange = Random.Range(rangeLow, rangeHigh);

                foreach (Vector3 position in lightPositions)
                {
                    GameObject light = getLightSources().gameObject.transform.Find("PointLight").gameObject;
                    light = GameObject.Instantiate(light);
                    light.SetActive(true);
                    light.transform.position = position;
                    light.GetComponent<Light>().intensity = lightIntensity;
                    light.GetComponent<Light>().range = lightRange;
                    lightSourceList.Add(light);
                }
                GameObject reflectionProbe = GameObject.Instantiate(getLightSources().gameObject.transform.Find("ReflectionProbe1").gameObject);
                reflectionProbe.SetActive(true);
                reflectionProbe.transform.position = lightPositions[rnd.Next(lightPositions.Count)];
                lightSourceList.Add(reflectionProbe);

                if (ceilingList.Count != 0)
                {
                    foreach (GameObject ceiling in ceilingList)
                    {
                        ceiling.AddComponent<Light>();
                        ceiling.GetComponent<Light>().intensity = 1.0f;
                        ceiling.GetComponent<Light>().range = lightRange;

                    }

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

                if (ceilingList.Count != 0)
                {
                    foreach (GameObject ceiling in ceilingList)
                    {
                        ceiling.AddComponent<Light>();
                        ceiling.GetComponent<Light>().intensity = UnityEngine.Random.Range(1.0f, 2.0f);
                    }

                }

                break;

            case "SpotLight":
                Debug.Log("SpotLight");
                var currentBounds_2 = GetMeshHierarchyBounds(room);
                GameObject spotLight = lightSources.gameObject.transform.Find("SpotLight").gameObject;
                GameObject spotSource = GameObject.Instantiate(spotLight);
                spotSource.SetActive(true);
                spotSource.GetComponent<Light>().intensity = Random.Range(indoorLightIntensityLow, indoorLightIntensityHigh);
                spotSource.GetComponent<Light>().range = Random.Range(rangeLow, rangeHigh); ;

                var modelBounds = GetMeshHierarchyBounds(curretModel);
                spotSource.transform.position = new Vector3(curretModel.transform.position.x, currentBounds_2.max.y - .75f, curretModel.transform.position.z);

                Debug.Log(spotLight.transform.position);
                
                lightSourceList.Add(spotSource);

                GameObject reflectionProbe2 = GameObject.Instantiate(getLightSources().gameObject.transform.Find("ReflectionProbe1").gameObject);
                reflectionProbe2.SetActive(true);
                reflectionProbe2.transform.position = spotSource.transform.position;
                lightSourceList.Add(reflectionProbe2);

                if (ceilingList.Count != 0)
                {
                    foreach (GameObject ceiling in ceilingList)
                    {
                        ceiling.AddComponent<Light>();
                        ceiling.GetComponent<Light>().intensity = 1.0f;

                        Color color;
                        ColorUtility.TryParseHtmlString(getColors()[rnd.Next(getColors().Count)], out color);
                        ceiling.GetComponent<Light>().color = color;
                    }

                }
                break;

            case "SpotLightCamera":
                //TODO: add light from camera
                break;

            case "Ceiling":

                if (ceilingList.Count != 0)
                {
                    foreach (GameObject ceiling in ceilingList)
                    {
                        ceiling.AddComponent<Light>();
                        ceiling.GetComponent<Light>().intensity = UnityEngine.Random.Range(0.5f, 2.0f);
                    }

                }
                break;

        }

        return lightSourceList;
    }

    public List<string> getColors()
    {
        return new List<string>() {"#FFF53B", "#FFFFFF", "#58D3D7", "#7BEA7E", "#E9C77A", "#FFFFFF" , "#FFFFFF" , "#FFFFFF" , "#FFFFFF" }; //more whites
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
