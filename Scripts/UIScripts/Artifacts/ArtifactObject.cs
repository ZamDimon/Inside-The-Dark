using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ArtifactObject : CellScript, IPointerClickHandler {
    public int index;

    public int index_manager;

    public override void OnPointerClick(PointerEventData eventData) {
        base.OnPointerClick(eventData);

        if (!GameObject.FindGameObjectWithTag("MainCamera").GetComponent<InventoryScript>().CanAddItem())
            return;

        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<InventoryScript>().AddItem(index, 1);
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<ArtifactManager>().RemoveArtifact(index_manager);

        transform.GetComponent<Image>().color = Color.white;
    }
}