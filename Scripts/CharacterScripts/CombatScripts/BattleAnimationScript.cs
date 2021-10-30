using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public delegate bool IsIncreasing();
public enum Direction {
    left, right, up, down
}

public enum AnimationStatement {
    Success = 0,
    Miss = 1,
    Critical = 2
}

public class BattleAnimationScript : MonoBehaviour {
    #region Events
    public delegate void EndingAnimationCharacter();
    //public event EndingAnimationCharacter onEndAnimationCharacter;

    public delegate void EndingAnimationEnemy();
    public event EndingAnimationEnemy onEndAnimationEnemy;
    #endregion

    #region Audio Settings
    [Header("Audio Settings")]
    public GameObject soundObject;
    public AudioClip passiveSound;
    public AudioClip[] battleSounds;
    public AudioClip HeroAttackSound;
    public AudioClip HeroMissSound;
    public AudioClip healSound;
    public AudioClip stunAttackSound;
    public AudioClip blockSound;
    public AudioClip lightningSound;
    public AudioClip startFightingSound;
    public AudioClip ClickSound;
    private AudioClip clipToPlay;
    private AudioSource audioSource;
    #endregion

    #region Character Sprites
    [Header("Character Sprites")]
    public GameObject playerAnimationObject;
    public Texture2D characterAttacking;
    public Texture2D characterDefending;
    public Texture2D characterMagic;
    public Texture2D characterStun;
    public Texture2D characterMagicDefence;
    public GameObject skullObject;
    #endregion

    #region Labels & Boxes settings
    [Header("Labels&Boxes settings")]
    public Color attackingColor;
    public Color missColor;
    public Color healColor;
    public GameObject numberObject;
    public GameObject darkeningObject;
    #endregion

    #region Animation Settings
    [Header("Animation Settings")]
    public float FOVspeed = 15f;
    public float criticalFOV = 50f;
    public float turningOnSpeed_screen = 0.9f;
    public float turningOnSpeed_characters = 1.5f;
    public float criticalAlpha = 0.6f;
    public float movingSpeed_attacker = 4.5f;
    public float movingSpeed_defender = 3.0f;
    public float duration = 2.0f;
    #endregion

    #region StartPositions
    private Vector3 startNumberPosition;
    private Vector3 startCharacterPosition;
    #endregion

    #region Private field
    private Coroutine killCoroutine;
    private bool canAttack;
    private CombatSystem system;
    private SkillSystem skillSystem;
    private GameManager manager;
    #endregion

    public bool CanAttack() => canAttack;

    private void InitializingComponents() {
        startNumberPosition = numberObject.GetComponent<RectTransform>().position;
        startCharacterPosition = playerAnimationObject.GetComponent<RectTransform>().position;
        system = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CombatSystem>();
        skillSystem = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<SkillSystem>();
        manager = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GameManager>();
    }

    private void Awake() {
        InitializingComponents();
    }

    private IEnumerator makeVisible(GameObject obj, float speed, float critA) {
        while (obj.GetComponent<Graphic>().color.a < critA) {
            obj.GetComponent<Graphic>().color = new Color(obj.GetComponent<Graphic>().color.r, obj.GetComponent<Graphic>().color.g, obj.GetComponent<Graphic>().color.b, obj.GetComponent<Graphic>().color.a + Time.deltaTime * speed);
            yield return null;
        }
    }

    private IEnumerator makeNotVisible(GameObject obj, float speed, bool returnBack, Vector3 startPosition, Action actionOnEnd) {
        while (obj.GetComponent<Graphic>().color.a > 0) {
            obj.GetComponent<Graphic>().color = new Color(obj.GetComponent<Graphic>().color.r, obj.GetComponent<Graphic>().color.g, obj.GetComponent<Graphic>().color.b, obj.GetComponent<Graphic>().color.a - Time.deltaTime * speed);
            yield return null;
        }

        if (!returnBack)
            yield break;

        actionOnEnd?.Invoke();
        obj.GetComponent<RectTransform>().position = startPosition;
        obj.GetComponent<Graphic>().color = new Color(1, 1, 1, 0);
    }

    private IEnumerator killObject(GameObject obj, GameObject skull, float speed) {
        while (obj.GetComponent<RawImage>().color.r > 0 || obj.GetComponent<RawImage>().color.g > 0 || obj.GetComponent<RawImage>().color.b > 0 || skull.GetComponent<Image>().color.a < 1) {
            if (obj.GetComponent<RawImage>().color.r > 0) obj.GetComponent<RawImage>().color = new Color(obj.GetComponent<RawImage>().color.r - Time.deltaTime * speed, obj.GetComponent<RawImage>().color.g, obj.GetComponent<RawImage>().color.b, obj.GetComponent<RawImage>().color.a);
            if (obj.GetComponent<RawImage>().color.g > 0) obj.GetComponent<RawImage>().color = new Color(obj.GetComponent<RawImage>().color.r, obj.GetComponent<RawImage>().color.g - Time.deltaTime * speed, obj.GetComponent<RawImage>().color.b, obj.GetComponent<RawImage>().color.a);
            if (obj.GetComponent<RawImage>().color.b > 0) obj.GetComponent<RawImage>().color = new Color(obj.GetComponent<RawImage>().color.r, obj.GetComponent<RawImage>().color.g, obj.GetComponent<RawImage>().color.b - Time.deltaTime * speed, obj.GetComponent<RawImage>().color.a);

            if (skull.GetComponent<Image>().color.a < 1) skull.GetComponent<Image>().color += new Color(0, 0, 0, speed * Time.deltaTime);
            yield return null;
        }

        skull.GetComponent<Image>().color = new Color(1, 1, 1, 0);
    }

    private IEnumerator moveObject(Direction direction, Vector3 startPosition, GameObject objectToMove, float speedMovement, float duration, Action actionOnEnd) {
        float _timer = duration;

        while (_timer >= 0) {
            _timer -= Time.deltaTime;

            switch (direction) {
                case Direction.down:
                    objectToMove.GetComponent<RectTransform>().position += new Vector3(0, -speedMovement * Time.deltaTime, 0);
                    break;
                case Direction.up:
                    objectToMove.GetComponent<RectTransform>().position += new Vector3(0, speedMovement * Time.deltaTime, 0);
                    break;
                case Direction.left:
                    objectToMove.GetComponent<RectTransform>().position += new Vector3(-speedMovement * Time.deltaTime, 0, 0);
                    break;
                case Direction.right:
                    objectToMove.GetComponent<RectTransform>().position += new Vector3(speedMovement * Time.deltaTime, 0, 0);
                    break;
            }

            yield return null;
        }

        StartCoroutine(makeNotVisible(objectToMove, turningOnSpeed_characters, true, startPosition, null));
        StartCoroutine(makeNotVisible(darkeningObject, turningOnSpeed_screen, false, new Vector3(0, 0, 0), actionOnEnd));
        StartCoroutine(turnFOV(60f, FOVspeed, false));
    }

    private IEnumerator turnFOV(float criticalFOV, float speed, bool isIncreasingFOV) {
        Camera cameraComponent = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        IsIncreasing isIncreasing = () => { return isIncreasingFOV? (cameraComponent.fieldOfView > criticalFOV) : (cameraComponent.fieldOfView < criticalFOV);};
        
        while (isIncreasing()) {
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().fieldOfView += (isIncreasingFOV? (-speed) : (speed)) * Time.deltaTime;
            isIncreasing = () => { return isIncreasingFOV? (cameraComponent.fieldOfView > criticalFOV) : (cameraComponent.fieldOfView < criticalFOV); };
            yield return null;
        }
    }

    public void UsePassiveSkill(Action actionOnEnd) {
        //Character settings
        StartCoroutine(makeVisible(darkeningObject, turningOnSpeed_screen, criticalAlpha));
        StartCoroutine(makeVisible(playerAnimationObject, turningOnSpeed_characters, 1f));
        StartCoroutine(moveObject(Direction.right, playerAnimationObject.GetComponent<RectTransform>().position, playerAnimationObject, movingSpeed_attacker, duration, null));

        //FOV settings
        StartCoroutine(turnFOV(criticalFOV, FOVspeed, true));
        playerAnimationObject.GetComponent<RawImage>().texture = characterMagic;
        StartCoroutine(characterTurnEventMaker(duration, actionOnEnd));
    }

    private void animationMethod_attackEnemy (int cellNumber, int damage, FocusAttackSkill skill) {
        //Character settings
        StartCoroutine(makeVisible(darkeningObject, turningOnSpeed_screen, criticalAlpha));
        StartCoroutine(makeVisible(playerAnimationObject, turningOnSpeed_characters, 1f));
        StartCoroutine(moveObject(Direction.right, playerAnimationObject.GetComponent<RectTransform>().position, playerAnimationObject, movingSpeed_attacker, duration, null));

        //FOV settings
        StartCoroutine(turnFOV(criticalFOV, FOVspeed, true));

        playerAnimationObject.GetComponent<RawImage>().texture = skill.characterAttackSprite;
        system.SetEnemyTexture(cellNumber, system.GetEnemy(cellNumber).GetDefendingTexture());
        StartCoroutine(makeVisible(system.GetEnemy(cellNumber).enemyAnimationObject, turningOnSpeed_characters, 1f));
        StartCoroutine(moveObject(Direction.right, system.GetEnemy(cellNumber).enemyAnimationObject.GetComponent<RectTransform>().position, system.GetEnemy(cellNumber).enemyAnimationObject, movingSpeed_defender, duration, null));
    }

    private void missAnimation_attackEnemy(int cellNumber, int damage) {
        //Number object settings
        numberObject.GetComponent<Text>().color = missColor;
        numberObject.GetComponent<Text>().text = "Missed!";
        StartCoroutine(makeVisible(numberObject, turningOnSpeed_characters, 1f));
        StartCoroutine(moveObject(Direction.up, startNumberPosition, numberObject, movingSpeed_defender / 2f, duration, null));

        //Sound settings
        AudioSource.PlayClipAtPoint(HeroMissSound, GameObject.FindGameObjectWithTag("MainCamera").transform.position);
    }

    private void successAnimation_attackEnemy (int cellNumber, int damage, Action actionOnEnd) {
        if (system.GetEnemy(cellNumber).GetHP() <= 0) {
            killCoroutine = StartCoroutine(killObject(system.GetEnemy(cellNumber).enemyAnimationObject, system.GetEnemy(cellNumber).skullObject, 0.5f));
        }

        numberObject.GetComponent<Text>().color = attackingColor;
        numberObject.GetComponent<Text>().text = "" + damage;
        StartCoroutine(makeVisible(numberObject, turningOnSpeed_characters, 1f));
        StartCoroutine(moveObject(Direction.up, startNumberPosition, numberObject, movingSpeed_defender / 2f, duration, null));
        StartCoroutine(characterTurnEventMaker(duration, actionOnEnd));
    }

    private IEnumerator enemyTurnEventMaker(float duration) {
        yield return new WaitForSeconds(duration);

        PassiveSkillsManager passiveSkillsManager = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<PassiveSkillsManager>();

        bool isDarkRebirth = (passiveSkillsManager.IsCanBeUsedSkill(PassiveSkillsManager.PassiveSkillType.DarkRebirth) &&
                passiveSkillsManager.IsOpenedSkill(PassiveSkillsManager.PassiveSkillType.DarkRebirth));
        bool wasDarkRebirthUsed = false;

        if (GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GameManager>().GetHP() <= 0) {

            if (isDarkRebirth) {
                GameObject.FindGameObjectWithTag("SkillManager").GetComponent<DarkRebirth>().PlayAnimation(() => onEndAnimationEnemy?.Invoke());
                wasDarkRebirthUsed = true;
            } else
                GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CharacterDeath>().Death();

            yield break;
        }

        if (!wasDarkRebirthUsed) {
            onEndAnimationEnemy?.Invoke();
        }
    }

    private IEnumerator characterTurnEventMaker(float duration, Action actionOnEnd) {
        yield return new WaitForSeconds(duration);
        actionOnEnd?.Invoke();
        //onEndAnimationCharacter?.Invoke();
    }

    private void ShakeCamera() {
        StartCoroutine(GetComponent<TraumaInducer>().Explosion());
    }

    public void AttackEnemy_focused (int cellNumber, int damage, FocusAttackSkill skill, AnimationStatement animationStatement, Action actionOnEnd) {
        animationMethod_attackEnemy(cellNumber, damage, skill);

        switch (animationStatement) {
            case AnimationStatement.Miss:
                missAnimation_attackEnemy(cellNumber, damage);
                break;
            case AnimationStatement.Success:
                successAnimation_attackEnemy(cellNumber, damage, actionOnEnd);
                break;
            case AnimationStatement.Critical:
                successAnimation_attackEnemy(cellNumber, damage, actionOnEnd);
                ShakeCamera();
                break;
        }
    }

    public void AttackCharacter(int cellNumber, int damage) {
        //Character settings
        StartCoroutine(makeVisible(darkeningObject, turningOnSpeed_screen, criticalAlpha));
        StartCoroutine(makeVisible(playerAnimationObject, turningOnSpeed_characters, 1f));
        StartCoroutine(moveObject(Direction.left, playerAnimationObject.GetComponent<RectTransform>().position, playerAnimationObject, movingSpeed_defender, duration, () => Debug.Log("Ready!")));

        //FOV settings
        StartCoroutine(turnFOV(criticalFOV, FOVspeed, true));

        //Enemy object settings
        playerAnimationObject.GetComponent<RawImage>().texture = characterDefending;
        system.SetEnemyTexture(cellNumber, system.GetEnemy(cellNumber).GetAttackingTexture());
        StartCoroutine(makeVisible(system.GetEnemy(cellNumber).enemyAnimationObject, turningOnSpeed_characters, 1f));
        StartCoroutine(moveObject(Direction.left, system.GetEnemy(cellNumber).enemyAnimationObject.GetComponent<RectTransform>().position, system.GetEnemy(cellNumber).enemyAnimationObject, movingSpeed_attacker, duration, () => Debug.Log("Ready!")));

        //Number object settings
        numberObject.GetComponent<Text>().text = "" + damage;
        numberObject.GetComponent<Text>().color = attackingColor;
        StartCoroutine(makeVisible(numberObject, turningOnSpeed_characters, 1f));
        StartCoroutine(moveObject(Direction.up, startNumberPosition, numberObject, movingSpeed_defender/2f, duration, null));

        //Sound settings
        AudioSource.PlayClipAtPoint(system.GetEnemy(cellNumber).enemyAttackSound, GameObject.FindGameObjectWithTag("MainCamera").transform.position);

        //Event settings
        StartCoroutine(enemyTurnEventMaker(duration));
    }
}
