using UnityEngine;
using Unity.Cinemachine;

public class DynamicCoasterSpeed : MonoBehaviour
{
    private CinemachineSplineCart cart;
    private float currentSpeed;

    [Header("Cài đặt tốc độ tàu lượn")]
    public float normalSpeed = 10f;   // Tốc độ khi đi đường bằng (m/s)
    public float maxSpeed = 30f;      // Tốc độ lao nhanh nhất khi thả dốc
    public float minSpeed = 3f;       // Tốc độ bò chậm nhất khi lên đỉnh dốc
    public float acceleration = 1.5f; // Quán tính (thay đổi tốc độ)

    void Start()
    {
        cart = GetComponent<CinemachineSplineCart>();
        currentSpeed = normalSpeed;
    }

    void Update()
    {
        if (cart == null) return;

        // Tính độ dốc (slope) dựa vào góc ngóc đầu của xe so với phương thẳng đứng
        float slope = Vector3.Dot(transform.forward, Vector3.up);
        float targetSpeed = normalSpeed;

        if (slope > 0)
        {
            // Xe ngóc đầu lên dốc -> Giảm tốc độ
            targetSpeed = Mathf.Lerp(normalSpeed, minSpeed, slope);
        }
        else if (slope < 0)
        {
            // Xe cắm đầu xuống dốc -> Tăng tốc độ
            targetSpeed = Mathf.Lerp(normalSpeed, maxSpeed, -slope);
        }

        // Tạo hiệu ứng quán tính
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * acceleration);

        // Đổi dòng bị lỗi này:
        // cart.Position += currentSpeed * Time.deltaTime;

        // Thành dòng mới này:
        cart.SplinePosition += currentSpeed * Time.deltaTime;
    }
}