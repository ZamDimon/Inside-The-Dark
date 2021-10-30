using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveSkillsManager : MonoBehaviour {
    private const int INF = 1000;

    public enum PassiveSkillType {
        DivineShield = 0,
        BloodBath = 1,
        DarkRebirth = 2,
        WeakPoints = 3,
        GoodAndEvil = 4
    }

    [System.Serializable] public struct PassiveSkill {
        public PassiveSkillType passiveSkillType;
        public Sprite passiveSkillSprite;
        public bool MakeActiveTillUsage;
        public int lastUsed;
        public int coolDown;
        public bool isOpened;
        public bool canBeUsed;
    }

    private List<GameObject> passiveUIObjects = new List<GameObject>();

    [SerializeField] private PassiveSkill[] passiveSkills = new PassiveSkill[3];
    [SerializeField] private GameObject passiveSkillUIPrefab;
    [SerializeField] private Transform listObject;

    private TurnNumberAnimation turnNumberAnimation;

    public bool IsOpenedSkill(PassiveSkillType skillType) {
        for (int i = 0; i < passiveSkills.Length; ++i) {
            if (passiveSkills[i].isOpened && passiveSkills[i].passiveSkillType == skillType)
                return true;
        }

        return false;
    }

    public bool IsCanBeUsedSkill(PassiveSkillType skillType) {
        for (int i = 0; i < passiveSkills.Length; ++i) {
            if (passiveSkills[i].canBeUsed && passiveSkills[i].passiveSkillType == skillType)
                return true;
        }

        return false;
    }

    public void SetCannotBeUsed(PassiveSkillType skillType) {
        for (int i = 0; i < passiveSkills.Length; ++i) {
            if (passiveSkills[i].passiveSkillType == skillType) {
                passiveSkills[i].canBeUsed = false;
                passiveSkills[i].lastUsed = turnNumberAnimation.GetCurrentTurn();
            }
        }
    }

    public void OpenSkill(PassiveSkillType skillType) {
        for (int i = 0; i < passiveSkills.Length; ++i) {
            if (passiveSkills[i].passiveSkillType == skillType) {
                passiveSkills[i].isOpened = true;
                passiveSkills[i].lastUsed = -INF;
            }
        }
    }

    public void UpdateSkills() {
        if (passiveUIObjects != null) {
            for (int i = 0; i < passiveUIObjects.Count; ++i)
                Destroy(passiveUIObjects[i]);
        }

        passiveUIObjects.Clear();
        
        for (int i = 0; i < passiveSkills.Length; ++i) {
            if (passiveSkills[i].isOpened) {
                if (!passiveSkills[i].MakeActiveTillUsage) {
                    GameObject createdUIInfo = Instantiate(passiveSkillUIPrefab, listObject);

                    if (turnNumberAnimation.GetCurrentTurn() % passiveSkills[i].coolDown == 0)
                        passiveSkills[i].canBeUsed = true;
                    else passiveSkills[i].canBeUsed = false;

                    createdUIInfo.GetComponent<PassiveSkillUIElement>().UpdateInformation(passiveSkills[i].passiveSkillSprite,
                        passiveSkills[i].coolDown, turnNumberAnimation.GetCurrentTurn(), passiveSkills[i].canBeUsed);

                    passiveUIObjects.Add(createdUIInfo);
                }
                else {
                    GameObject createdUIInfo = Instantiate(passiveSkillUIPrefab, listObject);

                    if (turnNumberAnimation.GetCurrentTurn() - passiveSkills[i].lastUsed >= passiveSkills[i].coolDown)
                        passiveSkills[i].canBeUsed = true;
                    else passiveSkills[i].canBeUsed = false;

                    createdUIInfo.GetComponent<PassiveSkillUIElement>().UpdateInformation_MakeActiveTillUsage(passiveSkills[i].passiveSkillSprite,
                        passiveSkills[i].lastUsed, turnNumberAnimation.GetCurrentTurn(), passiveSkills[i].coolDown);

                    passiveUIObjects.Add(createdUIInfo);
                }
            }
        }
    }

    private void Start() {
        turnNumberAnimation = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<TurnNumberAnimation>();
    }
}
