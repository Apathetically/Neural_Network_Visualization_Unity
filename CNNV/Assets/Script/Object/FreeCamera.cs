using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeCamera : MonoBehaviour
{
    private Vector3 oldMousePos;
    private Vector3 newMosuePos;

    //[SerializeField]
    //private float minimumY = 0.2f;

    [SerializeField]
    private float zoomSpeed = 30.0f;
    [SerializeField]
    private float keyBoardMoveSpeed = 1.0f;
    [SerializeField]
    private float rotSpeed = 0.05f;
    [SerializeField]
    //private float mouseMoveSpeed = 0.05f;//鼠标中键控制相机的速度

    private float distance = 5;
    //private Vector3 centerOffset = Vector3.zero;
    private Vector3 initPos = Vector3.zero;
    private Vector3 initRot = Vector3.zero;

    private void Awake()
    {
        initPos = transform.position;
        initRot = transform.eulerAngles;
    }

    private void OnEnable()
    {
        transform.position = initPos;
        transform.eulerAngles = initRot;
    }

    void Update()
    {
        MoveCameraKeyBoard();
        ZoomCamera();
        SuperViewMouse();

        oldMousePos = Input.mousePosition;
    }

    private void MoveCameraKeyBoard()
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))//(Input.GetAxis("Horizontal")<0)
        {
            transform.Translate(new Vector3(-keyBoardMoveSpeed, 0, 0), Space.Self);
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            transform.Translate(new Vector3(keyBoardMoveSpeed, 0, 0), Space.Self);
        }
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            transform.Translate(new Vector3(0, 0, keyBoardMoveSpeed), Space.Self);
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            transform.Translate(new Vector3(0, 0, -keyBoardMoveSpeed), Space.Self);
        }
        if (Input.GetKey(KeyCode.E))
        {
            transform.Translate(new Vector3(0, keyBoardMoveSpeed, 0), Space.World);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            //if (transform.transform.position.y - keyBoardMoveSpeed >= this.minimumY)
            transform.Translate(new Vector3(0, -keyBoardMoveSpeed, 0), Space.World);
        }
    }

    private void ZoomCamera()
    {
        float offset = Input.GetAxis("Mouse ScrollWheel");
        if (offset != 0)
        {
            offset *= zoomSpeed;
            //currentCamera.transform.position = currentCamera.transform.position + currentCamera.transform.forward * offset;//localPosition
            this.distance -= offset;
            transform.Translate(Vector3.forward * offset, Space.Self); //
        }
    }

    private void SuperViewMouse()
    {
        if (Input.GetMouseButton(1))
        {
            newMosuePos = Input.mousePosition;
            Vector3 dis = newMosuePos - oldMousePos;
            float angleX = dis.x * rotSpeed;//* Time.deltaTime
            float angleY = dis.y * rotSpeed;//* Time.deltaTime
            transform.Rotate(new Vector3(-angleY, 0, 0), Space.Self);
            transform.Rotate(new Vector3(0, angleX, 0), Space.World);
        }
    }
}