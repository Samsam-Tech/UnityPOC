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
        url = "https://drive.google.com/drive/folders/1O1uyJFHFS6_z0gng1JkkM7ZW7XS1IwHl";
        sourcePath = $"{Application.persistentDataPath}/Models/";
        string savePath = string.Format("{0}/{1}.zip", Application.persistentDataPath, "TestSave");
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
                    // ExtractFile(savePath, extractFilePath);

                    ExtractFile($"{Application.persistentDataPath}/Models/BlackSofa.zip", $"{Application.persistentDataPath}/Models/BlackSofa");
                    FindModelOBJ($"{Application.persistentDataPath}/Models/BlackSofa");
                    // LoadModel(path);
                    break;
            }
        }
    }

    public void FindModelOBJ(string targetDirectory)
    {
        // Process the list of files found in the directory.
        string[] models = Directory.GetFiles(targetDirectory, "*.obj");
        string[] modelTextures = Directory.GetFiles(targetDirectory, "*.png");
        Debug.Log(models[0]);
        Debug.Log(modelTextures[0]);
        modelLoader.LoadModel(models[0], modelTextures[0],targetDirectory, fileName);
        Destroy(this.gameObject);
    }

    void DownloadFile(string url)
    {
        string path = GetFilePath(url);

        if (File.Exists($"{Application.persistentDataPath}/Models/BlackSofa.zip"))
        {
            Debug.Log("Found file locally, loading...");
            // ExtractFile($"{Application.persistentDataPath}/Models/BlackSofa.zip", $"{Application.persistentDataPath}/Models/BlackSofa");
            FindModelOBJ($"{Application.persistentDataPath}/Models/BlackSofa");
            // 
            // LoadModel(path);
            return;
        }
        StartCoroutine(GetFileRequest(url));
    }


    string GetFilePath(string url)
    {
        string[] pieces = url.Split('/');
        fileName = pieces[pieces.Length - 1];

        savePath = string.Format("{0}/{1}.zip", sourcePath, fileName);

        return savePath;
    }

    public void ExtractFile(string sourcepZipFile, string extractPath)
    {
        ZipFile.ExtractToDirectory(sourcepZipFile, extractPath);
    }

}
