using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouserSensitivity = 900f;

    public Transform playerBody;

    float xRotation = 0f;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        /**
         *Input.GetAxis("Mouse Y")：获取鼠标沿垂直方向（Y 轴方向）的移动量，鼠标上下移动时，会影响垂直旋转（俯仰）。
         *mouserSensitivity：控制鼠标灵敏度，通过放大或缩小鼠标的移动量，决定旋转的速度。
         *Time.deltaTime：确保旋转量与帧率无关。如果不使用 Time.deltaTime，高帧率会导致鼠标移动量累计得过快，旋转过快。
         */
        float mouseX = Input.GetAxis("Mouse X") * mouserSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouserSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); //限制俯仰角度在180度之内，不能断了脖子

        /**
         * transform：指当前脚本所附加的游戏对象的 Transform 组件。
         */
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up, mouseX);
    }
}
