using UnityEngine;

public class SkyboxController : MonoBehaviour
{
    [SerializeField]
    private float rotationSpeed = 1f;   // 회전 속도
    [SerializeField]
    private float exposureMin = 0.5f;
    [SerializeField]
    private float exposureMax = 1.5f;
    [SerializeField]
    private float dayLength = 120f;      // 하루 길이 (초)

    Material skyboxMat;
    float time;

    void Start()
    {
        skyboxMat = RenderSettings.skybox;
    }

    void Update()
    {
        if (skyboxMat == null) return;

        time += Time.deltaTime;

        //Rotation
        float rotation = time * rotationSpeed;
        rotation = Mathf.Clamp(rotation , 0f, 360f);
        skyboxMat.SetFloat("_Rotation", rotation);

        //Exposure (사인파로 낮밤 표현)
        float t = Mathf.Sin(time / dayLength * Mathf.PI * 2f) * 0.5f + 0.5f;
        float exposure = Mathf.Lerp(exposureMin, exposureMax, t);
        skyboxMat.SetFloat("_Exposure", exposure);
    }
}
