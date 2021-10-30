using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bar : MonoBehaviour {
    [SerializeField] private GameObject mainBar;
    [SerializeField] private GameObject additionalBar;
    [SerializeField] private float delayBeforeChanging;
    [SerializeField] private float changingSpeed;
    [SerializeField] private Color increasingColor;
    [SerializeField] private Color decreasingColor;

    private Coroutine currentCoroutine;
    public void SetChanging(float oldValue, float newValue) {
        if (newValue == oldValue) {
            Debug.LogWarning("Value won't change");
            return;
        }
        
        if (newValue > oldValue)
            Bar_onIncrease(newValue);
        if (newValue < oldValue)
            Bar_onDecrease(newValue);
    }

    private void Bar_onIncrease(float value) {
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(OnIncreaseCoroutine(value));
    }

    private void Bar_onDecrease(float value) {
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(onDecreaseCoroutine(value));
    }

    private IEnumerator onDecreaseCoroutine(float value) {
        mainBar.GetComponent<Image>().fillAmount = value;
        additionalBar.GetComponent<Image>().color = decreasingColor;

        yield return new WaitForSeconds(delayBeforeChanging);

        while (mainBar.GetComponent<Image>().fillAmount < additionalBar.GetComponent<Image>().fillAmount) {
            additionalBar.GetComponent<Image>().fillAmount -= Time.deltaTime * changingSpeed;
            yield return null;
        }
    }

    private IEnumerator OnIncreaseCoroutine(float value) {
        additionalBar.GetComponent<Image>().fillAmount = value;
        additionalBar.GetComponent<Image>().color = increasingColor;

        yield return new WaitForSeconds(delayBeforeChanging);

        while (mainBar.GetComponent<Image>().fillAmount < additionalBar.GetComponent<Image>().fillAmount) {
            mainBar.GetComponent<Image>().fillAmount += Time.deltaTime * changingSpeed;
            yield return null;
        }
    }
}
