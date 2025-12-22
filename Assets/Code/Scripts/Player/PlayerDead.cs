using Unity.Cinemachine;
using UnityEngine;
using System.Collections;

public class PlayerDead : MonoBehaviour
{
    [SerializeField] CinemachineCamera fpsCam;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] AudioSource scream;
    [SerializeField] GameObject HUD;
    [SerializeField] float blackoutMoment = 2f;
    [SerializeField] float endGameMoment = 5f;
    [SerializeField] GameObject blackoutCanvas;
    Player player;

    void Awake()
    {
        player = GetComponent<Player>();
    }

    void OnTriggerEnter(Collider hit)
    {
        int colMask = 1 << hit.gameObject.layer;
        if ((colMask & enemyLayer) != 0)
        {
            HUD.SetActive(false);
            player.Shutdown(hit.transform);
            EnemyManager.Instance.Shutdown();
            SoundManager.Instance.MuteAllExceptScream();
            StartCoroutine(CoGameEndSequence());
        }
    }

    private IEnumerator CoGameEndSequence()
    {
        scream.Play();
        yield return new WaitForSeconds(blackoutMoment);
        blackoutCanvas.SetActive(true);
        yield return new WaitForSeconds(endGameMoment);
        SceneManager.Instance.GoToDead();
    }
}