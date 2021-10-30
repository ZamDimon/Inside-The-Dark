using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MathDogs.UIAnimations;
using UnityEngine.UI;

public class ExitMenu : MonoBehaviour
{
    public float bestiaryOpeningSpeed = 1.5f;
    public GameObject[] objectsToTurn;
    //public GameObject bestiaryObject;
    public GameObject[] bestiaryObjects;
    private UIElement[] elements;

    public AudioClip closeSound;
    public GameObject darkeningObject;
    public AudioClip clipToPlay;

    private void Start() {
        elements = UIConverter.GetElementsFromGameObjects(bestiaryObjects);

        for(int i = 0; i < bestiaryObjects.Length; ++i) {
            bestiaryObjects[i].GetComponent<Graphic>().color -= new Color(0, 0, 0, bestiaryObjects[i].GetComponent<Graphic>().color.a);
        }
    }

    public void QuitTheGame () {
        Application.Quit();
    }

    public void EndTheGame() {
        SceneManager.LoadScene((int)Scenes.Cutscene2);
    }

    public void ActivateObjects(bool ActivateMode) {
        if (darkeningObject.activeSelf && ActivateMode == true)
            return;

        for (int i = 0; i < objectsToTurn.Length; ++i)
            objectsToTurn[i].SetActive(ActivateMode);
    }

    public void ActivateBestiary () {
        if (darkeningObject.activeSelf)
            return;

        SetActiveBestiary(true);
        darkeningObject.SetActive(true);
        FadingAnimations.ShowArrayOfObjects(elements, bestiaryOpeningSpeed, null, "Bestiary");
        AudioManager.PlaySound(clipToPlay);
    }

    public void CloseBestiary() {
        darkeningObject.SetActive(false);
        FadingAnimations.HideArrayOfObjects(elements, bestiaryOpeningSpeed, () => SetActiveBestiary(false), "Bestiary");
        AudioManager.PlaySound(closeSound);
    }

    private void SetActiveBestiary(bool value) {
        for (int i = 0; i < bestiaryObjects.Length; ++i)
            bestiaryObjects[i].SetActive(value);
    }
}
