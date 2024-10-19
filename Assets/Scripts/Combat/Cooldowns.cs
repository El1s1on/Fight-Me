using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Cooldown
{
    public string cooldownName;
    public float cooldownTime;
    public float cooldownRemainingTime;

    public CooldownItem cooldownItemClone;
    
    public Cooldown(float cooldownTime, string cooldownName = "Null")
    {
        this.cooldownTime = cooldownTime;
        this.cooldownName = cooldownName;

        cooldownRemainingTime = this.cooldownTime;
    }
}


public class Cooldowns : MonoBehaviour
{
    [SerializeField] private CooldownItem cooldownItem;
    [SerializeField] private float cooldownTick = 0.1f;
    private List<Cooldown> cooldowns = new List<Cooldown>();

    private void Start() => StartCoroutine(CooldownsTimer());

    private IEnumerator CooldownsTimer()
    {
        while (true)
        {
            yield return new WaitForSeconds(cooldownTick);

            if (cooldowns.Count == 0) continue;

            for (int i = cooldowns.Count - 1; i >= 0; i--)
            {
                Cooldown cooldown = cooldowns[i];
                cooldown.cooldownRemainingTime -= cooldownTick;
                cooldown.cooldownItemClone.UpdateCooldownUI(cooldown);

                if (cooldown.cooldownRemainingTime <= 0)
                {
                    cooldowns.RemoveAt(i);
                    Destroy(cooldown.cooldownItemClone.gameObject);
                }
            }
        }
    }

    public void NewCooldown(string cooldownName, float time)
    {
        if (Cooldowned(cooldownName)) return;

        Cooldown cooldown = new Cooldown(time, cooldownName);
        cooldown.cooldownItemClone = Instantiate(cooldownItem, transform);
        cooldown.cooldownItemClone.gameObject.name = cooldownName;

        cooldowns.Add(cooldown);

        cooldown.cooldownItemClone.UpdateCooldownUI(cooldown);
    }

    public bool Cooldowned(string cooldownName)
    {
        var result = cooldowns.FirstOrDefault(o => o.cooldownName == cooldownName);

        return result != null;
    }
}
