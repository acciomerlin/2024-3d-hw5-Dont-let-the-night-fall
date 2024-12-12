using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public GameObject player;              // 玩家对象
    public GameObject startMenu;            // 开始菜单
    public Button startButton;             // 开始按钮
    public GameObject chaserPrefab;        // 追逐者预制件
    public Vector3 chaserSpawnPoint = new Vector3(47.8000031f, 103.599998f, 50.7000008f);     // 追逐者生成点
    private GameObject chaserInstance;
    public SkyboxCycle skyboxCycle;        // 天空盒控制器


    public int remainingArrows = 10;       // 玩家剩余弩箭数量
    public int totalTargetsHit = 0;       // 玩家击中的目标总数

    public int targetsToDayFromDusk = 2;  // 黄昏回白天需要的目标数
    public int targetsToDuskFromNight = 5; // 黑夜回黄昏需要的目标数

    private int currentTargetsHit = 0;    // 当前阶段击中的目标数

    public int totalTargets = 0;           // 当前场景中的目标总数
    public int maxTargets = 20;           // 最大目标数量

    public TextMeshProUGUI arrowsLeftText;       // 显示剩余弩箭数量的 UI
    public TextMeshProUGUI currentTargetsText;  // 显示当前击中目标数量的 UI
    public TextMeshProUGUI totalTargetsText;    // 显示总击中目标数量的 UI
    public TextMeshProUGUI targetInfoText;      // 显示目标信息的 UI

    public string currentSkyboxState = "Day";   // 当前天空盒状态

    private int highestTargetsHit = 0;      // 最高击中数
    private bool isFirstGame = true;        // 是否是第一轮游戏
    public TextMeshProUGUI highestHitsText;     // 显示最高击中目标的 UI

    // 定义事件
    public delegate void TargetHitEventHandler();
    public event TargetHitEventHandler OnTargetHitEvent;

    private Vector3 playerInitialPosition;  // 玩家初始位置
    private Quaternion playerInitialRotation; // 玩家初始旋转

    void Start()
    {
        // 初始化菜单和 UI
        startMenu.SetActive(true);
        if (startButton != null)
        {
            startButton.onClick.AddListener(StartGame);
        }

        // 锁定玩家控制直到游戏开始
        if (player != null)
        {
            playerInitialPosition = player.transform.localPosition;
            playerInitialRotation = player.transform.rotation;
            var mouseLook = player.GetComponentInChildren<MouseLook>();
            if (mouseLook != null)
            {
                mouseLook.enabled = false;
            }
        }

        UpdateUI(); // 初始化 UI
    }

    private void Update()
    {
        UpdateUI();
    }


    public void StartGame()
    {
        // 隐藏开始菜单
        startMenu.SetActive(false);

        totalTargets = 0;
        currentTargetsHit = 0;
        totalTargetsHit = 0;
        remainingArrows = 10;
        UpdateUI();

        // 启用玩家控制
        if (player != null)
        {
            Cursor.lockState = CursorLockMode.Locked;
            var mouseLook = player.GetComponentInChildren<MouseLook>();
            if (mouseLook != null)
            {
                mouseLook.enabled = true;
            }
        }

        // 初始化天空盒循环
        if (skyboxCycle != null)
        {
            skyboxCycle.StartSkyboxCycle();
        }

        // 生成追逐者
        if (chaserPrefab != null && chaserSpawnPoint != null)
        {
            chaserInstance = Instantiate(chaserPrefab, chaserSpawnPoint, Quaternion.identity);
            var chaserScript = chaserInstance.GetComponent<Chaser>();
            if (chaserScript != null)
            {
                chaserScript.player = player.transform;
            }
        }

        // 更新按钮文本
        if (!isFirstGame)
        {
            startButton.GetComponentInChildren<TextMeshProUGUI>().text = "Restart";
        }
        isFirstGame = false;

        Debug.Log("Game Started!");
    }

    public void GameOver()
    {
        Debug.Log("Game Over!");

        // 停止天空盒循环
        if (skyboxCycle != null)
        {
            skyboxCycle.StopSkyboxCycle();
        }

        // 销毁追逐者
        if (chaserInstance != null)
        {
            Destroy(chaserInstance);
        }

        // 清除所有箭矢
        foreach (var arrow in GameObject.FindGameObjectsWithTag("Arrow"))
        {
            Destroy(arrow);
        }

        // 更新最高记录
        if (totalTargetsHit > highestTargetsHit)
        {
            highestTargetsHit = totalTargetsHit;
        }

        // 更新 UI
        highestHitsText.text = $"Highest Hits: {highestTargetsHit}";

        // 重置数据
        totalTargetsHit = 0;

        // 显示开始菜单
        startMenu.SetActive(true);

        // 禁用玩家控制
        if (player != null)
        {
            player.transform.localPosition = playerInitialPosition;
            player.transform.rotation = playerInitialRotation;
            var mouseLook = player.GetComponentInChildren<MouseLook>();
            if (mouseLook != null)
            {
                mouseLook.enabled = false;
            }
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void AddArrow(int amount)
    {
        remainingArrows += amount;
        UpdateUI();
    }

    public void AddTarget()
    {
        if (totalTargets < maxTargets)
        {
            totalTargets++;
        }
        UpdateUI();
    }

    public void RemoveTarget()
    {
        if (totalTargets > 0)
        {
            totalTargets--;
        }
        UpdateUI();
    }

    public void ResetTargets()
    {
        totalTargets = 0;
        remainingArrows = 10;
        UpdateUI();
    }

    public void ResetCurrentTargets()
    {
        if (currentSkyboxState == "Day"&&remainingArrows<10)
        {
            remainingArrows += currentTargetsHit;
        }
        currentTargetsHit = 0;
        UpdateUI();
    }

    public void AddTargetHit()
    {
        currentTargetsHit++;
        totalTargetsHit++;
        UpdateUI();

        // 触发事件
        OnTargetHitEvent?.Invoke();
    }

    public int GetCurrentTargetsHit()
    {
        return currentTargetsHit;
    }

    private void UpdateUI()
    {
        if (arrowsLeftText != null)
        {
            arrowsLeftText.text = $"Arrows Left: {remainingArrows}";
        }

        if (currentTargetsText != null)
        {
            currentTargetsText.text = $"Current Targets Hit: {currentTargetsHit}";
        }

        if (totalTargetsText != null)
        {
            totalTargetsText.text = $"Total Targets Hit: {totalTargetsHit}";
        }

        if (targetInfoText != null)
        {
            if (currentSkyboxState == "Day")
            {
                targetInfoText.text = "Targets will be spawned after the day";
            }
            else if (currentSkyboxState == "Dusk" || currentSkyboxState == "Night")
            {
                targetInfoText.text = $"Remaining Targets: {totalTargets}";
            }
        }
    }
}