using CutsceneManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class IceWallSkill : SplashSkill {
    [System.Serializable] private struct Frame {
        public Sprite frameSprite;
        public GameObject lightObject;
        public bool makeShaking;
        public float delay;
        public AudioClip sound;
    }

    [Header("Animation settings")]
    [SerializeField] private GameObject animationObject;
    [SerializeField] private Frame[] frames = new Frame[15];
    [SerializeField] private float criticalAlpha = 0.40f;
    [SerializeField] private float fadingSpeedShowing = 1f;
    [SerializeField] private float fadingSpeedHiding = 1f;

    private void TurnLights(bool value) {
        foreach (Frame frame in frames) {
            frame.lightObject.SetActive(value);
        }
    }
    private void PlayFrame(Frame frame) {
        TurnLights(false);
        animationObject.GetComponent<SpriteRenderer>().sprite = frame.frameSprite;
        frame.lightObject.SetActive(true);

        if (frame.sound != null) AudioManager.PlaySound(frame.sound);
        if (frame.makeShaking) MakeShaking();
    }
    private IEnumerator playUseAnimation(Action actionOnEnd) {
        ShowObject(animationObject, fadingSpeedShowing, criticalAlpha);

        foreach (Frame frame in frames) {
            PlayFrame(frame);
            yield return new WaitForSeconds(frame.delay);
        }

        HideObject(animationObject, fadingSpeedHiding);
        TurnLights(false);
        actionOnEnd?.Invoke();
        DealDamage();
    }

    public override void Use(int position) {
        RaiseOnSkillStartedPlayingEvent();
        UseMana();
        //base.Use(position);
        GameObject.FindGameObjectWithTag("Character").GetComponent<PlayerAnimations>().SetCastingState(PlayerAnimations.PlayerState.IceCasting, true);
        StartCoroutine(playUseAnimation(() => {
            RaiseSkillPlayedEvent();
            GameObject.FindGameObjectWithTag("Character").GetComponent<PlayerAnimations>().SetCastingState(PlayerAnimations.PlayerState.IceCasting, false);
        }));
    }

    public override int GetScaledDamage() => (int)((float)gameManager.character_magicpower * 0.6f);

    public override void UseWithoutTurnSpending(int position, Action actionOnEnd) {
        RaiseOnSkillStartedPlayingEvent();
        StartCoroutine(playUseAnimation(() => MakeActionDelay(actionOnEnd)));
    }
}
