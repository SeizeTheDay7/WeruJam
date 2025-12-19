using UnityEngine;
using UnityEngine.Splines;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Unity.Mathematics;

public class SplineScaleBaker : MonoBehaviour
{
    [SerializeField] private SplineContainer splineContainer;

    [Button]
    public void BakeScaleIntoSplineAndResetScale()
    {
        if (splineContainer == null)
            splineContainer = GetComponent<SplineContainer>();

        if (splineContainer == null)
        {
            Debug.LogError("[SplineScaleBaker] SplineContainer를 찾지 못했습니다.");
            return;
        }

        Vector3 s = transform.localScale;

        // 스케일이 이미 1이면 아무것도 안 함
        if (Mathf.Approximately(s.x, 1f) && Mathf.Approximately(s.y, 1f) && Mathf.Approximately(s.z, 1f))
            return;

#if UNITY_EDITOR
        Undo.RecordObject(splineContainer, "Bake Scale Into Spline");
        Undo.RecordObject(transform, "Reset Scale");
#endif

        // localScale을 knot의 로컬 좌표계에 직접 곱해주면,
        // scale을 1로 바꿔도 월드에서 동일한 모양을 유지합니다.
        var splines = splineContainer.Splines;
        for (int si = 0; si < splines.Count; si++)
        {
            var spline = splines[si];
            int knotCount = spline.Count;

            for (int ki = 0; ki < knotCount; ki++)
            {
                BezierKnot knot = spline[ki];

                Vector3 pos = (Vector3)knot.Position;
                Vector3 tin = (Vector3)knot.TangentIn;
                Vector3 tout = (Vector3)knot.TangentOut;

                pos = Vector3.Scale(pos, s);
                tin = Vector3.Scale(tin, s);
                tout = Vector3.Scale(tout, s);

                knot.Position = (float3)pos;
                knot.TangentIn = (float3)tin;
                knot.TangentOut = (float3)tout;

                // knot.Rotation은 스케일 굽기와 무관하므로 그대로 둡니다.
                spline.SetKnot(ki, knot);
            }
        }

        // 마지막에 스케일 초기화
        transform.localScale = Vector3.one;

#if UNITY_EDITOR
        EditorUtility.SetDirty(splineContainer);
        EditorUtility.SetDirty(transform);
#endif
    }
}
