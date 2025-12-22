using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] Player player;
    [SerializeField] GameObject escCanvas;
    [SerializeField] Slider bgmSlider;
    [SerializeField] Slider sfxSlider;
    InputAction escAction;

    protected override void OnAwake()
    {
        float bgmVolume = PlayerPrefs.GetFloat("bgmVolume", 1f);
        bgmSlider.value = bgmVolume;

        float sfxVolume = PlayerPrefs.GetFloat("sfxVolume", 1f);
        sfxSlider.value = sfxVolume;

        escAction = InputSystem.actions.FindAction("ESC");
    }

    void OnEnable()
    {
        escAction.Enable();
        escAction.performed += OnClickESC;
    }

    void OnDisable()
    {
        escAction.Disable();
        escAction.performed -= OnClickESC;
    }

    public void OnClickESC(InputAction.CallbackContext context)
    {
        if (player.isDead)
        {
            escCanvas.SetActive(false);
            Time.timeScale = 1f;
            return;
        }
        escCanvas.SetActive(!escCanvas.activeSelf);
        Time.timeScale = escCanvas.activeSelf ? 0f : 1f;
        Cursor.visible = escCanvas.activeSelf;
        Cursor.lockState = escCanvas.activeSelf ? CursorLockMode.None : CursorLockMode.Locked;
    }

    public void OnClickContinue()
    {
        OnClickESC(new InputAction.CallbackContext());
    }
}
