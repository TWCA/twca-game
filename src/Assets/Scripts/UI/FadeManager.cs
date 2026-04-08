using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
* Creates a fade tween effect
*/
public class FadeManager : MonoBehaviour
{
    [NonSerialized] public float FadeOutTime = 0.5f;
    [NonSerialized] public float FadeInTime = 0.5f;
    [NonSerialized] public float MaxAlpha = 1f;
    [NonSerialized] public float MinAlpha = 0f;
    private float alpha;
    private float targetAlpha;
    private float alphaChangeTime;

    public Color GetRGBAatTime(Color baseColor, float time) {
        alpha = Mathf.MoveTowards(alpha, targetAlpha, time / alphaChangeTime);
        baseColor.a = alpha;
        return baseColor;
    }

    public bool HasReachedTarget() {
        return Mathf.Approximately(alpha, targetAlpha);
    }

    public void SetAlpha(float newAlpha) {
        alpha = newAlpha;
    }

    public bool HasReachedMinAlpha() {
        
        return alpha < MinAlpha || Mathf.Approximately(alpha, MinAlpha);
    }

    public bool HasReachedMaxAlpha() {
        return alpha > MaxAlpha || Mathf.Approximately(alpha, MaxAlpha);
    }
    
    public void FadeOut() {
        targetAlpha = MaxAlpha;
        alphaChangeTime = FadeOutTime;
    }

    public void FadeIn() {
        targetAlpha = MinAlpha;
        alphaChangeTime = FadeInTime;
    }
}
