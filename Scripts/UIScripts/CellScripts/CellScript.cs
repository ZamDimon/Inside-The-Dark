using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System;

public class CellScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {
    public bool isCampfire;
    public AudioClip AudioClip;
    private AudioSource audioSource;
    private InventoryScript inventoryScript;
    protected GameObject mainObject;
    protected bool IsOnHover;

    #region Description object settings
    [SerializeField] private GameObject skillNameObject;
    [SerializeField] private GameObject skillDescriptionObject;
    [SerializeField] private GameObject FotoObject;
    [SerializeField] private GameObject skillAttackObject;
    #endregion

    #region Description settings
    protected string cellObjectName;
    protected string artDescription;
    protected Sprite foto;
    [SerializeField] protected string actualDescription;
    #endregion

    #region Animation settings
    [SerializeField] private float resizeOffset;
    [SerializeField] private float resizingSpeed;

    private Vector2 startSize;
    private Coroutine currentPlayingResizingAnimation;
    #endregion

    public bool GetHoverState() => IsOnHover;

    private void Awake() {
        //Initialization stuff
        mainObject = GameObject.FindGameObjectWithTag("MainCamera");
        inventoryScript = mainObject.GetComponent<InventoryScript>();
        startSize = transform.GetComponent<RectTransform>().sizeDelta;
    }
    protected virtual void Update() => mainObject = GameObject.FindGameObjectWithTag("MainCamera");

    public void SetInfoSettings (string _cellObjectName, string _artDescription, Sprite _foto, string _actualDescription) {
        cellObjectName = _cellObjectName;
        artDescription = _artDescription;
        foto = _foto;
        actualDescription = _actualDescription;
    }

    public void SetInfoSettingsByItem(int itemID) {
        InventoryScript.ItemBase currentItem = inventoryScript.itemsBase[itemID]; 
        transform.GetComponent<Image>().sprite = currentItem.itemSprite;
        SetInfoSettings(currentItem.name, currentItem.description, currentItem.itemSprite, currentItem.DoDescription);
    }

    public void SetInfoSettingsByItem(ITEM item) => SetInfoSettingsByItem((int)item);

    private delegate bool ConditionForResizing();
    private IEnumerator ChangeSizeOfObject(RectTransform rectTransform, float offset, float speed) {
        if (speed < 0f) {
            Debug.LogError("The speed value mustn't be negative");
            yield break;
        }

        if (rectTransform == null) {
            Debug.LogError("Rect transform is missing");
            yield break;
        }

        Vector2 endSize = new Vector2(startSize.x + offset, startSize.y + offset);
        ConditionForResizing condition = () => true;

        while (condition()) {
            float koefficient = (offset > 0)? 1 : (-1);
            float temp_increasedValue = koefficient * Mathf.Min(Time.deltaTime * speed, Mathf.Abs(endSize.x - rectTransform.sizeDelta.x));
            
            rectTransform.sizeDelta += new Vector2(temp_increasedValue, temp_increasedValue);

            condition = () => {
                if (offset > 0) return endSize.x > rectTransform.sizeDelta.x;
                else return endSize.x < rectTransform.sizeDelta.x;
            };

            yield return null;
        }
    }

    private void StartAnimation() {
        if (currentPlayingResizingAnimation != null)
            StopCoroutine(currentPlayingResizingAnimation);

        gameObject.GetComponent<Graphic>().color = Color.grey;
        currentPlayingResizingAnimation = StartCoroutine(ChangeSizeOfObject(transform.GetComponent<RectTransform>(), resizeOffset, resizingSpeed));

    }

    private void EndAnimation() {
        if (currentPlayingResizingAnimation != null)
            StopCoroutine(currentPlayingResizingAnimation);

        gameObject.GetComponent<Image>().color = Color.white;
        currentPlayingResizingAnimation = StartCoroutine(ChangeSizeOfObject(transform.GetComponent<RectTransform>(), 0, resizingSpeed));
    }

    public virtual void OnPointerEnter(PointerEventData eventData) {
        StartAnimation();
        IsOnHover = true;

        if (!isCampfire) {
            skillNameObject.GetComponent<Text>().text = cellObjectName;
            skillDescriptionObject.GetComponent<Text>().text = artDescription;
            FotoObject.GetComponent<Image>().sprite = foto;
            skillAttackObject.GetComponent<Text>().text = actualDescription;
        }

        AudioManager.PlaySound(AudioClip);
    }

    public virtual void OnPointerClick(PointerEventData eventData) {
        IsOnHover = false;
    }

    public virtual void OnPointerExit(PointerEventData eventData) {
        EndAnimation();
        IsOnHover = false;
    }
}
