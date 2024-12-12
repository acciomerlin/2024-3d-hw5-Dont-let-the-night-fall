using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCrossbosMove : MonoBehaviour
{
    public float idleAmp = 0.05f; // 静止：弩左右或上下摆动的距离
    public float idleFreq = 1f;

    public float movingAmp = 0.05f; // 移动：弩左右或上下摆动的距离
    public float movingFreq = 5f;

    public Vector3 runOffset = new Vector3(-0.2f, -0.1f, 0.1f); // 跑动时弩的偏移量
    public Vector3 aimingPosition = new Vector3(0f, -0.57f, 0f);  // 瞄准（右键按住）时弩的位置

    private Vector3 initialPos;
    private PlayerMovement playerMovement;
    private bool isAiming = false; // 是否正在瞄准

    // Start is called before the first frame update
    void Start()
    {
        initialPos = transform.localPosition;
        playerMovement = GetComponentInParent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        isAiming = Input.GetMouseButton(1); // 鼠标右键

        if(isAiming)
        {
            // 瞄准状态：将弩移动到正中央
            transform.localPosition = Vector3.Lerp(transform.localPosition, aimingPosition, Time.deltaTime * 10f);
        }
        else if (playerMovement.isMoving)
        {
            // 跑动状态：将弩调整到斜着的状态
            transform.localPosition = Vector3.Lerp(transform.localPosition, initialPos + runOffset, Time.deltaTime * 5f);
        }
        else
        {
            //float amplitude = playerMovement.isMoving ? movingAmp : idleAmp;
            //float frequency = playerMovement.isMoving ? movingFreq : idleFreq;
            float amplitude = idleAmp;
            float frequency = idleFreq;

            float time = Time.time;

            float horizontalOffset = Mathf.Sin(time * frequency) * amplitude;
            float verticalOffset = Mathf.Cos(time * frequency * 0.5f) * amplitude * 0.5f;

            transform.localPosition = Vector3.Lerp(transform.localPosition, initialPos + new Vector3(horizontalOffset, verticalOffset, 0f), Time.deltaTime * 15f);
        }


    }
}
