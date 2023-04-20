using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    [SerializeField] private float MovingSpeed = 10;
    
    void Update()
    {
        gameObject.transform.position += new Vector3(MovingSpeed * Time.deltaTime, 0, 0);
    }



    private void OnTriggerEnter(Collider other)
    {
        
    }
}
