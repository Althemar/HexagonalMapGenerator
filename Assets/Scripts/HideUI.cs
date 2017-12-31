using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideUI : MonoBehaviour
{

    GameObject[] UItoHide;

    private void Start() {
        UItoHide = GameObject.FindGameObjectsWithTag("UIHide");
    }


    public void SetVisibility(bool visibility) {
        for (int i = 0; i < UItoHide.Length; i++) {
            UItoHide[i].SetActive(visibility);
        }
    }
}
