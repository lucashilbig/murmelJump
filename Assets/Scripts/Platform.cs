using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public float jumpForce;
    public PlatformType type;

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!(other.relativeVelocity.y <= 0f)) return;
        var rb = other.gameObject.GetComponent<Rigidbody2D>();
        if (rb == null) return;
        var vel = rb.velocity;
        if (BuffSystem.Instance == null)
            vel.y = jumpForce;
        else
            vel.y = (BuffSystem.Instance.IsJumpBuffed()) ? jumpForce + BuffSystem.Instance.JumpBuffAmount() : jumpForce;
        rb.velocity = vel;
        if (type != PlatformType.Breaking) return;
        var animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.Play("Breaking");//animation event will disable gameObject
        }
    }

    public void SetInactive() => gameObject.SetActive(false);//used in "Breaking" animation Event
    
    public static PlatformType GetRandomPlatformtype()
    {
        var rnd = UnityEngine.Random.Range(0, 100);
        if(rnd <= 50)
            return PlatformType.Default;
        if(rnd > 50 && rnd <= 80)
            return PlatformType.Moving;
        if(rnd > 80)
            return PlatformType.Breaking;
        
        return PlatformType.Default;     
    }
}

public enum PlatformType
{
    Default,
    Moving,
    Breaking
}