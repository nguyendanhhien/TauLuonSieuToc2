using UnityEngine;

public class ThrillScreamController : MonoBehaviour
{
    [Header("Kéo Audio Source tiếng hú hét vào đây")]
    public AudioSource screamAudio;

    [Header("Cài đặt độ nhạy (Cảm biến)")]
    // Giá trị từ 0 đến 1 (0.3 nghĩa là khi xe chúi mũi xuống khoảng 30 độ sẽ bắt đầu hét)
    public float dropThreshold = 0.3f;

    // Giá trị từ -1 đến 1 (0.3 nghĩa là khi xe nghiêng ngang sắp lật hoặc lộn ngược sẽ hét)
    public float invertThreshold = 0.3f;

    public float fadeSpeed = 3f; // Tốc độ to/nhỏ của tiếng hét (tránh bị giật âm thanh)

    void Start()
    {
        if (screamAudio != null) screamAudio.volume = 0f; // Luôn bắt đầu game trong im lặng
    }

    void Update()
    {
        if (screamAudio == null) return;

        // 1. Kiểm tra thả dốc: Tính toán mũi xe cắm xuống đất sâu bao nhiêu
        float dropAngle = Vector3.Dot(transform.forward, Vector3.down);
        bool isDropping = dropAngle > dropThreshold;

        // 2. Kiểm tra lộn vòng: Tính toán nóc xe bị nghiêng/lật ngửa bao nhiêu
        float invertAngle = Vector3.Dot(transform.up, Vector3.up);
        bool isLooping = invertAngle < invertThreshold;

        // Bật tắt âm thanh tự động
        if (isDropping || isLooping)
        {
            // Nội suy vặn to âm lượng dần lên 1
            screamAudio.volume = Mathf.MoveTowards(screamAudio.volume, 1f, Time.deltaTime * fadeSpeed);
        }
        else
        {
            // Nội suy vặn nhỏ âm lượng dần về 0 khi xe đi vào đường bằng
            screamAudio.volume = Mathf.MoveTowards(screamAudio.volume, 0f, Time.deltaTime * fadeSpeed);
        }
    }
}