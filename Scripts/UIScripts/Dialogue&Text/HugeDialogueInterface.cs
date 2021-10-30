using UnityEngine;
using UnityEditor;
using System.Collections;

#if UNITY_EDITOR
[CustomEditor(typeof(HugeDialogue))]
public class HugeDialogueInterface : Editor {
    private const int MAX_ARRAY_SIZE = 1000;

    private GUIStyle styleForTitles;
    private GUIStyle styleForSimpleText;
    private HugeDialogue hugeDialogue;
    private Vector2 scrollPositionAnimation;
    private Vector2 scrollPositionDialogueParts;

    private void SetStyleForLabels() {
        styleForTitles = new GUIStyle();

        styleForTitles.fontSize = 15;
        styleForTitles.alignment = TextAnchor.UpperCenter;
        styleForTitles.fontStyle = FontStyle.Bold;

        styleForSimpleText = new GUIStyle();

        styleForSimpleText.fontSize = 12;
        styleForSimpleText.alignment = TextAnchor.MiddleCenter;
        styleForSimpleText.fontStyle = FontStyle.Bold;
    }
    
    private void SetStartSettings() {
        hugeDialogue = (HugeDialogue)target;
        SetStyleForLabels();
    }

    private void MakeHugeSpace(int size) {
        for (int i = 0; i < size; ++i)
            EditorGUILayout.Space();
    }
    override public void OnInspectorGUI() {
        SetStartSettings();

        EditorGUILayout.BeginVertical("Window");
        GUIStyle style = new GUIStyle();

        EditorGUILayout.LabelField("Showing dialogue settings", styleForTitles);
        
        hugeDialogue.showingMainBoxSpeed = EditorGUILayout.Slider("Showing speed", hugeDialogue.showingMainBoxSpeed, 0.1f, 3f);

        EditorGUILayout.BeginVertical("Window");
        hugeDialogue.backgroundElement.UIObject = (GameObject)EditorGUILayout.ObjectField("Main Background Object:", hugeDialogue.backgroundElement.UIObject, typeof(GameObject), true);
        hugeDialogue.backgroundElement.startAlpha = EditorGUILayout.Slider("Start alpha component:", hugeDialogue.backgroundElement.startAlpha, 0f, 1f);
        EditorGUILayout.EndVertical();

        MakeHugeSpace(3);

        scrollPositionAnimation = EditorGUILayout.BeginScrollView(scrollPositionAnimation, GUILayout.Height(400));
        for (int i = 0; i < hugeDialogue.objectsToActivateLength; ++i) {
            EditorGUILayout.BeginVertical("Window");
            EditorGUILayout.LabelField($"Element {i+1}", styleForSimpleText);

            hugeDialogue.objectsToActivate[i].UIObject = (GameObject)EditorGUILayout.ObjectField("Object:", hugeDialogue.objectsToActivate[i].UIObject, typeof(GameObject), true);
            hugeDialogue.objectsToActivate[i].startAlpha = EditorGUILayout.Slider("Start alpha component:", hugeDialogue.objectsToActivate[i].startAlpha, 0f, 1f);

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }
        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("Add element")) {
            hugeDialogue.objectsToActivateLength++;
        }
        if (GUILayout.Button("Remove top element")) {
            hugeDialogue.RemoveTopElement(hugeDialogue.objectsToActivate, ref hugeDialogue.objectsToActivateLength);
        }
        if (GUILayout.Button("Remove all elements")) {
            hugeDialogue.RemoveAllElements(hugeDialogue.objectsToActivate, ref hugeDialogue.objectsToActivateLength);
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Set start transparency value")) hugeDialogue.InitializeAllElements();
        if (GUILayout.Button("Set start transparency to objects")) hugeDialogue.GetBackTransparency();
        if (GUILayout.Button("Hide all elements")) hugeDialogue.HideAllElements();

        EditorGUILayout.EndVertical();

        MakeHugeSpace(3);

        EditorGUILayout.BeginVertical("Window");
        EditorGUILayout.LabelField("Dialogue settings", styleForTitles);

        EditorGUILayout.BeginVertical("Window");
        EditorGUILayout.LabelField("Gameobjects settings", styleForSimpleText);
        
        hugeDialogue.textObject = (GameObject)EditorGUILayout.ObjectField("Text object:", hugeDialogue.textObject, typeof(GameObject), true);
        hugeDialogue.speakerImageObject = (GameObject)EditorGUILayout.ObjectField("Speaker image object:", hugeDialogue.speakerImageObject, typeof(GameObject), true);
        hugeDialogue.speakerNameObject = (GameObject)EditorGUILayout.ObjectField("Speaker name object:", hugeDialogue.speakerNameObject, typeof(GameObject), true);

        MakeHugeSpace(2);
        EditorGUILayout.EndVertical();

        MakeHugeSpace(2);
        EditorGUILayout.BeginVertical("Window");
        EditorGUILayout.LabelField("Audio settings", styleForSimpleText);

        hugeDialogue.typingSoundArraySize = EditorGUILayout.IntField("Sounds number:", hugeDialogue.typingSoundArraySize);
        for (int i = 0; i < hugeDialogue.typingSoundArraySize; ++i) {
            hugeDialogue.typingSounds[i] = (AudioClip)EditorGUILayout.ObjectField("Speaker name object:", hugeDialogue.typingSounds[i], typeof(AudioClip), true);
        }

        EditorGUILayout.EndVertical();

        MakeHugeSpace(2);

        EditorGUILayout.BeginVertical("Window");
        EditorGUILayout.LabelField("Skip object settings", styleForSimpleText);
        hugeDialogue.skipObject = (GameObject)EditorGUILayout.ObjectField("Skip object:", hugeDialogue.skipObject, typeof(GameObject), true);
        hugeDialogue.frequency = EditorGUILayout.FloatField("Frequency:", hugeDialogue.frequency);
        hugeDialogue.minGrayValue = EditorGUILayout.FloatField("Min gray value:", hugeDialogue.minGrayValue);
        hugeDialogue.maxGrayValue = EditorGUILayout.FloatField("Max gray value:", hugeDialogue.maxGrayValue);

        EditorGUILayout.EndVertical();

        MakeHugeSpace(2);

        EditorGUILayout.BeginVertical("Window");
        EditorGUILayout.LabelField("Pause settings", styleForSimpleText);

        hugeDialogue.pauseKey = EditorGUILayout.TextField("Pause key:", hugeDialogue.pauseKey);
        hugeDialogue.pausesDelay = EditorGUILayout.FloatField("Pause delays", hugeDialogue.pausesDelay);

        EditorGUILayout.EndVertical();

        MakeHugeSpace(2);

        EditorGUILayout.BeginVertical("Window");

        scrollPositionDialogueParts = EditorGUILayout.BeginScrollView(scrollPositionDialogueParts, GUILayout.Height(400), GUILayout.Width(00));

        for (int i = 0; i < hugeDialogue.dialoguePartsAmount; ++i) {
            EditorGUILayout.BeginVertical("Window");
            EditorGUILayout.LabelField($"Dialogue part {i + 1}", styleForSimpleText);
            
            hugeDialogue.dialogueParts[i].speakerSprite = (Sprite)EditorGUILayout.ObjectField("Speaker Sprite:", hugeDialogue.dialogueParts[i].speakerSprite, typeof(Sprite), true);
            hugeDialogue.dialogueParts[i].playersWords = EditorGUILayout.Toggle("Are these the player's words?", hugeDialogue.dialogueParts[i].playersWords);
            if (!hugeDialogue.dialogueParts[i].playersWords)
                hugeDialogue.dialogueParts[i].speakerName = EditorGUILayout.TextField("Speaker name", hugeDialogue.dialogueParts[i].speakerName);
            hugeDialogue.dialogueParts[i].typingDelay = EditorGUILayout.FloatField("Typing delay:", hugeDialogue.dialogueParts[i].typingDelay);
            hugeDialogue.dialogueParts[i].delayBeforeNextPart = EditorGUILayout.FloatField("Delay before next part:", hugeDialogue.dialogueParts[i].delayBeforeNextPart);
            hugeDialogue.dialogueParts[i].soundPitch = EditorGUILayout.Slider("Pitch sound:", hugeDialogue.dialogueParts[i].soundPitch, 0f, 4f);

            hugeDialogue.dialogueParts[i].dialogueText = EditorGUILayout.TextArea(hugeDialogue.dialogueParts[i].dialogueText, GUILayout.MaxHeight(75), GUILayout.Width(400));

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }
        EditorGUILayout.EndScrollView();


        if (GUILayout.Button("Add element"))  hugeDialogue.dialoguePartsAmount++;
        if (GUILayout.Button("Remove top element")) hugeDialogue.RemoveTopElement(hugeDialogue.dialogueParts, ref hugeDialogue.dialoguePartsAmount);
        if (GUILayout.Button("Remove all elements")) hugeDialogue.RemoveAllElements(hugeDialogue.dialogueParts, ref hugeDialogue.dialoguePartsAmount);

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndVertical();
    }
}
#endif