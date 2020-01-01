using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Sequencer : Animation
{
    Queue<Animation> animations = new Queue<Animation>();

    public Sequencer() : base()
    {

    }

    public void Add(Animation animation)
    {
        animations.Enqueue(animation);
    }

    override public void Update()
    {
        if (animations.Count > 0) {
            var animation = animations.Peek();
            if (!animation.HasStopped) {
                animation.Update();
            }
            else {
                animations.Dequeue();
            }
        }
        else {
            hasStopped = true;
        }
    }
}
