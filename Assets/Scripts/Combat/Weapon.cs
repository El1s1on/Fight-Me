using Mirror;
using System.Collections.Generic;
using UnityEngine;
using Drawing;
using System.Collections;

[System.Serializable]
public class Combo
{
    public float hitTime;
    public float damage;
}

public class Weapon : NetworkBehaviour
{
    [SerializeField] private Cooldowns cooldowns;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private string weaponName;
    [SerializeField] private List<Combo> combos;
    [SerializeField] private Transform hitboxSpawnPoint;
    [SerializeField] private float hitboxRadius;
    [SerializeField] private float comboCooldown;
    [SerializeField] private Combat combat;
    public Animator animator;
    private int comboIndex = 0;
    private bool attacking = false;
    private const string cooldownComboName = "Melee";

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable() => attacking = false;

    [Client]
    private void Update()
    {
        WeaponInput();
    }

    [Client]
    private void WeaponInput()
    {
        if (Input.GetMouseButton(0) && combos.Count != 0 && CurrentAnimationIsIdle() && !attacking && !cooldowns.Cooldowned(cooldownComboName) && !combat.CanBlock())
        {
            Attack();
        }
    }

    [Client]
    public virtual void Attack() => StartCoroutine(AttackCoroutine());

    private IEnumerator AttackCoroutine()
    {
        attacking = true;

        Combo combo = combos[comboIndex];
        animator.SetInteger("ComboCount", combos.IndexOf(combo));
        animator.SetTrigger("Combo");

        yield return new WaitForSeconds(combos[comboIndex].hitTime);
        CmdAttack();

        comboIndex++;

        if(comboIndex > combos.Count - 1)
        {
            comboIndex = 0;
            cooldowns.NewCooldown(cooldownComboName, comboCooldown);
        }

        attacking = false;
    }

    [Command]
    public virtual void CmdAttack()
    {
        List<PlayerHealth> victims = AllVictimsInRange();

        foreach (PlayerHealth health in victims)
        {
            health.TakeDamage(combos[comboIndex].damage);
        }
    }

    private List<PlayerHealth> AllVictimsInRange()
    {
        Collider[] hitColliders = Physics.OverlapSphere(hitboxSpawnPoint.position, hitboxRadius);

        using (Draw.ingame.WithColor(Color.red))
        {
            using (Draw.ingame.WithDuration(1))
            {
                Draw.ingame.WireSphere(hitboxSpawnPoint.position, hitboxRadius);
            }
        }

        List<PlayerHealth> victimsInRange = new List<PlayerHealth>();

        foreach (var hit in hitColliders)
        {
            if (!hit.gameObject.CompareTag("Player") || hit.gameObject == playerMovement.gameObject)
                continue;

            victimsInRange.Add(hit.GetComponent<PlayerHealth>());
        }

        return victimsInRange;
    }

    public string GetWeaponName() => weaponName;

    private bool CurrentAnimationIsIdle()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("Idle"))
            return true;
        else
            return false;
    }

    public bool IsAttacking() => attacking;
}
