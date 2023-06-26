using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseInteraction : MonoBehaviour
{
    public GameObject toSetVisiible;
    public void onSelect()
    {
        var cubeRenderer = GameObject.Find("CubeRayInteractable").GetComponent<Renderer>();
        cubeRenderer.material.SetColor("_Color", new Color(0, 0, 0));
        toSetVisiible.SetActive(true);
    }


    public void onHover()
    {
        var cubeRenderer = GameObject.Find("CubeRayInteractable").GetComponent<Renderer>();
        cubeRenderer.material.SetColor("_Color", new Color(255, 0, 0));
        toSetVisiible.SetActive(true);
    }
}
