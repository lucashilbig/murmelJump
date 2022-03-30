using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform _target;
    public void SetTarget(Transform targetPlayer) => _target = targetPlayer;  

    private void LateUpdate()
    {
        if (GameManager.IsGamerOver()) return;
        
        if (_target.position.y > transform.position.y)
        {
            var newPosition = new Vector3(transform.position.x, _target.position.y, transform.position.z);
            transform.position = newPosition;
        }
            
    }

}
