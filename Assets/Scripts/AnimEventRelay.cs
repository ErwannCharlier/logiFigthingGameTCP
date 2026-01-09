using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimEventRelay : MonoBehaviour
{
    private NetworkFighter fighter;

    void Awake()
    {
        fighter = GetComponentInParent<NetworkFighter>();
    }

    public void Hit()
    {
        fighter?.OnAnimHit();
    }

    public void FootL() { }
    public void FootR() { }
}
