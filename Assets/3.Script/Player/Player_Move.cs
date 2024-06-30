using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Move : MonoBehaviour
{
    [SerializeField] public float walkSpeed = 100f;
    [SerializeField] public float runSpeed = 150f;
   
    [SerializeField] public float rollSpeed = 100f;
    #region ���� rolling����
    //[SerializeField] public float rollDuration = 0.5f;
    //[SerializeField] public float dazeDuration = 0.5f; // �̵��� ���ϴ� �ð�
    #endregion
    #region ���� �Ѹ�/daze?����(���� : �ִϸ��̼� anystate�� bool������ ���ͼ� ������ �������� �ִ��ſ����ϴ�.trigger�� �����س��ڽ��ϴ�.)
    private bool isZeroDuration = false;
    private float AnimDuration = 0f;
    private AnimatorStateInfo AnimInfo;
    private Coroutine Rolling_Coroutine = null;
    private Coroutine Daze_Coroutine = null;
    private Coroutine Swing_Coroutine = null;
    #endregion
    #region ���� / ���콺 ����
    [Range(100, 1000)]
    [SerializeField] public float MouseSpeed = 500f;

    #endregion
    #region Audio_Clip
    [SerializeField] private AudioClip swing_sound;
    [SerializeField] private AudioClip crash_sound;
    [SerializeField] private AudioClip roll_sound;
    [SerializeField] private AudioClip walk_sound;
    [SerializeField] private AudioClip run_sound;
    #endregion

    #region Ŭ���� ����
    private float speed;
    private float DecreaseRollSpeed;
    private float hAxis;
    private float vAxis;
    private float fireDelay;
    private float rollStartTime;
    private float dazeStartTime;
    private bool fdown;
    private bool isFireReady;
    //private bool isRolling = false;
    //private bool isDazed = false;
    //private bool isAttacking = false; // ���� �� ���� ���� �߰�
    private bool isColliding = false; // �浹 ���� ���� �߰�
    private float add = 50f;
    #endregion

    Vector3 moveVec;
    Vector3 RollingForward;

    [SerializeField] private Weapon equiaWeapon;

    private AudioSource player_audio;
    private Animator playerAnimator;
    private Rigidbody player_r;

    private Player_Health player_Health;

    private Coroutine movementSoundCoroutine;

    private void Start()
    {
        // �ڽ� ������Ʈ���� Animator ������Ʈ�� ã�� �Ҵ�
        playerAnimator = GetComponentInChildren<Animator>();
        player_r = GetComponent<Rigidbody>();
        player_Health = GetComponent<Player_Health>();
        player_audio = GetComponent<AudioSource>();
        if (playerAnimator == null)
        {
            Debug.LogError("Animator component not found in children.");
        }

        speed = walkSpeed;
    }

    private void Update()
    {
        #region ���� ���� �ִϸ��̼� ������ ���� �����
        AnimInfo = playerAnimator.GetCurrentAnimatorStateInfo(0);
        #endregion

        if (player_Health.isDie)
        {
            return;
        }

        //���� �ѵ����̶� �������� ��� �ȱ� �Ǵ� �ٴ� ���� �����ϱ�
        if (Rolling_Coroutine != null || Daze_Coroutine != null || Swing_Coroutine != null)
        {
            if (movementSoundCoroutine != null)
            {
                StopCoroutine(movementSoundCoroutine);
                movementSoundCoroutine = null;
            }
        }

            // Dazed ������ ��
            if (Daze_Coroutine != null)
        {
            return; // Dazed ���� ���ȿ��� �ٸ� �Է��� ó������ ����
            
        }

        //��������
        if(Rolling_Coroutine != null)
        {
            // ������ ���� �̵� ������Ʈ
            DecreaseRollSpeed -= Time.deltaTime * 30f;
            Vector3 movePos = transform.position + RollingForward * moveVec.z * DecreaseRollSpeed * Time.deltaTime;
            player_r.MovePosition(movePos);

            return; // ������ ���ȿ��� �ٸ� �Է��� ó������ ����
        }

        // ���� ���� ��
        if (Swing_Coroutine != null)
        {
            return; // ���� �߿��� �ٸ� �Է��� ó������ ����
        }

        //// �浹 ���� ��
        //if (isColliding)
        //{
        //    return; // �浹 �߿��� �ٸ� �Է��� ó������ ����
        //}

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
            Vector3 moveDir = transform.right * moveVec.x + transform.forward * moveVec.z;
            Vector3 movePos = transform.position + moveDir * speed * Time.deltaTime;
            player_r.MovePosition(movePos);
            playerAnimator.SetBool("isWalking", true);

            if (movementSoundCoroutine == null)
            {
                movementSoundCoroutine = StartCoroutine(PlayMovementSound());
            }
        }
        else
        {
            playerAnimator.SetBool("isWalking", false);

            if (movementSoundCoroutine != null)
            {
                StopCoroutine(movementSoundCoroutine);
                movementSoundCoroutine = null;
            }
        }

        // Space Bar�� ������ ������ ����
        if (Input.GetKeyDown(KeyCode.Space) && moveVec != Vector3.zero)
        {
            
            StartRolling();
        }
        
            Attack(); // ���� ó��
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
        if (Rolling_Coroutine != null || Daze_Coroutine != null || Swing_Coroutine != null) return; // ���� �߿��� ȸ������ ����

        float mouseX = Input.GetAxis("Mouse X");
        transform.Rotate(transform.up, mouseX * Time.deltaTime * MouseSpeed);
    }

    private void StartRolling()
    {
       
        Rolling_Coroutine = StartCoroutine(Rolling_co());

        
    }
    private IEnumerator Rolling_co()
    {
        isZeroDuration = false;
        playerAnimator.SetTrigger("Rolling");
        player_audio.PlayOneShot(roll_sound);
        RollingForward = transform.forward;
        DecreaseRollSpeed = rollSpeed;        
        while (!isZeroDuration)
        {

            if (AnimInfo.IsName("Dodge"))
            {
                isZeroDuration = true;
                AnimDuration = AnimInfo.length;

            }
            yield return null;
        }
        yield return new WaitForSeconds(AnimDuration - (AnimDuration * 0.1f));
        isZeroDuration = false;
        Rolling_Coroutine = null;
        yield break;

    }
    private void Attack()
    {
        //fireDelay += Time.deltaTime;
        //isFireReady = (equiaWeapon.rate < fireDelay);

        if (fdown && Swing_Coroutine == null && Rolling_Coroutine == null && Daze_Coroutine == null)
        {
            //isAttacking = true; // ���� ����
            equiaWeapon.Use();
            //fireDelay = 0;

            
            // ���� �ִϸ��̼��� ���� ������ ���
            Swing_Coroutine = StartCoroutine(EndAttackAfterAnimation());
        }
    }

    private IEnumerator EndAttackAfterAnimation()
    {
        isZeroDuration = false;
        playerAnimator.SetTrigger("Swing");
        player_audio.PlayOneShot(swing_sound);
        while (!isZeroDuration)
        {

            if (AnimInfo.IsName("Swing"))
            {
                isZeroDuration = true;
                AnimDuration = AnimInfo.length;

            }
            yield return null;
        }
        yield return new WaitForSeconds(AnimDuration - (AnimDuration * 0.1f));
        isZeroDuration = false;
        Swing_Coroutine = null;
        yield break;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // �浹�� ������Ʈ�� prop �±׸� ������ �ִ��� Ȯ��
        if (collision.gameObject.CompareTag("prop")&&Daze_Coroutine == null)
        {
            Debug.Log("�浹");

            // �÷��̾��� ���� ������ ����Ͽ� �ݴ�� ���� ����
            Vector3 collisionDirection = -player_r.velocity;
            player_r.AddForce(collisionDirection*1.1f, ForceMode.Force);
           

            Daze_Coroutine = StartCoroutine(EndCollisionAfterDaze());
        }
        if (collision.gameObject.CompareTag("Enemy"))
        {
            //�߰��ؾ���~
        }
    }
    public void Land()
    {

    }

    private IEnumerator EndCollisionAfterDaze()
    {
        isZeroDuration = false;
        playerAnimator.SetTrigger("Dazed");
        player_audio.PlayOneShot(crash_sound);
       
        while (!isZeroDuration)
        {

            if (AnimInfo.IsName("Land"))
            {
                isZeroDuration = true;
                AnimDuration = AnimInfo.length;

            }
            yield return null;
        }
        yield return new WaitForSeconds(AnimDuration - (AnimDuration * 0.1f));
        isZeroDuration = false;
        Daze_Coroutine = null;
        yield break;
    }

    private IEnumerator PlayMovementSound()
    {
        while (true)
        {
            if (speed == runSpeed)
            {
                player_audio.PlayOneShot(run_sound);
                yield return new WaitForSeconds(run_sound.length);
            }
            else
            {
                player_audio.PlayOneShot(walk_sound);
                yield return new WaitForSeconds(0.8f);
            }
        }
    }
}
