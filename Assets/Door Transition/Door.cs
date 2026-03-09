using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    public string sceneName;
    public Material deemphasizedMat;
    public Material emphasizedMat;

    private MeshRenderer meshRenderer;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void Emphasize()
    {
        meshRenderer.material = emphasizedMat;
    }

    public void Deemphasize()
    {
        meshRenderer.material = deemphasizedMat;
    }

    public void Open()
    {
        SceneManager.LoadScene(sceneName);
    }
}
