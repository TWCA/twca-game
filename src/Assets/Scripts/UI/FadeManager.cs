using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Creates a fade tween effect
*/
public class FadeManager : MonoBehaviour
{
    [NonSerialized] public float FadeOutTime;
    [NonSerialized] public float FadeInTime;
    private float alpha;
    private float targetAlpha;
    private float alphaChangeTime;

    public Color GetRGBAatTime(Color baseColor, float time) {
        alpha = Mathf.MoveTowards(alpha, targetAlpha, time / alphaChangeTime);
        baseColor.a = alpha;
        return baseColor;
    }

    public bool HasReachedTarget() {
        return alpha.Equals(targetAlpha);
    }

    public void SetAlpha(float newAlpha) {
        alpha = newAlpha;
    }
    
    public void FadeOut() {
        targetAlpha = 1;
        alphaChangeTime = FadeOutTime;
    }

    public void FadeIn() {
        targetAlpha = 0;
        alphaChangeTime = FadeInTime;
    }
}
