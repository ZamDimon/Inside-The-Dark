using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using System;

public class HugeDialogue : MonoBehaviour {
    private const int MAX_ARRAY_SIZE = 1000;
    [System.Serializable] public struct DialoguePart {
        public string dialogueText;
        public string speakerName;
        public Sprite speakerSprite;
        public float typingDelay;
        public float soundPitch;
        public float delayBeforeNextPart;
        public bool playersWords;
    }
    [System.Serializable] public struct InterfaceElement {
        [SerializeField] public GameObject UIObject;
        [SerializeField] public float startAlpha;

        public InterfaceElement(GameObject _UIObject, float _startAlpha) {
            UIObject = _UIObject;
            startAlpha = _startAlpha;
        }
    }

    public event Action onDialogueEnd;

    #region Showing animation settings
    private List<Coroutine> currentShowingAnimations = new List<Coroutine>();
    private bool isPlayingCoroutine;

    public int objectsToActivateLength = 0;
    public InterfaceElement backgroundElement;
    public float showingMainBoxSpeed;
    public InterfaceElement[] objectsToActivate = new InterfaceElement[1000];
    #endregion

    #region Dialogue settings
    //Objects settings
    public GameObject speakerImageObject;
    public GameObject speakerNameObject;
    public GameObject textObject;

    //Pause settings
    public float pausesDelay;
    public string pauseKey;

    //Sound settings
    private float startPitch = 1f;
    public AudioClip[] typingSounds = new AudioClip[MAX_ARRAY_SIZE];
    public int typingSoundArraySize = 0;

    //Dialogue parts settings
    public GameObject skipObject;
    public float minGrayValue;
    public float maxGrayValue;
    public float frequency;

    public int dialoguePartsAmount = 0;
    public DialoguePart[] dialogueParts = new DialoguePart[MAX_ARRAY_SIZE];

    private bool isDialoguePartPlayed = false;
    private Coroutine showingPartCoroutine;
    #endregion

    #region Other settings
    private float time;
    private List<int> positionsWithDelay = new List<int>();
    #endregion

    public void InitializeAllElements() {
        backgroundElement.startAlpha = backgroundElement.UIObject.GetComponent<Graphic>().color.a;

        for (int i = 0; i < objectsToActivateLength; ++i) {
            objectsToActivate[i].startAlpha = objectsToActivate[i].UIObject.GetComponent<Graphic>().color.a;
        }
    }

    public void HideAllElements() {
        backgroundElement.UIObject.GetComponent<Graphic>().color -= new Color(0, 0, 0, backgroundElement.UIObject.GetComponent<Graphic>().color.a);

        for (int i = 0; i < objectsToActivateLength; ++i) {
            objectsToActivate[i].UIObject.GetComponent<Graphic>().color -= new Color(0, 0, 0, objectsToActivate[i].UIObject.GetComponent<Graphic>().color.a);
        }
    }

    public void GetBackTransparency() {
        backgroundElement.UIObject.GetComponent<Graphic>().color = new Color(
            backgroundElement.UIObject.GetComponent<Graphic>().color.r,
            backgroundElement.UIObject.GetComponent<Graphic>().color.g,
            backgroundElement.UIObject.GetComponent<Graphic>().color.b,
            backgroundElement.startAlpha);

        for (int i = 0; i < objectsToActivateLength; ++i) {
            objectsToActivate[i].UIObject.GetComponent<Graphic>().color = 
                new Color(objectsToActivate[i].UIObject.GetComponent<Graphic>().color.r,
                objectsToActivate[i].UIObject.GetComponent<Graphic>().color.g,
                objectsToActivate[i].UIObject.GetComponent<Graphic>().color.b, 
                objectsToActivate[i].startAlpha);
        }
    }

    public void RemoveAllElements<T>(T[] array, ref int totalAmount) {
        for (int i = 0; i < array.Length; ++i)
            array[i] = default(T);

        totalAmount = 0;
    }

    public void RemoveTopElement<T>(T[] array, ref int totalAmount) {
        array[totalAmount - 1] = default(T);
        totalAmount--;
    }

    public void OpenAndPlayDialogue() {
        if (currentShowingAnimations != null)
            StopCoroutinesFromList(currentShowingAnimations);
        currentShowingAnimations.Add(StartCoroutine(showElement(backgroundElement, showingMainBoxSpeed)));
        backgroundElement.UIObject.GetComponent<Image>().raycastTarget = true;
        for (int i = 0; i < objectsToActivateLength; ++i)
            currentShowingAnimations.Add(StartCoroutine(showElement(objectsToActivate[i], showingMainBoxSpeed)));
        PlayDialogue();
    }

    public void CloseDialogueMenu() {
        if (currentShowingAnimations != null)
            StopCoroutinesFromList(currentShowingAnimations);
        backgroundElement.UIObject.GetComponent<Image>().raycastTarget = false;

        currentShowingAnimations.Add(StartCoroutine(hideElement(backgroundElement, showingMainBoxSpeed, null)));

        for (int i = 0; i < objectsToActivateLength; ++i)
            currentShowingAnimations.Add(StartCoroutine(hideElement(objectsToActivate[i], showingMainBoxSpeed, null)));

        onDialogueEnd?.Invoke();
        gameObject.SetActive(false);
    }

    private void Awake() => startPitch = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>().pitch;

    private IEnumerator showElement(InterfaceElement element, float speed) {
        if (speed <= 0f) {
            Debug.LogError("The value of speed can't be less or equal to 0");
            yield break;
        }

        while (element.UIObject.GetComponent<Graphic>().color.a < element.startAlpha) {
            element.UIObject.GetComponent<Graphic>().color += new Color(0, 0, 0, Mathf.Min(Time.deltaTime * speed, element.startAlpha - element.UIObject.GetComponent<Graphic>().color.a));

            yield return null;
        }

        yield break;
    }

    private IEnumerator hideElement(InterfaceElement element, float speed, Action actionOnEnd) {
        if (speed <= 0f) {
            Debug.LogError("The value of speed can't be less or equal to 0");
            yield break;
        }

        while (element.UIObject.GetComponent<Graphic>().color.a >= 0) {
            element.UIObject.GetComponent<Graphic>().color -= new Color(0, 0, 0, Mathf.Min(Time.deltaTime * speed, element.UIObject.GetComponent<Graphic>().color.a));

            yield return null;
        }
        actionOnEnd?.Invoke();
    }

    private void StopCoroutinesFromList(List<Coroutine> list) {
        foreach (Coroutine coroutine in list) {
            if (coroutine != null)
                StopCoroutine(coroutine);
        }

        list.Clear();
    }

    private void PlayRandomSound(AudioClip[] audioClips, int arraySize) => AudioManager.PlaySound(audioClips[UnityEngine.Random.Range(0, arraySize)]);
    
    private bool AllElementsShowed() {
        for (int i = 0; i < objectsToActivateLength; ++i) {
            if (objectsToActivate[i].UIObject.GetComponent<Graphic>().color.a < objectsToActivate[i].startAlpha)
                return false;
        }

        return true;
    }

    private string RemovePauses (string text, string key, ref List<int> positionsWithDelay) {
        if (key == null || text == null || key == "" || text == "") {
            Debug.LogError("You can't modify this text");
            return "ERROR";
        }

        //Preparing some variables before launching algorithm
        if (positionsWithDelay.Count > 0) positionsWithDelay.Clear();
        string resultText = "", currentKey = "";

        for (int i = 0; i < text.Length; ++i) {
            if (key[currentKey.Length] == text[i]) {
                currentKey += text[i];
            } else currentKey = "";

            resultText += text[i];

            if (key == currentKey) {
                resultText = resultText.Remove(resultText.Length - key.Length, key.Length);
                positionsWithDelay.Add(resultText.Length);
                currentKey = "";
            }
        }

        return resultText;
    }

    private int GetPositionWithDelay(int index) => (index >= positionsWithDelay.Count) ? (-1) : positionsWithDelay[index];

    private IEnumerator WriteTextSmoothly(DialoguePart dialoguePart) {
        isDialoguePartPlayed = true;
        textObject.GetComponent<Text>().text = "";
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>().pitch = dialoguePart.soundPitch;

        int currentID = 0, positionWithDelayID = 0;
        string currentText = dialoguePart.dialogueText;
        currentText = RemovePauses(currentText, pauseKey, ref positionsWithDelay);

        while (textObject.GetComponent<Text>().text != currentText) {
            textObject.GetComponent<Text>().text += currentText[currentID];
            PlayRandomSound(typingSounds, typingSoundArraySize);
            ++currentID;

            float delayTime = 0f;

            if (currentID == GetPositionWithDelay(positionWithDelayID)) {
                delayTime = pausesDelay;
                ++positionWithDelayID;
            } else delayTime = dialoguePart.typingDelay;

            yield return new WaitForSeconds(delayTime);
        }
        isDialoguePartPlayed = false;
    }

    private string GetSpeakerName(DialoguePart dialoguePart) {
        if (dialoguePart.playersWords) return PlayerPrefs.GetString("CharacterName");
        else return dialoguePart.speakerName;
    }

    private void SetDialoguePart (DialoguePart dialoguePart) {
        speakerImageObject.GetComponent<Image>().sprite = dialoguePart.speakerSprite;
        speakerNameObject.GetComponent<Text>().text = GetSpeakerName(dialoguePart);
        showingPartCoroutine = StartCoroutine(WriteTextSmoothly(dialoguePart));
    }

    private void SkipDialoguePart(int currentID) {
        StopCoroutine(showingPartCoroutine);

        if (currentID == dialoguePartsAmount) 
            CloseDialogueMenu();
        else 
            SetDialoguePart(dialogueParts[currentID]);
    }

    private IEnumerator PlayAllDialogueParts() {
        int currentID = 0;

        while (currentID <= dialoguePartsAmount) {
            if (!isDialoguePartPlayed) {
                yield return new WaitForSeconds((currentID == 0) ? 0 : dialogueParts[currentID - 1].delayBeforeNextPart);
                if (currentID != dialoguePartsAmount) SetDialoguePart(dialogueParts[currentID]);
                ++currentID;
            } else if (Input.GetKeyDown(KeyCode.Space)) {
                SkipDialoguePart(currentID);
                ++currentID;
            }

            yield return null;
        }

        CloseDialogueMenu();
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>().pitch = startPitch;
    }

    private void PlayDialogue() => StartCoroutine(PlayAllDialogueParts());

    private float GetNormalizedValue(float value) => value / 255f;

    private void ChangeSkipButtonColor() {
        float currentColorComponent = (GetNormalizedValue(maxGrayValue) + GetNormalizedValue(minGrayValue)) / 2f + ((GetNormalizedValue(maxGrayValue) - GetNormalizedValue(minGrayValue)) / 2f) * Mathf.Sin(frequency * time);
        skipObject.GetComponent<Graphic>().color = new Color(currentColorComponent, currentColorComponent, currentColorComponent, 1);
        time += Time.deltaTime;
    }

    private void Update() {
        if (AllElementsShowed())
            ChangeSkipButtonColor();
    }
}