using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Combat : MonoBehaviour
{
    [SerializeField] private PlayerDashing playerDashing;
    private List<Weapon> weapons;
    private int weaponIndex = 0;

    [Header("UI")]
    [SerializeField] private TMP_Text weaponText;

    private void Start()
    {
        weapons = GetComponentsInChildren<Weapon>().ToList();

        if (playerDashing)
            playerDashing.onDash += () => StartCoroutine(OnDashed());

        weaponText.text = weapons[weaponIndex].GetWeaponName();
    }

    void Update()
    {

    }

    private IEnumerator OnDashed()
    {
        foreach (var weapon in weapons)
            weapon.gameObject.SetActive(false);

        yield return new WaitUntil(() => !playerDashing.isDashing);

        weapons[weaponIndex].gameObject.SetActive(true);
    }
}
