using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System;

public class RoomHelper: Helper
{
    //list of common object, least important to most important(roughly)
    public string[] commonObjects = {
          "bag", "person","whiteboard",  "Napkin", "Paper","dispenser","box", //misc
        "picture","deco","basket","shield", "candle","plant", "book","cd", // decorations
        "counter", "pillow","cushion", "duvet","mirror",  "clothes",  //bedroom
        "refrigerator", "television","tv","lcd", "lamp","nightStand","fridge", "light","pendant","speaker","laptop", "projector", "keyboard","stereo","dvd",//Devices
        "cup", "pot","plate","bottle","wine", "carafe", "bowl","vase","fruit", "cooker","exhaust","ventilator",//kitchen_dining_hall
        "door", "window", "curtain", "blinds", "carpet", "floor-mat", "painting", //Room
        "cabinet", "bed", "chair", "sofa", "table", "bookshelf","dresser", "desks", "shelves", "bench","shelf","furniture","drawer","stool", "bookcase",//Furnitures
        "wall", "floor","ceiling"};

    Dictionary<string, string> renameDictionary = new Dictionary<string, string>()
        {
            {"television", "tv"},{"lcd", "tv" },
            {"light","lamp"},{"pilow","pillow"}
        };

    private Shader shader;

    public RoomHelper(Shader shader)
    {
        this.shader = shader;
    }


    public void randomiseSkybox(List<Material> skyBoxList)
    {
        RenderSettings.skybox = skyBoxList[random.Next(skyBoxList.Count)];
    }


    public void changeShader(GameObject gameObject)
    {
        Transform[] allChildren = gameObject.GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            Renderer rend = child.GetComponent<Renderer>();
            if (rend != null)
            {
                rend.material.shader = shader;
            }
        }
    }

    public void preprocessRoom(GameObject roomModel)
    {
        Transform[] allChildren = roomModel.GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            renameGameobject(child.gameObject);
            attachMeshColliders(child.gameObject);
        }

        //Rigidbody gameObjectsRigidBody = roomModel.AddComponent<Rigidbody>();
    }

    private void renameGameobject(GameObject gameobject)
    {
        //Debug.Log("renameGameobject");

        string gameobjectName = gameobject.name.ToLower();
        //Debug.Log(gameobjectName);

        foreach(string key in renameDictionary.Keys)
        {
            if (gameobjectName.Contains(key))
            {
                gameobject.name = renameDictionary[key];
            }
        }
        foreach (string category in commonObjects)
        {
            if (gameobjectName.Contains(category)){
                gameobject.name = category;
                //return; #dont break, example: ceiling_lights needs to be renamed as light not ceiling
            }
        }

    }

    public void applyTextures(GameObject gameObject, string rootTexturePath)
    {
        string[] dirList = Directory.GetDirectories(rootTexturePath);

        Dictionary<string, string> textureDictionary = new Dictionary<string, string>();

        Transform[] allChildren = gameObject.GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            string textureFolder = rootTexturePath + Path.DirectorySeparatorChar + child.name.Split('.')[0];
            if (dirList.Contains(textureFolder))//Check if there is a texturefolder with name same as category
            {
                string[] textureList = Directory.GetFiles(textureFolder);
                string[] textureDirList = Directory.GetDirectories(textureFolder);

                string[] combinedList = textureList.ToList().Concat(textureDirList.ToList()).ToArray();

                string texturePath = combinedList[random.Next(combinedList.Length)];
                if (!textureDictionary.ContainsKey(child.name))
                {
                    textureDictionary.Add(child.name, texturePath);//As we iterate through the texturefolder add it to dictionary as a cache
                }

                Renderer rend = child.GetComponent<Renderer>();

                //textureChildren(child.gameObject, textureDictionary[child.name]);
                loadTexture(texturePath, rend);
                //break;
            }
        }
    }
}
