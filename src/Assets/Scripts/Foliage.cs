using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


[ExecuteInEditMode]
public class Foliage : MonoBehaviour
{
    public Sprite[] sprites;
    
    /*
     * Pick a "random" sprite based on the instanceID
     */
    private void Awake()
    {
        if (sprites.Length > 0)
        {
            int restoringSeed = Random.Range(int.MinValue, int.MaxValue);
            
            Random.InitState(GetInstanceID());
            int index = Random.Range(0, sprites.Length);
            
            GetComponent<SpriteRenderer>().sprite = sprites[index];
            
            Random.InitState(restoringSeed);
        }
    }
}