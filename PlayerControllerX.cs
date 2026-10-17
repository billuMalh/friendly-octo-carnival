using UnityEngine;
using UnityEngine.InputSystem;

// Flies the plane using the new Input System.
// Left stick up/down    -> how fast the nose pitches up/down
// Left stick left/right -> plane speed - left slows down, right speeds up
public class PlayerControllerX : MonoBehaviour
{
    [Header("Nose Tilt (stick up/down)")]
    // Degrees per second the plane pitches at full stick deflection. 
    public float maxPitchSpeed = 60f;

    // 1 = linear
    [Range(1f, 3f)] public float pitchCurve = 2f;

    // Nose tilt inversion
    public bool invertPitch = false;

    [Header("Speed (stick left/right)")]
    // Speed with the stick held fully left.
    public float minSpeed = 5f;

    // Speed with the stick centered.
    public float cruiseSpeed = 15f;

    // Speed with the stick held fully right.
    public float maxSpeed = 30f;

    // How quickly the plane's actual speed catches up to the stick's speed (units/s^2).
    public float speedChangeRate = 20f;

    // Read by other scripts (SpinPropellerX!) to react to the plane's speed.
    public float CurrentSpeed { get; private set; }

    private float pitchInput;
    private float throttleInput;

    void Start()
    {
        CurrentSpeed = cruiseSpeed;
    }

    void Update()
    {
        Vector2 stick = ReadStick();
        throttleInput = stick.x;
        pitchInput = stick.y;
    }

    void FixedUpdate()
    {
        // --- Speed ---
        // Centered stick = cruise speed. Moving it right makes go toward max speed,
        // moving it left makes it go toward min speed.
        float targetSpeed;
        if (throttleInput >= 0f)
        {
            targetSpeed = Mathf.Lerp(cruiseSpeed, maxSpeed, throttleInput);
        }
        else
        {
            targetSpeed = Mathf.Lerp(cruiseSpeed, minSpeed, -throttleInput);
        }

        // Makes speed change feel smoother
        CurrentSpeed = Mathf.MoveTowards(CurrentSpeed, targetSpeed, speedChangeRate * Time.deltaTime);
        transform.Translate(Vector3.forward * CurrentSpeed * Time.deltaTime);

        // --- Pitch (nose tilt) ---
        // How far the stick is pushed sets how fast the nose moves.
        // The curve makes small stick movements give small, precise pitch changes.
        float shapedPitch = Mathf.Sign(pitchInput) * Mathf.Pow(Mathf.Abs(pitchInput), pitchCurve);
        if (invertPitch)
        {
            shapedPitch = -shapedPitch;
        }

        transform.Rotate(Vector3.right * maxPitchSpeed * shapedPitch * Time.deltaTime);
    }

    // Returns the stick position, each axis from -1 to 1.
    Vector2 ReadStick()
    {
        // The Input System already applies a dead zone to the stick, so tiny
        // movements near the center are ignored and the rest isn't.
        if (Gamepad.current != null)
        {
            Vector2 stick = Gamepad.current.leftStick.ReadValue();
            if (stick != Vector2.zero)
            {
                return stick;
            }
        }

        // For Keyboard: only gives -1, 0 or 1.
        Vector2 keys = Vector2.zero;
        Keyboard keyboard = Keyboard.current;
        if (keyboard != null)
        {
            if (keyboard.rightArrowKey.isPressed || keyboard.dKey.isPressed) keys.x += 1f;
            if (keyboard.leftArrowKey.isPressed || keyboard.aKey.isPressed) keys.x -= 1f;
            if (keyboard.upArrowKey.isPressed || keyboard.wKey.isPressed) keys.y += 1f;
            if (keyboard.downArrowKey.isPressed || keyboard.sKey.isPressed) keys.y -= 1f;
        }
        return keys;
    }
}