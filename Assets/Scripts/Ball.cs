using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public Paddle paddle;
    float mag;
    GameObject Col;
    public Rigidbody2D Rg;
    public bool isMagnet;

    private void Update()
    {
        mag = Rg.velocity.magnitude; // 속력의 크기
        
        if (paddle.ballSpeed == 250)
        {
           if(mag < 4.7f && mag > 5.1f) paddle.BallAddForce(Rg);
        }
        else
        {
            if (mag < 5.7f && mag > 6) paddle.BallAddForce(Rg);
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        Col = col.gameObject;
        StartCoroutine(paddle.BallCollisionEnter2D(transform, GetComponent<Rigidbody2D>(), GetComponent<Ball>(), Col, Col.transform, Col.GetComponent<SpriteRenderer>(), Col.GetComponent<Animator>()));
       

    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        Col = col.gameObject;
        if (Col.CompareTag("TriggerBlock")) paddle.BlockBreak(Col, Col.transform, Col.GetComponent<Animator>());
    }
}
