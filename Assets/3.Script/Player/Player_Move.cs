using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Move : MonoBehaviour
{
    [SerializeField] public float walkSpeed = 100f;
    [SerializeField] public float runSpeed = 150f;
    [SerializeField] public float rollSpeed = 200f;
    [SerializeField] public float rollDuration = 0.5f;
    [SerializeField] public float dazeDuration = 0.5f; // 이동을 못하는 시간

    private float speed;
    private float hAxis;
    private float vAxis;
    private bool isRolling = false;
    private bool isDazed = false;
    private float rollStartTime;
    private float dazeStartTime;
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
        if (isDazed)
        {
            // Dazed 상태일 때
            if (Time.time - dazeStartTime > dazeDuration)
            {
                // Dazed 종료
                isDazed = false;
                playerAnimator.SetBool("isDazed", false);
            }
            else
            {
                return; // Dazed 상태 동안에는 다른 입력을 처리하지 않음
            }
        }

        if (isRolling)
        {
            // 구르는 중일 때
            if (Time.time - rollStartTime > rollDuration)
            {
                // 구르기 종료
                isRolling = false;
                playerAnimator.SetBool("isRolling", false);
                StartDazed(); // 구른 후 Dazed 상태로 전환
            }
            else
            {
                // 구르는 동안 이동 업데이트
                transform.position += moveVec * rollSpeed * Time.deltaTime;
                return; // 구르는 동안에는 다른 입력을 처리하지 않음
            }
        }

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
            transform.position += moveVec * speed * Time.deltaTime;
            playerAnimator.SetBool("isWalking", true);
        }
        else
        {
            playerAnimator.SetBool("isWalking", false);
        }

        // Space Bar를 누르면 구르기 시작
        if (Input.GetKeyDown(KeyCode.Space) && moveVec != Vector3.zero)
        {
            StartRolling();
        }

        // 마우스 위치를 향해 플레이어 회전
        RotatePlayerToMouse();
    }

    private void RotatePlayerToMouse()
    {
        // 구르는 중이나 Dazed 상태에서는 회전하지 않음
        if (isRolling || isDazed) return;

        // 마우스 위치를 월드 좌표로 가져오기
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 target = hit.point;
            target.y = transform.position.y; // y 좌표는 플레이어와 동일하게 유지
            Vector3 direction = (target - transform.position).normalized;

            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                // 플레이어 회전 업데이트 (부드럽게 회전)
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * speed);
            }
        }
    }

    private void StartRolling()
    {
        isRolling = true;
        rollStartTime = Time.time;
        playerAnimator.SetBool("isRolling", true);
    }

    private void StartDazed()
    {
        isDazed = true;
        dazeStartTime = Time.time;
        playerAnimator.SetBool("isDazed", true);
    }
}
