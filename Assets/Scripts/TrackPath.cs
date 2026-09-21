using UnityEngine;
using System.Collections.Generic;

// Tạo một struct để lưu cả Vị trí và Hướng mặt phẳng
[System.Serializable]
public struct TrackPoint
{
    public Vector3 position;
    public Vector3 normal;
}

public class TrackPath : MonoBehaviour
{
    [Header("Track Settings")]
    public List<TrackPoint> trackPoints = new List<TrackPoint>();
    public Color pathColor = Color.cyan;
    public float pointSize = 0.2f;

    private void OnDrawGizmos()
    {
        if (trackPoints == null || trackPoints.Count == 0) return;

        for (int i = 0; i < trackPoints.Count; i++)
        {
            Gizmos.color = pathColor;
            Gizmos.DrawSphere(trackPoints[i].position, pointSize);

            // MỚI: Vẽ một tia màu xanh lá cây để hiển thị hướng "lên" (Up) của xe tại điểm này
            Gizmos.color = Color.green;
            Gizmos.DrawRay(trackPoints[i].position, trackPoints[i].normal * 2f);

            if (i < trackPoints.Count - 1)
            {
                Gizmos.color = pathColor;
                Gizmos.DrawLine(trackPoints[i].position, trackPoints[i + 1].position);
            }
        }
    }

    public void ClearPath()
    {
        trackPoints.Clear();
    }
}