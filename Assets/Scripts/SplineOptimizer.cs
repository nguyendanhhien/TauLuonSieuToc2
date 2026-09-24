using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

public class SplineOptimizer : MonoBehaviour
{
    // Nút bấm ảo trong Context Menu của Unity
    [ContextMenu("1. Tự động sửa lỗi mượt Spline")]
    public void FixSpline()
    {
        SplineContainer container = GetComponent<SplineContainer>();
        if (container == null)
        {
            Debug.LogWarning("Không tìm thấy Spline Container trên object này!");
            return;
        }

        Spline spline = container.Spline;

        // Quét qua toàn bộ các điểm Knot trên đường ray
        for (int i = 0; i < spline.Count; i++)
        {
            BezierKnot knot = spline[i];

            // --- 1. SỬA LỖI XOAY CAMERA ---
            // Chuyển đổi định dạng góc xoay để can thiệp vào trục Z
            Quaternion rot = knot.Rotation;
            Vector3 euler = rot.eulerAngles;

            // Ép chết trục Z về 0 độ (xe luôn giữ thẳng đứng ngang hàng với mặt đất, không bị lật)
            euler.z = 0f;

            // Ép ngược góc xoay mới vào lại điểm Knot
            knot.Rotation = Quaternion.Euler(euler);
            spline[i] = knot;

            // --- 2. SỬA LỖI ĐƯỜNG CONG GÃY ---
            // Yêu cầu Unity tự động tính toán lại 2 đầu tay cầm cho mượt nhất (AutoSmooth)
            spline.SetTangentMode(i, TangentMode.AutoSmooth);

            // Ngay sau khi tính xong, chuyển nó về trạng thái Bezier (Continuous) 
            // để giữ form và cho phép bạn kéo chỉnh tay sau này
            spline.SetTangentMode(i, TangentMode.Continuous);
        }

        Debug.Log($"Đã tối ưu thành công {spline.Count} điểm Knot!");
    }
}