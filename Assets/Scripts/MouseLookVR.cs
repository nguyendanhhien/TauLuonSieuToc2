using UnityEngine;
using UnityEngine.InputSystem; // Thêm thư viện hệ thống điều khiển mới

public class MouseLookVR : MonoBehaviour
{
    [Header("Tốc độ xoay chuột")]
    public float sensitivity = 0.2f;

    private float xRotation = 0f;
    private float yRotation = 0f;

    void Update()
    {
        // Kiểm tra xem có chuột được kết nối hay không
        if (Mouse.current == null) return;

        // Chỉ cho phép xoay khi nhấn giữ chuột trái hoặc chuột phải
        if (Mouse.current.leftButton.isPressed || Mouse.current.rightButton.isPressed)
        {
            // Lấy thông số quãng đường rê chuột trên màn hình
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();

            float mouseX = mouseDelta.x * sensitivity;
            float mouseY = mouseDelta.y * sensitivity;

            yRotation += mouseX;
            xRotation -= mouseY;

            // Giới hạn góc ngước lên/cúi xuống để không bị gãy cổ
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            // Xoay ghế ngồi ảo
            transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
        }
    }
}