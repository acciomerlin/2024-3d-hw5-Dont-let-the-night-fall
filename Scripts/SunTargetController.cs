using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunTargetController : MonoBehaviour
{
    public GameObject particleEffectPrefab;  // 粒子效果预制件
    public Color pickupAvailableColor = Color.green; // 进入回收范围时的粒子颜色
    public Color defaultColor = Color.yellow; // 默认粒子颜色
    public float pickupRadius = 2f;          // 回收检测半径

    public AudioClip hitSound;              // 射中目标的音效
    public AudioClip pickupSound;           // 回收目标的音效

    private GameObject player;              // 玩家对象
    private bool isPickupAvailable = false; // 是否可以回收
    private GameObject particleEffectInstance; // 实例化的粒子效果
    private GameObject currentFakeArrow;    // 记录当前的假箭矢
    private GameController gameController;  // 全局控制器
    private AudioSource audioSource;        // 音频源

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player not found in the scene!");
        }

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
        if (currentFakeArrow == null || particleEffectInstance == null)
            return;

        // 检测玩家是否进入回收范围
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (distanceToPlayer <= pickupRadius)
        {
            if (!isPickupAvailable)
            {
                EnablePickup();
            }

            // 按下 F 键回收目标
            if (Input.GetKeyDown(KeyCode.F))
            {
                PickupTarget();
            }
        }
        else if (isPickupAvailable)
        {
            DisablePickup();
        }
    }

    void OnArrowSettled(GameObject fakeArrow)
    {
        Debug.Log("Received OnArrowSettled from: " + fakeArrow.name);
        currentFakeArrow = fakeArrow;
        ActivateParticleEffect(fakeArrow);

        // 播放射中音效
        PlaySound(hitSound);
    }

    void ActivateParticleEffect(GameObject target)
    {
        if (particleEffectPrefab != null)
        {
            // 在 target 的位置生成粒子效果
            particleEffectInstance = Instantiate(particleEffectPrefab, target.transform.position, Quaternion.identity);
            UpdateParticleEffectColor(defaultColor); // 设置默认颜色
        }
        else
        {
            Debug.LogError("ParticleEffectPrefab is not assigned.");
        }
    }

    void EnablePickup()
    {
        isPickupAvailable = true;
        UpdateParticleEffectColor(pickupAvailableColor);
        Debug.Log("Pickup enabled! Press F to collect target.");
    }

    void DisablePickup()
    {
        isPickupAvailable = false;
        UpdateParticleEffectColor(defaultColor);
        Debug.Log("Pickup disabled.");
    }

    void PickupTarget()
    {
        Debug.Log("Target picked up!");

        // 增加弩箭数量
        if (gameController != null)
        {
            gameController.AddArrow(1);
        }

        // 播放回收音效
        PlaySound(pickupSound);

        // 销毁假箭矢、粒子效果和目标
        if (currentFakeArrow != null)
        {
            Destroy(currentFakeArrow);
        }
        if (particleEffectInstance != null)
        {
            Destroy(particleEffectInstance);
        }
        Destroy(gameObject); // 销毁目标本身
    }

    void UpdateParticleEffectColor(Color color)
    {
        if (particleEffectInstance != null)
        {
            var main = particleEffectInstance.GetComponent<ParticleSystem>().main;
            main.startColor = color;
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