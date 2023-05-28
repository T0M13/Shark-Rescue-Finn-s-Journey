using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private float laneSwitchForce = 2.5f;
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

    [Header("Speed Power Up")]
    [SerializeField] private float speedMultiplier = 1.5f;
    [SerializeField] private float speedPowerUpTime = 10f;
    [SerializeField] private GameObject speedPowerUpEffect;
    public Action OnSpeedPowerUp;


    private void Awake()
    {
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

        OnSpeedPowerUp += SpeedPowerUp;
    }

    private void OnDisable()
    {
        InputManager.instance.swipeDetector.OnSwipeUp -= SwipeUp;
        InputManager.instance.swipeDetector.OnSwipeDown -= SwipeDown;
        InputManager.instance.swipeDetector.OnSwipeLeft -= SwipeLeft;
        InputManager.instance.swipeDetector.OnSwipeRight -= SwipeRight;

        OnSpeedPowerUp -= SpeedPowerUp;
    }


    private void SwipeUp()
    {
        if (currentUndulate == Undulate.Center)
        {
            currentUndulate = Undulate.Up;
            StartCoroutine(RotatePlayer(-laneSwitchRotation, false));
        }
        if (currentUndulate == Undulate.Down)
        {
            currentUndulate = Undulate.Center;
            StartCoroutine(RotatePlayer(-laneSwitchRotation, false));
        }
    }

    private void SwipeDown()
    {
        if (currentUndulate == Undulate.Center)
        {
            currentUndulate = Undulate.Down;
            StartCoroutine(RotatePlayer(laneSwitchRotation, false));
        }
        if (currentUndulate == Undulate.Up)
        {
            currentUndulate = Undulate.Center;
            StartCoroutine(RotatePlayer(laneSwitchRotation, false));
        }
    }

    private void SwipeLeft()
    {
        if (currentLane == Lane.Middle)
        {
            currentLane = Lane.Left;
            StartCoroutine(RotatePlayer(-laneSwitchRotation, true));
        }
        if (currentLane == Lane.Right)
        {
            currentLane = Lane.Middle;
            StartCoroutine(RotatePlayer(-laneSwitchRotation, true));
        }
    }

    private void SwipeRight()
    {
        if (currentLane == Lane.Middle)
        {
            currentLane = Lane.Right;
            StartCoroutine(RotatePlayer(laneSwitchRotation, true));
        }
        if (currentLane == Lane.Left)
        {
            currentLane = Lane.Middle;
            StartCoroutine(RotatePlayer(laneSwitchRotation, true));
        }
    }

    private void GetStartPosition()
    {
        if (GameManager.instance != null)
            startPosition = GameManager.instance.StartPosition;
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

        //pos = startPosition + (laneXDistance * (float)currentLane) * Vector3.right + (laneYDistance * (float)currentUndulate) * Vector3.up;

        transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime * laneSwitchForce);

        // Rotation Issue when Diagonal Swipe + Normal Swipe Fix Test
        //if (transform.rotation.x != 0 || transform.rotation.y != 0)
        //{
        //     transform.rotation = Quaternion.Euler(0,0,0);
        //}
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
        StartCoroutine(RotatePlayerBack(0, horizontal));
    }

    private IEnumerator RotatePlayerBack(float targetAngle, bool horizontal)
    {
        Quaternion initialRotation = transform.rotation;
        Quaternion targetRotation;

        if (horizontal)
            targetRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, targetAngle, transform.rotation.eulerAngles.z);
        else
            targetRotation = Quaternion.Euler(targetAngle, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * rotationSpeed;
            transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, t);
            yield return null;
        }

        // Ensure precise alignment on the X and Z axes
        Vector3 euler = transform.rotation.eulerAngles;
        euler.x = 0f;
        euler.z = 0f;
        transform.rotation = Quaternion.Euler(euler);
    }




    //---

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
    private void SpeedPowerUp()
    {
        StartCoroutine(SpeedPowerUpIE());
    }

    /// <summary>
    /// Activates Power Up for Speed and after *speedPowerUpTime* it deactivates it
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpeedPowerUpIE()
    {
        laneSwitchForce = laneSwitchForce * speedMultiplier;
        if (speedPowerUpEffect != null)
            speedPowerUpEffect.SetActive(true);
        yield return new WaitForSeconds(speedPowerUpTime);
        laneSwitchForce = defaultLaneSwitchForce;
        if (speedPowerUpEffect != null)
            speedPowerUpEffect.SetActive(false);
    }


    #endregion

}

