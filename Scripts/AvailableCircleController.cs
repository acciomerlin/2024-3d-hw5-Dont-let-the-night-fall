using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvailableCircleController : MonoBehaviour
{
    public GameObject embeddedArrowPrefab; // 嵌入的箭矢预制件
    public Material sunMaterial;          // 替换的材质

    private GameController gameController;

    void Start()
    {
        gameController = FindObjectOfType<GameController>();
        if (gameController == null)
        {
            Debug.LogError("GameController not found in the scene!");
        }
    }

    public void HandleHit(Vector3 hitPoint, Vector3 hitNormal)
    {
        gameController.AddTargetHit();
        gameController.RemoveTarget();
        Debug.Log("HandleHit!!!!");

        // 在击中位置生成嵌入箭矢
        GameObject newArrow = Instantiate(embeddedArrowPrefab, hitPoint, Quaternion.identity);

        // 调整箭矢方向，使其看起来嵌入目标
        newArrow.transform.forward = -hitNormal;
        newArrow.transform.parent = FindObjectOfType<SunTargetController>().transform;

        // 开始箭矢的晃动协程
        StartCoroutine(ArrowSettle(newArrow));
    }

    IEnumerator ArrowSettle(GameObject newArrow)
    {
        float duration = 0.5f; // 晃动持续时间
        float elapsedTime = 0f;

        Transform arrowTransform = newArrow.transform;

        while (elapsedTime < duration)
        {
            // 计算晃动角度
            float angle = 2f * Mathf.Sin(elapsedTime * Mathf.PI * 2) * (1f - elapsedTime / duration) * 10f; // 晃动幅度逐渐减小

            // 旋转箭矢
            arrowTransform.Rotate(arrowTransform.right, angle * Time.deltaTime, Space.World);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 最终静止
        arrowTransform.rotation = Quaternion.LookRotation(arrowTransform.forward, Vector3.up);

        // 替换所有 supporterCube 的材质
        ReplaceAllSupporterMaterials();

        // 删除箭矢对象并向父级发送消息
        NotifyParentAndDestroy(newArrow);
    }

    void ReplaceAllSupporterMaterials()
    {
        Transform parentTransform = transform.parent;
        if (parentTransform == null)
        {
            Debug.LogError("Parent transform not found!");
            return;
        }

        foreach (Transform child in parentTransform)
        {
            if (child.CompareTag("SupporterCube"))
            {
                MeshRenderer meshRenderer = child.GetComponent<MeshRenderer>();
                if (meshRenderer != null)
                {
                    meshRenderer.material = sunMaterial;
                    Debug.Log($"SupporterCube {child.name} material replaced.");
                }
                else
                {
                    Debug.LogError($"SupporterCube {child.name} does not have a MeshRenderer component.");
                }
            }
        }
    }

    void NotifyParentAndDestroy(GameObject newArrow)
    {
        // 向父级发送消息
        if (transform.parent != null)
        {
            transform.parent.SendMessage("OnArrowSettled", newArrow, SendMessageOptions.DontRequireReceiver);
        }

        // 删除当前目标
        Destroy(gameObject);
    }
}