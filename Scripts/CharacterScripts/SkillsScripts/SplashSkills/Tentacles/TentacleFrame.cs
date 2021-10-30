using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TentacleFrame : MonoBehaviour {
    [Header("Object settings")]
    [SerializeField] public List<GameObject> tentacleBodyComponents = new List<GameObject>();
    [SerializeField] public GameObject headspearingTentacle;

    [Header("AnimationSettings")]
    [SerializeField] private int frameIndex;
    [SerializeField] private float delayBetweenFrames;
    [SerializeField] private int headSpearingTentacleOffset = 1;
    [SerializeField] private int framesAmount = 3;
    [SerializeField] private List<Sprite> tentacleBodyAnimation = new List<Sprite>();
    [SerializeField] private List<Sprite> headspearingTentacleAnimation = new List<Sprite>();

    public void SetActiveFrame(bool state) {
        for (int i = 0; i < tentacleBodyComponents.Count; ++i)
            tentacleBodyComponents[i].SetActive(state);

        headspearingTentacle.SetActive(state);
    }
    private IEnumerator PlayAnimation_IE(Action actionOnEnd, float actionDelay) {
        int currentFrame = 0;
        while (currentFrame < framesAmount) {
            if (currentFrame < headspearingTentacleAnimation.Count)
                headspearingTentacle.GetComponent<SpriteRenderer>().sprite = headspearingTentacleAnimation[currentFrame + headSpearingTentacleOffset];
            
            for (int j = 0; j < tentacleBodyComponents.Count; ++j) {
                if (currentFrame < tentacleBodyAnimation.Count)
                    tentacleBodyComponents[j].GetComponent<SpriteRenderer>().sprite = tentacleBodyAnimation[currentFrame];
            }

            yield return new WaitForSeconds(delayBetweenFrames);
            ++currentFrame;
        }

        yield return new WaitForSeconds(actionDelay);
        actionOnEnd?.Invoke();
    }
    public void PlayAnimation(Action actionOnEnd, float actionDelay) => StartCoroutine(PlayAnimation_IE(actionOnEnd, actionDelay));

    public void SetExpanding(bool state) {
        int numberForHeadspearingTentacle = (frameIndex == 0)? 0 : 1;
        headspearingTentacle.GetComponent<SpriteRenderer>().sprite = headspearingTentacleAnimation[(state? (headspearingTentacleAnimation.Count - 1) : (numberForHeadspearingTentacle))];
        for (int j = 0; j < tentacleBodyComponents.Count; ++j) 
            tentacleBodyComponents[j].GetComponent<SpriteRenderer>().sprite = tentacleBodyAnimation[(state? (tentacleBodyAnimation.Count - 1) : (0))];
    }
}
