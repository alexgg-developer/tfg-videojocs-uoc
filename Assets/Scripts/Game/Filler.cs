using UnityEngine;
using System.Collections;
using System;

public class Filler : Animation
{
    private float duration, elapsed;

    public Filler(float givenDuration, GameObject node) : base(node)
    {
        duration = givenDuration;
        elapsed = 0.0f;
    }

    override public void Update()
    {
        hasStopped = elapsed >= duration;
        elapsed += Time.deltaTime;
    }
}
