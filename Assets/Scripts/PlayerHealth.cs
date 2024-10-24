using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : NetworkBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private Image healthBarFill;
    [SerializeField] private Combat combat;
    private float health;
    private bool died = false;

    public override void OnStartClient()
    {
        if (isLocalPlayer)
            health = playerMovement.PlayerData.maxHealth;

        UpdateUI();
    }

    public override void OnStartServer()
    {
        health = playerMovement.PlayerData.maxHealth;
    }

    [Server]
    public void TakeDamage(float amount, bool roundHealth = true)
    {
        if (died) return;

        if (combat.CanParry()) return;

        float damage = combat.isBlocking ? amount / 2 : amount;
        health -= damage;

        if (health <= 0)
        {
            died = true;
            health = 0;
        }

        RpcUpdateHealth(health, roundHealth);
    }

    [ClientRpc]
    private void RpcUpdateHealth(float health, bool roundHealth)
    {
        this.health = health;

        UpdateUI(roundHealth);
    }

    private void UpdateUI(bool roundHealth = true)
    {
        //updateglobalhealthUI

        if (isLocalPlayer)
        {
            healthBarFill.fillAmount = health / playerMovement.PlayerData.maxHealth;

            string healthString = (roundHealth ? Mathf.Round(health) : health).ToString();
            healthText.text = $"{healthString}/{playerMovement.PlayerData.maxHealth}";
        }
    }
}
