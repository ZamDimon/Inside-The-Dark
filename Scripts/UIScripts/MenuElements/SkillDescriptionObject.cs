using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillDescriptionObject : MonoBehaviour
{
    [SerializeField] private GameObject statObject;
    private bool IsActive;

    private void Update() {
        IsActive = false;
        GameObject[] buttonObjects = GameObject.FindGameObjectsWithTag("SkillButtons");

        for (int i = 0; i < buttonObjects.Length; ++i) {
            if (buttonObjects[i].GetComponent<CellScript>().GetHoverState())
                IsActive = true;
        }

        statObject.GetComponent<NewDescription>().UpdatePosition();
        statObject.SetActive(IsActive);
    }
}
