using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateBox : MonoBehaviour
{
    public GameObject boxToActivate;
    bool isActive = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void EnableBoxToggle()
    {
        isActive = !isActive;
        boxToActivate.SetActive(isActive);
    }

    public void DisableBox()
    {
        boxToActivate.SetActive(false);
        isActive = false;
    }
}
