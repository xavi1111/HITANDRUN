using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    GameObject player = null;
    [SerializeField] float speed = 5f;
    [SerializeField] float cannonHealth = 20f;
    [SerializeField] int moneyOrbsDropped = 5;
    [SerializeField] GameObject cannonBallPrefab;
    [SerializeField] GameObject moneyOrbPrefab;
    bool canAttack = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null) 
        { 
            Vector3 direction =  player.transform.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, speed * Time.deltaTime);
            Attack();
        }

    }

    private void Attack()
    {
        if (canAttack)
        {
            GameObject cannonBall = Instantiate(cannonBallPrefab, transform.position, Quaternion.identity) as GameObject;
            cannonBall.GetComponent<Rigidbody2D>().velocity = new Vector2(player.GetComponent<Transform>().position.x - transform.position.x, player.GetComponent<Transform>().position.y - transform.position.y);
            StartCoroutine(AttackCooldown());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            player = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            player = null;
        }
    }

    private IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(3);
        canAttack = true;
    }
}
