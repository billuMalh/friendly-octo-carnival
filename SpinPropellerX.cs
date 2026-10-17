using UnityEngine;

// Spins the propeller.
// If a PlayerControllerX is found on a parent, the propeller spins faster or slower based on the plane's speed. Otherwise, it spins at a constant speed.
public class SpinPropellerX : MonoBehaviour
{
    // Spin speed (degrees per second) for each unit of plane speed.
    public float spinPerUnitSpeed = 60f;

    // Spin speed used if no PlayerControllerX is found.
    public float backupSpinSpeed = 1000f;

    private PlayerControllerX player;

    void Start()
    {
        player = GetComponentInParent<PlayerControllerX>();
    }

    void Update()
    {
        float spinSpeed = player != null ? player.CurrentSpeed * spinPerUnitSpeed : backupSpinSpeed;
        transform.Rotate(Vector3.forward * spinSpeed * Time.deltaTime);
    }
}