using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CooldownItem : MonoBehaviour
{
    [SerializeField] private TMP_Text cooldownText;
    [SerializeField] private Image cooldownFill;
    private float targetFillAmount = 1f;

    public void UpdateCooldownUI(Cooldown cooldown)
    {
        cooldownText.text = cooldown.cooldownName;
        targetFillAmount = cooldown.cooldownRemainingTime / cooldown.cooldownTime;
    }

    private void Update()
    {
        cooldownFill.fillAmount = Mathf.Lerp(cooldownFill.fillAmount, targetFillAmount, 15f * Time.deltaTime);
    }
}
