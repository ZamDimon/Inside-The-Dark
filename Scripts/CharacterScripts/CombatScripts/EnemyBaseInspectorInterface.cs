using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomEditor(typeof(EnemyBase))]
public class EnemyBaseInspectorInterface : Editor
{

    private void MakeHugeSpace(int size) {
        for (int i = 0; i < size; ++i)
            EditorGUILayout.Space();
    }
    override public void OnInspectorGUI() {
        GUIStyle styleForTitles = new GUIStyle();

        styleForTitles.fontSize = 15;
        styleForTitles.alignment = TextAnchor.UpperCenter;
        styleForTitles.fontStyle = FontStyle.Bold;

        EnemyBase enemyBase = (EnemyBase)target;
        
        EditorGUILayout.BeginVertical("Window");
        EditorGUILayout.LabelField("Enemy base settings", styleForTitles);

        for (int i = 0; i < enemyBase.GetEnemyArrayLength(); ++i) {
            EditorGUILayout.BeginVertical("Window");
            EditorGUILayout.LabelField($"Enemy {(i+1).ToString()}", styleForTitles);
            enemyBase.enemies[i].enemyType = (EnemyType)EditorGUILayout.EnumPopup("Enemy type:", enemyBase.enemies[i].enemyType);
            enemyBase.enemies[i].enemyObject = (GameObject)EditorGUILayout.ObjectField("Enemy prefab:", enemyBase.enemies[i].enemyObject, typeof(GameObject), false);
            enemyBase.enemies[i].beforeAttackSound = (AudioClip)EditorGUILayout.ObjectField("Before attack sound:", enemyBase.enemies[i].beforeAttackSound, typeof(AudioClip), true);
            enemyBase.enemies[i].enemyAttackSound = (AudioClip)EditorGUILayout.ObjectField("Enemy attack sound:", enemyBase.enemies[i].enemyAttackSound, typeof(AudioClip), true);
            enemyBase.enemies[i].maxHealth = EditorGUILayout.IntField("Max HP:", enemyBase.enemies[i].maxHealth);
            enemyBase.enemies[i].enemyAnimationObject = (GameObject)EditorGUILayout.ObjectField("Enemy animation object:", enemyBase.enemies[i].enemyAnimationObject, typeof(GameObject), true);
            enemyBase.enemies[i].minDamage = EditorGUILayout.IntField("Min damage:", enemyBase.enemies[i].minDamage);
            enemyBase.enemies[i].maxDamage = EditorGUILayout.IntField("Max Damage:", enemyBase.enemies[i].maxDamage);
            enemyBase.enemies[i].attackingTexture = (Texture2D)EditorGUILayout.ObjectField("Attacking texture:", enemyBase.enemies[i].attackingTexture, typeof(Texture2D), true);
            enemyBase.enemies[i].defendingTexture = (Texture2D)EditorGUILayout.ObjectField("Defending texture:", enemyBase.enemies[i].defendingTexture, typeof(Texture2D), true);
            enemyBase.enemies[i].skullObject = (GameObject)EditorGUILayout.ObjectField("Skull object:", enemyBase.enemies[i].skullObject, typeof(GameObject), true);
            enemyBase.enemies[i].basicMissChance = EditorGUILayout.FloatField("Basic miss chance:", enemyBase.enemies[i].basicMissChance);
            enemyBase.enemies[i].XPReward = EditorGUILayout.IntField("XP reward:", enemyBase.enemies[i].XPReward);
            enemyBase.enemies[i].enemyIcon = (Sprite)EditorGUILayout.ObjectField("Enemy icon:", enemyBase.enemies[i].enemyIcon, typeof(Sprite), true);
            enemyBase.enemies[i].abilityName = (string)EditorGUILayout.TextField("Ability name:", enemyBase.enemies[i].abilityName);
            enemyBase.enemies[i].offset = (Vector3)EditorGUILayout.Vector3Field("Offset:", enemyBase.enemies[i].offset);
            enemyBase.enemies[i].moneyReward = EditorGUILayout.IntField("Money reward:", enemyBase.enemies[i].moneyReward);

            for (int j = 0; j < 4; ++j) {
                EditorGUILayout.BeginVertical("Window");
                enemyBase.enemies[i].effectResists[j].effectType = (EffectType)EditorGUILayout.EnumPopup("Effect type:", enemyBase.enemies[i].effectResists[j].effectType);
                enemyBase.enemies[i].effectResists[j].resist = EditorGUILayout.IntField("Resist:", enemyBase.enemies[i].effectResists[j].resist);
                EditorGUILayout.EndVertical();
            }
            
            EditorGUILayout.EndVertical();
            MakeHugeSpace(2);
        }

        if (GUILayout.Button("Add element")) enemyBase.IncreaseEnemyArray();
        if (GUILayout.Button("Remove element")) enemyBase.RemoveLastElement();

        EditorGUILayout.EndVertical();
    }
 }
 #endif
