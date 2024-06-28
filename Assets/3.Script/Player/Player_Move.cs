using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Move : MonoBehaviour
{
    [SerializeField] public float walkSpeed = 100f;
    [SerializeField] public float runSpeed = 150f;
    [SerializeField] public float rollSpeed = 100f;
    [SerializeField] public float rollDuration = 0.5f;
    [SerializeField] public float dazeDuration = 0.5f; // �̵��� ���ϴ� �ð�

    #region ���� / ���콺 ����
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
    private bool isAttacking = false; // ���� �� ���� ���� �߰�

    Vector3 moveVec;
    Vector3 RollingForward;

    [SerializeField] private Weapon equiaWeapon;

    private Animator playerAnimator;
    private Rigidbody player_r;

    private void Start()
    {
        // �ڽ� ������Ʈ���� Animator ������Ʈ�� ã�� �Ҵ�
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
        // Dazed ������ ��
        if (isDazed)
        {
            if (Time.time - dazeStartTime > dazeDuration)
            {
                // Dazed ����
                isDazed = false;
                playerAnimator.SetBool("isDazed", false);
            }
            else
            {
                return; // Dazed ���� ���ȿ��� �ٸ� �Է��� ó������ ����
            }
        }

        // ������ ���� ��
        if (isRolling)
        {
            if (Time.time - rollStartTime > rollDuration)
            {
                // ������ ����
                isRolling = false;
                playerAnimator.SetBool("isRolling", false);
                StartDazed(); // ���� �� Dazed ���·� ��ȯ
            }
            else
            {
                // ������ ���� �̵� ������Ʈ
                DecreaseRollSpeed -= Time.deltaTime * 30f;
                Vector3 movePos = transform.position;
                movePos += RollingForward * moveVec.z * DecreaseRollSpeed * Time.deltaTime;
                transform.position = movePos;
                return; // ������ ���ȿ��� �ٸ� �Է��� ó������ ����
            }
        }

        // ���� ���� ��
        if (isAttacking)
        {
            return; // ���� �߿��� �ٸ� �Է��� ó������ ����
        }

        Attack(); // ���� ó��

        // �̵� �Է� �ޱ�
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        fdown = Input.GetButtonDown("Fire1");
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        // �ӵ� ���� (Shift Ű �Է¿� ����)
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

        // �÷��̾� ��ġ ������Ʈ
        if (moveVec != Vector3.zero)
        {
            #region ����
            //transform.position += moveVec * speed * Time.deltaTime;
            #endregion
            #region ����
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

        // Space Bar�� ������ ������ ����
        if (Input.GetKeyDown(KeyCode.Space) && moveVec != Vector3.zero)
        {
            RollingForward = transform.forward;
            DecreaseRollSpeed = rollSpeed;
            StartRolling();
        }

        // ���콺 ��ġ�� ���� �÷��̾� ȸ��
        RotatePlayerToMouse();
    }

    private void FixedUpdate()
    {
        player_r.angularVelocity = Vector3.zero;
    }

    private void RotatePlayerToMouse()
    {
        // ������ ���̳� Dazed ���¿����� ȸ������ ����
        if (isRolling || isDazed || isAttacking) return; // ���� �߿��� ȸ������ ����

        #region ����
        // ���콺 ��ġ�� ���� ��ǥ�� ��������
        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //if (Physics.Raycast(ray, out RaycastHit hit))
        //{
        //    Vector3 target = hit.point;
        //    target.y = transform.position.y; // y ��ǥ�� �÷��̾�� �����ϰ� ����
        //    Vector3 direction = (target - transform.position).normalized;

        //    if (direction != Vector3.zero)
        //    {
        //        Quaternion lookRotation = Quaternion.LookRotation(direction);
        //        // �÷��̾� ȸ�� ������Ʈ (�ε巴�� ȸ��)
        //        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * speed);
        //    }
        //}
        #endregion

        #region ����
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
            isAttacking = true; // ���� ����
            equiaWeapon.Use();
            playerAnimator.SetTrigger("DoSwing");
            fireDelay = 0;

            // ���� �ִϸ��̼��� ���� ������ ���
            StartCoroutine(EndAttackAfterAnimation());
        }
    }

    private IEnumerator EndAttackAfterAnimation()
    {
        // �ִϸ��̼� ���̸�ŭ ���
        yield return new WaitForSeconds(playerAnimator.GetCurrentAnimatorStateInfo(0).length);
        isAttacking = false; // ���� ����
    }
}
