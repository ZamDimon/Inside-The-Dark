using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public delegate void EventDelegate();

public class FireballObject : MonoBehaviour {
    private const float ERROR = 0.1f;

    public event EventDelegate onExplosion;
    public event EventDelegate onDamage;
    public event EventDelegate onEndAnimation;

    [System.Serializable] private struct Frame {
        public Sprite sprite;
        public GameObject lightObject;
    }

    private enum FireballAnimationStatement {
        Flying = 0,
        Exploding = 1
    }

    private FireballAnimationStatement currentStatement;

    [Header("Flying settings")]
    #region Flying part settings
    [SerializeField] private float offsetX;
    [SerializeField] private float startSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private Transform targetTransform;
    [SerializeField] private GameObject startLightObject;
    [SerializeField] private List<Sprite> flyingAnimationFrames = new List<Sprite>();

    private int currentFlyingAnimationFrame = 0;
    private float animationTimer;
    #endregion

    #region Explosion part settings
    [Header("Explosion animation settings")]
    [SerializeField] private GameObject explosionAnimationObject;
    [SerializeField] private float delayBetweenFrames;
    [SerializeField] private float hidingSpeed = 5f;
    [SerializeField] private List<Frame> frames = new List<Frame>();
    [SerializeField] private int frameOnDamage;
    #endregion

    public void StartMoving(float offsetX) => StartCoroutine(movingTowardsTarget(targetTransform, offsetX));
    
    public void SetFireballSettings(float _offsetX, Transform _targetTransform) {
        offsetX = _offsetX;
        targetTransform = _targetTransform;
    }

    private void ChangeFlyingPartFrame() {
        transform.GetComponent<SpriteRenderer>().sprite = flyingAnimationFrames[currentFlyingAnimationFrame++];
        if (currentFlyingAnimationFrame >= flyingAnimationFrames.Count)
            currentFlyingAnimationFrame = 0;
    }

    private void ChangeAnimationState() {
        onExplosion?.Invoke();
        currentStatement = FireballAnimationStatement.Exploding;
        startLightObject.SetActive(false);
        transform.GetComponent<SpriteRenderer>().color -= new Color(0, 0, 0, transform.GetComponent<SpriteRenderer>().color.a);
        explosionAnimationObject.GetComponent<SpriteRenderer>().color = new Color(
            explosionAnimationObject.GetComponent<SpriteRenderer>().color.r,
            explosionAnimationObject.GetComponent<SpriteRenderer>().color.g,
            explosionAnimationObject.GetComponent<SpriteRenderer>().color.b, 1);
        StartCoroutine(PlayExplosionAnimation());
    }

    private IEnumerator movingTowardsTarget(Transform targetObject, float offsetX) {
        float currentSpeed = startSpeed;
        currentStatement = FireballAnimationStatement.Flying;

        while (transform.position.x < targetObject.position.x + offsetX) {
            transform.position += new Vector3(Mathf.Min(currentSpeed * Time.deltaTime, Mathf.Abs(targetObject.position.x + offsetX - transform.position.x + ERROR)), 0f, 0f);
            currentSpeed += Time.deltaTime * acceleration;
            yield return null;
        }

        ChangeAnimationState();
    }

    private void DisableAllLightning() {
        for (int i = 0; i < frames.Count; ++i)
            frames[i].lightObject.SetActive(false); 
    }

    private void SetFrame(int frameNumber) {
        explosionAnimationObject.GetComponent<SpriteRenderer>().sprite = frames[frameNumber].sprite;
        DisableAllLightning();
        frames[frameNumber].lightObject.SetActive(true);
    }

    private IEnumerator PlayExplosionAnimation() {
        float explosionAnimationTimer = 0f;
        int currentFrameNumber = 0;
        SetFrame(0);

        while (currentFrameNumber < frames.Count - 1) {
            explosionAnimationTimer += Time.deltaTime;
            if (explosionAnimationTimer > delayBetweenFrames) {
                ++currentFrameNumber;
                SetFrame(currentFrameNumber);

                if (currentFrameNumber == frameOnDamage)
                    onDamage?.Invoke();

                explosionAnimationTimer = 0f;
            }

            yield return null;
        }

        StartCoroutine(hideExplosionAnimationObject(hidingSpeed));
    }

    private void TimerLogic() {
        if (currentStatement == FireballAnimationStatement.Exploding)
            return;

        animationTimer += Time.deltaTime;
        if (animationTimer > delayBetweenFrames) {
            ChangeFlyingPartFrame();
            animationTimer = 0f;
        }
    }

    private IEnumerator hideExplosionAnimationObject(float speed) {
        while (explosionAnimationObject.GetComponent<SpriteRenderer>().color.a > 0) {
            explosionAnimationObject.GetComponent<SpriteRenderer>().color -= new Color(0, 0, 0,
                Mathf.Min(explosionAnimationObject.GetComponent<SpriteRenderer>().color.a, Time.deltaTime * speed));

            yield return null;
        }

        frames[frames.Count - 1].lightObject.SetActive(false);
        onEndAnimation?.Invoke();
        Destroy(this);
    }

    private void Update() {
        TimerLogic();
    }
}
