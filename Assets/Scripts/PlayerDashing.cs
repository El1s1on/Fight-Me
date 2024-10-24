using System;
using System.Collections;
using UnityEngine;

public class PlayerDashing : MonoBehaviour
{
    [SerializeField] private Cooldowns cooldowns;
    [SerializeField] private PlayerCamera playerCamera;
    [SerializeField] private Combat combat;
    [SerializeField] private Animator dashAnimator;
    private PlayerMovement playerMovement;
    private AudioSource audioSource;
    public bool isDashing { get; private set; }
    public Action onDash;
    private const string cooldownDashName = "Dash";

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        audioSource = dashAnimator.GetComponent<AudioSource>();
    }

    void Update()
    {
        if (CanDash())
            StartCoroutine(DashCoroutine());
    }

    private IEnumerator DashCoroutine()
    {
        cooldowns.NewCooldown(cooldownDashName, playerMovement.PlayerData.dashCooldown + playerMovement.PlayerData.dashTime);

        isDashing = true;
        onDash?.Invoke();

        dashAnimator.SetTrigger(DashAnimString());
        
        audioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(playerMovement.PlayerData.dashingSFX[UnityEngine.Random.Range(0, playerMovement.PlayerData.dashingSFX.Count)]);
        
        playerCamera.ViewPunch(new Vector3(playerMovement.GetMoveVector().x, playerMovement.GetMoveVector().z,playerMovement.GetMoveVector().x) * 5f);

        yield return new WaitForSeconds(playerMovement.PlayerData.dashTime);
        isDashing = false;
    }

    private bool CanDash()
    {
        if (combat.CanBlock())
            return false;

        if (!Input.GetKey(KeyCode.LeftShift))
            return false;

        if (!playerMovement.IsMoving())
            return false;

        if (cooldowns.Cooldowned(cooldownDashName))
            return false;

        return true;
    }

    

    private string DashAnimString()
    {
        if (playerMovement.GetMoveVector().x > 0.1f)
            return "DashRight";

        if (playerMovement.GetMoveVector().x < -0.1f)
            return "DashLeft";

        return "";
    }
}
