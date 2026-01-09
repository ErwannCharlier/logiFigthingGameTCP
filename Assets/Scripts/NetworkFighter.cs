using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;
using Mirror;

public class NetworkFighter : NetworkBehaviour
{
    [Header("Stats")]
    [SyncVar] public int Health = 500;
    public int Damage = 25;

    [Header("Refs")]
    public Animator animator;
    public Collider weaponHitbox; // Collider trigger sur l'arme

    [Header("Move")]
    public float moveSpeed = 6f;
    public float rotationSpeed = 10f;

    bool canHitThisSwing;

    void Awake()
    {
        if (!animator) animator = GetComponent<Animator>();
        if (weaponHitbox) weaponHitbox.enabled = false;
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        // Input simple WASD
        float h = 0f;
        float v = 0f;
        if (Input.GetKey(KeyCode.A)) h = -1f;
        if (Input.GetKey(KeyCode.D)) h = 1f;
        if (Input.GetKey(KeyCode.W)) v = 1f;
        if (Input.GetKey(KeyCode.S)) v = -1f;

        Vector3 dir = new Vector3(h, 0, v).normalized;

        animator.SetFloat("Input X", h);
        animator.SetFloat("Input Z", v);
        animator.SetBool("Moving", dir != Vector3.zero);

        if (dir != Vector3.zero)
        {
            transform.position += dir * moveSpeed * Time.deltaTime;

            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger("Attack1Trigger"); // instant sur toi
            CmdPlayAttack();                       // pour l'autre joueur
        }

    }

    [Command]
    void CmdPlayAttack()
    {
        RpcPlayAttack();
    }

    [ClientRpc]
    void RpcPlayAttack()
    {
        // évite de retrigger chez le joueur local 
        if (isLocalPlayer) return;
        animator.SetTrigger("Attack1Trigger");
    }



    [ClientRpc]
    void RpcStartSwing()
    {
        if (!isLocalPlayer) return;
        StartCoroutine(SwingWindow());
    }

    public void OnAnimHit()
    {
        if (!isLocalPlayer) return;

        canHitThisSwing = true;
        StartCoroutine(SwingWindow());
    }

    System.Collections.IEnumerator SwingWindow()
    {
        if (!weaponHitbox) yield break;
        weaponHitbox.enabled = true;
        yield return new WaitForSeconds(0.25f);
        weaponHitbox.enabled = false;
        canHitThisSwing = false;
    }

    public void TryHit(NetworkFighter other)
    {
        if (!isLocalPlayer) return;
        if (!canHitThisSwing) return;

        canHitThisSwing = false; // 1 hit max par swing
        CmdDealDamage(other.netId, Damage);
    }

    [Command]
    void CmdDealDamage(uint targetNetId, int amount)
    {
        if (!NetworkServer.spawned.TryGetValue(targetNetId, out NetworkIdentity id)) return;

        var target = id.GetComponent<NetworkFighter>();
        if (target == null) return;

        target.Health -= amount;
        if (target.Health <= 0)
        {
            target.Health = 0;
            target.gameObject.SetActive(false);
        }
    }
}
