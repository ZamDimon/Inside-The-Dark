using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class IceArrowObject : MonoBehaviour
{
    private const float ERROR = 0.1f;
    private bool isFlying = true;
    private float timer = 0f;

    #region Flying part settings
    [SerializeField] private float startSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private List<Sprite> flyingAnimationFrames = new List<Sprite>();

    private Transform targetTransform;
    private float offsetX;
    private int currentFlyingAnimationFrame = 0;
    #endregion

    [SerializeField] private float delayBetweenFrames;
    [SerializeField] private List<Sprite> explosionFrames = new List<Sprite>();

    #region Events
    public event Action onExplosion;
    public event Action onAnimationEnd;
    #endregion

    public void SetIceArrowSettings(float _offsetX, Transform _targetTransform) {
        offsetX = _offsetX;
        targetTransform = _targetTransform;
    }

    public void StartMoving() => StartCoroutine(MovingTowardsTarget(targetTransform, offsetX));

    private void ChangeFlyingPartFrame() {
        if (!isFlying)
            return;

        transform.GetComponent<SpriteRenderer>().sprite = flyingAnimationFrames[currentFlyingAnimationFrame++];
        
        if (currentFlyingAnimationFrame >= flyingAnimationFrames.Count)
            currentFlyingAnimationFrame = 0;
    }

    private IEnumerator MovingTowardsTarget(Transform targetObject, float offsetX) {
        float currentSpeed = startSpeed;

        while (transform.position.x < targetObject.position.x + offsetX) {
            transform.position += new Vector3(Mathf.Min(currentSpeed * Time.deltaTime, Mathf.Abs(targetObject.position.x + offsetX - transform.position.x + ERROR)), 0f, 0f);
            currentSpeed += Time.deltaTime * acceleration;
            yield return null;
        }

        onExplosion?.Invoke();
        isFlying = false;
        StartCoroutine(StartExplosion());
    }

    private IEnumerator StartExplosion() {
        int currentFrame = 0;

        while (currentFrame < explosionFrames.Count) {
            transform.GetComponent<SpriteRenderer>().sprite = explosionFrames[currentFrame++];
            yield return new WaitForSeconds(delayBetweenFrames);
        }

        onAnimationEnd?.Invoke();
        Destroy(gameObject);
    }

    private void ApplyTimerLogic() {
        timer += Time.deltaTime;
        if (timer >= delayBetweenFrames) {
            ChangeFlyingPartFrame();
            timer = 0f;
        }
    }

    private void Update() => ApplyTimerLogic();
}
