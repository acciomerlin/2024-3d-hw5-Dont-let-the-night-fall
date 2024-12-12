using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SkyboxCycle : MonoBehaviour
{
    public Material daySkybox;
    public Material duskSkybox;
    public Material nightSkybox;

    public TextMeshProUGUI uiText;         // 用于显示状态和目标数
    public Image stateImage;              // 用于显示状态对应的贴图
    public Sprite daySprite;              // 白天对应的贴图
    public Sprite duskSprite;             // 黄昏对应的贴图
    public Sprite nightSprite;            // 黑夜对应的贴图

    private float stateDuration = 10f;    // 每个阶段持续时间
    private float elapsedTime = 0f;       // 当前状态经过的时间
    private string currentState = "Day"; // 当前状态

    private GameController gameController;
    private TargetSpawner targetSpawner;

    public bool isCycleActive = false;   // 天空盒循环是否激活

    void Start()
    {
        gameController = FindObjectOfType<GameController>();
        targetSpawner = FindObjectOfType<TargetSpawner>();

        if (gameController == null)
        {
            Debug.LogError("GameController not found in the scene!");
            return;
        }
        if (targetSpawner == null)
        {
            Debug.LogError("TargetSpawner not found in the scene!");
        }

        // 订阅 GameController 的事件
        gameController.OnTargetHitEvent += OnTargetHit;

        // 初始化为白天
        SetState("Day");
    }

    void Update()
    {
        if (!isCycleActive) return;

        elapsedTime += Time.deltaTime;

        // 检查当前状态是否需要转换
        if (currentState == "Day" && elapsedTime >= stateDuration)
        {
            SetState("Dusk");
        }
        else if (currentState == "Dusk" && elapsedTime >= stateDuration)
        {
            SetState("Night");
        }

        // 更新 UI 提示
        UpdateUIText();
    }

    public void StartSkyboxCycle()
    {
        isCycleActive = true;
        SetState("Day");
    }

    public void StopSkyboxCycle()
    {
        isCycleActive = false;
        SetState("Day"); // 游戏结束时强制回到白天
    }

    public void OnTargetHit()
    {
        Debug.Log("SkyboxCycle received OnTargetHit!");

        if (currentState == "Dusk" && gameController.GetCurrentTargetsHit() >= gameController.targetsToDayFromDusk)
        {
            SetState("Day");
        }
        else if (currentState == "Night" && gameController.GetCurrentTargetsHit() >= gameController.targetsToDuskFromNight)
        {
            SetState("Dusk");
        }
    }

    private void SetState(string newState)
    {
        currentState = newState;
        elapsedTime = 0f; // 重置计时

        if (gameController != null)
        {
            gameController.currentSkyboxState = newState;
        }

        // 根据状态切换天空盒
        switch (currentState)
        {
            case "Day":
                RenderSettings.skybox = daySkybox;
                stateImage.sprite = daySprite;
                targetSpawner.ClearAllTargets(); // 清空目标
                gameController.ResetCurrentTargets();
                NotifyChasers("Day");
                break;

            case "Dusk":
                RenderSettings.skybox = duskSkybox;
                stateImage.sprite = duskSprite;
                gameController.ResetCurrentTargets();
                targetSpawner.EnableSpawning(); // 启动目标生成
                NotifyChasers("Dusk");
                break;

            case "Night":
                RenderSettings.skybox = nightSkybox;
                stateImage.sprite = nightSprite;
                gameController.ResetCurrentTargets();
                NotifyChasers("Night");
                break;
        }

        DynamicGI.UpdateEnvironment(); // 更新全局光照
    }

    private void NotifyChasers(string timeOfDay)
    {
        Chaser[] chasers = FindObjectsOfType<Chaser>();
        foreach (var chaser in chasers)
        {
            chaser.UpdateTimeOfDay(timeOfDay);
        }
    }

    private void UpdateUIText()
    {
        string statusMessage = $"Current State: {currentState}";

        if (currentState == "Day")
        {
            float remainingTime = Mathf.Ceil(stateDuration - elapsedTime);
            statusMessage += $"     Dusk in: {remainingTime} s";
        }
        else if (currentState == "Dusk")
        {
            float remainingTime = Mathf.Ceil(stateDuration - elapsedTime);
            int remainingHits = gameController.targetsToDayFromDusk - gameController.GetCurrentTargetsHit();
            statusMessage += $"     Night in: {remainingTime} s\n";
            statusMessage += $"Hit {remainingHits} more targets to return to Day.";
        }
        else if (currentState == "Night")
        {
            int remainingHits = gameController.targetsToDuskFromNight - gameController.GetCurrentTargetsHit();
            statusMessage += $"\nHit {remainingHits} more targets to return to Dusk.";
        }

        if (uiText != null)
        {
            uiText.text = statusMessage;
        }
    }
}