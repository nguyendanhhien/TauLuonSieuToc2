using UnityEngine;

public class SmoothVRSuspension : MonoBehaviour
{
    [Header("Kéo object CartTarget vào đây")]
    public Transform targetCart;

    [Header("Cài đặt hệ thống giảm xóc")]
    // Vị trí bám sát chặt để camera không bị lọt ra ngoài thân xe
    public float positionSmoothness = 20f;

    // Góc xoay nội suy chậm lại để hấp thụ toàn bộ độ rung lắc của đường ray
    public float rotationSmoothness = 5f;

    void LateUpdate()
    {
        if (targetCart == null) return;

        // Bắt buộc dùng LateUpdate để đảm bảo xe đã di chuyển xong hoàn toàn thì ghế mới chạy theo

        // 1. Kéo vị trí ghế bám theo xe
        transform.position = Vector3.Lerp(transform.position, targetCart.position, Time.deltaTime * positionSmoothness);

        // 2. Nội suy góc nghiêng mượt mà (chống chóng mặt)
        transform.rotation = Quaternion.Slerp(transform.rotation, targetCart.rotation, Time.deltaTime * rotationSmoothness);
    }
}