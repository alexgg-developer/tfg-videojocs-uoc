using UnityEngine;
using System.Collections;
using System;

public class MapResource
{
    public enum ResourceKind { NONE, CORN, FISH };

    private GameObject instance;
    public GameObject Instance
    {
        get
        {
            return instance;
        }

        set
        {
            instance = value;
        }
    }

    ResourceKind kind = ResourceKind.NONE;
    public ResourceKind Kind
    {
        get
        {
            return kind;
        }

        set
        {
            kind = value;
        }
    }

    public MapResource(ResourceKind givenKind = ResourceKind.NONE)
    {
        kind = givenKind;
    }

}
