using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogPlacement : MonoBehaviour
{
    private AudioSource river;
    private bool placed = false;
    private ItemDropNode node;
    private bool AlreadyThere = false;
    // Start is called before the first frame update
    void Start()
    {
        river = gameObject.GetComponent<AudioSource>();
        node = gameObject.GetComponent<ItemDropNode>();
        if (node.ActiveItem != null)
        {
            if (node.ActiveItem.name[0..3].ToString() == "Log")
            {
                AlreadyThere = true;
            }
        }
       
    }

    // Update is called once per frame
    void Update()
    {
        if (node.ActiveItem != null && !AlreadyThere)
        {
            if (node.ActiveItem.name[0..3].ToString() == "Log" && placed == false)
            {
                Debug.Log("log placed");
                river.Play();
                placed = true;
            }
        }
        if (AlreadyThere)
        {
            if (node.ActiveItem == null || node.ActiveItem.name[0..3].ToString() != "Log")
            {
                AlreadyThere = false;
            }
        }
    }
}
