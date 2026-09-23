using UnityEngine;
using UnityEngine.InputSystem; // Bắt buộc phải khai báo thư viện này

public class MapSwitcher : MonoBehaviour
{
    [Header("Kéo thả 2 object Map vào đây")]
    public GameObject map1;
    public GameObject map2;

    void Start()
    {
        if (map1 != null) map1.SetActive(true);
        if (map2 != null) map2.SetActive(false);
    }

    void Update()
    {
        // Kiểm tra bàn phím và nhận diện phím Space theo chuẩn mới
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            SwitchMap();
        }
    }

    public void SwitchMap()
    {
        bool isMap1Active = map1.activeSelf;

        map1.SetActive(!isMap1Active);
        map2.SetActive(isMap1Active);

        Debug.Log("Đã đổi Map!");
    }
}