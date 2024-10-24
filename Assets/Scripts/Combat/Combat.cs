using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Combat : NetworkBehaviour
{
    [SerializeField] private Cooldowns cooldowns;
    [SerializeField] private PlayerDashing playerDashing;
    private List<Weapon> weapons;
    private int weaponIndex = 0;
    private float blockTime;
    public bool isBlocking { get; private set; }
    private const string cooldownBlockName = "Blocking";
    private bool canCooldown = false;

    [Header("UI")]
    [SerializeField] private TMP_Text weaponText;

    private void Start()
    {
        weapons = GetComponentsInChildren<Weapon>().ToList();

        if (playerDashing)
            playerDashing.onDash += () => StartCoroutine(OnDashed());

        weaponText.text = weapons[weaponIndex].GetWeaponName();
    }

    private void Update()
    {
        if (!isLocalPlayer) return;

        Blocking();
    }

    private void Blocking() => CmdBlocking(CanBlock());

    private void CmdBlocking(bool blocking)
    {
        isBlocking = blocking;

        if (isBlocking)
        {
            canCooldown = true;
            blockTime += Time.deltaTime;
        }
        else
        {
            blockTime = 0f;

            if (!cooldowns.Cooldowned(cooldownBlockName) && canCooldown)
            {
                cooldowns.NewCooldown(cooldownBlockName, 0.5f);
                canCooldown = false;
            }
        }

        RpcBlocking(isBlocking);
    }

    private void RpcBlocking(bool blocking)
    {
        CurrentWeapon().animator.SetBool("Block", blocking);
    }

    private IEnumerator OnDashed()
    {
        foreach (var weapon in weapons)
            weapon.gameObject.SetActive(false);

        yield return new WaitUntil(() => !playerDashing.isDashing);

        CurrentWeapon().gameObject.SetActive(true);
    }

    [Client]
    public bool CanBlock()
    {
        if (!Input.GetMouseButton(1))
            return false;

        if (!CurrentWeapon().gameObject.activeInHierarchy)
            return false;

        if (CurrentWeapon().animator.GetCurrentAnimatorStateInfo(0).IsName("Equip"))
            return false;

        if (CurrentWeapon().IsAttacking())
            return false;

        if (cooldowns.Cooldowned(cooldownBlockName))
            return false;

        return true;
    }

    [Server]
    public bool CanParry() => isBlocking && blockTime < 0.5f;

    private Weapon CurrentWeapon() => weapons[weaponIndex];
}
