using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SceneLighting : MonoBehaviour
{
    [SerializeField] public Material paletteSwapMaterial;
    [Range(0, 1)] public float time;

    // Update is called once per frame
    void Update()
    {
        paletteSwapMaterial.SetFloat("_LightingTime", time);
    }
}
