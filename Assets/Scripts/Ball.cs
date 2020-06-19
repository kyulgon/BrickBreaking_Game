using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public Paddle paddle;

    private void OnCollisionEnter2D(Collision2D col)
    {
        GameObject Col = col.gameObject;
        StartCoroutine(paddle.BallCollisionEnter2D(transform, GetComponent<Rigidbody2D>(), GetComponent<Ball>(), Col, Col.transform, Col.GetComponent<SpriteRenderer>(), Col.GetComponent<Animator>()));
       

    }
}
