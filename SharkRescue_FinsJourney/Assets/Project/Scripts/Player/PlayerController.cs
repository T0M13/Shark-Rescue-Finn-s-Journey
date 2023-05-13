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
    [SerializeField] private float defaultRotationSpeed = 5f;
    [SerializeField] private Lane currentLane = Lane.Middle;
    [SerializeField] private Undulate currentUndulate = Undulate.Center;
    [Header("Lane Settings")]
    [SerializeField] private float laneXDistance = 4f;
    [SerializeField] private float laneYDistance = 3f;
    [SerializeField] private int lanes = 3;
    [Header("Lane Settings")]
    [SerializeField] private bool showGizmos = true;
    [Header("Variables")]
    private Vector3 pos;
    private Quaternion rotation;
    private Vector3 right;
    private Vector3 up;
    [SerializeField] private float powerUpTime = 10f;


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
    }

    private void OnDisable()
    {
        InputManager.instance.swipeDetector.OnSwipeUp -= SwipeUp;
        InputManager.instance.swipeDetector.OnSwipeDown -= SwipeDown;
        InputManager.instance.swipeDetector.OnSwipeLeft -= SwipeLeft;
        InputManager.instance.swipeDetector.OnSwipeRight -= SwipeRight;
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
        rotation = transform.rotation;
        right = rotation * Vector3.right;
        up = rotation * Vector3.up;

        pos = startPosition + (laneXDistance * (float)currentLane) * right + (laneYDistance * (float)currentUndulate) * up;

        transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime * laneSwitchForce);
    }


    //---- Lane Switch Animation
    private IEnumerator RotatePlayer(float targetAngle, bool horizontal)
    {
        Quaternion initialRotation = transform.rotation;
        Quaternion targetRotation = new Quaternion();
        if (horizontal)
            targetRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, targetAngle, transform.rotation.eulerAngles.z);
        else
            targetRotation = Quaternion.Euler(targetAngle, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);


        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * rotationSpeed;
            transform.rotation = Quaternion.Lerp(initialRotation, targetRotation, t);
            yield return null;
        }

        transform.rotation = targetRotation;
        StartCoroutine(RotatePlayerBack(0, horizontal));
    }

    private IEnumerator RotatePlayerBack(float targetAngle, bool horizontal)
    {
        Quaternion initialRotation = transform.rotation;
        Quaternion targetRotation = new Quaternion();
        if (horizontal)
             targetRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, targetAngle, transform.rotation.eulerAngles.z);
        else
             targetRotation = Quaternion.Euler(targetAngle, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * rotationSpeed;
            transform.rotation = Quaternion.Lerp(initialRotation, targetRotation, t);
            yield return null;
        }

        transform.rotation = targetRotation;
    }
    //---

    private void OnDrawGizmos()
    {
        if (!showGizmos) return;
        Gizmos.color = Color.green;

        if (!Application.isPlaying)
        {
            rotation = transform.rotation;
            right = rotation * Vector3.right;
            up = rotation * Vector3.up;
        }

        for (int i = 0; i < lanes; i++)
        {
            for (int j = 0; j < lanes; j++)
            {
                Vector3 pos = startPosition + (-laneXDistance + laneXDistance * j) * right + (-laneYDistance + laneYDistance * i) * up;
                Gizmos.DrawSphere(pos, 0.2f);
            }
        }

    }

}

