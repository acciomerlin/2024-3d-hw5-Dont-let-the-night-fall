using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // 确保引用 TextMeshPro

public class MyCrossbowShoot : MonoBehaviour
{
    public GameObject arrowPrefab;       // 弩箭预制件
    public Transform arrowLocation;      // 弩箭生成位置
    public float shotPower = 2000f;       // 射击力
    public LayerMask hitMask;            // 可射击的层

    private bool isAiming = false;       // 是否在瞄准状态
    public GameObject aimingUI;          // 辅助瞄准 UI

    public TextMeshProUGUI powerDisplayUI;              // 显示射击力的 UI 文本
    public float minShotPower = 2000f;       // 最小射击力
    public float maxShotPower = 5000f;       // 最大射击力
    private float currentShotPower = 2000f;  // 当前射击力

    private GameController gameController;

    // 音效
    public AudioClip aimingSound;          // 瞄准音效
    public AudioClip shootSound;           // 射出箭时音效

    private AudioSource audioSource;       // 音频源

    void Start()
    {
        if (arrowLocation == null)
            arrowLocation = transform;

        gameController = FindObjectOfType<GameController>();
        if (gameController == null)
        {
            Debug.LogError("GameController not found in the scene!");
        }

        // 初始化音频源
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        // 检测右键瞄准状态
        bool wasAiming = isAiming; // 记录之前的状态
        isAiming = Input.GetMouseButton(1);

        // 如果瞄准状态发生变化，播放音效
        if (isAiming && !wasAiming)
        {
            PlaySound(aimingSound);
        }

        // 更新瞄准 UI
        UpdateAimingUI();

        // 更新power UI
        UpdateAdjustShotPowerUI();

        // 检测射击强度的调整
        if (isAiming)
        {
            AdjustShotPower();
        }

        // 只有在右键瞄准状态且有弩箭时可以发射
        if (isAiming && Input.GetButtonDown("Fire1") && gameController.remainingArrows > 0)
        {
            ShootArrow();
        }
    }

    void ShootArrow()
    {
        Debug.Log("Fired a physical arrow!");

        // 创建物理箭矢
        GameObject arrow = Instantiate(arrowPrefab, arrowLocation.position, arrowLocation.rotation);
        arrow.GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * currentShotPower);

        // 播放射击音效
        PlaySound(shootSound);

        // 减少弩箭数量
        gameController.remainingArrows--;
    }

    void UpdateAimingUI()
    {
        if (aimingUI != null)
        {
            if (isAiming)
            {
                aimingUI.SetActive(true);

                // 使用 Raycast 预测命中点
                Ray ray = new Ray(arrowLocation.position, arrowLocation.forward);
                RaycastHit hit;

                Vector3 targetPosition;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, hitMask))
                {
                    // 检测到目标，设置 UI 到命中点
                    targetPosition = hit.point;
                }
                else
                {
                    // 没有命中目标，设置到最大射程位置
                    targetPosition = arrowLocation.position + arrowLocation.forward * 100f;
                }

                // 将世界坐标转换为屏幕坐标
                Vector3 screenPosition = Camera.main.WorldToScreenPoint(targetPosition);
                aimingUI.transform.position = screenPosition;
            }
            else
            {
                aimingUI.SetActive(false);
            }
        }
    }

    void UpdateAdjustShotPowerUI()
    {
        if (!isAiming)
        {
            powerDisplayUI.text = "";
        }
        else
        {
            powerDisplayUI.text = $"Power: {currentShotPower:F0}";
        }
    }

    void AdjustShotPower()
    {
        float scrollDelta = Input.GetAxis("Mouse ScrollWheel");
        if (scrollDelta != 0)
        {
            currentShotPower = Mathf.Clamp(currentShotPower + scrollDelta * 500f, minShotPower, maxShotPower);
            shotPower = currentShotPower;

            // 更新 UI 显示
            if (powerDisplayUI != null)
            {
                powerDisplayUI.text = $"Power: {currentShotPower:F0}";
            }
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }
}