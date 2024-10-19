using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/Player/PlayerData", order = 1)]
public class PlayerData : ScriptableObject
{
    public float runSpeed = 12f;
    public float walkSpeed = 8f;

    [Header("Dashing")]
    public float dashCooldown = 1.25f;
    public float dashSpeed = 17.5f;
    public float dashTime = 0.3f;

    [Header("Health")]
    public float maxHealth = 100f;

    [Header("Audio")]
    public List<AudioClip> dashingSFX;
}
