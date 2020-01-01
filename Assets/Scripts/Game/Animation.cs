using UnityEngine;
using System.Collections;

public class Animation
{
    protected bool hasStopped = false;
    public bool HasStopped
    {
        get
        {
            return hasStopped;
        }
        set
        {
            hasStopped = value;
        }
    }

    protected GameObject node;

    public Animation()
    {

    }

    public Animation(GameObject givenNode)
    {
        node = givenNode;
    }

    virtual public void Update()
    {

    }
}
