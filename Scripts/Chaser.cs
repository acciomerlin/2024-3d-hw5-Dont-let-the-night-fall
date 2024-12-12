using UnityEngine;
using System.Collections;

public class Chaser : MonoBehaviour
{
    public Transform player;                 // 玩家目标
    public float triggerDistance = 6f;       // 与玩家触发的距离
    public float daySpeed = 5f;              // 白天速度
    public float duskSpeed = 10f;            // 黄昏速度
    public float nightStartSpeed = 10f;      // 夜晚初始速度
    public float nightAcceleration = 2f;    // 夜晚加速度
    public float maxSpeed = 20f;             // 最大速度
    public GameObject fakeArrowPrefab;       // 假箭矢预制件

    public Renderer block1;                  // 第一个方块的 Renderer
    public Renderer block2;                  // 第二个方块的 Renderer

    public Material block1DayMaterial;
    public Material block1DuskMaterial;
    public Material block1NightMaterial;

    public Material block2DayMaterial;
    public Material block2DuskMaterial;
    public Material block2NightMaterial;

    private float currentSpeed;              // 当前速度
    private string currentTimeOfDay = "Day"; // 当前时间段
    private bool gameOver = false;           // 是否触发游戏结束
    private bool isFrozen = false;           // 是否静止状态

    public AudioClip dayMusic;                // 白天音乐
    public AudioClip duskMusic;               // 黄昏音乐
    public AudioClip nightMusic;              // 夜晚音乐

    private AudioSource audioSource;          // 音频源
    public float maxVolume = 1f;              // 最大音量
    public float minVolume = 0.1f;            // 最小音量
    public float maxDistance = 50f;           // 最大声音距离

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("Player is not assigned! Please assign the player's Transform.");
        }

        if (block1 == null || block2 == null)
        {
            Debug.LogError("Block Renderers are not assigned! Please assign the Renderers.");
        }

        currentSpeed = daySpeed;
        UpdateMaterials("Day"); // 初始化材质为白天

        // 初始化音频源
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        PlayMusic(dayMusic);
    }

    void Update()
    {
        if (gameOver || player == null || isFrozen) return;

        CheckDistanceToPlayer();

        // 根据当前状态更新速度
        UpdateSpeed(currentTimeOfDay);

        // 调整音量
        AdjustVolume();

        // 计算追逐方向
        Vector3 direction = (player.position - transform.position).normalized;

        // 移动追逐者
        transform.position += direction * currentSpeed * Time.deltaTime;

        // 面向玩家
        transform.LookAt(player);
    }

    void AdjustVolume()
    {
        if (player == null || audioSource == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // 根据距离调整音量（距离越近，音量越大）
        if (distanceToPlayer < maxDistance)
        {
            float volume = Mathf.Lerp(maxVolume, minVolume, distanceToPlayer / maxDistance);
            audioSource.volume = volume;
        }
        else
        {
            audioSource.volume = minVolume; // 超过最大距离时，设置为最小音量
        }
    }

    void CheckDistanceToPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= triggerDistance)
        {
            GameController gameController = FindObjectOfType<GameController>();
            if (gameController != null)
            {
                gameController.GameOver();
            }
        }
    }

    public void UpdateTimeOfDay(string timeOfDay)
    {
        if (currentTimeOfDay != timeOfDay)
        {
            currentTimeOfDay = timeOfDay;
            UpdateMaterials(timeOfDay);
            UpdateMusic(timeOfDay);
        }
    }

    private void UpdateSpeed(string timeOfDay)
    {
        if (timeOfDay == "Night")
        {
            currentSpeed = Mathf.Min(currentSpeed + nightAcceleration * Time.deltaTime, maxSpeed);
        }
    }

    private void UpdateMaterials(string timeOfDay)
    {
        switch (timeOfDay)
        {
            case "Day":
                block1.material = block1DayMaterial;
                block2.material = block2DayMaterial;
                currentSpeed = daySpeed;
                break;
            case "Dusk":
                block1.material = block1DuskMaterial;
                block2.material = block2DuskMaterial;
                currentSpeed = duskSpeed;
                break;
            case "Night":
                block1.material = block1NightMaterial;
                block2.material = block2NightMaterial;
                currentSpeed = nightStartSpeed;
                break;
            default:
                Debug.LogError("Invalid time of day!");
                break;
        }
    }

    private void UpdateMusic(string timeOfDay)
    {
        switch (timeOfDay)
        {
            case "Day":
                PlayMusic(dayMusic);
                break;
            case "Dusk":
                PlayMusic(duskMusic);
                break;
            case "Night":
                PlayMusic(nightMusic);
                break;
            default:
                Debug.LogError("Invalid time of day!");
                break;
        }
    }

    private void PlayMusic(AudioClip clip)
    {
        if (audioSource.clip != clip)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameController gameController = FindObjectOfType<GameController>();
            if (gameController != null)
            {
                gameController.GameOver();
            }
        }
        else if (collision.gameObject.CompareTag("Arrow"))
        {
            ContactPoint contact = collision.contacts[0];
            Vector3 hitPoint = contact.point;
            Vector3 hitNormal = contact.normal;

            InstantiateFakeArrow(hitPoint, hitNormal);
            StartCoroutine(FreezeMovement(3f));
        }
    }

    private void InstantiateFakeArrow(Vector3 position, Vector3 normal)
    {
        if (fakeArrowPrefab != null)
        {
            GameObject fakeArrow = Instantiate(fakeArrowPrefab, position, Quaternion.identity);
            fakeArrow.transform.forward = -normal;
            fakeArrow.transform.SetParent(transform);
        }
    }

    private IEnumerator FreezeMovement(float freezeDuration)
    {
        isFrozen = true; // 停止移动
        float originalSpeed = currentSpeed; // 保存静止前的速度
        currentSpeed = 0f;

        yield return new WaitForSeconds(freezeDuration);

        isFrozen = false; // 恢复移动
        currentSpeed = originalSpeed;
    }
}