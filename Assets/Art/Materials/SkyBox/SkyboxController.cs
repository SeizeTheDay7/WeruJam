using UnityEngine;

public class SkyboxController : MonoBehaviour
{
    [SerializeField]
    private float rotationSpeed;   // 회전 속도
    [SerializeField]
    private float exposureMin;
    [SerializeField]
    private float exposureMax;
    [SerializeField]
    private float dayLength;      // 하루 길이 (초)

    Material skyboxMat;
    float time;

    void Start()
    {
        skyboxMat = RenderSettings.skybox;
        time = 0f;
    }

    void Update()
    {
        if (skyboxMat == null) return;

        time += Time.deltaTime;

        //Rotation
        float rotation = time * rotationSpeed;
        rotation = Mathf.Repeat(rotation , 360f);
        skyboxMat.SetFloat("_Rotation", rotation);

        //Exposure (사인파로 낮밤 표현)
        float t = Mathf.Sin(time / dayLength * Mathf.PI * 2f) * 0.5f + 0.5f;
        float exposure = Mathf.Lerp(exposureMin, exposureMax, t);
        skyboxMat.SetFloat("_Exposure", exposure);
    }
}
