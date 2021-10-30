using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using MathDogs.UIAnimations;

public class TurnNumberAnimation : MonoBehaviour {
    private const int MAX_TURN_NUMBER = 4;
    private int currentTurn = 0;
    private PassiveSkillsManager passiveSkillsManager;
    private HolyShieldSkill holyShieldSkill;

    [Header("Interface objects settings")]
    [SerializeField] private GameObject[] interfaceObjectsToShow = new GameObject[MAX_TURN_NUMBER];
    [SerializeField] private GameObject[] turnIcons = new GameObject[MAX_TURN_NUMBER];
    [SerializeField] private float fadingSpeed;
    [SerializeField] private float delayBeforeHiding;

    [SerializeField] private UIElement[] elements_interfaceObjectsToShow;
    [SerializeField] private UIElement[] elements_turnIcons;
    private UIElement[] elements_interfaceAndTurnIcons;

    [Header("Sounds effects")]
    [SerializeField] private AudioClip soundEffect;

    public int GetCurrentTurn() => currentTurn;
    public int SetCurrentTurnToZero() => currentTurn = 0;

    private void InitializeUIElementsArray() {
        elements_interfaceObjectsToShow = UIConverter.GetElementsFromGameObjects(interfaceObjectsToShow);
        elements_turnIcons = UIConverter.GetElementsFromGameObjects(turnIcons);
        elements_interfaceAndTurnIcons = new UIElement[elements_interfaceObjectsToShow.Length + elements_turnIcons.Length];

        for (int i = 0; i < elements_interfaceObjectsToShow.Length; ++i) {
            elements_interfaceAndTurnIcons[i] = elements_interfaceObjectsToShow[i];
            elements_interfaceObjectsToShow[i].UIObject.GetComponent<Graphic>().color -= new Color(0, 0, 0, elements_interfaceObjectsToShow[i].UIObject.GetComponent<Graphic>().color.a);
        }

        for (int i = elements_interfaceObjectsToShow.Length, j = 0; i < elements_interfaceAndTurnIcons.Length; ++i) {
            elements_interfaceAndTurnIcons[i] = elements_turnIcons[j++];
        }

        for (int i = 0; i < elements_turnIcons.Length; ++i) {
            elements_turnIcons[i].UIObject.GetComponent<Graphic>().color -= new Color(0, 0, 0, elements_turnIcons[i].UIObject.GetComponent<Graphic>().color.a);
        }
    }

    private void Awake() {
        InitializeUIElementsArray();
        passiveSkillsManager = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<PassiveSkillsManager>();
        holyShieldSkill = GameObject.FindGameObjectWithTag("SkillManager").GetComponent<HolyShieldSkill>();
    }

    private IEnumerator DoActionAfterDelay(float delay, Action action) {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }

    public void PlayAnimation(int turnNumber, Action actionOnEnd) {
        if (turnNumber <= 0 || turnNumber > MAX_TURN_NUMBER) {
            Debug.LogError($"Turn number must be between 1 and {MAX_TURN_NUMBER.ToString()}");
            return;
        }

        passiveSkillsManager.UpdateSkills();
        ++currentTurn;

        Action[] actionsToDo = new Action[turnNumber];
        UIElement[] turnIconsToDisplay = new UIElement[turnNumber];

        for (int i = 0; i < turnNumber; ++i) {
            actionsToDo[i] = () => AudioManager.PlaySound(soundEffect);
            turnIconsToDisplay[i] = elements_turnIcons[i];
        }

        Action actionAfterTheEnd = null;
                
        actionAfterTheEnd += () => FadingAnimations.HideArrayOfObjects(elements_interfaceAndTurnIcons, fadingSpeed, () => {}, "TurnNumberAnimation");
        actionAfterTheEnd += actionOnEnd;

        actionsToDo[turnNumber - 1] += () => StartCoroutine(DoActionAfterDelay(delayBeforeHiding, actionAfterTheEnd));

        if (passiveSkillsManager.IsOpenedSkill(PassiveSkillsManager.PassiveSkillType.DivineShield) &&
            passiveSkillsManager.IsCanBeUsedSkill(PassiveSkillsManager.PassiveSkillType.DivineShield))
            holyShieldSkill.PlayAnimation(() => FadingAnimations.ShowArrayOfObjects(elements_interfaceObjectsToShow, fadingSpeed, () =>
                SequentialAnimations.PlayShowingAnimation_MultipleActions(turnIconsToDisplay, fadingSpeed, actionsToDo, "TurnNumberAnimation"), "TurnNumberAnimation"));
        else
            FadingAnimations.ShowArrayOfObjects(elements_interfaceObjectsToShow, fadingSpeed, () =>
                SequentialAnimations.PlayShowingAnimation_MultipleActions(turnIconsToDisplay, fadingSpeed, actionsToDo, "TurnNumberAnimation"), "TurnNumberAnimation");
    }
}
