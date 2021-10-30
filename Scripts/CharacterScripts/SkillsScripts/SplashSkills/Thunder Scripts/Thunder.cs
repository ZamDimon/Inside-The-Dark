using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Thunder : SplashSkill {
    private struct FrameAction {
        public int frame;
        public Action action;

        public FrameAction(int frame, Action action) {
            this.frame = frame;
            this.action = action;
        }
    }

    [Header("Transition settings")]
    [SerializeField] private Vector3 endCoordinate;
    [SerializeField] private float lerpTime;
    [SerializeField] private float beginningYOffset = 0f;
    [SerializeField] private float endingYOffset = 0.2f;
    private Vector3 startCoordinate;
    private Vector3 goalCoordinate;

    [Header("Animation settings")]
    [SerializeField] private int startHitFrame;
    [SerializeField] private int endHitFrame;
    [SerializeField] private int throwingFrame;
    [SerializeField] private float delayBetweenFrames = 0.2f;
    [SerializeField] private GameObject cloudObject;
    [SerializeField] private GameObject lightningObject;
    [SerializeField] private List<Sprite> cloudSprites = new List<Sprite>();
    [SerializeField] private List<Sprite> characterEnteringAnimation = new List<Sprite>();
    [SerializeField] private List<Sprite> characterEndingAnimation = new List<Sprite>();

    [Header("Sounds")]
    [SerializeField] private AudioClip dashSound;
    [SerializeField] private AudioClip throwingSound;
    [SerializeField] private AudioClip hitSound;

    private List<FrameAction> lightningActions = new List<FrameAction>();

    private void DealDamage_Thunder() {
        int[] damageArray = new int[EnemyBase.START_ENEMY_NUMBER];

        for (int i = 0; i < combatSystem.GetEnemyAmount(); ++i)
            damageArray[i] = ((combatSystem.GetEnemy(i).GetEffectDurancy(EffectType.Stun) > 0)? 
            UnityEngine.Random.Range(GetBoostedDamage() - 1, GetBoostedDamage() + 2) : 
            UnityEngine.Random.Range(GetScaledDamage() - 1, GetScaledDamage() + 2));
        
        combatSystem.TakeMultipleDamage(damageArray);
    }

    private void Start() {
        startCoordinate = GameObject.FindGameObjectWithTag("Character").transform.position;
        lightningActions.Add(new FrameAction(startHitFrame, () => lightningObject.SetActive(true)));
        lightningActions.Add(new FrameAction(startHitFrame, () => AudioManager.PlaySound(hitSound)));
        lightningActions.Add(new FrameAction(startHitFrame, DealDamage_Thunder));
        lightningActions.Add(new FrameAction(startHitFrame, MakeShaking));
        lightningActions.Add(new FrameAction(endHitFrame, () => lightningObject.SetActive(false)));
    }

    private IEnumerator MakeSmoothTransition_IE(Vector3 endCoordinate) {
        float parameter = 0f;
        Vector3 startCoordinate = GameObject.FindGameObjectWithTag("Character").transform.position;

        while (parameter < 1f) {
            parameter += Time.deltaTime / lerpTime;
            GameObject.FindGameObjectWithTag("Character").transform.position = Vector3.Lerp(startCoordinate, endCoordinate, parameter);
            yield return null;
        }
    }

    private IEnumerator PlayAnimation_IE(GameObject animatedObject, List<Sprite> animationFrames, Action action, float offsetY, List<FrameAction> frameActions) {
        int currentFrame = 0;
        animatedObject.transform.position += new Vector3(0, offsetY, 0);

        while (currentFrame < animationFrames.Count) {
            if (frameActions != null) {
                for (int i = 0; i < frameActions.Count; ++i) {
                    if (frameActions[i].frame == currentFrame) {
                        frameActions[i].action?.Invoke();
                    }
                }
            }

            animatedObject.GetComponent<SpriteRenderer>().sprite = animationFrames[currentFrame];
            yield return new WaitForSeconds(delayBetweenFrames);
            ++currentFrame;
        }

        animatedObject.transform.position += new Vector3(0, -offsetY, 0);
        action?.Invoke();
    }
    
    private void PlayAnimation() {
        GameObject.FindGameObjectWithTag("Character").GetComponent<CharacterMovement>().SetActiveAnimation(false);
        cloudObject.SetActive(true);

        StartCoroutine(MakeSmoothTransition_IE(endCoordinate));
        AudioManager.PlaySound(dashSound);

        List<FrameAction> frameActionsEntering = new List<FrameAction>();
        frameActionsEntering.Add(new FrameAction(throwingFrame, () => PlaySound(throwingSound)));

        StartCoroutine(PlayAnimation_IE(GameObject.FindGameObjectWithTag("Character"), characterEnteringAnimation, () => {
            AudioManager.PlaySound(dashSound);
            StartCoroutine(PlayAnimation_IE(GameObject.FindGameObjectWithTag("Character"), characterEndingAnimation, () => GameObject.FindGameObjectWithTag("Character").GetComponent<CharacterMovement>().SetActiveAnimation(true), endingYOffset, null));
            StartCoroutine(MakeSmoothTransition_IE(startCoordinate));
        }, beginningYOffset, frameActionsEntering));

        StartCoroutine(PlayAnimation_IE(cloudObject, cloudSprites, () => cloudObject.SetActive(false), 0f, lightningActions));
    }

    public override int GetScaledDamage() => (int)(0.50f * (float)gameManager.character_magicpower); 
    public int GetBoostedDamage() => (int)(0.80f * (float)gameManager.character_magicpower);



    public override void Use(int position) {
        UseMana();
        PlayAnimation();
    }
        
    public override void UseWithoutTurnSpending(int position, Action actionOnEnd) => throw new System.NotImplementedException();
}
