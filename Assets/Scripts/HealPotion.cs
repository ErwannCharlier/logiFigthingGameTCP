using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class HealPotion : NetworkBehaviour
{
    public int healAmount = 100;

    [ServerCallback]
    void OnTriggerEnter(Collider other)
    {
        var fighter = other.GetComponentInParent<NetworkFighter>();
        if (fighter == null) return;

        fighter.Heal(healAmount);

        NetworkServer.Destroy(gameObject);
    }
}