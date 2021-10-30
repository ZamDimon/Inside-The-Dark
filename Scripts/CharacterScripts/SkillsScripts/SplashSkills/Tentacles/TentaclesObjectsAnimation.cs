using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TentaclesObjectsAnimation : MonoBehaviour {
    [SerializeField] private List<GameObject> frameObjects = new List<GameObject>();
    [SerializeField] private float delayBetweenFrames;
    private List<TentacleFrame> tentacleFrames = new List<TentacleFrame>();

    private void InitializeTentacleFrames() {
        for (int i = 0; i < frameObjects.Count; ++i)
            tentacleFrames.Add(frameObjects[i].GetComponent<TentacleFrame>());
    }

    private void DisactivateAllFrames() {
        for (int i = 0; i < tentacleFrames.Count; ++i)
            frameObjects[i].SetActive(false);
    }

    private void Start() => InitializeTentacleFrames();
    
    private IEnumerator PlayAnimation_IE(int frameNumber, Action actionOnEnd, Action actionOnDamage) {
        if (frameNumber > tentacleFrames.Count) yield break;

        int currentFrame = 0;
        while (currentFrame < frameNumber) {
            DisactivateAllFrames();
            frameObjects[currentFrame].SetActive(true);

            yield return new WaitForSeconds(delayBetweenFrames);
            ++currentFrame;
        }

        tentacleFrames[frameNumber - 1].PlayAnimation(() =>{ 
            StartCoroutine(PlayBackAnimationIE(frameNumber, actionOnEnd));
            actionOnDamage?.Invoke();
        }, 0.2f); 
    }

    private IEnumerator PlayBackAnimationIE(int frameNumber, Action actionOnEnd) {
        int currentFrame = frameNumber - 1;
        Debug.Log("Start playing end animation");
        while (currentFrame >= 0) {
            DisactivateAllFrames();
            frameObjects[currentFrame].SetActive(true);
            tentacleFrames[currentFrame].SetExpanding(true);

            Debug.Log("Current frame: " + currentFrame);
            yield return new WaitForSeconds(delayBetweenFrames);
            --currentFrame;
        }

        DisactivateAllFrames();

        for (int i = 0; i < frameNumber; ++i)
            tentacleFrames[i].SetExpanding(false);

        actionOnEnd?.Invoke();
    }

    public void PlayAnimation(int frameNumber, Action actionOnEnd, Action actionOnDamage) => StartCoroutine(PlayAnimation_IE(frameNumber, actionOnEnd, actionOnDamage));
}
