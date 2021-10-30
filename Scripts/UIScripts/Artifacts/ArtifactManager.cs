using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ArtifactManager : MonoBehaviour
{
    [System.Serializable]
    public struct cell {
        public int index;
        public GameObject imageObject;
        public GameObject imageObject_campfire;
    };

    public cell[] cells = new cell[4];

    public GameObject weightObject;
    public GameObject textObject;

    public GameObject weightObject_campfire;
    public GameObject textObject_campfire;

    private InventoryScript inventoryScript;

    public void SaveData() {
        for (int i = 0; i < cells.Length; ++i)
            PlayerPrefs.SetInt("artifact" + i, cells[i].index);
    }

    public void LoadData() {
        for (int i = 0; i < cells.Length; ++i)
            cells[i].index = PlayerPrefs.GetInt("artifact" + i);
    }

    public int getTotalWeight() {
        int result = 0;

        for (int i = 0; i < cells.Length; ++i) {
            if (cells[i].index != -1) 
                result += GameObject.FindGameObjectWithTag("MainCamera").GetComponent<InventoryScript>().itemsBase[cells[i].index].weight;
        }

        return result;
    }

    private void SetStartIndexes() {
        for (int i = 0; i < cells.Length; ++i) {
            cells[i].index = -1;
            cells[i].imageObject.GetComponent<ArtifactObject>().index_manager = i;
            cells[i].imageObject_campfire.GetComponent<ArtifactObject>().index_manager = i;
        }
    }

    private void UpdateVisibility () {
        for (int i = 0; i < cells.Length; ++i) {
            cells[i].imageObject.GetComponent<ArtifactObject>().index = cells[i].index;
            cells[i].imageObject_campfire.GetComponent<ArtifactObject>().index = cells[i].index;

            cells[i].imageObject.SetActive(cells[i].index != -1);
            cells[i].imageObject_campfire.SetActive(cells[i].index != -1);

            if (cells[i].index != -1) {
                cells[i].imageObject.GetComponent<Image>().overrideSprite =
                    inventoryScript.itemsBase[cells[i].index].itemSprite;
                cells[i].imageObject_campfire.GetComponent<Image>().overrideSprite =
                    inventoryScript.itemsBase[cells[i].index].itemSprite;
            }
        }
    }

    public bool CanAddArtifact(int index) {
        if (!inventoryScript.itemsBase[index].IsArtifact) {
            return false;
        }

        for (int i = 0; i < cells.Length; ++i) {
            if (cells[i].index == index) {
                return false;
            }
        }

        bool allCellsBlocked = true;
        for (int i = 0; i < cells.Length; ++i) {
            if (cells[i].index < 0) 
                allCellsBlocked = false;
        }
        return !allCellsBlocked;
    }

    public bool IsArtifactWithWhatToDo (string whatToDo) {
        for (int i = 0; i < cells.Length; ++i) {
            if (cells[i].index < 0)
                continue;

            if (inventoryScript.itemsBase[cells[i].index].whatToDo == whatToDo)
                return true;
        }

        return false;
    }

    public void AddArtifact(int index) {
        if (!CanAddArtifact(index)) {
            //Против дураков, которые додумались сунуть сюда не артефакты, а юзлесс шмотки
            Debug.Log("Ты идиот?"); //Чтобы точно знал...
            return; 
        }

        GameManager gameManager = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GameManager>();
        switch (inventoryScript.itemsBase[index].whatToDo) {
            case "InfantrySword":
                gameManager.character_attack += 2;
                break;
            case "MagicSword":
                gameManager.SetMaxMP(gameManager.GetMaxMP() + 30);
                gameManager.character_magicpower++;
                break;
            case "WoodenShield":
                gameManager.SetMaxHP(gameManager.GetMaxHP() + 15);
                break;
            case "KnightShield":
                gameManager.SetMaxHP(gameManager.GetMaxHP() + 25);
                gameManager.character_defence += 5;
                break;
            case "InfantryShield":
                gameManager.SetMaxHP(gameManager.GetMaxHP() + 20);
                break;
            case "Rapier":
                gameManager.character_speed += 2;
                break;
            case "ShipSword":
                gameManager.character_speed += 2;
                break;
        }

        for (int i = 0; i < cells.Length; ++i) {
            if (cells[i].index == -1) {
                cells[i].index = index;
                cells[i].imageObject.GetComponent<ArtifactObject>().index = cells[i].index;
                cells[i].imageObject_campfire.GetComponent<ArtifactObject>().index = cells[i].index;
                
                cells[i].imageObject.GetComponent<ArtifactObject>().SetInfoSettings(
                    inventoryScript.itemsBase[cells[i].index].name,
                    inventoryScript.itemsBase[cells[i].index].description,
                    inventoryScript.itemsBase[cells[i].index].itemSprite,
                    inventoryScript.itemsBase[cells[i].index].DoDescription
                );
                //cells[i].imageObject.GetComponent<ArtifactObject>().
                break;
            }
        }
    }

    public void RemoveArtifact (int number) {
        GameManager gameManager = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GameManager>();
        Debug.Log("Number: " + cells[number].imageObject.GetComponent<ArtifactObject>().index_manager.ToString());

        switch (inventoryScript.itemsBase[cells[number].index].whatToDo) {
            case "InfantrySword":
                gameManager.character_attack -= 2;
                break;
            case "MagicSword":
                gameManager.SetMaxMP(gameManager.GetMaxMP() - 30);
                gameManager.character_magicpower--;
                break;
            case "WoodenShield":
                gameManager.SetMaxHP(gameManager.GetMaxHP() - 15);
                break;
            case "KnightShield":
                gameManager.SetMaxHP(gameManager.GetMaxHP() - 25);
                gameManager.character_defence -= 5;
                break;
            case "InfantryShield":
                gameManager.SetMaxHP(gameManager.GetMaxHP() - 20);
                break;
            case "Rapier":
                gameManager.character_speed -= 2;
                break;
            case "ShipSword":
                gameManager.character_speed -= 2;
                break;
        }

        cells[number].index = -1;
    }

    private void Start() {
        SetStartIndexes();

        inventoryScript = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<InventoryScript>();
        if (SceneManager.GetActiveScene().buildIndex != 1)
            LoadData();
    }

    private void Update() {
        weightObject.GetComponent<Image>().fillAmount = (float)getTotalWeight() / GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GameManager>().maxWeight;
        textObject.GetComponent<Text>().text = "Weight: " + getTotalWeight() + "/" + GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GameManager>().maxWeight;

        weightObject_campfire.GetComponent<Image>().fillAmount = (float)getTotalWeight() / GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GameManager>().maxWeight;
        textObject_campfire.GetComponent<Text>().text = "Weight: " + getTotalWeight() + "/" + GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GameManager>().maxWeight;

        UpdateVisibility();
    }
}
