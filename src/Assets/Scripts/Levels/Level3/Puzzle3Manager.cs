using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puzzle3Manager : MonoBehaviour
{
    public ItemDropNode FillUpPointInitial;
    public ItemDropNode FillUpPointFilled;
    public TimePortal TimePortal;

    // Start is called before the first frame update
    void Start()
    {
        TimeManager.Instance.onTimeChanged += HandleTimeChanged;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void HandleTimeChanged() {
        if (FillUpPointInitial.ActiveItem != null) {
            FillUpPointFilled.gameObject.SetActive(true);
            FillUpPointInitial.gameObject.SetActive(false);
            TimePortal.gameObject.SetActive(false);
        }
    }
}
