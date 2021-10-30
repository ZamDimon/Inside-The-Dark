using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathDogs.UIAnimations;
using TMPro;
using UnityEngine.UI;

public class Notification : MonoBehaviour
{
    [Header("Object settings")]
    [SerializeField] private GameObject imageObject;
    [SerializeField] private GameObject textObject;
    [SerializeField] private GameObject[] interfaceElements;
 
    [Header("Animation settings")]
    [SerializeField] private float fadingSpeed;
    [SerializeField] private float delayBeforeHiding;

    private UIElement[] UIElements;
    private Coroutine currentShowingCoroutine;

    [SerializeField] private Sprite paperSprite;

    private void Start() {
        UIElements = UIConverter.GetElementsFromGameObjects(interfaceElements);
        for (int i = 0; i < interfaceElements.Length; ++i) 
            interfaceElements[i].GetComponent<Graphic>().color -= new Color(0, 0, 0, interfaceElements[i].GetComponent<Graphic>().color.a);
    }

    public void ShowNotification(Sprite icon, string text) {
        imageObject.GetComponent<Image>().sprite = icon;
        textObject.GetComponent<TextMeshProUGUI>().text = text;

        if (currentShowingCoroutine != null) StopCoroutine(currentShowingCoroutine);

        FadingAnimations.ShowArrayOfObjects(UIElements, fadingSpeed, () => 
            currentShowingCoroutine = StartCoroutine(HideNotificationCoroutine()), "Notification");
    }

    private IEnumerator HideNotificationCoroutine() {
        yield return new WaitForSeconds(delayBeforeHiding);
        FadingAnimations.HideArrayOfObjects(UIElements, fadingSpeed, null, "Notification");
    }
}
