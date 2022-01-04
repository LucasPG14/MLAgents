using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playAnimation : MonoBehaviour
{
    public Animation anim;
    public AnimationClip[] clips;
    // Start is called before the first frame update
    void Start()
    {
        anim.clip = anim.GetClip("Anim_TrapCutter_Play");
        anim.Play();
    }
}
