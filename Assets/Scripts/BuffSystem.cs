using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GameManager))]
public class BuffSystem : MonoBehaviour
{
    private static BuffSystem _instance;
    public static BuffSystem Instance => _instance;


    public GameObject snowGenerator;
    private GameObject player;
    private bool isJumpBuffed;
    private float jumpBuffDuration;
    private int jumpBuffAmount;
    private bool isMovespeedDebuffed;
    private float movespeedDebuffDuration;

    public void Init(GameObject activePlayer)
    {
        player = activePlayer;
        ResetJumpBuff();
        ResetMovespeedDebuff();
    }
    
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }

    }
    

    // Update is called once per frame
    void Update()
    {
        if (isJumpBuffed)
        {
            jumpBuffDuration -= Time.deltaTime;
            if (jumpBuffDuration <= 0)
                ResetJumpBuff();
        }

        if (isMovespeedDebuffed)
        {
            movespeedDebuffDuration -= Time.deltaTime;
            if(movespeedDebuffDuration <= 0)
                ResetMovespeedDebuff();
        }
    }

    private void ResetJumpBuff()
    {
        isJumpBuffed = false;
        jumpBuffDuration = 0f;
        jumpBuffAmount = 0;
        
        //disable buff animations
        player.transform.Find("MurmelHatRed").gameObject.SetActive(false);
        player.transform.Find("MurmelHatGreen").gameObject.SetActive(false);
        snowGenerator.GetComponent<ParticleSystem>().Stop();
    }

    private void ResetMovespeedDebuff()
    {
        isMovespeedDebuffed = false;
        movespeedDebuffDuration = 0f;

        //disable debuff animation/gameobject
        player.transform.Find("MurmelRainbow").gameObject.SetActive(false);
    }

    private void SetBuffAnimation(BuffType type)
    {
        switch (type)
        {
            case BuffType.ChristmasBuffRed:
                player.transform.Find("MurmelHatRed").gameObject.SetActive(true);
                snowGenerator.GetComponent<ParticleSystem>().Play();
                break;
            case BuffType.ChristmasBuffGreen:
                player.transform.Find("MurmelHatGreen").gameObject.SetActive(true);
                snowGenerator.GetComponent<ParticleSystem>().Play();
                break;
            case BuffType.FoodDebuff:
                player.transform.Find("MurmelRainbow").gameObject.SetActive(true);
                break;
        }
    }

    /// <summary>
    /// Activates the player velocity jump buff for the given duration and jump bonus amount.
    /// If the buff is already active the duration will reset but wont add up.
    /// If a currantly active buff has lower amount, then it will be replaced with the new one.
    /// If the new amount is lower than current, the new buff will be ignored
    /// </summary>
    /// <param name="duration">Duration of the buff in seconds</param>
    /// <param name="amount">Amount of vertical velocity bonus the buff gives</param>
    /// <param name="type">type of the buff</param>
    public void ActivateJumpBuff(float duration, int amount, BuffType type)
    {
        if (amount < jumpBuffAmount) return;
        jumpBuffDuration = duration;
        jumpBuffAmount = amount;
        isJumpBuffed = true;
        SetBuffAnimation(type);
    }

    /// <summary>
    /// Sets the player's movespeed to half for the given duration.
    /// </summary>
    /// <param name="duration">Duration of the debuff in seconds</param>
    /// <param name="type">type of debuff</param>
    public void ActivateMovespeedDebuff(float duration, BuffType type)
    {
        movespeedDebuffDuration = duration;
        isMovespeedDebuffed = true;
        SetBuffAnimation(type);
    }

    public bool IsJumpBuffed() => isJumpBuffed;
    public int JumpBuffAmount() => jumpBuffAmount;
    
    public bool IsMovespeedDebuffed() => isMovespeedDebuffed;
}


public enum BuffType
{
    ChristmasBuffRed,//jump buff small
    ChristmasBuffGreen,//jump buff big
    FoodDebuff//movespeed debuff
}