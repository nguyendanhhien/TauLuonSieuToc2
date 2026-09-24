using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.Splines;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class KnotEditorViewer : MonoBehaviour
{
    [Header("Kéo CartTarget chứa xe vào đây")]
    public CinemachineSplineCart cart;

    [Header("Kéo Spline_DuongRayTuDong vào đây")]
    public SplineContainer trackSpline;

    [Header("Kéo VR_Seat chứa Camera vào đây")]
    public Transform vrSeat;

    [Header("Đang xem Knot số:")]
    public int currentKnotIndex = 0;

    [Header("Độ nhạy nghiêng Camera")]
    public float rollSpeed = 2f;

#if UNITY_EDITOR
    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (cart == null || trackSpline == null || trackSpline.Spline == null) return;

        Event e = Event.current;

        if (e.type == EventType.KeyDown)
        {
            if (e.keyCode == KeyCode.UpArrow) { JumpKnot(1); e.Use(); }
            else if (e.keyCode == KeyCode.DownArrow) { JumpKnot(-1); e.Use(); }
            else if (e.keyCode == KeyCode.LeftArrow) { RollCamera(rollSpeed); e.Use(); }
            else if (e.keyCode == KeyCode.RightArrow) { RollCamera(-rollSpeed); e.Use(); }
        }

        Handles.BeginGUI();
        GUIStyle style = new GUIStyle();
        style.fontSize = 16; style.fontStyle = FontStyle.Bold; style.normal.textColor = Color.green;
        GUI.Label(new Rect(10, 10, 800, 30), $"▶ KNOT: {currentKnotIndex} | [LÊN/XUỐNG] Đổi Knot | [TRÁI/PHẢI] Nghiêng Camera ◀", style);
        Handles.EndGUI();

        // --- GỌI HÀM VẼ VÔ LĂNG ẢO ---
        DrawKnotRollVisualizer();
    }

    // Hàm vẽ hiển thị độ nghiêng trực quan
    // Hàm vẽ hiển thị độ nghiêng trực quan (Đã nâng cấp)
    private void DrawKnotRollVisualizer()
    {
        var spline = trackSpline.Spline;
        if (currentKnotIndex < 0 || currentKnotIndex >= spline.Count) return;

        BezierKnot knot = spline[currentKnotIndex];

        Vector3 worldPos = trackSpline.transform.TransformPoint((Vector3)knot.Position);
        Quaternion worldRot = trackSpline.transform.rotation * knot.Rotation;

        // Lệnh này ép Unity vẽ mũi tên đè lên trên mọi vật thể, không bị đường ray che khuất
        Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;

        float arrowSize = 6f; // Kích thước mũi tên

        // Vẽ mũi tên 3D màu Hồng chỉ hướng đỉnh đầu (Up)
        Handles.color = Color.magenta;
        Handles.ArrowHandleCap(0, worldPos, Quaternion.LookRotation(worldRot * Vector3.up), arrowSize, EventType.Repaint);

        // Vẽ mũi tên 3D màu Xanh lơ chỉ sang cánh phải (Right)
        Handles.color = Color.cyan;
        Handles.ArrowHandleCap(0, worldPos, Quaternion.LookRotation(worldRot * Vector3.right), arrowSize, EventType.Repaint);

        // Chấm tâm hình tròn màu Vàng ở giữa
        Handles.color = Color.yellow;
        Handles.DrawSolidDisc(worldPos, worldRot * Vector3.forward, 0.5f);

        HandleUtility.Repaint();
    }
#endif

    public void JumpKnot(int step)
    {
        var spline = trackSpline.Spline;
        currentKnotIndex += step;

        if (currentKnotIndex >= spline.Count) currentKnotIndex = 0;
        if (currentKnotIndex < 0) currentKnotIndex = spline.Count - 1;

        SyncCamera(); // Chỉ cần gọi hàm ép Camera, bỏ qua chiếc xe
    }

    public void RollCamera(float angleDelta)
    {
        var spline = trackSpline.Spline;
        BezierKnot knot = spline[currentKnotIndex];

#if UNITY_EDITOR
        Undo.RecordObject(trackSpline, "Roll Camera");
#endif

        Quaternion currentRot = knot.Rotation;
        Vector3 euler = currentRot.eulerAngles;
        euler.z += angleDelta;

        knot.Rotation = Quaternion.Euler(euler);
        spline[currentKnotIndex] = knot;

        SyncCamera();
    }

    // Hàm ép Camera bay thẳng đến tọa độ thật của Knot
    private void SyncCamera()
    {
        if (vrSeat != null && trackSpline != null)
        {
            var spline = trackSpline.Spline;
            BezierKnot knot = spline[currentKnotIndex];

            // Tính toán vị trí và góc xoay chính xác tuyệt đối của Knot trong không gian 3D
            Vector3 worldPos = trackSpline.transform.TransformPoint((Vector3)knot.Position);
            Quaternion worldRot = trackSpline.transform.rotation * knot.Rotation;

            // Đặt Camera ngồi đúng vào vị trí đó
            vrSeat.position = worldPos;
            vrSeat.rotation = worldRot;

            // Yêu cầu Unity vẽ lại cửa sổ Scene để cập nhật mũi tên
#if UNITY_EDITOR
            HandleUtility.Repaint();
#endif
        }
    }
}