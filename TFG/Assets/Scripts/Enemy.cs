using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Rigidbody2D myRigidBody2D;
    CapsuleCollider2D myCollider2D;
    SpriteRenderer mySpriteRenderer;
    Animator myAnimator;
    [SerializeField] float enemySpeed = 1;
    [SerializeField] float enemyHealth = 100;
    [SerializeField] public float damage = 20;
    [SerializeField] public float timeToColor = 2;
    [SerializeField] GameObject moneyOrbPrefab;
    [SerializeField] int moneyOrbsDropped = 3;
    // Start is called before the first frame update
    void Start()
    {
        myRigidBody2D = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myCollider2D = GetComponent<CapsuleCollider2D>();
        mySpriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyHealth <= 0)
        {
            Destroy(gameObject);
            for (int i = 0; i < moneyOrbsDropped; i++) 
            { 
                GameObject moneyOrb = Instantiate(moneyOrbPrefab, transform.position, Quaternion.identity);
                moneyOrb.GetComponent<Transform>().position= new Vector2(transform.position.x + UnityEngine.Random.Range(-1.0f, 1.0f) , 0f);
            }
        }
        if (enemyMovingRight())
            myRigidBody2D.velocity = new Vector2(enemySpeed, 0);
        else
            myRigidBody2D.velocity = new Vector2(-enemySpeed, 0);
    }


    bool enemyMovingRight()
    {
        return transform.localScale.x > 0;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!(collision.gameObject.tag.Equals("Enemy") || collision.gameObject.tag.Equals("MoneyOrb") || collision.gameObject.tag.Equals("Cannon")))
        {
            transform.localScale = new Vector2(3 * -(Mathf.Sign(myRigidBody2D.velocity.x)), 3f);
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Arrow"))
        {
            enemyHealth -= collision.gameObject.GetComponent<Arrow>().damage;
            Destroy(collision.gameObject);
            StartCoroutine(DamageAnimation());
        }
        if (collision.gameObject.tag.Equals("Slash"))
        {
            enemyHealth -= collision.gameObject.GetComponent<Slash>().damage;
            StartCoroutine(DamageAnimation());
        }
        if (collision.gameObject.tag.Equals("Enemy"))
        {
            transform.localScale = new Vector2(3 * -(Mathf.Sign(myRigidBody2D.velocity.x)), 3f);
        }
    }

    private IEnumerator DamageAnimation()
    {
        Color currentColor = mySpriteRenderer.color;
        mySpriteRenderer.color = new Color(1, 0, 0, 1);
        yield return new WaitForSeconds(timeToColor);
        mySpriteRenderer.color = currentColor;
    }
}
