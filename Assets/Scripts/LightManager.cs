using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightManager
{
    private GameObject lightSources;

    public LightManager(GameObject lightSources)
    {
        this.lightSources = lightSources;

    }

    public void randomizeLight(GameObject curretModel)
    {
        GameObject source = lightSources.gameObject.transform.Find("Directional Light").gameObject;
        GameObject alternate = lightSources.gameObject.transform.Find("Point Light").gameObject;

        
        if (Random.value < 0.5f)    
        {
            source.SetActive(false);
            alternate.SetActive(true);
        }
        else
        {

            source.SetActive(true);
            alternate.SetActive(false);
        }
    }
}
