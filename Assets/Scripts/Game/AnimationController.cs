using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimationController : MonoBehaviour
{
    LinkedList<Animation> animations = new LinkedList<Animation>();
    public bool IsAnimationOn
    {
        get
        {
            return animations.Count > 0;
        }
    }

    public void Add(Animation animation)
    {
        animations.AddLast(animation);
    }

    // Update is called once per frame
    void Update()
    {
        var animationNode = animations.First;
        while (animationNode != null) {
            var nextNode = animationNode.Next;
            if (!animationNode.Value.HasStopped) {
                animationNode.Value.Update();
            }
            else {
                animations.Remove(animationNode);
            }
            animationNode = nextNode;
        }
    }
}
