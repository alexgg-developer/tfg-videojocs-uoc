using UnityEngine;
using System.Collections;
using System;

public class MoveBy : Animation
{
    private	float duration, elapsed;
    private Vector3 movement;
    private Vector3 startPosition, previousPosition;

    public MoveBy(Vector3 givenMovement, float givenDuration, GameObject node): base(node)
    {
        movement = givenMovement;
        duration = givenDuration;
        elapsed = 0.0f;
        previousPosition = startPosition = node.transform.position;
    }

    override public void Update()
    {
        hasStopped = elapsed >= duration;
        float uniformT = elapsed / duration;
        uniformT = Math.Min(uniformT, 1.0f);
        var currentPos = node.transform.position;
        var diff = currentPos - previousPosition;
        startPosition = startPosition + diff;
        var newPosition = startPosition + (movement * uniformT);
        node.transform.position = newPosition;
        previousPosition = newPosition;

        elapsed += Time.deltaTime;
    }
}
