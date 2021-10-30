using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BloodAttack : SplashSkill {
    [System.Serializable]
    private struct Frame {
        public Sprite frameSprite;
        public GameObject lightObject;
        public float delayAfterFrame;
    } 

    [Header("Damage number settings")]
    [SerializeField] private int missChance;
    [SerializeField] private int bloodEffectDurancy = 3;
    [SerializeField] private int bloodEffectPower = 1;
    
    [Header("Animation settings")]
    [SerializeField] private GameObject animationObject;
    [SerializeField] private List<Frame> frames = new List<Frame>(5);
    [SerializeField] private float maxAlpha;
    [SerializeField] private float showingSpeed = 1f;
    [SerializeField] private float disapearingSpeed = .1f;
    [SerializeField] private int frameWithSound = 0;

    private void ChangeFrame (int index) {
        if (frames[index - 1].lightObject != null)
            frames[index - 1].lightObject.SetActive(false);
        if (frames[index].lightObject != null)
            frames[index].lightObject.SetActive(true);
        if (frameWithSound == index) {
            audioSource.PlayOneShot(useSound);
            MakeShaking();
            DealDamage();
        }
        animationObject.GetComponent<SpriteRenderer>().sprite = frames[index].frameSprite;
    }

    private void setStartFrame() {
        animationObject.GetComponent<SpriteRenderer>().sprite = frames[0].frameSprite;
        frames[0].lightObject?.SetActive(true);
        ShowObject(animationObject, showingSpeed, maxAlpha);
    }
    private IEnumerator animationPlayer () {
        int currentID = 0;
        float timer = 0f;
        setStartFrame();

        while (timer < frames[currentID].delayAfterFrame && currentID + 1 < frames.Count) {
            timer += Time.deltaTime;

            if (timer >= frames[currentID].delayAfterFrame) {
                timer = 0f;
                currentID++;
                ChangeFrame(currentID);
            }

            yield return null;
        }

        yield return new WaitForSeconds(frames[currentID].delayAfterFrame);
        HideObject(animationObject, disapearingSpeed);
        RaiseSkillPlayedEvent();

        if (frames[currentID].lightObject != null)
            frames[currentID].lightObject?.SetActive(false);
    }

    private void PlayAnimation() => StartCoroutine(animationPlayer());

    public override void Use(int position) {
        RaiseOnSkillStartedPlayingEvent();
        UseMana();
        //base.Use(position);

        //NumberSettings(position);
        PlayAnimation();
    }

    public override int GetScaledDamage() => (int)((float)0.8f * gameManager.character_attack);

    public override void UseWithoutTurnSpending(int position, Action actionOnEnd) => throw new System.NotImplementedException();
}
