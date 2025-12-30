using UnityEngine;
using Unity.Cinemachine;
using System.Collections;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    [Header("Component")]
    [SerializeField] Transform playerBody;
    [SerializeField] CinemachineCamera player_vcam;
    [SerializeField] private CinemachineImpulseSource impulse_walk;
    [SerializeField] private CinemachineImpulseSource impulse_run;
    private CinemachineBasicMultiChannelPerlin noise;
    private CharacterController characterController;

    [Header("Speed")]
    public float moveSpeed = 5f;
    [SerializeField] float jumpMult = 1f;
    [SerializeField] float runMult = 1.5f;

    [Header("Sound")]
    [SerializeField] float walk_gap = 0.5f;
    [SerializeField] float run_gap = 0.25f;
    [SerializeField] AudioClip[] footstepSounds;
    [SerializeField] private AudioSource footstep;

    [Header("Camera")]
    [SerializeField] float fov_Die = 30f;
    [SerializeField] float amplitudeGain_walk = 2f;
    [SerializeField] float frequencyGain_walk = 1.5f;
    [SerializeField] float amplitudeGain_run = 3f;
    [SerializeField] float frequencyGain_run = 2.2f;
    [SerializeField] float amplitudeGain_Die = 4f;
    [SerializeField] float frequencyGain_Die = 10f;

    [Header("Action")]
    InputAction moveAction;
    InputAction leftShiftAction;
    InputAction jumpAction;

    [Header("Calculation")]
    private bool wait_nextFootstep = false;
    private float targetAmplitudeGain;
    private float targetFrequencyGain;
    private float gravity = -9.81f;
    private float verticalVelocity;
    Vector3 direction = new();
    bool isDead = false;
    Vector3 targetPos;

    public void Shutdown(Enemy enemy)
    {
        isDead = true;

        enemy.transform.LookAt(transform);

        player_vcam.Lens.FieldOfView = fov_Die;
        player_vcam.GetComponent<CinemachinePanTilt>().enabled = false;
        var perlin = player_vcam.GetComponent<CinemachineBasicMultiChannelPerlin>();
        perlin.AmplitudeGain = amplitudeGain_Die;
        perlin.FrequencyGain = frequencyGain_Die;

        targetPos = enemy.transform.position + enemy.agent.height * Vector3.up * 2 / 3;
    }

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
        if (isDead)
        {
            LerpLookAtTarget();
            SyncPlayerBody();
            return;
        }
        SyncPlayerBody();
        GetInput();
        AddCameraMove(direction);
        AddGravity();
        direction.y = verticalVelocity;

        CollisionFlags flags = characterController.Move(direction * moveSpeed * Time.deltaTime);
        if ((flags & CollisionFlags.Above) != 0 && verticalVelocity > 0)
            verticalVelocity = 0f; // 천장에 닿았으면 속도 제거
    }

    private void LerpLookAtTarget()
    {
        // player_vacm을 회전
        Vector3 directionToTarget = targetPos - player_vcam.transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        player_vcam.transform.rotation = Quaternion.Slerp(player_vcam.transform.rotation, targetRotation, Time.deltaTime * 20);

    }

    private void SyncPlayerBody()
    {
        Quaternion vcamRotation = Quaternion.Euler(0f, player_vcam.transform.eulerAngles.y, 0f);
        playerBody.rotation = vcamRotation;
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