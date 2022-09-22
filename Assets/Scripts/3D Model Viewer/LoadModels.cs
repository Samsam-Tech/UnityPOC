using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadModels : MonoBehaviour
{
    [SerializeField] Camera cameraView;
    GameObject loadedModel;
    GameObject EmptyObj;
    string modelPath = "sofa/sofa";
    string texPath = "sofa/sofaTex";

    // Start is called before the first frame update
    void Start()
    {
        
        GameObject model = Resources.Load<GameObject>(modelPath);
        loadedModel = Instantiate(model);
        loadedModel.SetActive(true);

        GameObject child = loadedModel.transform.GetChild(0).gameObject;

        child.AddComponent<ModelRotate>();
        child.AddComponent<CameraPan>();

        Renderer rendr = child.GetComponent<Renderer>();

        Texture texture = Resources.Load<Texture>(texPath);
        rendr.material.mainTexture = texture;

        cameraView.transform.LookAt(loadedModel.transform.position);

        Destroy(this.gameObject);
    }

}
