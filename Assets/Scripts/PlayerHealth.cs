using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : NetworkBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private Image healthBarFill;
    private float health;

    public override void OnStartClient()
    {
        if (isLocalPlayer)
        {
            health = playerMovement.PlayerData.maxHealth;
        }
    }

    public override void OnStartServer()
    {
        health = playerMovement.PlayerData.maxHealth;
    }

    [Server]
    public void TakeDamage(float amount, bool roundHealth = true)
    {
        health -= amount;

        if (health <= 0)
        {
            health = 0;
        }

        RpcUpdateHealth(health, roundHealth);
    }

    [ClientRpc]
    private void RpcUpdateHealth(float health, bool roundHealth)
    {
        this.health = health;

        //updateglobalhealthUI

        if (isLocalPlayer)
        {
            healthBarFill.fillAmount = health / playerMovement.PlayerData.maxHealth;

            string healthString = (roundHealth ? Mathf.Round(health) : health).ToString();
            healthText.text = $"{healthString}/{playerMovement.PlayerData.maxHealth}";
            //updatelocalhealthUI
        }
    }
}
