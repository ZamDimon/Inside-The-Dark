using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OnHoverScript : MonoBehaviour {
    [System.Serializable] private struct UIElement {
        public GameObject UIObject;
        private float startAlpha;

        public void SetStartAlpha() => startAlpha = UIObject.GetComponent<Graphic>().color.a;
        public float GetStartAlpha() => startAlpha; 
    }

    #region Links to the other scripts
    private SkillSystem skillSystem;
    private EnemySettings enemySettings;
    private CharacterTurnHandler turnHandler;
    private Battle battle;
    #endregion

    #region Fade settings description
    private bool IsHover;
    [SerializeField] private UIElement[] descriptionPanel;
    public float fadeSpeed;
    public GameObject selectionObject;
    public float selectionFadeSpeed = 5f;
    #endregion

    #region Material Settings
    private Material startMaterial;
    public Material materialToChange;
    public Color colorToChange;
    #endregion

    #region Coroutines settings
    private List<Coroutine> coroutinesToClose = new List<Coroutine>();
    #endregion

    private void InitializateComponents() {
        battle = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Battle>();
        enemySettings = transform.GetComponent<EnemySettings>();
        skillSystem = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<SkillSystem>();
        turnHandler = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CharacterTurnHandler>();
        startMaterial = transform.GetComponent<SpriteRenderer>().material;

        for (int i = 0; i < descriptionPanel.Length; ++i) {
            descriptionPanel[i].SetStartAlpha();
            descriptionPanel[i].UIObject.GetComponent<Graphic>().color -= new Color(0, 0, 0, descriptionPanel[i].UIObject.GetComponent<Graphic>().color.a);
        }
    }
    private void Awake() => InitializateComponents();

    private Color getColorComponent(GameObject obj) => obj.GetComponent<Graphic>().color;

    private void Update() {
        if ((IsHover && CanAttack()) || ((battle.GetCurrentTurn() - 1) == enemySettings.GetNumber()))
            selectionObject.GetComponent<Image>().fillAmount += (selectionObject.GetComponent<Image>().fillAmount >= 1f) ? 0 : Time.deltaTime * selectionFadeSpeed;
        else
            selectionObject.GetComponent<Image>().fillAmount -= (selectionObject.GetComponent<Image>().fillAmount <= 0) ? 0 : Time.deltaTime * selectionFadeSpeed;
    }

    private IEnumerator CloseElementCoroutine(UIElement elementToClose, float closingSpeed) {
        while (elementToClose.UIObject.GetComponent<Graphic>().color.a > 0) {
            elementToClose.UIObject.GetComponent<Graphic>().color -= new Color (0, 0, 0, 
            Mathf.Min(closingSpeed * Time.deltaTime, elementToClose.UIObject.GetComponent<Graphic>().color.a));

            yield return null;
        }
    }

    private IEnumerator OpenElementCoroutine(UIElement elementToOpen, float openingSpeed) {
        while (elementToOpen.UIObject.GetComponent<Graphic>().color.a < elementToOpen.GetStartAlpha()) {
            elementToOpen.UIObject.GetComponent<Graphic>().color += new Color(0, 0, 0, 
            Mathf.Min(Time.deltaTime * openingSpeed, elementToOpen.GetStartAlpha() - elementToOpen.UIObject.GetComponent<Graphic>().color.a));

            yield return null;
        }
    }

    private void OpenDescriptionPanel() {
        foreach (UIElement element in descriptionPanel) {
            coroutinesToClose.Add(StartCoroutine(OpenElementCoroutine(element, fadeSpeed)));
        }
    }

    private void CloseDescriptionPanel() {
        foreach (UIElement element in descriptionPanel) {
            coroutinesToClose.Add(StartCoroutine(CloseElementCoroutine(element, fadeSpeed)));
        }
    }

    private void StopCoroutines() {
        for (int i = 0; i < coroutinesToClose.Count; ++i) {
            if (coroutinesToClose[i] != null)
                StopCoroutine(coroutinesToClose[i]);
        }

        coroutinesToClose.Clear();
    }

    private bool CanAttack () {
        if (!turnHandler.GetAbilityToAttack())
            return false;
        
        bool _result = false;

        switch (skillSystem.GetCurrentSkillCell().skill.getSkillType()) {
            case SkillType.Passive:

                break;
            case SkillType.Single:
                FocusAttackSkill skillToCheck = (FocusAttackSkill)skillSystem.GetCurrentSkillCell().skill;
                _result = (enemySettings.GetNumber() <= skillToCheck.maxRange &&
                        enemySettings.GetNumber() >= skillToCheck.minRange &&
                        battle.GetCurrentTurn() == 0);
                break;
            case SkillType.Splash:
                _result = (battle.GetCurrentTurn() == 0);
                break;
        }

        GameManager gameManager = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GameManager>();
        PassiveSkillsManager passiveSkillsManager = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<PassiveSkillsManager>();

        if (passiveSkillsManager.IsOpenedSkill(PassiveSkillsManager.PassiveSkillType.GoodAndEvil)) {
            if (skillSystem.GetCurrentSkillCell().skill.GetManaCost() > gameManager.GetMP() &&
                skillSystem.GetCurrentSkillCell().skill.GetManaCost() >= gameManager.GetHP())
                _result = false;
        }    
        else {
            if (skillSystem.GetCurrentSkillCell().skill.GetManaCost() > gameManager.GetMP())
                _result = false;
        }

        return _result;
    }

    private void OnMouseOver() {
        if (Input.GetMouseButtonDown(0) && CanAttack()) {
            PassiveSkillsManager passiveSkillsManager = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<PassiveSkillsManager>();
            GameManager gameManager = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GameManager>();

            if (passiveSkillsManager.IsOpenedSkill(PassiveSkillsManager.PassiveSkillType.GoodAndEvil)) {
                if (skillSystem.GetCurrentSkillCell().skill.GetManaCost() > gameManager.GetMP()) {
                    if (skillSystem.GetCurrentSkillCell().skill.GetManaCost() < gameManager.GetHP())
                        gameManager.SetGoodAndEvil(true);
                }
            }

            skillSystem.GetCurrentSkillCell().skill.Use(enemySettings.GetNumber());
        }
    }

    private void OnMouseEnter() {
        //Visual settings
        IsHover = true;
        StopCoroutines();

        OpenDescriptionPanel();

        transform.GetComponent<SpriteRenderer>().material = materialToChange;
        transform.GetComponent<SpriteRenderer>().color = colorToChange;
    }

    private void OnMouseExit() {
        IsHover = false;
        StopCoroutines();
        CloseDescriptionPanel();
        
        transform.GetComponent<SpriteRenderer>().material = startMaterial;
        transform.GetComponent<SpriteRenderer>().color = Color.white;
    }
}
