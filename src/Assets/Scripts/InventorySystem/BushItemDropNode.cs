using System;
using System.Collections;
using UnityEngine;

public class BushItemDropNode : ItemDropNode
{
    [SerializeField] private GameObject FireParticals;
    
    protected override void OnActiveChange(bool active)
    {
        FireParticals.SetActive(!active);
    }
}