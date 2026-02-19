using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiverManager : MonoBehaviour
{
    public static RiverManager Instance { get; private set; }
    public GameObject Water1;
    public GameObject Water2;
    public GameObject Water3;

    void Awake() {
        Instance = this;
    }

    public void BlockFirstBranch() { }
    public void BlockSecondBranch() { }

    private void RevealSecondBranch() { }
    private void RevealThirdBranch() { }

}
