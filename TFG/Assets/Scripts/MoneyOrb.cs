using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyOrb : MonoBehaviour
{
    Rigidbody2D myRigidBody2D;
    // Start is called before the first frame update
    void Start()
    {
        myRigidBody2D = GetComponent<Rigidbody2D>();
        Player.FollowMe += OnFollowMe;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        Player.FollowMe -= OnFollowMe;
    }

    private void OnFollowMe(Vector3 position)
    {
        myRigidBody2D.velocity = new Vector3(0, 0, 0);
        Vector3 velocityToAdd = new Vector3(transform.position.x < position.x ? 2 : (transform.position.x > position.x ? -2 : 0), transform.position.y < position.y ? 2 : (transform.position.y > position.y ? -2 : 0), 0);
        myRigidBody2D.velocity = velocityToAdd;
    }
}
