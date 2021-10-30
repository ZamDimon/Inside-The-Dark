using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillCell : CellScript, IPointerClickHandler {
    public AudioClip clickSound;
    private CharacterTurnHandler turnHandler;

    private void Start() => turnHandler = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CharacterTurnHandler>();
    public void Update() => gameObject.GetComponent<Image>().color = turnHandler.GetAbilityToAttack() ? Color.white : Color.gray;
    public void OnPointerClick(PointerEventData eventData) => AudioManager.PlaySound(clickSound);
    
}

