using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TechnologyInfo;

public class TechnologyManager : MonoBehaviour
{    
    HashSet<TechnologyType> technologies = new HashSet<TechnologyType>();

    public void Add(TechnologyType technology)
    {
        technologies.Add(technology);
    }

    public bool HasTechnology(TechnologyType technology)
    {
        return technologies.Contains(technology);
    }
}
