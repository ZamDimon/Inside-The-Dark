using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleOrderUI : MonoBehaviour {
    private const int MAX_ENEMY_AMOUNT = 5;

    [System.Serializable]
    private struct TurnOrderCell {
        [SerializeField] public GameObject interfaceObject;
        [HideInInspector] public Vector3 interfaceObjectStartPosition; 
    }

    private Battle battle;
    [SerializeField] private TurnOrderCell[] interfaceOrderObjects = new TurnOrderCell[MAX_ENEMY_AMOUNT];
    [SerializeField] private Sprite characterIcon;
    [SerializeField] private float fadingSpeed;

    private float startAlphaComponent;
    private Coroutine currentPlayingCoroutine;

    private void SetStartPositions() {
        for (int i = 0; i < interfaceOrderObjects.Length; ++i) {
            interfaceOrderObjects[i].interfaceObjectStartPosition = interfaceOrderObjects[i].interfaceObject.GetComponent<RectTransform>().position;
        }
    }

    private void InitializeLinksToOtherScripts() {
        battle = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Battle>();    
        //battle.OnTurnChanged += () => Debug.Log("There was a change!");
    }
    private void InitializeStartAlphaComponent() => startAlphaComponent = interfaceOrderObjects[0].interfaceObject.GetComponent<Graphic>().color.a;

    private void SubscribeToEvents() {
        battle.OnStartBattle += () => {SetVisibilityToIcons(true);};
        battle.OnTurnChanged += ShowVisibilityToIconsWithAnimation;
    }
    private void Awake() {
        InitializeLinksToOtherScripts();
        SetStartPositions();
        InitializeStartAlphaComponent();
        SubscribeToEvents();
    }

    public void SetVisibilityToIcons(bool changeTransparency) {
        for (int i = 0; i < interfaceOrderObjects.Length; ++i) {
            if (i < battle.GetEnemyAmount()) {
                interfaceOrderObjects[i].interfaceObject.GetComponent<Image>().overrideSprite = (battle.GetTurnByID(i) == 0) ? characterIcon : battle.GetEnemySprite(i);
                if (changeTransparency)
                    interfaceOrderObjects[i].interfaceObject.GetComponent<Image>().color = new Color(interfaceOrderObjects[i].interfaceObject.GetComponent<Image>().color.r, interfaceOrderObjects[i].interfaceObject.GetComponent<Image>().color.g, interfaceOrderObjects[i].interfaceObject.GetComponent<Image>().color.b, 1);
            } else interfaceOrderObjects[i].interfaceObject.GetComponent<Image>().color = new Color(interfaceOrderObjects[i].interfaceObject.GetComponent<Image>().color.r, interfaceOrderObjects[i].interfaceObject.GetComponent<Image>().color.g, interfaceOrderObjects[i].interfaceObject.GetComponent<Image>().color.b, 0);
        }
    }

    private IEnumerator HideElementsCoroutine(float speed) {
        while (interfaceOrderObjects[0].interfaceObject.GetComponent<Graphic>().color.a > 0) {
            for (int i = 0; i < interfaceOrderObjects.Length; ++i) {
                float valueToAdd = -Mathf.Min(Time.deltaTime * speed, interfaceOrderObjects[i].interfaceObject.GetComponent<Graphic>().color.a);
                interfaceOrderObjects[i].interfaceObject.GetComponent<Graphic>().color += new Color(0, 0, 0, valueToAdd);
            }

            yield return null;
        }

        SetVisibilityToIcons(false);
        yield return currentPlayingCoroutine = StartCoroutine(ShowElementsCoroutine(speed));
    }

    private IEnumerator ShowElementsCoroutine(float speed) {
        while (interfaceOrderObjects[0].interfaceObject.GetComponent<Graphic>().color.a < 1) {
            for (int i = 0; i < interfaceOrderObjects.Length; ++i) {
                if (i >= battle.GetEnemyAmount()) 
                    continue;

                float valueToAdd = Mathf.Min(Time.deltaTime * speed, 1 - interfaceOrderObjects[0].interfaceObject.GetComponent<Graphic>().color.a);
                interfaceOrderObjects[i].interfaceObject.GetComponent<Graphic>().color += new Color(0, 0, 0, valueToAdd);
            }

            yield return null;
        }
    }

    private void StopAnimationCoroutine() {
        if (currentPlayingCoroutine == null)
            return;

        StopCoroutine(currentPlayingCoroutine);

        for (int i = 0; i < interfaceOrderObjects.Length; ++i) {
            if (i >= battle.GetEnemyAmount()) 
                continue;

            interfaceOrderObjects[i].interfaceObject.GetComponent<Graphic>().color = new Color(
                interfaceOrderObjects[i].interfaceObject.GetComponent<Graphic>().color.r,
                interfaceOrderObjects[i].interfaceObject.GetComponent<Graphic>().color.g,
                interfaceOrderObjects[i].interfaceObject.GetComponent<Graphic>().color.b,
                1.0f
            );
        }
    }

    public void ShowVisibilityToIconsWithAnimation(){ 
        StopAnimationCoroutine();
        currentPlayingCoroutine = StartCoroutine(HideElementsCoroutine(fadingSpeed));
    }
}
