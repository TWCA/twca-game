using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogPlacement : MonoBehaviour
{
    private AudioSource river;
    private ItemDropNode node;
    private bool AlreadyThere = false;
    // Start is called before the first frame update
    void Start()
    {
        river = gameObject.GetComponent<AudioSource>();
        node = gameObject.GetComponent<ItemDropNode>();

        if (node.ActiveItem != null)
        {
            if (node.ActiveItem != null)
            {
                AlreadyThere = true;
            }
        }

        node.ItemPlaced += OnPlaced;
        node.ItemRemoved += OnRemoved;
    }

    void OnPlaced() {
        if (!AlreadyThere) {
            river.Play();
        }
    }

    void OnRemoved() {
        if (AlreadyThere) {
            AlreadyThere = false;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
