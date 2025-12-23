using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Collections;

public class EndingCutscene : MonoBehaviour
{
    [SerializeField] PlayableDirector director;
    [SerializeField] GameObject[] clearList;
    [SerializeField] AudioSource[] audios;
    [SerializeField] BoidSpawner spawner;
    [SerializeField] EnemyManager enemyManager;
    [SerializeField] UIManager ui;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            StartCoroutine(CoEndingSequence());
        }
    }

    private IEnumerator CoEndingSequence()
    {
        // 필요없는거 전부 제거
        spawner.Clear();
        enemyManager.Clear();
        ui.enabled = false;
        foreach (var s in audios)
        {
            s.Stop();
        }
        foreach (var go in clearList)
        {
            Destroy(go);
        }

        // 타임라인 재생 후 endingscene으로 넘어간다
        director.Play();
        yield return new WaitForSeconds((float)director.duration);
        SceneManager.Instance.GoToEnding();
    }
}
