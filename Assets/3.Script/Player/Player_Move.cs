using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Move : MonoBehaviour
{
    [SerializeField] public float walkSpeed = 100f;
    [SerializeField] public float runSpeed = 150f;
    [SerializeField] public float rollSpeed = 100f;
    [SerializeField] public float rollDuration = 0.5f;
    [SerializeField] public float dazeDuration = 0.5f; // 이동을 못하는 시간

    #region 영훈 / 마우스 감도
    [Range(100, 1000)]
    [SerializeField] public float MouseSpeed = 500f;
    #endregion

    private float speed;
    private float DecreaseRollSpeed;
    private float hAxis;
    private float vAxis;
    private float fireDelay;
    private float rollStartTime;
    private float dazeStartTime;
    private bool fdown;
    private bool isFireReady;
    private bool isRolling = false;
    private bool isDazed = false;
    private bool isAttacking = false; // 공격 중 상태 변수 추가

    Vector3 moveVec;
    Vector3 RollingForward;

    [SerializeField] private Weapon equiaWeapon;

    private Animator playerAnimator;
    private Rigidbody player_r;

    private void Start()
    {
        // 자식 오브젝트에서 Animator 컴포넌트를 찾아 할당
        playerAnimator = GetComponentInChildren<Animator>();
        player_r = GetComponent<Rigidbody>();
        if (playerAnimator == null)
        {
            Debug.LogError("Animator component not found in children.");
        }

        speed = walkSpeed;
    }

    private void Update()
    {
        // Dazed 상태일 때
        if (isDazed)
        {
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

        // 구르는 중일 때
        if (isRolling)
        {
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
                DecreaseRollSpeed -= Time.deltaTime * 30f;
                Vector3 movePos = transform.position;
                movePos += RollingForward * moveVec.z * DecreaseRollSpeed * Time.deltaTime;
                transform.position = movePos;
                return; // 구르는 동안에는 다른 입력을 처리하지 않음
            }
        }

        // 공격 중일 때
        if (isAttacking)
        {
            return; // 공격 중에는 다른 입력을 처리하지 않음
        }

        Attack(); // 공격 처리

        // 이동 입력 받기
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        fdown = Input.GetButtonDown("Fire1");
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

        // Space Bar를 누르면 구르기 시작
        if (Input.GetKeyDown(KeyCode.Space) && moveVec != Vector3.zero)
        {
            RollingForward = transform.forward;
            DecreaseRollSpeed = rollSpeed;
            StartRolling();
        }

        // 마우스 위치를 향해 플레이어 회전
        RotatePlayerToMouse();
    }

    private void FixedUpdate()
    {
        player_r.angularVelocity = Vector3.zero;
    }

    private void RotatePlayerToMouse()
    {
        // 구르는 중이나 Dazed 상태에서는 회전하지 않음
        if (isRolling || isDazed || isAttacking) return; // 공격 중에도 회전하지 않음

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
        transform.Rotate(transform.up, mouseX * Time.deltaTime * MouseSpeed);
        #endregion 
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

    private void Attack()
    {
        fireDelay += Time.deltaTime;
        isFireReady = equiaWeapon.rate < fireDelay;

        if (fdown && isFireReady && !isRolling && !isDazed)
        {
            isAttacking = true; // 공격 시작
            equiaWeapon.Use();
            playerAnimator.SetTrigger("DoSwing");
            fireDelay = 0;

            // 공격 애니메이션이 끝날 때까지 대기
            StartCoroutine(EndAttackAfterAnimation());
        }
    }

    private IEnumerator EndAttackAfterAnimation()
    {
        // 애니메이션 길이만큼 대기
        yield return new WaitForSeconds(playerAnimator.GetCurrentAnimatorStateInfo(0).length);
        isAttacking = false; // 공격 종료
    }
}
