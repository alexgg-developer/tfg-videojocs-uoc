using UnityEngine;
using System.Collections;
using System;

public class Caller : Animation
{
    Action actionFunction;
    public Action ActionFunction
    {
        set
        {
            actionFunction = value;
        }
    }

    
    public Caller(): base()
    {

    }

    // Update is called once per frame
    override public void Update()
    {
        actionFunction();
        hasStopped = true;
    }
}
