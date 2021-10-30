using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreManager : MonoBehaviour
{
    [System.Serializable]
    public struct cell_inventory {
        public GameObject imageObject, amountObject;
    };

    [System.Serializable]
    public struct cell_store {
        public int index;
        public GameObject imageObject, priceObject, coinObject;
    };

    public cell_inventory[] cells_inventory;
    public cell_store[] cells_store;

    public GameObject description_image, description_text, description_name;
    public GameObject darkScreen;

    [HideInInspector]
    public bool IsBought;

    public void CloseStore() {
        darkScreen.SetActive(false);
        gameObject.SetActive(false);
        AudioManager.PlayMusic(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<BattleAnimationScript>().passiveSound);
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GameManager>().IsClosedStore = false;
    }

    private void UpdateCellsVisibility() {
        for (int i = 0; i < GameObject.FindGameObjectWithTag("MainCamera").GetComponent<InventoryScript>().items.Length; ++i) {
            cells_inventory[i].imageObject.SetActive(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<InventoryScript>().items[i].amount > 0);
            cells_inventory[i].amountObject.SetActive(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<InventoryScript>().items[i].amount > 0);

            if (GameObject.FindGameObjectWithTag("MainCamera").GetComponent<InventoryScript>().items[i].amount > 0) {
                cells_inventory[i].imageObject.GetComponent<Image>().overrideSprite = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<InventoryScript>().items[i].itemTexture;
                cells_inventory[i].amountObject.GetComponent<Text>().text = "" + GameObject.FindGameObjectWithTag("MainCamera").GetComponent<InventoryScript>().items[i].amount;
            }
        }
    }

    private void SetIndexToImages() {
        for (int i = 0; i < cells_store.Length; ++i) {
            cells_store[i].imageObject.GetComponent<StoreButton>().index_store = i;
            cells_store[i].index = -1;
        }
    }

    private void UpdateCellsStoreVisibility() {
        for (int i = 0; i < cells_store.Length; ++i) {
            cells_store[i].imageObject.GetComponent<StoreButton>().index = cells_store[i].index;

            if (cells_store[i].index == -1) {
                cells_store[i].imageObject.SetActive(false);
                cells_store[i].priceObject.SetActive(false);
                cells_store[i].coinObject.SetActive(false);
            }
            else {
                cells_store[i].imageObject.GetComponent<Image>().overrideSprite = 
                    GameObject.FindGameObjectWithTag("MainCamera").GetComponent<InventoryScript>().itemsBase[cells_store[i].index].itemSprite;
                cells_store[i].priceObject.GetComponent<Text>().text = 
                    "" + GameObject.FindGameObjectWithTag("MainCamera").GetComponent<InventoryScript>().itemsBase[cells_store[i].index].price;

                cells_store[i].imageObject.GetComponent<StoreButton>().index = cells_store[i].index;
                cells_store[i].imageObject.GetComponent<StoreButton>().priceObject = cells_store[i].priceObject;
            }
        }
    }


    void Start() {
        //Some start staff here
        SetIndexToImages();

        cells_store[0].index = 0;
        cells_store[1].index = 1;

        int chance = Random.Range(0, 100);
        if (chance >= 0 && chance < 25)
            cells_store[2].index = 3;
        if (chance >= 25 && chance < 50)
            cells_store[2].index = 5;
        if (chance >= 50 && chance < 75)
            cells_store[2].index = 6;
        if (chance >= 75 && chance <= 100)
            cells_store[2].index = 8;

        cells_store[3].index = Random.Range(9, 11);
        cells_store[4].index = Random.Range(11, 15);

        int chance2 = Random.Range(0, 100);
        if (chance2 >= 0 && chance2 < 33)
            cells_store[5].index = 16;
        if (chance2 >= 33 && chance2 < 66)
            cells_store[5].index = 17;
        if (chance2 >= 66 && chance2 <= 100)
            cells_store[5].index = 20;

        cells_store[6].index = 19;

        UpdateCellsStoreVisibility();
    }

    void Update() {
        UpdateCellsVisibility();
        UpdateCellsStoreVisibility();
    }
}
