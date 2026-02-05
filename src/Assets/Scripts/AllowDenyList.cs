using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AllowDenyList
{
    public bool Allow = true;
    public List<InteractionObject> InteractionObjects;

    public AllowDenyList()
    {

    }
}
