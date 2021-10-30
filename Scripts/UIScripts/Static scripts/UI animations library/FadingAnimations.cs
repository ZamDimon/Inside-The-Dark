using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace MathDogs.UIAnimations {
    [System.Serializable] public struct UIElement {
        public GameObject UIObject;
        public float startAlpha;
        private bool isPlayingCoroutine;

        public bool GetPlayingCoroutineState() => isPlayingCoroutine;
        public void SetPlayingCoroutine(bool value) => isPlayingCoroutine = value;

        public UIElement(GameObject UIObject, float startAlpha) {
            this.UIObject = UIObject;
            this.startAlpha = startAlpha;
            isPlayingCoroutine = false;
        }
    }

    public class UIConverter {
        public static UIElement[] GetElementsFromGameObjects(GameObject[] gameObjects) {
            UIElement[] result = new UIElement[gameObjects.Length];
            for (int i = 0; i < gameObjects.Length; ++i) {
                result[i] = new UIElement(gameObjects[i], gameObjects[i].GetComponent<Graphic>().color.a);
            }

            return result;
        }
    }

    public class FadingAnimations : MonoBehaviour {
        private static FadingAnimations instance;
        private static Dictionary<string, List<Coroutine>> currentCoroutines = new Dictionary<string, List<Coroutine>>();

        private void Awake() => instance = this;

        private static void ClearCoroutineList(string key) {
            if (!currentCoroutines.ContainsKey(key)) return;
            if (currentCoroutines[key] == null) return;
            
            for (int i = 0; i < currentCoroutines[key].Count; ++i) {
                if (currentCoroutines[key][i] != null)
                    instance.StopCoroutine(currentCoroutines[key][i]);
            }

            currentCoroutines[key].Clear();
        }

        private static void AddCoroutineToList(string key, Coroutine coroutine) {
            if (!currentCoroutines.ContainsKey(key)) {
                List<Coroutine> newCoroutine = new List<Coroutine>();
                newCoroutine.Add(coroutine);
                currentCoroutines.Add(key, newCoroutine);
                return;
            }

            currentCoroutines[key].Add(coroutine);
        }

        public static void ShowObject(UIElement element, float fadingSpeed, Action actionOnEnded, string key) {
            ClearCoroutineList(key);
            AddCoroutineToList(key, instance.StartCoroutine(instance.ShowObjectCoroutine(element, fadingSpeed, actionOnEnded, null, null)));
        }

        public static void HideObject(UIElement element, float fadingSpeed, Action actionOnEnded, string key) {
            ClearCoroutineList(key);
            AddCoroutineToList(key, instance.StartCoroutine(instance.HideObjectCoroutine(element, fadingSpeed, actionOnEnded, null, null)));
        }

        public static void ShowArrayOfObjects(UIElement[] elements, float fadingSpeed, Action actionOnEnded, string key) {
            ClearCoroutineList(key);
            
            for (int i = 0; i < elements.Length; ++i) {
                int currentIndex = i;
                AddCoroutineToList(key, instance.StartCoroutine(instance.ShowObjectCoroutine(elements[i], fadingSpeed, null,
                () => elements[currentIndex].SetPlayingCoroutine(true), () => elements[currentIndex].SetPlayingCoroutine(false))));
            }

            AddCoroutineToList(key, instance.StartCoroutine(instance.DoActionAfterAllAnimations(elements, actionOnEnded)));
        }

        public static void HideArrayOfObjects(UIElement[] elements, float fadingSpeed, Action actionOnEnded, string key) {
            ClearCoroutineList(key);

            for (int i = 0; i < elements.Length; ++i) {
                int currentIndex = i;
                currentCoroutines[key].Add(instance.StartCoroutine(instance.HideObjectCoroutine(elements[i], fadingSpeed, null, 
                () => elements[currentIndex].SetPlayingCoroutine(true), () => elements[currentIndex].SetPlayingCoroutine(false))));
            } 

            AddCoroutineToList(key, instance.StartCoroutine(instance.DoActionAfterAllAnimations(elements, actionOnEnded)));
        }

        private IEnumerator ShowObjectCoroutine(UIElement element, float fadingSpeed, Action actionOnEnded, Action SetTrue, Action SetFalse) {
            SetTrue?.Invoke();

            if (element.UIObject.GetComponent<Graphic>() == null) {
                Debug.LogError("Element's color cannot be modified");
                SetFalse?.Invoke();
                yield break;
            }
            
            while (element.UIObject.GetComponent<Graphic>().color.a < element.startAlpha) {
                float alphaComponentToAdd = Mathf.Min(Time.deltaTime * fadingSpeed, element.startAlpha - element.UIObject.GetComponent<Graphic>().color.a);
                element.UIObject.GetComponent<Graphic>().color += new Color(0, 0, 0, alphaComponentToAdd);

                yield return null;
            }

            SetFalse?.Invoke();
            actionOnEnded?.Invoke();
        }

        private IEnumerator HideObjectCoroutine(UIElement element, float fadingSpeed, Action actionOnEnded, Action SetTrue, Action SetFalse) {
            SetTrue?.Invoke();
            
            if (element.UIObject == null) {
                Debug.LogError("Element's object was not found");
                SetFalse.Invoke();
                yield break;
            }

            if (element.UIObject.GetComponent<Graphic>() == null) {
                Debug.LogError("Element's color cannot be modified");
                SetFalse.Invoke();
                yield break;
            }
            
            while (element.UIObject.GetComponent<Graphic>().color.a > 0) {
                float alphaComponentToAdd = -Mathf.Min(Time.deltaTime * fadingSpeed, element.UIObject.GetComponent<Graphic>().color.a);
                element.UIObject.GetComponent<Graphic>().color += new Color(0, 0, 0, alphaComponentToAdd);

                yield return null;
            }

            SetFalse?.Invoke();
            actionOnEnded?.Invoke();
        }

        private static bool IsDisactiveAllAnimationsInElements(UIElement[] elements) {
            for (int i = 0; i < elements.Length; ++i) {
                if (elements[i].GetPlayingCoroutineState())
                    return false;
            }

            return true;
        }

        private IEnumerator DoActionAfterAllAnimations(UIElement[] elements, Action action) {
            while (!IsDisactiveAllAnimationsInElements(elements)) {
                yield return null;
            }

            action?.Invoke();
        }
    }
}