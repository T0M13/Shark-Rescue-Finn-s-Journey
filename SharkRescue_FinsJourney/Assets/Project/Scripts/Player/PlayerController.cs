using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private float laneSwitchForce = 2.5f;
    [SerializeField] private float interpolationFactor = 2f;
    [SerializeField] private float defaultLaneSwitchForce = 2.5f;
    [SerializeField] private float laneSwitchRotation = 10f;
    [SerializeField] private float rotationSpeed = 5f;
    //[SerializeField] private float defaultRotationSpeed = 5f;
    [SerializeField] private Lane currentLane = Lane.Middle;
    [SerializeField] private Undulate currentUndulate = Undulate.Center;
    [Header("Lane Settings")]
    [SerializeField] private float laneXDistance = 4f;
    [SerializeField] private float laneYDistance = 3f;
    [SerializeField] private int lanes = 3;
    [SerializeField] private FocusedDirection direction;
    [Header("Gizmos")]
    [SerializeField] private bool showGizmos = true;
    [Header("Variables")]
    private Vector3 pos;
    private Quaternion rotation;
    private Vector3 right;
    private Vector3 up;
    [SerializeField] private PlayerReferences playerRef;
    private bool vibrated;

    private Coroutine coroutine;
    private Coroutine vibrate;

    [Header("Star Power Up")]
    [SerializeField] private float starSpeedMultiplier = 1.5f;
    [SerializeField] private float starPowerUpTime = 10f;
    [SerializeField] private GameObject starPowerUpEffect;
    public Action OnStarPowerUp;
    private Coroutine starPowerUp;


    private void Awake()
    {
        playerRef = GetComponent<PlayerReferences>();
        GetStartPosition();
        currentLane = Lane.Middle;
        currentUndulate = Undulate.Center;
    }

    private void OnEnable()
    {
        InputManager.instance.swipeDetector.OnSwipeUp += SwipeUp;
        InputManager.instance.swipeDetector.OnSwipeDown += SwipeDown;
        InputManager.instance.swipeDetector.OnSwipeLeft += SwipeLeft;
        InputManager.instance.swipeDetector.OnSwipeRight += SwipeRight;

        OnStarPowerUp += StarPowerUp;
    }

    private void OnDisable()
    {
        InputManager.instance.swipeDetector.OnSwipeUp -= SwipeUp;
        InputManager.instance.swipeDetector.OnSwipeDown -= SwipeDown;
        InputManager.instance.swipeDetector.OnSwipeLeft -= SwipeLeft;
        InputManager.instance.swipeDetector.OnSwipeRight -= SwipeRight;

        OnStarPowerUp -= StarPowerUp;
    }


    private void SwipeUp()
    {
        if (currentUndulate == Undulate.Up)
        {
            if (!vibrated)
                vibrate = StartCoroutine(VibrateOnNotAllowed());
        }

        if (currentUndulate == Undulate.Center)
        {
            currentUndulate = Undulate.Up;

            if (AudioManager.instance)
                AudioManager.instance.Play("swipe");

            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            coroutine = StartCoroutine(RotatePlayer(-laneSwitchRotation, false));
        }
        if (currentUndulate == Undulate.Down)
        {
            currentUndulate = Undulate.Center;

            if (AudioManager.instance)
                AudioManager.instance.Play("swipe");

            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            coroutine = StartCoroutine(RotatePlayer(-laneSwitchRotation, false));
        }


    }

    private void SwipeDown()
    {
        if (currentUndulate == Undulate.Down)
        {
            if (!vibrated)
                vibrate = StartCoroutine(VibrateOnNotAllowed());
        }

        if (currentUndulate == Undulate.Center)
        {
            currentUndulate = Undulate.Down;

            if (AudioManager.instance)
                AudioManager.instance.Play("swipe");

            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            coroutine = StartCoroutine(RotatePlayer(laneSwitchRotation, false));
        }
        if (currentUndulate == Undulate.Up)
        {
            currentUndulate = Undulate.Center;

            if (AudioManager.instance)
                AudioManager.instance.Play("swipe");

            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            coroutine = StartCoroutine(RotatePlayer(laneSwitchRotation, false));
        }

    }

    private void SwipeLeft()
    {
        if (currentLane == Lane.Left)
        {
            if (!vibrated)
                vibrate = StartCoroutine(VibrateOnNotAllowed());
        }

        if (currentLane == Lane.Middle)
        {
            currentLane = Lane.Left;

            if (AudioManager.instance)
                AudioManager.instance.Play("swipeCL");

            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            coroutine = StartCoroutine(RotatePlayer(-laneSwitchRotation, true));
        }
        if (currentLane == Lane.Right)
        {
            currentLane = Lane.Middle;

            if (AudioManager.instance)
                AudioManager.instance.Play("swipeRC");

            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            coroutine = StartCoroutine(RotatePlayer(-laneSwitchRotation, true));
        }
    }

    private void SwipeRight()
    {
        if (currentLane == Lane.Right)
        {
            if (!vibrated)
                vibrate = StartCoroutine(VibrateOnNotAllowed());
        }

        if (currentLane == Lane.Middle)
        {
            currentLane = Lane.Right;

            if (AudioManager.instance)
                AudioManager.instance.Play("swipeCR");

            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            coroutine = StartCoroutine(RotatePlayer(laneSwitchRotation, true));
        }
        if (currentLane == Lane.Left)
        {
            currentLane = Lane.Middle;

            if (AudioManager.instance)
                AudioManager.instance.Play("swipeLC");

            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            coroutine = StartCoroutine(RotatePlayer(laneSwitchRotation, true));
        }
    }


    private IEnumerator VibrateOnNotAllowed()
    {
        vibrated = true;
        yield return new WaitForSeconds(.4f);
        //playerRef.PlayerAnimator.SetTrigger("DamageTrigger");
        Handheld.Vibrate();
        vibrated = false;
    }

    private void GetStartPosition()
    {
        if (GameManager.Instance != null)
            startPosition = GameManager.Instance.StartPosition;
        else
            startPosition = transform.position;
    }


    private void Update()
    {
        pos = transform.position;

        if (direction == FocusedDirection.XAxis)
            pos = startPosition + (laneXDistance * (float)currentLane) * Vector3.forward + (laneYDistance * (float)currentUndulate) * Vector3.up;
        if (direction == FocusedDirection.ZAxis)
            pos = startPosition + (laneXDistance * (float)currentLane) * Vector3.right + (laneYDistance * (float)currentUndulate) * Vector3.up;


        transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime * laneSwitchForce * interpolationFactor);

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * rotationSpeed);

    }


    //---- Lane Switch Animation
    private IEnumerator RotatePlayer(float targetAngle, bool horizontal)
    {
        Quaternion initialRotation = transform.rotation;
        Quaternion targetRotation;

        if (horizontal)
            targetRotation = Quaternion.AngleAxis(targetAngle, Vector3.up) * initialRotation;
        else
            targetRotation = Quaternion.AngleAxis(targetAngle, Vector3.right) * initialRotation;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * rotationSpeed;
            transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, t);
            yield return null;
        }

        transform.rotation = targetRotation;

    }

    private void OnDrawGizmos()
    {
        if (!showGizmos) return;
        Gizmos.color = Color.green;

        for (int i = 0; i < lanes; i++)
        {
            for (int j = 0; j < lanes; j++)
            {
                Vector3 pos = Vector3.one;
                if (direction == FocusedDirection.XAxis)
                    pos = startPosition + (-laneXDistance + laneXDistance * j) * Vector3.forward + (-laneYDistance + laneYDistance * i) * Vector3.up;
                if (direction == FocusedDirection.ZAxis)
                    pos = startPosition + (-laneXDistance + laneXDistance * j) * Vector3.right + (-laneYDistance + laneYDistance * i) * Vector3.up;
                Gizmos.DrawSphere(pos, 0.2f);

            }
        }

    }


    #region Speed Power Up

    /// <summary>
    /// Starts the Coroutine Speed Power Up
    /// </summary>
    private void StarPowerUp()
    {
        starPowerUp = StartCoroutine(StarPowerUpIE());
    }

    /// <summary>
    /// Activates Power Up for Speed and after *speedPowerUpTime* it deactivates it
    /// </summary>
    /// <returns></returns>
    private IEnumerator StarPowerUpIE()
    {
        laneSwitchForce = laneSwitchForce * starSpeedMultiplier;
        if (starPowerUpEffect != null)
            starPowerUpEffect.SetActive(true);
        yield return new WaitForSeconds(starPowerUpTime);
        laneSwitchForce = defaultLaneSwitchForce;
        GameManager.Instance.ResetStar();
        if (starPowerUpEffect != null)
            starPowerUpEffect.SetActive(false);
    }


    #endregion

}

