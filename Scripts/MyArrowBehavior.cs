using System.Collections;
using UnityEngine;

public class MyArrowBehavior : MonoBehaviour
{
    private Rigidbody arrowRb;

    private float initialDelay = 0.1f; // 初始延迟时间
    private bool isReadyToDetect = false; // 是否可以检测碰撞

    public GameObject arrowMarker;  // 红光标记预制件
    private bool hasMarked = false;

    private GameController gameController; // 全局控制器

    private GameObject arrowMarkerInstance;
    private ParticleSystem arrowMarkerParticle;
    public float pickupRadius = 2f; // 可拾取的检测半径
    private bool isPickupAvailable = false;
    private GameObject player;

    void Start()
    {
        arrowRb = GetComponent<Rigidbody>();
        gameController = FindObjectOfType<GameController>();
        player = GameObject.FindGameObjectWithTag("Player");

        if (gameController == null)
        {
            Debug.LogError("GameController not found in the scene!");
        }
        if (player == null)
        {
            Debug.LogError("Player not found in the scene!");
        }

        StartCoroutine(EnableStillDetection());
    }

    void Update()
    {
        if (isReadyToDetect && IsStill())
        {
            if (!hasMarked)
            {
                CreateMarker(transform.localPosition);
                hasMarked = true;
            }

            // 检测玩家是否进入可拾取半径
            if (player != null && Vector3.Distance(transform.position, player.transform.position) <= pickupRadius)
            {
                if (!isPickupAvailable)
                {
                    EnablePickup(); // 玩家进入范围，激活拾取逻辑
                }

                // 按下 F 键回收箭矢
                if (Input.GetKeyDown(KeyCode.F))
                {
                    PickupArrow();
                }
            }
            else if (isPickupAvailable)
            {
                DisablePickup(); // 玩家离开范围，取消拾取状态
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Shootable"))
        {
            HandleShootableHit(collision.gameObject);
        }
    }

    bool IsStill()
    {
        // 设定一个非常小的阈值，考虑到浮动误差
        float threshold = 0.001f;

        // 检查线性速度和角速度是否都接近零
        if (arrowRb.velocity.magnitude < threshold && arrowRb.angularVelocity.magnitude < threshold)
        {
            arrowRb.isKinematic = true;
            return true;
        }
        return false;
    }

    IEnumerator EnableStillDetection()
    {
        // 等待初始延迟，避免生成时的误触碰
        yield return new WaitForSeconds(initialDelay);
        isReadyToDetect = true;
    }

    void HandleShootableHit(GameObject target)
    {
        Debug.Log("Arrow hit a shootable target: " + target.name);

        // 获取击中点和法线
        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo))
        {
            // 调用目标的 HandleHit 方法
            AvailableCircleController targetBehavior = target.GetComponent<AvailableCircleController>();
            if (targetBehavior != null)
            {
                targetBehavior.HandleHit(hitInfo.point, hitInfo.normal);
            }
        }

        // 销毁当前箭矢（不回收）
        Destroy(gameObject);
    }

    void CreateMarker(Vector3 position)
    {
        // 生成箭矢落地标记
        arrowMarkerInstance = Instantiate(arrowMarker, position, Quaternion.identity);

        // 获取粒子系统
        arrowMarkerParticle = arrowMarkerInstance.GetComponentInChildren<ParticleSystem>();
        if (arrowMarkerParticle != null)
        {
            // 设置默认颜色为 Color.yellow
            var main = arrowMarkerParticle.main;
            main.startColor = Color.yellow;
        }
    }

    void EnablePickup()
    {
        isPickupAvailable = true;

        // 改变粒子效果颜色为绿色（表示可拾取）
        if (arrowMarkerParticle != null)
        {
            var main = arrowMarkerParticle.main;
            main.startColor = Color.green;
        }
    }

    void DisablePickup()
    {
        isPickupAvailable = false;

        // 恢复粒子效果颜色为 Color.yellow
        if (arrowMarkerParticle != null)
        {
            var main = arrowMarkerParticle.main;
            main.startColor = Color.yellow;
        }
    }

    void PickupArrow()
    {
        // 增加弩箭数量并销毁当前弩箭和标记
        Debug.Log("Picked up arrow!");
        if (gameController != null)
        {
            gameController.remainingArrows++;
            Debug.Log($"Arrows left: {gameController.remainingArrows}");
        }

        if (arrowMarkerInstance != null)
        {
            Destroy(arrowMarkerInstance); // 销毁标记
        }

        Destroy(gameObject); // 销毁箭矢
    }
}