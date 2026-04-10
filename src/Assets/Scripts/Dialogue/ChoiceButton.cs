using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceButton : MonoBehaviour
{
    private Image image;
    private float growth = 0.0f;
    
    void Start()
    {
        transform.localScale = new Vector3(1, growth, 1);
    }

    // Update is called once per frame
    void Update()
    {
        growth = Mathf.MoveTowards(growth, 1, Time.deltaTime / 0.3f);
        growth = Mathf.Lerp(growth, 1, Time.deltaTime / 0.1f);
        transform.localScale = new Vector3(1, growth, 1);
    }
}
