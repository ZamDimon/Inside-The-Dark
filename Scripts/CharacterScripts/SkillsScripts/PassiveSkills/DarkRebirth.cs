using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DarkRebirth : MonoBehaviour {
    [SerializeField] private List<Sprite> darkRebirthAnimationFrames = new List<Sprite>();
    [SerializeField] private float delayBetweenFrames = 0.2f;
    [SerializeField] private int frameOnHealing;
    [SerializeField] private AudioClip healingSound;
    [SerializeField] private Vector3 offset;

    private IEnumerator IEPlayAnimation(Action actionOnEnd) {
        GameObject.FindGameObjectWithTag("Character").GetComponent<CharacterMovement>().SetActiveAnimation(false);
        GameObject.FindGameObjectWithTag("Character").GetComponent<Animator>().enabled = false;
        GameObject.FindGameObjectWithTag("Character").transform.position += offset;

        int currentFrame = 0;
        while (currentFrame < darkRebirthAnimationFrames.Count) {
            if (frameOnHealing == currentFrame) {
                GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GameManager>().HealHP(
                    GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GameManager>().GetMaxHP()/2);

                AudioManager.PlaySound(healingSound);
            }

            GameObject.FindGameObjectWithTag("Character").GetComponent<SpriteRenderer>().sprite =
                darkRebirthAnimationFrames[currentFrame++];
            yield return new WaitForSeconds(delayBetweenFrames);
        }

        actionOnEnd?.Invoke();
        GameObject.FindGameObjectWithTag("Character").transform.position -= offset;
        GameObject.FindGameObjectWithTag("Character").GetComponent<CharacterMovement>().SetActiveAnimation(true);
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<PassiveSkillsManager>().SetCannotBeUsed(PassiveSkillsManager.PassiveSkillType.DarkRebirth);
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<PassiveSkillsManager>().UpdateSkills();
    }

    public void PlayAnimation(Action actionOnEnd) => StartCoroutine(IEPlayAnimation(actionOnEnd));

    private void Update() {
        /*if (Input.GetKeyDown(KeyCode.I)) {
            PlayAnimation(() => Debug.Log("Es war gemacht"));
        }*/
    }
}
