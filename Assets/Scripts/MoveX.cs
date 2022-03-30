using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class MoveX : MonoBehaviour
{
    public float speed;
    
    private Vector3 leftBound;
    private Vector3 rightBound;
    
    void Awake()
    {
        Init();
    }

    private void FixedUpdate()
    {
        if (GameManager.IsGamerOver()) return;
        transform.position = Vector3.Lerp (leftBound, rightBound, (Mathf.Sin(speed * Time.time) + 1.0f) / 2.0f);
    }

    public void Init()
    {
        var leftX = Random.Range(-2f, 0.75f);
        leftBound = transform.position;
        leftBound.x = leftX*SceneSelector.ScaleFactor();
        var rightX = Random.Range(leftX+1.25f, 2f);
        rightBound = leftBound;
        rightBound.x = rightX*SceneSelector.ScaleFactor();
        speed = Random.Range(1f, 3f);
    }
}
