using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Move : MonoBehaviour
{
    [SerializeField] public float walkSpeed = 100f;
    [SerializeField] public float runSpeed = 150f;
    [SerializeField] public float rollSpeed = 200f;
    [SerializeField] public float rollDuration = 0.5f;
    [SerializeField] public float dazeDuration = 0.5f; // �̵��� ���ϴ� �ð�

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
        // �ڽ� ������Ʈ���� Animator ������Ʈ�� ã�� �Ҵ�
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
            // Dazed ������ ��
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

        if (isRolling)
        {
            // ������ ���� ��
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
                transform.position += moveVec * rollSpeed * Time.deltaTime;
                return; // ������ ���ȿ��� �ٸ� �Է��� ó������ ����
            }
        }

        // �̵� �Է� �ޱ�
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
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
            transform.position += moveVec * speed * Time.deltaTime;
            playerAnimator.SetBool("isWalking", true);
        }
        else
        {
            playerAnimator.SetBool("isWalking", false);
        }

        // Space Bar�� ������ ������ ����
        if (Input.GetKeyDown(KeyCode.Space) && moveVec != Vector3.zero)
        {
            StartRolling();
        }

        // ���콺 ��ġ�� ���� �÷��̾� ȸ��
        RotatePlayerToMouse();
    }

    private void RotatePlayerToMouse()
    {
        // ������ ���̳� Dazed ���¿����� ȸ������ ����
        if (isRolling || isDazed) return;

        // ���콺 ��ġ�� ���� ��ǥ�� ��������
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 target = hit.point;
            target.y = transform.position.y; // y ��ǥ�� �÷��̾�� �����ϰ� ����
            Vector3 direction = (target - transform.position).normalized;

            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                // �÷��̾� ȸ�� ������Ʈ (�ε巴�� ȸ��)
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
