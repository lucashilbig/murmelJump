using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    public float leftBound; //left screen boundary in world space
    public float rightBound; //right screen boundary in world space
    public float bufferBounds; //buffer for player model, so it disappears completely
    public Rigidbody2D rb;

    private float moveX;

    private void Start()
    {
        leftBound *= SceneSelector.ScaleFactor();
        rightBound *= SceneSelector.ScaleFactor();
        bufferBounds *= SceneSelector.ScaleFactor();
    }

    void Update()
    {
        if (GameManager.IsGamerOver()) return;

        var inputHor = (Application.platform == RuntimePlatform.Android) ? Input.acceleration.x * 3f : Input.GetAxis("Horizontal");
        if (BuffSystem.Instance == null)
            moveX = inputHor * moveSpeed;
        else
            moveX = BuffSystem.Instance.IsMovespeedDebuffed() ? inputHor * (moveSpeed / 3f) : inputHor * moveSpeed;

        //change player orientation
        var newRotation = transform.rotation;
        newRotation.y = inputHor switch
        {
            < 0.1f => 0f,
            > 0.1f => 180f,
            _ => newRotation.y
        };
        transform.rotation = newRotation;
    }

    private void FixedUpdate()
    {
        if (GameManager.IsGamerOver()) return;

        //player movement horizontally
        Vector2 vel = rb.velocity;
        vel.x = moveX;
        rb.velocity = vel;

        //screen out of bounds check
        var position = transform.position;
        if (position.x < leftBound - bufferBounds)
            position.x = rightBound + bufferBounds;
        else if (position.x > rightBound + bufferBounds)
            position.x = leftBound - bufferBounds;
        transform.position = position;
    }
}