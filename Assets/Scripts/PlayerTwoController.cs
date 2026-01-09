using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTwoController : Entity
{
    private Animator animator;
    private float rotationSpeed = 30;
    private Vector3 inputVec;
    private Vector3 targetDirection;

    public Slider slider;

    void Start()
    {
        SetPlayerTwo();
        animator = GetComponent<Animator>();
    }

    void SetPlayerTwo()
    {
        Name = "PlayerTwo";
        Health = 700;
        Damage = 50;
    }

    void Update()
    {
        float horizontal = Input.GetAxisRaw("HorizontalPlayerTwo");
        float vertical = Input.GetAxisRaw("VerticalPlayerTwo");
        inputVec = new Vector3(horizontal, 0, vertical);

        animator.SetFloat("Input X", horizontal);
        animator.SetFloat("Input Z", vertical);

        if (vertical != 0 || horizontal != 0)
        {
            animator.SetBool("Moving", true);
        }
        else
            animator.SetBool("Moving", false);

        if (Input.GetButtonDown("AttackPlayerTwo"))
        {
            animator.SetTrigger("Attack1Trigger");
            StartCoroutine(AnimPause(1.2f));
        }
        UpdateMovement();
    }

    IEnumerator AnimPause(float pauseTime)
    {
        yield return new WaitForSeconds(pauseTime);
    }

    void UpdateMovement()
    {
        GetCameraRelativeMovement();
        RotateTowardMovementDirection();
    }

    void GetCameraRelativeMovement()
    {
        Transform cameraTransform = Camera.main.transform;

        Vector3 forward = cameraTransform.TransformDirection(Vector3.forward);
        forward.y = 0;
        forward = forward.normalized;

        Vector3 right = new Vector3(forward.z, 0, -forward.x);

        float horizontal = Input.GetAxisRaw("HorizontalPlayerTwo");
        float vertical = Input.GetAxisRaw("VerticalPlayerTwo");

        targetDirection = horizontal * right + vertical * forward;
    }
	
	void RotateTowardMovementDirection()
    {
        if (inputVec != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetDirection), Time.deltaTime * rotationSpeed);
        }
    }

    void FootR() { }
    void FootL() { }
    void Hit() { }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.Equals("SwordL") || collision.gameObject.name.Equals("SwordR"))
        {
            Entity Enemy = collision.gameObject.GetComponentInParent<PlayerOneController>();
            Health -= Enemy.Damage;
            slider.value = Health;
        }
        if (Health <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains("Health"))
        {
            Destroy(other.gameObject);
            if (Health < 700)
            {
                Health += 100;
                slider.value = Health;
                if (Health > 700)
                    Health = 700;
            }
        }
        if (other.gameObject.name.Contains("Berserk"))
        {
            Destroy(other.gameObject);
            StartCoroutine(BoostTime(10f));
        }
    }

    IEnumerator BoostTime(float time)
    {
        int playerPrevDamage = Damage;
        Damage = playerPrevDamage * 2;
        yield return new WaitForSeconds(time);
        Damage = playerPrevDamage;
    }
}
