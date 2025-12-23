using System.Buffers;
using UnityEngine;

public class CampFireScript : MonoBehaviour
{
    MeshRenderer _renderer;
    Camera cam;

    void Start()
    {
        cam = Camera.main;
        _renderer = GetComponent<MeshRenderer>();
        _renderer.material.SetFloat("_TimeSpeed", Random.Range(0.3f, 0.7f));
        _renderer.material.SetFloat("_StepOffset", Random.Range(1.5f, 1.54f));

        Vector2 temp = new Vector2(0.5f, Random.Range(30f, 50f));
        _renderer.material.SetVector("_R_Pow_Mul", temp);
        _renderer.material.SetVector("_B_Pow_Mul", temp);

        transform.localScale = Vector3.one * Random.Range(0.005f, 0.010f);
    }

    void LateUpdate()
    {
        Vector3 camPos = cam.transform.position;

        camPos.y = transform.position.y;

        transform.LookAt(camPos);
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y + 180, 0);
    }
}
