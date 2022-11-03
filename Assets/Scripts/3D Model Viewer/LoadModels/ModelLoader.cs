using UnityEngine;
using Dummiesman;


public class ModelLoader : MonoBehaviour
{
    Camera cameraMain;
    string relativePath;
    string absolutePath;

    private void Start()
    {
        cameraMain = Camera.main;
    }

    public void LoadModel(string modelPath, string texPath, string targetDirectory, string fileName)
    {
        //import obj model
        OBJLoader objLoader = new OBJLoader();

        GameObject model = objLoader.Load(modelPath);
        GameObject child = model.transform.GetChild(0).gameObject;

        //get texture
        Texture2D texture = new MTLLoader().TextureLoadFunction(texPath, false);

        //apply texture
        Renderer rendr = child.GetComponent<Renderer>();
        rendr.material.mainTexture = texture;

        //another way to get and apply texture
        // byte[] bytes = File.ReadAllBytes(texPath);
        // Texture2D texture = new Texture2D(2, 2);
        // texture.LoadImage(bytes);
        // texture.Apply();

        GameObject inputManager = new GameObject("InputManager");
        inputManager.AddComponent<InputManager>();
        inputManager.GetComponent<InputManager>().model = model;

        /*
                //create material
                // Material mat = child.GetComponent<Material>();
                // Material material = new Material(Shader.Find("Standard (Specular setup)"));
                // material.SetTexture("_MainTex", texture);
                // AssetDatabase.CreateAsset(material, $"Assets/Prefabs/{model.name}.mat");

                 //create prefab
                // absolutePath = System.IO.Path.Combine(targetDirectory, $"{model.name}.prefab");

                // if (absolutePath.StartsWith(Application.persistentDataPath))
                // {
                //     string relativeTo = Application.persistentDataPath;
                //     string path = absolutePath;

                //     relativePath = Path.GetRelativePath(relativeTo, path); 
                // }

                // // prefab utility can only save prefab inside unity project
                // string localPath = "Assets/Prefabs/" + model.name + ".prefab";
                // localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);

                // // Create the new Prefab and log whether Prefab was saved successfully.
                // bool prefabSuccess;

                // PrefabUtility.SaveAsPrefabAssetAndConnect(child, localPath, InteractionMode.UserAction, out prefabSuccess);
                // if (prefabSuccess == true)
                //     Debug.Log("Prefab was saved successfully");
                // else
                //     Debug.Log("Prefab failed to save" + prefabSuccess);

                // // Load the contents of the Prefab Asset.
                // GameObject contentsRoot = PrefabUtility.LoadPrefabContents(localPath);

                // // Modify Prefab contents.
                // contentsRoot.GetComponent<MeshFilter>().mesh = child.GetComponent<MeshFilter>().mesh;
                // Texture2D texturePrefab = (Texture2D)AssetDatabase.LoadAssetAtPath($"Assets/Prefabs/{model.name}.mat", typeof(Texture2D));
                // Debug.Log(child.GetComponent<MeshFilter>().mesh);
                // contentsRoot.GetComponent<Renderer>().material.mainTexture = texturePrefab;

                // // Save contents back to Prefab Asset and unload contents.
                // PrefabUtility.SaveAsPrefabAsset(contentsRoot, localPath);
                // // PrefabUtility.LoadPrefabContents(relativePath);
                // try
                // {
                //     File.Move(localPath, absolutePath);
                //     Debug.Log("Moved");
                // }
                // catch (IOException ex)
                // {
                //     Debug.Log(ex);
                // }
        */

        Vector3 cameraNewPos = child.GetComponent<Renderer>().bounds.center;
        cameraNewPos.z -= 250f; //To be change initial zoom
        cameraMain.transform.position = cameraNewPos;
        cameraMain.transform.LookAt(child.GetComponent<Renderer>().bounds.center);

        Destroy(this);
    }
}
