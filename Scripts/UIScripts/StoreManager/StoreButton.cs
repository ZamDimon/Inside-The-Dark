using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StoreButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {
    public int index;
    public AudioClip clipToPlay;
    private InventoryScript inventoryScript;
    private GameManager gameManager;
    private MoneyManager moneyManager;
    private StoreManager storeManager;
    private DialogueWriting dialogueWriting;

    [HideInInspector] public bool IsOnHover;

    [HideInInspector] public GameObject priceObject;

    [HideInInspector] public int index_store;

    private void Awake() {
        inventoryScript = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<InventoryScript>();
        gameManager = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GameManager>();
        moneyManager = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<MoneyManager>();
        storeManager = GameObject.FindGameObjectWithTag("StoreManager").GetComponent<StoreManager>();
        dialogueWriting = GameObject.FindGameObjectWithTag("StoreDialogue").GetComponent<DialogueWriting>();
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (moneyManager.GetMoney() >= inventoryScript.itemsBase[index].price) {

            gameObject.GetComponent<Image>().color = Color.white;
            inventoryScript.AddItem(index, 1);

            moneyManager.TryTakeMoney(inventoryScript.itemsBase[index].price);
            storeManager.cells_store[index_store].index = -1;

            AudioManager.PlaySound(clipToPlay);

            if (!storeManager.IsBought)
                dialogueWriting.AddToArray("Good choice, maybe you wish something else?");

            storeManager.IsBought = true;
          }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        IsOnHover = true;
        storeManager.description_name.GetComponent<Text>().text = inventoryScript.itemsBase[index].name;
        storeManager.description_text.GetComponent<Text>().text = inventoryScript.itemsBase[index].description;
        storeManager.description_image.GetComponent<Image>().overrideSprite = inventoryScript.itemsBase[index].itemSprite;

        gameObject.GetComponent<Image>().color = Color.grey;

        /*skillNameObject.GetComponent<Text>().text = skillName;
        skillDescriptionObject.GetComponent<Text>().text = skillDescription;
        FotoObject.GetComponent<Image>().sprite = skillFoto;*/
    }

    public void OnPointerExit(PointerEventData eventData) {
        gameObject.GetComponent<Image>().color = Color.white;

        IsOnHover = false;
    }
}
