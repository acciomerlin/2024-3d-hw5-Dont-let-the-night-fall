using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    public GameObject targetPrefab;       // 目标预制件
    public Vector3 circleCenter = new Vector3(111, 0, 107); // 圆心位置
    public float innerRadius = 50f;        // 内圆半径
    public float outerRadius = 150f;       // 外圆半径
    public Vector2 verticalRange = new Vector2(100f, 120f); // 垂直范围

    public float spawnInterval = 5f;      // 每次生成的时间间隔
    private float timer = 0f;             // 计时器

    public float avoidColliderRadius = 5f; // 避开 Collider 的半径
    public LayerMask avoidLayerMask;       // 检查的层级（Collider 的层级）

    private GameController gameController;
    private int maxAttempts = 20;         // 最大尝试次数

    private bool spawningEnabled = false; // 是否启用生成逻辑

    void Start()
    {
        gameController = FindObjectOfType<GameController>();
        if (gameController == null)
        {
            Debug.LogError("GameController not found in the scene!");
            return;
        }
    }

    void Update()
    {
        if (!spawningEnabled) return;

        // 如果当前目标数量已达上限，不生成新的目标
        if (gameController.totalTargets >= gameController.maxTargets)
            return;

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnTarget();
            timer = 0f;
        }
    }

    void SpawnTarget()
    {
        if (targetPrefab != null && gameController.totalTargets < gameController.maxTargets)
        {
            Vector3 spawnPosition;
            bool validPosition = GetValidRandomPosition(out spawnPosition);

            if (validPosition)
            {
                Instantiate(targetPrefab, spawnPosition, Quaternion.identity);
                gameController.AddTarget();
            }
            else
            {
                Debug.LogWarning("Failed to find a valid spawn position after multiple attempts.");
            }
        }
    }

    bool GetValidRandomPosition(out Vector3 validPosition)
    {
        for (int i = 0; i < maxAttempts; i++)
        {
            Vector3 potentialPosition = GenerateRandomPosition();
            if (!Physics.CheckSphere(potentialPosition, avoidColliderRadius, avoidLayerMask))
            {
                validPosition = potentialPosition;
                return true;
            }
        }

        validPosition = Vector3.zero;
        return false;
    }

    Vector3 GenerateRandomPosition()
    {
        float angle = Random.Range(0f, Mathf.PI * 2);
        float radius = Random.Range(innerRadius, outerRadius);
        float x = Mathf.Cos(angle) * radius + circleCenter.x;
        float z = Mathf.Sin(angle) * radius + circleCenter.z;
        float y = Random.Range(verticalRange.x, verticalRange.y) + circleCenter.y;
        return new Vector3(x, y, z);
    }

    public void EnableSpawning()
    {
        spawningEnabled = true;
        timer = 0f;

        // 初始化生成目标
        for (int i = 0; i < gameController.maxTargets; i++)
        {
            SpawnTarget();
        }
    }

    public void ClearAllTargets()
    {
        spawningEnabled = false;

        // 单独清理所有名字包含 "FakeArrow" 的对象
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (var obj in allObjects)
        {
            if (obj.name.Contains("FakeArrow(Clone)")) // 假设假箭矢名称中包含 "FakeArrow"
            {
                Destroy(obj);
            }
        }

        foreach (var target in GameObject.FindGameObjectsWithTag("Target"))
        {
            Destroy(target);
        }
        gameController.ResetTargets();
    }
}