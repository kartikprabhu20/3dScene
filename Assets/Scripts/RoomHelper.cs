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
        "cabinet", "bed", "chair", "sofa", "table", "bookshelf","dresser", "desks", "shelves", "bench","shelf","furniture","drawer","stool", //Furnitures
        "wall", "floor","ceiling"};

    Dictionary<string, string> renameDictionary = new Dictionary<string, string>()
        {
            {"television", "tv"},{"lcd", "tv" },
            {"light","lamp"},{"pilow","pillow"}
        };

    private System.Random random = new System.Random();
    private Shader shader;
    private Texture2D tex;

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

    public void applyTextures(GameObject gameObject,string rootTexturePath)
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

                Renderer rend = child.GetComponent<Renderer>();
                if (rend != null)
                {
                    //Debug.Log("applying texture");
                    if (!textureDictionary.ContainsKey(child.name))
                    {
                        textureDictionary.Add(child.name, combinedList[random.Next(combinedList.Length)]);//As we iterate through the texturefolder add it to dictionary as a cache
                    }
                    
                     loadTexture(textureDictionary[child.name], rend);

                    if (child.name == "floor") {
                        rend.material.mainTextureScale = new Vector2(5, 5);//Tiling 10x10
                    }

                }
                //break;
            }
        }
    }


    void loadTexture(string filePath, Renderer rend)
    {
        
        if (File.Exists(filePath))
        {
            this.tex = new Texture2D(2, 2);
            byte[] fileData = File.ReadAllBytes(filePath);
            this.tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
            rend.material.mainTexture = tex;
        }
        else if (Directory.Exists(filePath)){

            string[] textureList = Directory.GetFiles(filePath);

            Debug.Log(filePath);
            rend.material.EnableKeyword("_NORMALMAP");
            rend.material.EnableKeyword("_METALLICGLOSSMAP");

            TextureHelper texHelper = new TextureHelper();
            foreach (string texture in textureList)
            {
                string textureKey = texHelper.getTextureKey(texture);
                rend.material.EnableKeyword(textureKey);

                this.tex = new Texture2D(2, 2);
                byte[] fileData = File.ReadAllBytes(texture);
                this.tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
                rend.material.SetTexture(textureKey, this.tex);

            }
 
        }
    }
}
