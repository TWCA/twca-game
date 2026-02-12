using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
/*
* An "allowlist" ("whitelist") or a "denylist" ("blacklist") for items
* Ideally designed to be easily editable from the editor and integrated where needed
*/
public class AllowDenyList
{
    public bool Allow = true; // whether or not this is an "allowlist" ("whitelist") or a "denylist" ("blacklist") for items
    public List<GameObject> InteractionObjects;

    public AllowDenyList()
    {

    }

    /*
    * Determine if an item is allowed based on the allowdeny list prerequisites
    * Here is how it works because I will forget probably
    * Let T be "allow=true=1" or "deny=false=0"
    * Let C be if the item is in InteractionObjects
    * Let T ^ C be the decision
        T | C | T ^ C
        1 | 1 |   1
        1 | 0 |   0
        0 | 1 |   0
        0 | 0 |   1
    */
    public bool IsItemAllowed(string itemName) {
        if (InteractionObjects.Count == 0) {
            // Always return true if there is nothing in the filter
            return true;
        }

        // Allowlist = true, denylist = false
        return Allow == InteractionObjects.Exists(interactionObject => interactionObject.name == itemName);
    }
}
