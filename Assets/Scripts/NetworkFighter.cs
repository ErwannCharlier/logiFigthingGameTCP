using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkFighter : NetworkBehaviour
{
    [Header("Stats")]
    public static int maxHealth = 500;
    [SyncVar] public int Health = maxHealth;
    public int Damage = 25;

    [Header("Refs")]
    public Animator animator;
    public Collider weaponHitbox;

    [Header("Move")]
    public float forwardSpeed = 6f;
    public float backwardSpeed = 4.5f;
    public float acceleration = 25f;

    [Header("Turn")]
    public float turnSpeed = 220f;
    public float mouseSensitivity = 180f;
    public bool allowStrafeWithAlt = true;
    public float deplacementSpeed = 5.5f;

    float yaw;
    Vector3 currentMove;
    bool canHitThisSwing;

    [Server]
    public void Heal(int amount)
    {
        Health = Mathf.Min(Health + amount, maxHealth);
    }


    public override void OnStartLocalPlayer()
    {
        yaw = transform.eulerAngles.y;
        Application.runInBackground = true;
    }

    void Awake()
    {
        if (!animator) animator = GetComponent<Animator>();
        if (weaponHitbox) weaponHitbox.enabled = false;
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        float h = Input.GetAxisRaw("HorizontalPlayerOne");
        float v = Input.GetAxisRaw("VerticalPlayerOne");

        // ----- TURN (clavier + souris) -----
        float yawDelta = 0f;

        // Clavier:
        yawDelta += h * turnSpeed * Time.deltaTime;

        // Souris:
        float mouseX = Input.GetAxis("Mouse X");
        yawDelta += mouseX * mouseSensitivity * Time.deltaTime;

        yaw += yawDelta;
        Quaternion yawRot = Quaternion.Euler(0f, yaw, 0f);
        transform.rotation = yawRot;

        // ----- MOVE -----
        Vector3 input = new Vector3(h, 0f, v);

        //ne pas aller plus vite en diagonal
        input = Vector3.ClampMagnitude(input, 1f);

        Vector3 desiredMove = yawRot * input * deplacementSpeed;

        currentMove = Vector3.Lerp(currentMove, desiredMove, 1f - Mathf.Exp(-acceleration * Time.deltaTime));
        transform.position += currentMove * Time.deltaTime;

        animator.SetFloat("Input X", h);
        animator.SetFloat("Input Z", v);
        animator.SetBool("Moving", input.sqrMagnitude > 0.0001f);

        // ----- ATTACK -----
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger("Attack1Trigger");
            CmdPlayAttack();
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
    public void OnAnimHit()
    {
        if (!isLocalPlayer) return;

        canHitThisSwing = true;
        StartCoroutine(SwingWindow());
    }

    IEnumerator SwingWindow()
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
