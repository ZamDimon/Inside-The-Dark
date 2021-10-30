using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace MathDogs.UIAnimations {
    public class SequentialAnimations : MonoBehaviour {
        private delegate void AnimationAction(UIElement element, float speed, Action action, string key); 
        private enum AnimationType {
            Hiding = 0,
            Showing = 1
        }

        private static SequentialAnimations instance;

        private void Awake() => instance = this;

        private static AnimationAction GetDelegateFromType(AnimationType animationType) {
            switch (animationType) {
                case AnimationType.Hiding: return FadingAnimations.HideObject;
                case AnimationType.Showing: return FadingAnimations.ShowObject;
            }

            Debug.Log($"The delegate with type {animationType.ToString()} was not found");
            return null;
        }

        private static void Animation_MultipleActions(UIElement[] elements, int id, float fadingSpeed, Action[] actions, AnimationType animationType, string key) {
            if (elements == null) {
                Debug.LogError("Elements are null");
                return;
            }

            if (id >= elements.Length)
                return;

            actions[id]?.Invoke();

            GetDelegateFromType(animationType)?.Invoke(elements[id], fadingSpeed, 
            () => Animation_MultipleActions(elements, id+1, fadingSpeed, actions, animationType, key), key);
        }

        private static void Animation_OneAction(UIElement[] elements, int id, float fadingSpeed, Action actionOnEnd, AnimationType animationType, string key) {
            if (elements == null) {
                Debug.LogError("Elements are missing");
                return;
            }

            if (id >= elements.Length) {
                actionOnEnd?.Invoke();
                return;
            }

            GetDelegateFromType(animationType)?.Invoke(elements[id], fadingSpeed, 
            () => Animation_OneAction(elements, id+1, fadingSpeed, actionOnEnd, animationType, key), key);
        }

        public static void PlayHidingAnimation_MultipleActions(UIElement[] elements, float fadingSpeed, Action[] actions, string key) => 
            Animation_MultipleActions(elements, 0, fadingSpeed, actions, AnimationType.Hiding, key);
        public static void PlayHidingAnimation_OneAction(UIElement[] elements, float fadingSpeed, Action actionOnEnded, string key) => 
            Animation_OneAction(elements, 0, fadingSpeed, actionOnEnded, AnimationType.Hiding, key);
        public static void PlayShowingAnimation_MultipleActions(UIElement[] elements, float fadingSpeed, Action[] actions, string key) => 
            Animation_MultipleActions(elements, 0, fadingSpeed, actions, AnimationType.Showing, key);
        public static void PlayShowingAnimation_OneAction(UIElement[] elements, float fadingSpeed, Action actionOnEnded, string key) => 
            Animation_OneAction(elements, 0, fadingSpeed, actionOnEnded, AnimationType.Showing, key);  
    }
}