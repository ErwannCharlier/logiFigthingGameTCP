using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WeaponHitbox : MonoBehaviour
{
    NetworkFighter owner;

    void Start()
    {
        owner = GetComponentInParent<NetworkFighter>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (owner == null) return;

        NetworkFighter target = other.GetComponentInParent<NetworkFighter>();
        if (target == null) return;
        if (target == owner) return;

        owner.TryHit(target);
    }
}
