using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour
{
    Rigidbody2D myRigidBody2D;
    Collider2D myCollider2D;
    Animator myAnimator;
    GameObject player;
    [SerializeField] GameObject arrowPrefab;
    [SerializeField] float playerSpeed = 5;
    [SerializeField] float jumpSpeed = 5;
    [SerializeField] float dashDistance = 8;
    [SerializeField] float maxDashTime = 50;
    [SerializeField] long dashCooldown = 180;
    [SerializeField] long attackCooldown = 180;
    [SerializeField] float playerHealth = 50;
    [SerializeField] AudioClip arrowSFX;
    [SerializeField] AudioClip dashSFX;
    [SerializeField] AudioClip jumpSFX;
    float jumpsRemaining = 1;

    long currentDashCooldown = 0;
    long currentAttackCooldown = 0;
    float currentDashTime = 0;
    float lastDirection = 0;
    // Start is called before the first frame update
    void Start()
    {
        myRigidBody2D = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myCollider2D = GetComponent<Collider2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        currentAttackCooldown = attackCooldown;
        currentDashCooldown = dashCooldown;
    }

    // Update is called once per frame
    void Update()
    {
        if (myCollider2D.IsTouchingLayers(LayerMask.GetMask("Ground")) && jumpsRemaining == 0)
            jumpsRemaining++;

        if(playerHealth <= 0) {
            SceneManager.LoadScene(2);
        }
        if (myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack") && myAnimator.GetCurrentAnimatorStateInfo(0).length < myAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime)
        {
            myAnimator.SetBool("Attack", false);
            
        }
        if (currentDashTime.Equals(0) && !myAnimator.GetBool("Dash") && !myAnimator.GetBool("Attack") && !myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack")) {
            player.layer = LayerMask.NameToLayer("Enemy");
            Jump();
            Run();
            FlipSprite();
            if (currentAttackCooldown.Equals(attackCooldown))
                Attack();
            if (currentDashCooldown == dashCooldown)
                Dash();
        }

        if (currentDashCooldown < dashCooldown && currentDashCooldown > 0)
            currentDashCooldown--;
        else if (currentDashCooldown == 0)
            currentDashCooldown = dashCooldown;

        if (currentAttackCooldown < attackCooldown && currentAttackCooldown > 0)
            currentAttackCooldown--;
        else if (currentAttackCooldown == 0)
            currentAttackCooldown = attackCooldown;

        if (currentDashTime > 0)
            currentDashTime--;
        else if (currentDashTime.Equals(0))
        {
            myAnimator.SetBool("Dash", false);
        }

    }

    private void Dash()
    {
        if (CrossPlatformInputManager.GetButtonDown("Dash"))
        {
            if (currentDashTime.Equals(0))
                currentDashTime = maxDashTime;
            player.layer = LayerMask.NameToLayer("Player");
            myAnimator.SetBool("Dash", true);
            AudioSource.PlayClipAtPoint(dashSFX, UnityEngine.Camera.main.transform.position, 15);
            Vector2 playerVelocity = new Vector2(0, 0);
            myRigidBody2D.velocity = playerVelocity;
            playerVelocity = new Vector2((lastDirection > 0 ? 1 : -1) * dashDistance, 0);
            myRigidBody2D.velocity = playerVelocity;
            currentDashCooldown--;
        }
    }

    private void Jump()
    {
        if (CrossPlatformInputManager.GetButtonDown("Jump"))
        {
            if (!myCollider2D.IsTouchingLayers(LayerMask.GetMask("Ground")))
            {
                if (jumpsRemaining == 0)
                    return;
                else
                {
                    Vector2 velocityToAdd = new Vector2(0, jumpSpeed);
                    myRigidBody2D.velocity += velocityToAdd;
                    AudioSource.PlayClipAtPoint(jumpSFX, UnityEngine.Camera.main.transform.position, 1);
                    jumpsRemaining--;
                }
            }
            else { 

                Vector2 velocityToAdd = new Vector2(0, jumpSpeed);
                myRigidBody2D.velocity += velocityToAdd;
                AudioSource.PlayClipAtPoint(jumpSFX, UnityEngine.Camera.main.transform.position, 1);
            }
        }

    }

    private void Run()
    {
        float controlThrow = CrossPlatformInputManager.GetAxis("Horizontal");
        if (!controlThrow.Equals(0))
            lastDirection = controlThrow;
        Vector2 playerVelocity = new Vector2(controlThrow * playerSpeed, myRigidBody2D.velocity.y);
        myRigidBody2D.velocity = playerVelocity;
        bool playerIsMovingHorizontaly = Mathf.Abs(myRigidBody2D.velocity.x) > Mathf.Epsilon;
        myAnimator.SetBool("Running", playerIsMovingHorizontaly);
    }

    private void Attack()
    {
        if (CrossPlatformInputManager.GetButtonDown("Fire1"))
        {
            Vector2 playerVelocity = new Vector2(0, myRigidBody2D.velocity.y);
            myRigidBody2D.velocity = playerVelocity;
            myAnimator.SetBool("Attack", true);
            currentAttackCooldown--;
            GameObject arrow = Instantiate(arrowPrefab, transform.position, Quaternion.identity) as GameObject;
            arrow.GetComponent<Rigidbody2D>().velocity = new Vector2((lastDirection > 0 ? 1 : -1) * arrow.GetComponent<Arrow>().arrowSpeed, 0f);
            arrow.GetComponent<Transform>().localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            AudioSource.PlayClipAtPoint(arrowSFX, UnityEngine.Camera.main.transform.position, 15);
        }
    }

    private void FlipSprite()
    {
        bool playerIsMovingHorizontaly = Mathf.Abs(myRigidBody2D.velocity.x) > Mathf.Epsilon;
        if (playerIsMovingHorizontaly)
        {
            transform.localScale = new Vector2(Mathf.Sign(myRigidBody2D.velocity.x), 1);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Enemy"))
        {
            playerHealth -= collision.gameObject.GetComponent<Enemy>().damage;
        }
    }
}

        

