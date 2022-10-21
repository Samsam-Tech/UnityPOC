using System.IO;
using UnityEngine;
using Dummiesman;
using UnityEditor;


public class ModelLoader : MonoBehaviour
{
    string relativePath;
    string absolutePath;
    public void LoadModel(string modelPath, string texPath, string targetDirectory, string fileName)
    {
        //import obj model
        GameObject model = new OBJLoader().Load(modelPath);

        GameObject child = model.transform.GetChild(0).gameObject;
        child.AddComponent<ModelRotate>();
        child.AddComponent<CameraPan>();

        //Get texture
        byte[] bytes = File.ReadAllBytes(texPath);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(bytes);
        texture.Apply();
        //Set texture
        Renderer rendr = child.GetComponent<Renderer>();
        rendr.material.mainTexture = texture;


        //CREATE PREFAB
        /*  absolutePath = string.Format("{0}/{1}.prefab", targetDirectory,model.name);


        if (absolutePath.StartsWith(Application.persistentDataPath))
         {
              relativePath = "Viewer" + absolutePath.Substring(Application.persistentDataPath.Length);
             //  relativePath = relativePath.Replace("\\\\", "/");
         }
         Debug.Log(relativePath);

        // Create the new Prefab and log whether Prefab was saved successfully.
         bool prefabSuccess;
         PrefabUtility.SaveAsPrefabAssetAndConnect(gameObject, relativePath, InteractionMode.UserAction, out prefabSuccess);
         if (prefabSuccess == true)
             Debug.Log("Prefab was saved successfully");
         else
           Debug.Log("Prefab failed to save" + prefabSuccess);
        */

        Camera.main.transform.LookAt(model.transform.position);
    }
}
