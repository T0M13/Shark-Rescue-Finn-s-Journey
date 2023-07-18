using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadMe : MonoBehaviour
{
    [Header("In der Scene findest du einen Chunk-Floor, den du als \nReference f�r die Environment-Chunks nehmen kannst.\nSchaue dabei, dass die Chunks etwas vorne und hinten herausstechen,\ndamit es sich InGame smooth �berlappen kann ohne L�cken.")]
    [Space(5)]
    public bool AfterReadingClick;
    [Space(15)]
    [SerializeField] private GameObject readMeCanvas;


    private void OnValidate()
    {
        if(AfterReadingClick)
            readMeCanvas.SetActive(false);
        else
            readMeCanvas.SetActive(true);
    }
}
