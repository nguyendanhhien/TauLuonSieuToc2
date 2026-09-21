using UnityEngine;
using UnityEditor;
using UnityEngine.Splines;
using Unity.Mathematics;

[CustomEditor(typeof(TrackPath))]
public class TrackPathEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        TrackPath track = (TrackPath)target;

        GUILayout.Space(10);
        if (GUILayout.Button("1. Xóa toàn bộ điểm đã vẽ"))
        {
            Undo.RecordObject(track, "Clear Path");
            track.ClearPath();
            EditorUtility.SetDirty(track);
        }

        GUILayout.Space(10);
        if (GUILayout.Button("2. Tạo Spline Container (Hỗ trợ lộn ngược)"))
        {
            CreateSplineContainer(track);
        }

        GUILayout.Space(10);
        EditorGUILayout.HelpBox("Lưu ý khi vẽ vòng Loop (lộn ngược): Hãy chắc chắn bạn luôn click vào đúng MẶT TRÊN của thanh ray, kể cả khi thanh ray đó đang bị lật úp xuống đất.", MessageType.Warning);
    }

    private void CreateSplineContainer(TrackPath track)
    {
        if (track.trackPoints.Count < 2)
        {
            Debug.LogWarning("Cần ít nhất 2 điểm để tạo Spline!");
            return;
        }

        GameObject splineGO = new GameObject("Spline_DuongRayTuDong");
        SplineContainer container = splineGO.AddComponent<SplineContainer>();
        Spline spline = container.Spline;
        spline.Clear();

        for (int i = 0; i < track.trackPoints.Count; i++)
        {
            Vector3 pos = track.trackPoints[i].position;
            Vector3 up = track.trackPoints[i].normal; // Lấy hướng lên từ bề mặt lưới

            // Tính hướng nhìn về phía trước (Forward)
            Vector3 forward = Vector3.forward;
            if (i < track.trackPoints.Count - 1)
                forward = (track.trackPoints[i + 1].position - pos).normalized;
            else if (i > 0)
                forward = (pos - track.trackPoints[i - 1].position).normalized;

            // MỚI: Ép Spline xoay đúng theo hướng ngửa/úp của đường ray
            Quaternion rot = Quaternion.LookRotation(forward, up);

            // Thêm điểm vào spline kèm theo góc xoay (rot)
            spline.Add(new BezierKnot(new float3(pos.x, pos.y, pos.z), 0, 0, rot));
        }

        // Làm mượt đường cong
        for (int i = 0; i < spline.Count; i++)
        {
            spline.SetTangentMode(i, TangentMode.AutoSmooth);
        }

        Undo.RegisterCreatedObjectUndo(splineGO, "Create Spline Container");
        Selection.activeGameObject = splineGO;
        Debug.Log("Đã tạo Spline 3D thành công!");
    }

    private void OnSceneGUI()
    {
        TrackPath track = (TrackPath)target;
        Event e = Event.current;

        if (e.type == EventType.MouseDown && e.button == 0 && e.shift)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.gameObject == track.gameObject)
                {
                    Undo.RecordObject(track, "Add Waypoint");

                    // MỚI: Thu thập cả vị trí VÀ pháp tuyến bề mặt (normal)
                    track.trackPoints.Add(new TrackPoint
                    {
                        position = hit.point,
                        normal = hit.normal
                    });

                    EditorUtility.SetDirty(track);
                    e.Use();
                }
            }
        }
    }
}