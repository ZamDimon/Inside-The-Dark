using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class HolyShieldSkill : MonoBehaviour {
    [System.Serializable] private struct Frame {
        public Sprite frameSprite;
        public GameObject lightObject;
        public float delay;
        public float alpha;
    }

    [Header("On Player Animation")]
    [SerializeField] private List<Frame> shieldFrames = new List<Frame>();
    [SerializeField] private List<Frame> wingFrames = new List<Frame>();
    [SerializeField] private GameObject wingAnimationObject;
    [SerializeField] private GameObject shieldAnimationObject;

    [Header("On Interface Animation")]
    [SerializeField] private GameObject breakingShieldObject;
    [SerializeField] private List<Sprite> breakingShieldFrames;
    [SerializeField] private float delayBetweenFrames;

    private Color GetColorByFrame(GameObject gObject, Frame frame) =>
        new Color(gObject.GetComponent<SpriteRenderer>().color.r, gObject.GetComponent<SpriteRenderer>().color.g, gObject.GetComponent<SpriteRenderer>().color.b, frame.alpha);

    private void HideObjects() {
        wingAnimationObject.GetComponent<SpriteRenderer>().color = new Color(wingAnimationObject.GetComponent<SpriteRenderer>().color.r, wingAnimationObject.GetComponent<SpriteRenderer>().color.g, wingAnimationObject.GetComponent<SpriteRenderer>().color.b, 0);
        shieldAnimationObject.GetComponent<SpriteRenderer>().color = new Color(shieldAnimationObject.GetComponent<SpriteRenderer>().color.r, shieldAnimationObject.GetComponent<SpriteRenderer>().color.g, shieldAnimationObject.GetComponent<SpriteRenderer>().color.b, 0);
    }

    private void DisactivateLight() {
        for (int i = 0; i < shieldFrames.Count; ++i)
            shieldFrames[i].lightObject.SetActive(false);
    }

    private void ActivateLight(int index) {
        DisactivateLight();
        shieldFrames[index].lightObject.SetActive(true);
    }

    private IEnumerator IEPlayAnimation(Action actionOnEnd) {
        if (shieldFrames.Count != wingFrames.Count) {
            Debug.LogError("Shield frames array must have the same size as the wing frames one");
            yield break;
        }

        int currentIndex = 0;
        while (currentIndex < shieldFrames.Count) {
            shieldAnimationObject.GetComponent<SpriteRenderer>().color = GetColorByFrame(shieldAnimationObject, shieldFrames[currentIndex]);
            wingAnimationObject.GetComponent<SpriteRenderer>().color = GetColorByFrame(wingAnimationObject, wingFrames[currentIndex]);
            shieldAnimationObject.GetComponent<SpriteRenderer>().sprite = shieldFrames[currentIndex].frameSprite;
            wingAnimationObject.GetComponent<SpriteRenderer>().sprite = wingFrames[currentIndex].frameSprite;
            ActivateLight(currentIndex);
            yield return new WaitForSeconds(shieldFrames[currentIndex].delay);
            ++currentIndex;
        }

        HideObjects();
        DisactivateLight();
        actionOnEnd?.Invoke();
    }

    private IEnumerator IE_BreakingShield() {
        breakingShieldObject.SetActive(true);

        int currentFrame = 0;
        while (currentFrame < breakingShieldFrames.Count) {
            breakingShieldObject.GetComponent<Image>().sprite = breakingShieldFrames[currentFrame++];
            yield return new WaitForSeconds(delayBetweenFrames);
        }

        yield return new WaitForSeconds(delayBetweenFrames);
        breakingShieldObject.SetActive(false);
    }

    public void PlayBreakingShieldAnimation() => StartCoroutine(IE_BreakingShield());

    public void PlayAnimation(Action actionOnEnd) => StartCoroutine(IEPlayAnimation(actionOnEnd));

    private void Start() {
        HideObjects();
        DisactivateLight();
    }
}
