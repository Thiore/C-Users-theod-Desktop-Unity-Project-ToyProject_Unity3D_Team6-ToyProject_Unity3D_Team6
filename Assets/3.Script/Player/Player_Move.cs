using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Move : MonoBehaviour
{
    [SerializeField] public float walkSpeed = 100f;
    [SerializeField] public float runSpeed = 150f;
    private float speed;
    private float hAxis;
    private float vAxis;
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

        // ���콺 ��ġ�� ���� �÷��̾� ȸ��
        RotatePlayerToMouse();
    }

    private void RotatePlayerToMouse()
    {
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
}
