using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryCell : CellScript, IPointerClickHandler {
    public int index;
    private GameManager manager;
    private LevelUpScript levelUpScript;

    private void Awake() {
        manager = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GameManager>();
        levelUpScript = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<LevelUpScript>();
    }

    public override void OnPointerClick(PointerEventData eventData) {
        base.OnPointerClick(eventData);
        ArtifactManager artifactManager = mainObject.GetComponent<ArtifactManager>();

        if (mainObject.GetComponent<InventoryScript>().itemsBase[mainObject.GetComponent<InventoryScript>().items[index].id].IsArtifact) {
            bool CanWeight = mainObject.GetComponent<InventoryScript>().itemsBase[mainObject.GetComponent<InventoryScript>().items[index].id].weight +
                    mainObject.GetComponent<ArtifactManager>().getTotalWeight() <= mainObject.GetComponent<GameManager>().maxWeight;
            bool canBeAdded = artifactManager.CanAddArtifact(mainObject.GetComponent<InventoryScript>().items[index].id);

            if (CanWeight && canBeAdded) {
                mainObject.GetComponent<ArtifactManager>().AddArtifact(mainObject.GetComponent<InventoryScript>().items[index].id);
                mainObject.GetComponent<InventoryScript>().RemoveObject(index);

                IsOnHover = false;
            }

            return;
        }

        AudioManager.PlaySound(mainObject.GetComponent<InventoryScript>().items[index].usingSound);

        if (mainObject.GetComponent<InventoryScript>().items[index].amount == 1)
            IsOnHover = false;
        mainObject.GetComponent<InventoryScript>().items[index].amount--;

        GameManager gameManager = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GameManager>();

        switch (mainObject.GetComponent<InventoryScript>().items[index].whatToDo) {
            case "Heal":
                float coefficient = ((artifactManager.IsArtifactWithWhatToDo("MagicBag"))? 1.5f : 1f);
                manager.HealHP((int)((float)gameManager.GetMaxHP() * 0.35f * coefficient));
                manager.UsedPotions++;
                break;
            case "Mana_Heal":
                manager.HealMP((int)((float)gameManager.GetMaxMP() * 0.4f));
                break;
            case "Scroll":
                manager.character_magicpower++;
                manager.SetMaxMP(manager.GetMaxMP() + 5);
                break;
            case "Apple":
                manager.HealHP(5);
                break;
            case "Enchanted Grindstone":
                manager.character_attack += 2;
                break;
            case "Heart":
                manager.SetMaxHP(manager.GetMaxHP() + 5);
                manager.HealHP(5);
                break;
            case "WisdomBook":
                levelUpScript.LevelUp();
                break;
            case "SimpleBook":
                manager.character_magicpower++;
                break;
        }
    }

    public override void OnPointerEnter(PointerEventData eventData) {
        base.OnPointerEnter(eventData);
    }
}
