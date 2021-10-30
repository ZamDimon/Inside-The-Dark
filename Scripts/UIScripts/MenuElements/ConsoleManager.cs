using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ConsoleManager : MonoBehaviour
{
    #region Dictionary settings
    private Dictionary<string, Action<string[]>> commands = new Dictionary<string, Action<string[]>>();
    #endregion

    #region Objects and text settings
    public int TextSize = 8;
    public GameObject inputFieldObject;
    #endregion

    #region References to other scripts
    private InventoryScript inventoryScript;
    private SkillSystem skillSystem;
    private LevelGenerator levelGenerator;
    private CombatSystem combatSystem;
    private GameManager gameManager;
    private Loot lootSystem;
    private LevelUpScript levelUpScript;
    #endregion

    public void RunCommand () {
        ApplyCommand_TextFormat(inputFieldObject.GetComponent<InputField>().text);
        inputFieldObject.GetComponent<InputField>().text = ""; 
    }

    public void WriteLine (string line) {
        transform.GetComponent<Text>().text += "\n" + ">" + line;
        gameObject.GetComponent<Text>().rectTransform.sizeDelta = new Vector2(gameObject.GetComponent<Text>().rectTransform.sizeDelta.x,
            gameObject.GetComponent<Text>().rectTransform.sizeDelta.y + TextSize);
    }

    private void InitializeClasses() {
        GameObject mainObject = GameObject.FindGameObjectWithTag("MainCamera");

        inventoryScript = mainObject.GetComponent<InventoryScript>();
        skillSystem = mainObject.GetComponent<SkillSystem>();
        levelGenerator = mainObject.GetComponent<LevelGenerator>();
        combatSystem = mainObject.GetComponent<CombatSystem>();
        gameManager = mainObject.GetComponent<GameManager>();
        lootSystem = mainObject.GetComponent<Loot>();
        levelUpScript = mainObject.GetComponent<LevelUpScript>();
    }

    private void InputFieldController() {
        if (EventSystem.current.currentSelectedGameObject == inputFieldObject && Input.GetKeyDown(KeyCode.Return)) {
            RunCommand();
        }
    } 

    private void CreateCommandsDictionary() {
        commands.Add("AddItem", (args) => inventoryScript.AddItem(int.Parse(args[0]), int.Parse(args[1])));
        commands.Add("SetSkill", (args) => skillSystem.SetSkill(int.Parse(args[0]), int.Parse(args[1])));
        commands.Add("OpenMap", (args) => levelGenerator.SetAllVisible());
        commands.Add("LevelUp", (args) => levelUpScript.LevelUp());
        commands.Add("AddEnemy", (args) => combatSystem.AddEnemyFront(int.Parse(args[0])));
        commands.Add("DealSelfDamage", (args) => gameManager.TryDealDamage(int.Parse(args[0])));
        commands.Add("HealHPPlayer", (args) => gameManager.HealHP(int.Parse(args[0])));
        commands.Add("HealMPPlayer", (args) => gameManager.HealMP(int.Parse(args[0])));
        commands.Add("AddItemLootSystem", (args) => lootSystem.AddItem(int.Parse(args[0]), int.Parse(args[1])));
    }

    private string RemoveSpacesFromString(string text) => text.Replace(" ", string.Empty);

    private void ApplyCommand_TextFormat(string commandText) {
        commandText = RemoveSpacesFromString(commandText);
        string key = "", argsText = ""; string[] args; 
        int indexOnLeftBracket = 0;
        
        for (int i = 0; i < commandText.Length; ++i) {
            if (commandText[i] == '(') {
                indexOnLeftBracket = i;
                break;
            }

            key += commandText[i];
        }

        if (key == commandText) {
            Debug.LogError("Left bracket is missing");
            WriteLine("<color=red>The command was not defined</color>");
            return;
        }

        for (int i = indexOnLeftBracket + 1; i < commandText.Length; ++i) {
            if (commandText[i] == ')')
                break;

            argsText += commandText[i];
        }

        args = argsText.Split(',');

        TryRunCommand(key, args);
    }

    private string GetTextWithCommas(string[] inputValues) {
        string result = "";

        for (int i = 0; i < inputValues.Length; ++i) {
            result += inputValues[i] + ((i == inputValues.Length - 1)? string.Empty : ", ");
        }

        return result;
    }
    private void TryRunCommand(string key, string[] inputValues) {
        Action<string[]> value;
        if (commands.TryGetValue(key, out value)) {
            value.Invoke(inputValues);
            WriteLine($"{key}({GetTextWithCommas(inputValues)})");
        }
    }

    private void Awake() {
        InitializeClasses();
        CreateCommandsDictionary();
    }

    private void Update() {
        InputFieldController();
    }
}
