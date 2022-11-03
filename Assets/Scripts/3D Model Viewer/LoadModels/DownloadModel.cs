using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using System.IO.Compression;

public class DownloadModel : MonoBehaviour
{
    public string savePath;
    public string fileName;
    public string sourcePath;
    public string extractFilePath;
    public string url;
    ModelLoader modelLoader;
    // Start is called before the first frame update

    void Start()
    {
        modelLoader = gameObject.GetComponent<ModelLoader>();
        url = "https://drive.google.com/drive/folders/1O1uyJFHFS6_z0gng1JkkM7ZW7XS1IwHl"; //change url
        sourcePath = System.IO.Path.Combine(Application.persistentDataPath, "Assets");

        DownloadFile(url);
    }

    IEnumerator GetFileRequest(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            webRequest.method = UnityWebRequest.kHttpVerbGET; // GET request
            DownloadHandlerFile downloadHandlerFile = new DownloadHandlerFile(GetFilePath(url)); //create file with path and filename provided
            downloadHandlerFile.removeFileOnAbort = true; //can abort download whioh will remove the unfinished downloaded file
            webRequest.downloadHandler = downloadHandlerFile;

            yield return webRequest.SendWebRequest();

            string[] pages = url.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:

                    Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler);

                    extractFilePath = $"{sourcePath}/{fileName}";

                    ExtractFile(savePath, extractFilePath);
                    // ExtractFile($"{Application.persistentDataPath}/Assets/BlackSofa.zip", $"{Application.persistentDataPath}/Models/BlackSofa");

                    FindModelOBJ(extractFilePath, fileName);
                    // FindModelOBJ($"{Application.persistentDataPath}/Assets/BlackSofa");

                    break;
            }
        }
    }

    public void FindModelOBJ(string targetDirectory, string fileName)
    {
        // Process the list of files found in the directory.
        string[] models = Directory.GetFiles(targetDirectory, "*.obj");
        string[] modelTextures = Directory.GetFiles(targetDirectory, "*.png");

        modelLoader.LoadModel(models[0], modelTextures[0], targetDirectory, fileName);
        Destroy(this.gameObject);
    }

    void DownloadFile(string url)
    {
        string path = GetFilePath(url);

        if (File.Exists(path))
        // if (File.Exists($"{Application.persistentDataPath}/Assets/BlackSofa.zip"))
        {
            FindModelOBJ(path,fileName); //get obj and texture
            // FindModelOBJ($"{Application.persistentDataPath}/Assets/BlackSofa", fileName);

            return;
        }
        StartCoroutine(GetFileRequest(url));
    }


    string GetFilePath(string url)
    {
        string[] pieces = url.Split('/');
        fileName = pieces[pieces.Length - 1];
        string savePath = System.IO.Path.Combine(sourcePath, $"{fileName}.zip");

        return savePath;
    }

    public void ExtractFile(string sourcepZipFile, string extractPath)
    {
        ZipFile.ExtractToDirectory(sourcepZipFile, extractPath);

        //Move zip file inside
        try
        {
            File.Move(extractPath, sourcepZipFile);
            Debug.Log("Moved");
        }
        catch (IOException ex)
        {
            Debug.Log(ex);
        }
    }

}
