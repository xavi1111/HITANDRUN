using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData : MonoBehaviour
{
    bool hasDoubleJump;
    bool hasDash;
    bool hasArrow;
    float health;
    float mana;
    float coins;

    public GameData()
    {
        hasDoubleJump = false;
        hasDash = false;
        hasArrow = false;
        health = 100.0f;
        mana = 100.0f;
        coins = 0.0f;
    }
}
