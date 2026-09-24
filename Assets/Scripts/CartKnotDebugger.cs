using UnityEngine;
using UnityEngine.Splines;

// Thư viện UnityEditor chỉ dùng trong lúc code/chỉnh sửa, không đưa vào bản build game
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CartKnotDebugger : MonoBehaviour
{
    [Header("Kéo file Spline đường ray vào đây")]
    public SplineContainer trackSpline;

    // Hàm này vẽ các chỉ báo trực quan trong cửa sổ Scene View
    private void OnDrawGizmos()
    {
        if (trackSpline == null || trackSpline.Spline == null) return;

        int closestKnotIndex = 0;
        float minDistance = float.MaxValue;
        Vector3 closestKnotWorldPos = Vector3.zero;

        // Quét tìm điểm Knot nào đang gần chiếc xe nhất
        for (int i = 0; i < trackSpline.Spline.Count; i++)
        {
            // Vị trí của Knot đang ở dạng Local, cần chuyển sang World Space để đo đạc chính xác
            Vector3 knotPos = trackSpline.transform.TransformPoint((Vector3)trackSpline.Spline[i].Position);
            float dist = Vector3.Distance(transform.position, knotPos);

            if (dist < minDistance)
            {
                minDistance = dist;
                closestKnotIndex = i;
                closestKnotWorldPos = knotPos;
            }
        }

#if UNITY_EDITOR
        // Cài đặt font chữ to, màu vàng nổi bật
        GUIStyle style = new GUIStyle();
        style.fontSize = 20;
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.yellow;

        // Vẽ dòng chữ báo cáo ngay trên đỉnh đầu chiếc xe
        Handles.Label(transform.position + Vector3.up * 3f, "▶ XE ĐANG Ở KNOT SỐ: " + closestKnotIndex + " ◀", style);
#endif

        // Vẽ một tia laser màu vàng nối từ xe cắm thẳng vào điểm Knot đó để bạn dễ nhận diện
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, closestKnotWorldPos);
        Gizmos.DrawSphere(closestKnotWorldPos, 0.5f);
    }
}