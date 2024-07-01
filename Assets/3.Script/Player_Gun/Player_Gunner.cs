using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Gunner : MonoBehaviour
{
    [SerializeField] public float walkSpeed = 100f;
    [SerializeField] public float runSpeed = 150f;
    [SerializeField] public float rollSpeed = 100f;
    [SerializeField] public float rollDuration = 0.5f;
    [SerializeField] public float dazeDuration = 0.5f; // 이동을 못하는 시간
    private AudioSource player_audio;

    #region 영훈 / 마우스 감도
    [Range(100, 1000)]
    [SerializeField] public float MouseSpeed = 500f;
    [SerializeField] private AudioClip swing_sound;
    [SerializeField] private AudioClip crash_sound;
    [SerializeField] private AudioClip roll_sound;
    [SerializeField] private AudioClip walk_sound;
    [SerializeField] private AudioClip run_sound;
    #endregion

    #region 클래스 변수
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
    //private bool isAttacking = false; // 공격 중 상태 변수 추가
    private bool isColliding = false; // 충돌 상태 변수 추가
    private float add = 50f;
    #endregion

    Vector3 moveVec;
    Vector3 RollingForward;

    [SerializeField] private Weapon equiaWeapon;

    private Animator playerAnimator;
    private Rigidbody player_r;

    private Player_Health player_Health;

    private Coroutine movementSoundCoroutine;
    #region 영훈 롤링/daze?수정(이유 : 애니메이션 anystate에 bool값으로 들어와서 앞으로 몸던지고 있던거였습니다.trigger로 수정해놓겠습니다.)
    private bool isZeroDuration = false;
    private float AnimDuration = 0f;
    private AnimatorStateInfo AnimInfo;
    private Coroutine Rolling_Coroutine = null;
    private Coroutine Daze_Coroutine = null;
    private Coroutine isAttacking = null;
    #endregion
    #region 승주 - Gunner
    public Weapon[] weapons; //스왑 장비들 (HandGun, Submachinegun)
   private bool sDown1; //1번장비
    private bool sDown2; //2번장비
    private Weapon equipWeapon; //기존 장착된 무기 저장 변수
    private bool isSwap = false;
    //[SerializeField] private Weapon equiWeapon;
    #endregion

    private void Start()
    {
        // 자식 오브젝트에서 Animator 컴포넌트를 찾아 할당
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
        //셋중 한동작이라도 진행중일 경우 걷기 또는 뛰는 사운드 종료하기
        if (Rolling_Coroutine != null || Daze_Coroutine != null || isAttacking != null)
        {
            if (movementSoundCoroutine != null)
            {
                StopCoroutine(movementSoundCoroutine);
                movementSoundCoroutine = null;
            }
        }

        // Dazed 상태일 때
        if (Daze_Coroutine != null)
        {
            return; // Dazed 상태 동안에는 다른 입력을 처리하지 않음

        }

        // 구르는 중일 때
        if (Rolling_Coroutine != null)
        {
            // 구르는 동안 이동 업데이트
            DecreaseRollSpeed -= Time.deltaTime * 30f;
            Vector3 movePos = transform.position + RollingForward * moveVec.z * DecreaseRollSpeed * Time.deltaTime;
            player_r.MovePosition(movePos);
            return; // 구르는 동안에는 다른 입력을 처리하지 않음
        }

        // 공격 중일 때
        if (isAttacking != null)
        {
            return; // 공격 중에는 다른 입력을 처리하지 않음
        }

        // 충돌 중일 때
        if (isColliding)
        {
            return; // 충돌 중에는 다른 입력을 처리하지 않음
        }

        // 이동 입력 받기
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        fdown = Input.GetButton("Fire1");
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

        // Space Bar를 누르면 구르기 시작
        if (Input.GetKeyDown(KeyCode.Space) && moveVec != Vector3.zero)
        {
            player_audio.PlayOneShot(roll_sound);
            RollingForward = transform.forward;
            DecreaseRollSpeed = rollSpeed;
            //StartDazed();
            StartRolling();
        }
        Attack(); // 공격 처리
        // 마우스 위치를 향해 플레이어 회전
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
                equipWeapon.gameObject.SetActive(false); //손에 이미 들려있기 때문에
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
        // 구르는 중이나 Dazed 상태에서는 회전하지 않음
        if (Rolling_Coroutine != null || Daze_Coroutine != null || isAttacking != null) return; // 공격 중에도 회전하지 않음

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

            Debug.Log("사용");
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
    //        isAttacking = true; // 공격 시작
    //        equipWeapon.Use();
    //        playerAnimator.SetTrigger("doShot");
    //        fireDelay = 0;

    //        player_audio.PlayOneShot(swing_sound);
    //        // 공격 애니메이션이 끝날 때까지 대기
    //        StartCoroutine(EndAttackAfterAnimation());
    //    }
    //}

    //private IEnumerator EndAttackAfterAnimation()
    //{
    //    // 애니메이션 길이만큼 대기
    //    yield return new WaitForSeconds(playerAnimator.GetCurrentAnimatorStateInfo(0).length);
    //    isAttacking = null; // 공격 종료
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
        // 충돌한 오브젝트가 prop 태그를 가지고 있는지 확인
        if (collision.gameObject.CompareTag("prop") && Daze_Coroutine == null)
        {
            Debug.Log("충돌");

            // 플레이어의 진행 방향을 계산하여 반대로 힘을 가함
            Vector3 collisionDirection = -player_r.velocity;
            player_r.AddForce(collisionDirection * 1.2f, ForceMode.Force);


            Daze_Coroutine = StartCoroutine(EndCollisionAfterDaze());
        }
        if (collision.gameObject.CompareTag("Enemy"))
        {
            //추가해야함~
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
