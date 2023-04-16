using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Lanes")]
    [SerializeField] private Transform leftLaneTransform;
    [SerializeField] private Transform middleLaneTransform;
    [SerializeField] private Transform rightLaneTransform;
    [Header("Settings")]
    [SerializeField] private Transform currentLane;

    private void Awake()
    {
        currentLane = middleLaneTransform;
    }


}
