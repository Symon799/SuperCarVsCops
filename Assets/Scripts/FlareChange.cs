using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlareChange : MonoBehaviour
{
    public GameObject flareBlue;
    public GameObject flareRed;

    private Vector3 correctRotation;

    private float nextTime = 0.0f;
    public float interval = 1f;
    // Start is called before the first frame update
    void Start()
    {
        correctRotation = new Vector3(0f, 90f, 0f);
        
    }

    // Update is called once per frame
    void Update()
    {
        flareRed.transform.eulerAngles = correctRotation;
        flareBlue.transform.eulerAngles = correctRotation;
        if (Time.time >= nextTime) {
            //do something here every interval seconds
            flareBlue.SetActive(!flareBlue.activeSelf);
            flareRed.SetActive(!flareRed.activeSelf);
            nextTime += interval;
         }
    }
}
