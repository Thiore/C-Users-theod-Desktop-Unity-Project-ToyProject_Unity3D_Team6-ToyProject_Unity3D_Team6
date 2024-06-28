using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Move : MonoBehaviour
{
    [SerializeField] public float walkSpeed = 15f;
    [SerializeField] public float runSpeed = 30f;

    #region 영훈
    [Range(100,1000)]
    [SerializeField] public float MouseSpeed = 500f;
    #endregion

    private float speed;
    private float hAxis;
    private float vAxis;
    Vector3 moveVec;

    private Animator playerAnimator;

    private void Start()
    {
        // 자식 오브젝트에서 Animator 컴포넌트를 찾아 할당
        playerAnimator = GetComponentInChildren<Animator>();
        if (playerAnimator == null)
        {
            Debug.LogError("Animator component not found in children.");
        }

        speed = walkSpeed;
    }

    private void Update()
    {
        // 이동 입력 받기
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;
        // 속도 조정 (Shift 키 입력에 따라)
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            speed = runSpeed;
            playerAnimator.SetBool("isRunning", true);
        }
        else
        {
            speed = walkSpeed;
            playerAnimator.SetBool("isRunning", false);
        }

        // 플레이어 위치 업데이트
        if (moveVec != Vector3.zero)
        {
            #region 재현
            //transform.position += moveVec * speed * Time.deltaTime;
            #endregion
            #region 영훈
            Vector3 movePos = transform.position;
            movePos += transform.right * moveVec.x * speed * Time.deltaTime + transform.forward * moveVec.z * speed * Time.deltaTime;
            transform.position = movePos;
            #endregion
            playerAnimator.SetBool("isWalking", true);
        }
        else
        {
            playerAnimator.SetBool("isWalking", false);
        }

        // 마우스 위치를 향해 플레이어 회전
        RotatePlayerToMouse();
    }

    private void RotatePlayerToMouse()
    {
        #region 재현
        // 마우스 위치를 월드 좌표로 가져오기
        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //if (Physics.Raycast(ray, out RaycastHit hit))
        //{
        //    Vector3 target = hit.point;
        //    target.y = transform.position.y; // y 좌표는 플레이어와 동일하게 유지
        //    Vector3 direction = (target - transform.position).normalized;

        //    if (direction != Vector3.zero)
        //    {
        //        Quaternion lookRotation = Quaternion.LookRotation(direction);
        //        // 플레이어 회전 업데이트 (부드럽게 회전)
        //        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * speed);
        //    }
        //}
        #endregion

        #region 영훈
        float mouseX = Input.GetAxis("Mouse X");
        transform.Rotate(transform.up, mouseX*Time.deltaTime*MouseSpeed);
        #endregion
    }
}
