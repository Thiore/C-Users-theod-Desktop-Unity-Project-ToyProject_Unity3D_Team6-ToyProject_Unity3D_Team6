using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Gunner : MonoBehaviour
{
    [SerializeField] public float walkSpeed = 100f;
    [SerializeField] public float runSpeed = 150f;
    [SerializeField] public float rollSpeed = 100f;
    [SerializeField] public float rollDuration = 0.5f;
    [SerializeField] public float dazeDuration = 0.5f; // �̵��� ���ϴ� �ð�
    private AudioSource player_audio;

    #region ���� / ���콺 ����
    [Range(100, 1000)]
    [SerializeField] public float MouseSpeed = 500f;
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
    private bool isRolling = false;
    private bool isDazed = false;
    //private bool isAttacking = false; // ���� �� ���� ���� �߰�
    private bool isColliding = false; // �浹 ���� ���� �߰�
    private float add = 50f;
    #endregion

    Vector3 moveVec;
    Vector3 RollingForward;

    [SerializeField] private Weapon equiaWeapon;

    private Animator playerAnimator;
    private Rigidbody player_r;

    private Player_Health player_Health;

    private Coroutine movementSoundCoroutine;
    #region ���� �Ѹ�/daze?����(���� : �ִϸ��̼� anystate�� bool������ ���ͼ� ������ �������� �ִ��ſ����ϴ�.trigger�� �����س��ڽ��ϴ�.)
    private bool isZeroDuration = false;
    private float AnimDuration = 0f;
    private AnimatorStateInfo AnimInfo;
    private Coroutine Rolling_Coroutine = null;
    private Coroutine Daze_Coroutine = null;
    private Coroutine isAttacking = null;
    #endregion
    #region ���� - Gunner
    public Weapon[] weapons; //���� ���� (HandGun, Submachinegun)
   private bool sDown1; //1�����
    private bool sDown2; //2�����
    private Weapon equipWeapon; //���� ������ ���� ���� ����
    private bool isSwap = false;
    //[SerializeField] private Weapon equiWeapon;
    #endregion

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
        equipWeapon = weapons[0];
        speed = walkSpeed;
        fireDelay = 0;
    }

    private void Update()
    {
        AnimInfo = playerAnimator.GetCurrentAnimatorStateInfo(0);
        if (player_Health.isDie)
        {
            return;
        }
        //���� �ѵ����̶� �������� ��� �ȱ� �Ǵ� �ٴ� ���� �����ϱ�
        if (Rolling_Coroutine != null || Daze_Coroutine != null || isAttacking != null)
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

        // ������ ���� ��
        if (Rolling_Coroutine != null)
        {
            // ������ ���� �̵� ������Ʈ
            DecreaseRollSpeed -= Time.deltaTime * 30f;
            Vector3 movePos = transform.position + RollingForward * moveVec.z * DecreaseRollSpeed * Time.deltaTime;
            player_r.MovePosition(movePos);
            return; // ������ ���ȿ��� �ٸ� �Է��� ó������ ����
        }

        // ���� ���� ��
        if (isAttacking != null)
        {
            return; // ���� �߿��� �ٸ� �Է��� ó������ ����
        }

        // �浹 ���� ��
        if (isColliding)
        {
            return; // �浹 �߿��� �ٸ� �Է��� ó������ ����
        }

        // �̵� �Է� �ޱ�
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        fdown = Input.GetButton("Fire1");
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
            player_audio.PlayOneShot(roll_sound);
            RollingForward = transform.forward;
            DecreaseRollSpeed = rollSpeed;
            //StartDazed();
            StartRolling();
        }
        Attack(); // ���� ó��
        // ���콺 ��ġ�� ���� �÷��̾� ȸ��
        RotatePlayerToMouse();

        //Gun
        
        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2");
        Swap();
    }
    //private void GetInput()
    //{
    //    sDown1 = Input.GetButtonDown("Swap1");
    //    sDown2 = Input.GetButtonDown("Swap2");
    //}

    private void Swap()
    {

        int weaponIndex = -1;
        if (sDown1) weaponIndex = 0;
        if (sDown2) weaponIndex = 1;

        if ((sDown1 || sDown2) && !isDazed)
        {
            //if(equipWeapon.activeSelf)
                equipWeapon.gameObject.SetActive(false); //�տ� �̹� ����ֱ� ������
            equipWeapon = weapons[weaponIndex];
            equipWeapon.gameObject.SetActive(true);

            playerAnimator.SetTrigger("doSwap");

            isSwap = true;

            Invoke("SwapOut", 0.5f);
        }
    }



    private void SwapOut()
    {
       
        isSwap = false;
    
    }
    private void FixedUpdate()
    {
        player_r.angularVelocity = Vector3.zero;
    }

    private void RotatePlayerToMouse()
    {
        // ������ ���̳� Dazed ���¿����� ȸ������ ����
        if (Rolling_Coroutine != null || Daze_Coroutine != null || isAttacking != null) return; // ���� �߿��� ȸ������ ����

        float mouseX = Input.GetAxis("Mouse X");
        transform.Rotate(transform.up, mouseX * Time.deltaTime * MouseSpeed);
    }

    private void StartRolling()
    {
        isRolling = true;
        rollStartTime = Time.time;
        playerAnimator.SetTrigger("Rolling");
    }

    private void StartDazed()
    {
        isDazed = true;
        dazeStartTime = Time.time;
        playerAnimator.SetTrigger("Dazed");
    }

    private void Attack()
    {
        if (equipWeapon == null)
        {
            return;
        }
            //Debug.Log("dd");
            

        fireDelay += Time.deltaTime;
        isFireReady = equipWeapon.rate < fireDelay;

        if (fdown && isFireReady && !isDazed && !isSwap)
        {

            Debug.Log("���");
            equipWeapon.Use();
            playerAnimator.SetTrigger("doShot");
            fireDelay = 0;
            player_audio.PlayOneShot(swing_sound);
            StartCoroutine(EndAttackAfterAnimation());
        }
    }
    //private void Attack()
    //{
    //    fireDelay += Time.deltaTime;
    //    isFireReady = (equipWeapon.rate < fireDelay);

    //    if (fdown && isFireReady && !isRolling && !isDazed)
    //    {
    //        isAttacking = true; // ���� ����
    //        equipWeapon.Use();
    //        playerAnimator.SetTrigger("doShot");
    //        fireDelay = 0;

    //        player_audio.PlayOneShot(swing_sound);
    //        // ���� �ִϸ��̼��� ���� ������ ���
    //        StartCoroutine(EndAttackAfterAnimation());
    //    }
    //}

    //private IEnumerator EndAttackAfterAnimation()
    //{
    //    // �ִϸ��̼� ���̸�ŭ ���
    //    yield return new WaitForSeconds(playerAnimator.GetCurrentAnimatorStateInfo(0).length);
    //    isAttacking = null; // ���� ����
    //}
    private IEnumerator EndAttackAfterAnimation()
    {
        isZeroDuration = false;
        playerAnimator.SetTrigger("doShot");
        player_audio.PlayOneShot(swing_sound);
        while (!isZeroDuration)
        {

            if (AnimInfo.IsName("doShot"))
            {
                isZeroDuration = true;
                AnimDuration = AnimInfo.length;

            }
            yield return null;
        }
        yield return new WaitForSeconds(AnimDuration - (AnimDuration * 0.1f));
        isZeroDuration = false;
        isAttacking = null;
        yield break;
    }
    private void OnCollisionEnter(Collision collision)
    {
        // �浹�� ������Ʈ�� prop �±׸� ������ �ִ��� Ȯ��
        if (collision.gameObject.CompareTag("prop") && Daze_Coroutine == null)
        {
            Debug.Log("�浹");

            // �÷��̾��� ���� ������ ����Ͽ� �ݴ�� ���� ����
            Vector3 collisionDirection = -player_r.velocity;
            player_r.AddForce(collisionDirection * 1.2f, ForceMode.Force);


            Daze_Coroutine = StartCoroutine(EndCollisionAfterDaze());
        }
        if (collision.gameObject.CompareTag("Enemy"))
        {
            //�߰��ؾ���~
        }
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
