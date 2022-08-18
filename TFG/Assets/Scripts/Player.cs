using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour, IDataPersistence
{
    public static event Action<Vector3> FollowMe;
    Rigidbody2D myRigidBody2D;
    Collider2D myCollider2D;
    Animator myAnimator;
    GameObject player;
    Vector3 previousPosition;
    SpriteRenderer mySpriteRenderer;
    [SerializeField] float followDistance;
    [SerializeField] HealthBar healthBar;
    [SerializeField] ManaBar manaBar;
    [SerializeField] GameObject arrowPrefab;
    [SerializeField] GameObject slashPrefab;
    [SerializeField] float playerSpeed = 5;
    [SerializeField] float jumpSpeed = 5;
    [SerializeField] float dashDistance = 8;
    [SerializeField] float maxDashTime = 50;
    [SerializeField] long dashCooldown = 180;
    [SerializeField] long attackCooldown = 180;
    [SerializeField] float playerHealth = 50;
    [SerializeField] float playerMana = 100;
    [SerializeField] float healManaCost = 0.01f;
    [SerializeField] float healPerFrame = 0.01f;
    [SerializeField] public float timeToColor = 2;
    float arrowManaCost = 20;
    float manaGain = 10;
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
        healthBar.setMaxHealth(playerHealth);
        healthBar.setCurrentHealth(playerHealth);
        manaBar.setMaxMana(playerMana);
        manaBar.setCurrentMana(playerMana);
        mySpriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (FollowMe != null)
        {
            FollowMe.Invoke(transform.position);
        }

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
            Heal();
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

    private void Heal()
    {
        if (CrossPlatformInputManager.GetButtonDown("Fire2") || CrossPlatformInputManager.GetButton("Fire2"))
        {
            if (manaBar.getCurrentMana() >= (healManaCost * Time.deltaTime) && healthBar.getCurrentHealth() < healthBar.getMaxHealth())
            {
                manaBar.setCurrentMana(manaBar.getCurrentMana() - (healManaCost * Time.deltaTime));
                healthBar.setCurrentHealth(healthBar.getCurrentHealth() + (healPerFrame * Time.deltaTime));
            }
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
                    myRigidBody2D.velocity = new Vector2(myRigidBody2D.velocity.x, 0);
                    Vector2 velocityToAdd = new Vector2(0, jumpSpeed);
                    myRigidBody2D.velocity += velocityToAdd;
                    AudioSource.PlayClipAtPoint(jumpSFX, UnityEngine.Camera.main.transform.position, 1);
                    jumpsRemaining--;
                }
            }
            else {
                myRigidBody2D.velocity = new Vector2(myRigidBody2D.velocity.x, 0);
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
                float controlThrow = CrossPlatformInputManager.GetAxis("Vertical");
                GameObject slash = Instantiate(slashPrefab, transform.position + new Vector3(controlThrow.Equals(0) ? (lastDirection > 0 ? 1 : -1) * 0.5f : 0, controlThrow.Equals(0) ? 0 : (controlThrow > 0 ? 1 : -1) * 0.5f, 0), Quaternion.identity);
                slash.GetComponent<Transform>().localScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
                AudioSource.PlayClipAtPoint(arrowSFX, UnityEngine.Camera.main.transform.position, 15);
                if (manaBar.getCurrentMana() < manaBar.getMaxMana() && (manaBar.getCurrentMana() + manaGain) <= manaBar.getMaxMana())
                    manaBar.setCurrentMana(manaBar.getCurrentMana() + manaGain);
                else if (manaBar.getCurrentMana() < manaBar.getMaxMana() && (manaBar.getCurrentMana() + manaGain) >= manaBar.getMaxMana())
                    manaBar.setCurrentMana(manaBar.getMaxMana());
        }
    }

    private void AttackArrow()
    {
        if (CrossPlatformInputManager.GetButtonDown("Fire1"))
        {
            if (manaBar.getCurrentMana() >= arrowManaCost)
            {
                Vector2 playerVelocity = new Vector2(0, myRigidBody2D.velocity.y);
                myRigidBody2D.velocity = playerVelocity;
                myAnimator.SetBool("Attack", true);
                currentAttackCooldown--;
                GameObject arrow = Instantiate(arrowPrefab, transform.position, Quaternion.identity) as GameObject;
                arrow.GetComponent<Rigidbody2D>().velocity = new Vector2((lastDirection > 0 ? 1 : -1) * arrow.GetComponent<Arrow>().arrowSpeed, 0f);
                arrow.GetComponent<Transform>().localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                AudioSource.PlayClipAtPoint(arrowSFX, UnityEngine.Camera.main.transform.position, 15);
                manaBar.setCurrentMana(manaBar.getCurrentMana() - arrowManaCost);
            }

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
            if (!mySpriteRenderer.color.Equals(new Color(1, 0, 0, 1)))
            {
                playerHealth -= collision.gameObject.GetComponent<Enemy>().damage;
                healthBar.setCurrentHealth(playerHealth);
                StartCoroutine(DamageAnimation());
            }
        }
        if (collision.gameObject.tag.Equals("CannonBall"))
        {
            if (!mySpriteRenderer.color.Equals(new Color(1, 0, 0, 1)))
            {
                playerHealth -= collision.gameObject.GetComponent<CannonBall>().damage;
                healthBar.setCurrentHealth(playerHealth);
                StartCoroutine(DamageAnimation());
            }
        }
        if (collision.gameObject.tag.Equals("HealthIncreaseOrb"))
        {
            playerHealth = healthBar.getMaxHealth() + 50;
            healthBar.setMaxHealth(playerHealth);
            healthBar.setCurrentHealth(playerHealth);
        }
        if (collision.gameObject.tag.Equals("ManaIncreaseOrb"))
        {
            playerMana = manaBar.getMaxMana() + 50;
            manaBar.setMaxMana(playerHealth);
            manaBar.setCurrentMana(playerHealth);
        }

        if (collision.gameObject.tag.Equals("Wall"))
        {
            myRigidBody2D.velocity = new Vector2(myRigidBody2D.velocity.x, 0);
        }
    }
    private IEnumerator DamageAnimation()
    {
        Color currentColor = mySpriteRenderer.color;
        mySpriteRenderer.color = new Color(1, 0, 0, 1);
        yield return new WaitForSeconds(timeToColor);
        mySpriteRenderer.color = currentColor;
    }

    public void LoadData(GameData data)
    {
        throw new NotImplementedException();
    }

    public void SaveData(ref GameData data)
    {
        throw new NotImplementedException();
    }
}

        

