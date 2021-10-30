using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[System.Serializable]
public abstract class Skill : MonoBehaviour {
    #region Event settings
    public event Action OnSkillPlayed;
    public event Action OnSkillStartedPlaying; 

    private IEnumerator RaiseEventCoroutine(float delay) {
        yield return new WaitForSeconds(delay);
        OnSkillPlayed?.Invoke();
    }
    protected void RaiseSkillPlayedEvent() => StartCoroutine(RaiseEventCoroutine(delayBetweenSwitch));
    protected void RaiseOnSkillStartedPlayingEvent() => OnSkillStartedPlaying?.Invoke();
    #endregion

    #region Protected Fields
    protected GameObject mainObject;
    protected CombatSystem combatSystem;
    protected AudioSource audioSource;
    protected BattleAnimationScript battleAnimationScript;
    protected GameManager gameManager;
    #endregion

    [Header("Sprite & Sound")]
    public AudioClip useSound;
    public Sprite skillSprite;

    [Header("Text settings")]
    public string skillName;
    public string skillDescription;
    public string skillAttack;
       
    [Header("Delay settings")]
    [SerializeField] private float delayBetweenSwitch;
    [SerializeField] private SkillType skillType;

    [Header("Stats settings")]
    [SerializeField] protected int manaCost;

    public int GetManaCost() => manaCost;
    public SkillType getSkillType() => skillType;

    protected void UseMana() => gameManager.TryUseMana(manaCost);

    protected virtual void Update() {
        mainObject = GameObject.FindGameObjectWithTag("MainCamera");
        combatSystem = mainObject.GetComponent<CombatSystem>();
        audioSource = mainObject.GetComponent<AudioSource>();
        battleAnimationScript = mainObject.GetComponent<BattleAnimationScript>();
        gameManager = mainObject.GetComponent<GameManager>();
    }

    private IEnumerator IEMakeActionDelay(Action actionOnEnd) {
        yield return new WaitForSeconds(delayBetweenSwitch);
        actionOnEnd?.Invoke();
    }

    protected void MakeActionDelay(Action actionOnEnd) => StartCoroutine(IEMakeActionDelay(actionOnEnd));
    protected void MakeShaking() => GameObject.FindGameObjectWithTag("Inducer").GetComponent<TraumaInducer>().MakeExplosion();
    protected void PlaySound(AudioClip clipToPlay) => AudioManager.PlaySound(clipToPlay);
    public abstract void Use(int position);
    public abstract int GetScaledDamage();
}