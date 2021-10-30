using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class MoneyUI : MonoBehaviour {
    #region Main settings
    [SerializeField] private GameObject moneyText;
    private MoneyManager moneyManager;
    #endregion

    #region Addition money settings
    [Header("Object settings")]
    [SerializeField] private GameObject additionMoneyImage;
    [SerializeField] private GameObject additionMoneyText;
    #endregion

    #region Animation settings
    private float startAlpha;
    [Header("Animation speed settings")]
    [SerializeField] private float fadingSpeed;
    [SerializeField] private float delayBetweenAdding;
    [SerializeField] private float delayBeforeAddingMoney;
    
    private Coroutine currentPlayingCoroutine;
    #endregion

    private void Awake() {
        moneyManager = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<MoneyManager>();
        startAlpha = additionMoneyText.GetComponent<Graphic>().color.a;

        moneyManager.OnMoneyChanged += UpdateMoneyText;
    }

    private int GetCurrentMoneyInText(string text) {
        string result = ""; int currentID = 0;

        if (text == null) {
            Debug.LogError("Text variable is null");
            return -1;
        }

        while (!Char.IsDigit(text[currentID]) && currentID < text.Length) {
            ++currentID;
        }

        if (currentID == text.Length) {
            Debug.LogError("A number in text is missing");
            return -1;
        }

        for (int i = currentID; i < text.Length; ++i) {
            result += text[i];
        }

        return int.Parse(result);
    }
     
    private IEnumerator ShowElementCoroutine(int value) {
        int difference = moneyManager.GetMoney() - GetCurrentMoneyInText(moneyText.GetComponent<TextMeshProUGUI>().text);

        string signString = (difference < 0)? "-" : "+";
        additionMoneyText.GetComponent<TextMeshProUGUI>().text = signString + Mathf.Abs(difference).ToString();

        while (additionMoneyText.GetComponent<Graphic>().color.a < startAlpha) {
            Debug.Log("Everything is working");
            additionMoneyText.GetComponent<TextMeshProUGUI>().color += new Color(0, 0, 0, Mathf.Min(Time.deltaTime * fadingSpeed, startAlpha - additionMoneyText.GetComponent<TextMeshProUGUI>().color.a));
            additionMoneyImage.GetComponent<Graphic>().color += new Color(0, 0, 0, Mathf.Min(Time.deltaTime * fadingSpeed, startAlpha - additionMoneyImage.GetComponent<Graphic>().color.a));

            yield return null;
        }

        yield return new WaitForSeconds(delayBeforeAddingMoney);
        yield return currentPlayingCoroutine = StartCoroutine(PlayAddingMoneyAnimation());
    }

    private IEnumerator PlayAddingMoneyAnimation() {
        while (GetCurrentMoneyInText(moneyText.GetComponent<TextMeshProUGUI>().text) != moneyManager.GetMoney()) {
            if (GetCurrentMoneyInText(moneyText.GetComponent<TextMeshProUGUI>().text) > moneyManager.GetMoney()) 
                moneyText.GetComponent<TextMeshProUGUI>().text = $"Money: {GetCurrentMoneyInText(moneyText.GetComponent<TextMeshProUGUI>().text) - 1}";
            else 
                moneyText.GetComponent<TextMeshProUGUI>().text = $"Money: {GetCurrentMoneyInText(moneyText.GetComponent<TextMeshProUGUI>().text) + 1}";
        
            yield return new WaitForSeconds(delayBetweenAdding);
        }

        yield return currentPlayingCoroutine = StartCoroutine(HideElementCoroutine());
    }

    private IEnumerator HideElementCoroutine() {
        while (additionMoneyText.GetComponent<Graphic>().color.a > 0) {
            additionMoneyText.GetComponent<TextMeshProUGUI>().color -= new Color(0, 0, 0, Mathf.Min(Time.deltaTime * fadingSpeed, additionMoneyText.GetComponent<TextMeshProUGUI>().color.a));
            additionMoneyImage.GetComponent<Graphic>().color -= new Color(0, 0, 0, Mathf.Min(Time.deltaTime * fadingSpeed, additionMoneyImage.GetComponent<Graphic>().color.a));

            yield return null;
        }     
    }

    private void UpdateMoneyText(int value) {
        if (currentPlayingCoroutine != null) 
            StopCoroutine(currentPlayingCoroutine);

        currentPlayingCoroutine = StartCoroutine(ShowElementCoroutine(value));
    }
}
