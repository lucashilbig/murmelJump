using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffDebuffItem : MonoBehaviour
{
    public BuffType type;
    public float duration;
    public int amount;//positive values buff and negativ debuff the jumping velocity

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(type == BuffType.ChristmasBuffGreen || type == BuffType.ChristmasBuffRed)
            BuffSystem.Instance.ActivateJumpBuff(duration, amount, type);
        if(type == BuffType.FoodDebuff)
            BuffSystem.Instance.ActivateMovespeedDebuff(duration, type);
        Destroy(gameObject);
    }
}

