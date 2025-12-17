using UnityEngine;
using Unity.Cinemachine;
using System.Collections;
using UnityEngine.InputSystem;

public enum IMove
{
    Idle,
    Walk,
    Run,
}

public class PlayerMove : MonoBehaviour
{
    [Header("Component")]
    [SerializeField] CinemachineCamera player_vcam;
    [SerializeField] private CinemachineImpulseSource impulse_walk;
    [SerializeField] private CinemachineImpulseSource impulse_run;
    private CinemachineBasicMultiChannelPerlin noise;
    private CharacterController characterController;

    [Header("Parameter")]
    public float moveSpeed = 5f;
    [SerializeField] private float gravityMult = 3f;
    [SerializeField] private float jumpMult = 1f;
    [SerializeField] float runMult = 1.5f;
    [SerializeField] float walk_gap = 0.5f;
    [SerializeField] float run_gap = 0.25f;
    [SerializeField] float amplitudeGain_walk = 2f;
    [SerializeField] float frequencyGain_walk = 1.5f;
    [SerializeField] float amplitudeGain_run = 3f;
    [SerializeField] float frequencyGain_run = 2.2f;

    [Header("Audio")]
    [SerializeField] AudioClip[] footstepSounds;
    [SerializeField] private AudioSource footstep;

    [Header("Calculation")]
    private bool wait_nextFootstep = false;
    private float targetAmplitudeGain;
    private float targetFrequencyGain;
    private float gravity = -9.81f;
    private float verticalVelocity;

    [Header("Action")]
    InputAction moveAction;
    InputAction leftShiftAction;
    InputAction jumpAction;
    Vector3 direction = new();

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        noise = player_vcam.GetComponent<CinemachineBasicMultiChannelPerlin>();
        footstep = GetComponent<AudioSource>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        InputActionAsset actions = InputSystem.actions;
        moveAction = actions.FindAction("Move");
        jumpAction = actions.FindAction("Jump");
        leftShiftAction = actions.FindAction("LeftShift");
    }

    void OnEnable()
    {
        wait_nextFootstep = false;
        moveAction.Enable();
        jumpAction.Enable();
        leftShiftAction.Enable();
    }

    void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
        leftShiftAction.Disable();
    }

    void Update()
    {
        GetInput();
        AddCameraMove(direction);
        AddGravity();
        direction.y = verticalVelocity;

        CollisionFlags flags = characterController.Move(direction * moveSpeed * Time.deltaTime);
        if ((flags & CollisionFlags.Above) != 0 && verticalVelocity > 0)
            verticalVelocity = 0f; // 천장에 닿았으면 속도 제거
    }

    private void GetInput()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();
        direction = Vector3.zero;
        if (input.y > 0) direction += player_vcam.transform.forward;
        if (input.y < 0) direction += -player_vcam.transform.forward;
        if (input.x < 0) direction += -player_vcam.transform.right;
        if (input.x > 0) direction += player_vcam.transform.right;

        direction.y = 0f; // y축 성분 제거하여 정규화 크기 오염 방지
        direction.Normalize();
        if (leftShiftAction.IsPressed()) direction *= runMult;
    }

    private void AddGravity()
    {
        if (characterController.isGrounded)
        {
            if (jumpAction.IsPressed()) verticalVelocity = Mathf.Sqrt(-2f * gravity * jumpMult); // 점프
            else verticalVelocity = -2f;
        }
        else verticalVelocity += gravity * Time.deltaTime; // 땅에 붙어있는게 아니면 떨어져
    }

    private void AddCameraMove(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            // 뛰는거
            if (leftShiftAction.IsPressed())
            {
                targetAmplitudeGain = amplitudeGain_run;
                targetFrequencyGain = frequencyGain_run;
                if (!wait_nextFootstep && characterController.isGrounded)
                {
                    wait_nextFootstep = true;
                    StartCoroutine(GenerateRunImpulseWithDelay());
                }
            }
            // 걷는거
            else
            {
                targetAmplitudeGain = amplitudeGain_walk;
                targetFrequencyGain = frequencyGain_walk;
                if (!wait_nextFootstep && characterController.isGrounded)
                {
                    wait_nextFootstep = true;
                    StartCoroutine(GenerateWalkImpulseWithDelay());
                }
            }
        }
        // 가만히 있는거
        else
        {
            targetAmplitudeGain = 0.7f;
            targetFrequencyGain = 0.7f;
        }

        noise.AmplitudeGain = Mathf.Lerp(noise.AmplitudeGain, targetAmplitudeGain, Time.deltaTime * 5f);
        noise.FrequencyGain = Mathf.Lerp(noise.FrequencyGain, targetFrequencyGain, Time.deltaTime * 5f);
    }

    private IEnumerator GenerateWalkImpulseWithDelay()
    {
        impulse_walk.GenerateImpulse();
        PlayFootstepSound();
        yield return new WaitForSeconds(walk_gap);
        wait_nextFootstep = false;
    }

    private IEnumerator GenerateRunImpulseWithDelay()
    {
        impulse_run.GenerateImpulse();
        PlayFootstepSound();
        yield return new WaitForSeconds(run_gap);
        wait_nextFootstep = false;
    }

    private void PlayFootstepSound()
    {
        if (footstepSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, footstepSounds.Length);
            footstep.clip = footstepSounds[randomIndex];
            footstep.Play();
        }
    }

}