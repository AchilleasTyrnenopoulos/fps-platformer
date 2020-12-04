using System;
using System.Collections;
using System.Collections.Generic;

using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Variables
    public static PlayerController instance;//this is a single uninique version of this script, because it is static

    public bool test;

    public CharacterController charCon;
    //[SerializeField] private LayerMask layerMask = new LayerMask();
    //[SerializeField] private LayerMask wallRunClimbLayerMask = new LayerMask();
    [Header("PLAYER ATTRIBUTES")]
    public float strength;//determines the maxHealth, the baseDamage, damage dealt with strength weapons(strModifier), jumpPower, 
    public float agility;//moveSpeed, runSpeed, crouchSpeed, JumpPower, dashingTime, 'dashSpeed', damage dealt with agility type weapons, criticals
    public float endurance;//determines the maxStamina, healthRegenRate, the number of Inventory slots
    public float intelligence;//determines the 'strength' of spells, the maxMana
    public float willpower;//determines the staminaRegenRate, the manaRegenRate

    [Header("MOVEMENT")]
    public float moveSpeed ;
    public float runSpeed ;
    public bool running = false;

    [Space()]
    public Vector3 moveInput;//change to private

    [Space()]
    public Transform camTrans;
    public float mouseSensitivity;

    [Header("Crouching")]
    //handle crouching
    public bool crouching = false;
    public float crouchSpeed;

    [Header("Dashing")]
    public bool abilityCanDash;
    public bool canDash = true;
    public bool dashing = false;
    public float dashingDuration, canDashCooldown, dashingTime = 0.3f, canDashCooldownTime = 2f;
    [Space]
    public bool abilityCanDodge;
    public bool canDodge = true;
    public bool dodging = false;
    public float dodgingDuration, canDodgeCooldown, dodgingTime = 0.1f, canDodgeCooldownTime = 1.5f;


    [Header("Camera Inversion")]
    public bool invertX;
    public bool invertY;
    [Space(10)]

    [Header("JUMPING")]
    public bool abilityCanDoubleJump;
    public float gravityModifier;
    //public float stdGravityModifier = 2f;
    public float jumpPower;
    public bool canJump, canDoubleJump ;//it was private, needs to be changed to private
    public float fallDamageModifier;

    [Space()]
    public Transform groundCheckPoint;
    public LayerMask whatIsGround;
    
    [Space()]
    private float bounceAmount;
    private bool bounce;

    [Header("AMIMATION")]
    public Animator anim;

    
   

    [Header("THROWING")]
    public ThrowingWeapon activeThrWeapon;

    [Header("Throwing Weapons")]
    //public GameObject throwingWeapon;
    public Transform firePoint;
    
    [Space(10)]
    public static bool enableThrowingSystem;
    public GameObject throwingHolderSystem;
    public GameObject meleeHolderSystem;
    public GameObject offhandMeleeHolderSystem;

    //[Header("Movement on Moving Platforms")]    

    //public List<Gun> allGuns = new List<Gun>();
    //public List<Gun> unlockableGuns = new List<Gun>();
    //public int currentGun;

    //handle melee
    //public Melee activeMelee;

    //public Transform adsPoint, gunHolder;
    //private Vector3 gunStartPos;
    //public float adsSpeed = 2f;

    //public GameObject muzzleFlash; //footstepsSlow, footstepsFast;
    //public AudioSource footstepsFast, footstepsSlow;

    public float maxViewAngle = 60f;


    //public float dashSpeed; not needed if handled by agility

    //[Header("Wall Running")]
    //public bool wallRunning = false;
    //public bool wallRight = false, wallLeft = false, /*wallForRight = false*/ /*wallForLeft= false,*/ wallForward = false;
    //RaycastHit hitRight;
    //RaycastHit hitLeft;
    //RaycastHit hitForward;
    ////RaycastHit hitForRight;
    ////RaycastHit hitForLeft;
    ////Vector3 vForLeft = new Vector3();
    ////Vector3 vForRight = new Vector3();
    //Vector3 vForward = new Vector3();
    //Vector3 vLeft = new Vector3();
    //Vector3 vRight = new Vector3();
    //float fHitLeft, fHitRight, fHitForLeft, fHitForRight, fHitForward;
    //public bool rayWallRight = false, rayWallLeft = false,/* rayWallForRight = false, rayWallForLeft = false,*/ rayWallForward = false;
    //public float wallRunCounter = 2f;
    //public float wallRunRate;
    //public float wallRunningCounter = 0.2f;
    //public float wallRunningRate;
    //public bool cantWallRun = false;

    //float rotateSpeed = -15;
    //Vector3 rotationVector;
    //Vector3 camRotationVector;

    //float smooth = 1f;
    //Quaternion targetRotation;

    [Header("Fighting")]
    //public Melee melee;
    public bool attack1;//to help register different damage depending on the attack
    public bool attack2;
    public bool attack3;
    public bool attack4;
    public Collider meleeCollider;
    
    //counter for sprint SpendStamina()
    private float sprintSpendStamina = 0.2f;
    #endregion

    private void Awake()
    {
        instance = this;//as soon as the scene starts, this particular player game object we wanna set it to be the instance that is attached to this script 
    }

    // Start is called before the first frame update
    void Start()
    {
        strength = AttributesManager.instance.globalStrength;
        agility = AttributesManager.instance.globalAgility;
        intelligence = AttributesManager.instance.globalIntelligense;
        endurance = AttributesManager.instance.globalEndurance;
        willpower = AttributesManager.instance.globalWillpower;

       

        //set move speed
        moveSpeed = 2 + (agility / 2);
        runSpeed = moveSpeed * 2f;
        crouchSpeed = moveSpeed / 2;
        //set jump power
        jumpPower = (agility / 2) + (strength / 2);//make jumpPower property to access different 'value' depending on the strength and agility attributes
        
        //set dashing time
        dashingTime = 0.3f - (agility / 100);
        dodgingTime = 0.1f - (agility / 100);//if sDashingTime < 0, sDashingTime = 0.01f;
        //set canDashCooldown 

        //set timers
        dashingDuration = dashingTime;
        canDashCooldown = canDashCooldownTime;
        dodgingDuration = dodgingTime;
        canDodgeCooldown = canDodgeCooldownTime;

        //currentGun--;

        //enableGunSystem = true;

        //SwitchGun();

        //gunStartPos = gunHolder.localPosition;

        //character = GetComponent<CharacterController>;


        //wallRunRate = wallRunCounter;
        //wallRunningRate = wallRunningCounter;

        UIController.instance.ammoText.text = "SHURIKEN: " + activeThrWeapon.currentAmmo;


    }

    // Update is called once per frame
    void Update()
    {
        if (!UIController.instance.pauseScreen.activeInHierarchy && !GameManager.instance.levelEnding)
        {

            //Playdir = PlayerController.instance.transform.position;
            if (canJump == true)
            {
                //PlayerHealthController.instance.StaminaRegen();
            }


            //strore y velocity
            float yStore = moveInput.y;//equals whatever the moveinput.y is at the start of the frame.At the start of the frame will be what we will calculate it to be on the last frame|||must be put before the movement input
                                       //set move speed
            moveSpeed = 2 + (agility / 2);
            runSpeed = moveSpeed * 2;
            crouchSpeed = moveSpeed / 2;

            //******************************************************************************HANDLE MOVING*******************************************************************************************

            //handle movement from keyboard input (WASD etc.)
            Vector3 vertMove = transform.forward * Input.GetAxisRaw("Vertical");//with GetAxisRaw player stops immediately when stop pressisg WASD, other wise use GetAxis
            Vector3 horiMove = transform.right * Input.GetAxisRaw("Horizontal");
            moveInput = horiMove + vertMove;
            moveInput.Normalize();
            

            //running or moving or dashing
            if (!crouching)//otherwise the following run during crouching too
            {
                if (Input.GetKey(KeyCode.LeftShift) && PlayerHealthController.instance.currentStamina >= 2)
                {
                    //reset stamina regeneration counter
                    PlayerHealthController.instance.staminaRegenCounter = PlayerHealthController.instance.staminaRegenRate;

                    moveInput = moveInput * runSpeed;

                    running = true;

                    sprintSpendStamina -= Time.deltaTime;

                    if (sprintSpendStamina <= 0)
                    {
                        PlayerHealthController.instance.SpendStamina(1);
                        sprintSpendStamina = 0.2f;
                    }
                }
                else
                {
                    moveInput = moveInput * moveSpeed;
                    running = false;
                }

                //dashing/dodging
                if (Input.GetKey(KeyCode.F) && canDash && abilityCanDash && PlayerHealthController.instance.currentMana > 1)
                {
                    if (canDash)
                    {
                        PlayerHealthController.instance.SpendMana(10);
                    }

                    canDash = false;
                    dashing = true;
                }
                if (Input.GetKey(KeyCode.CapsLock) && canDodge && abilityCanDodge && PlayerHealthController.instance.currentStamina > 1)
                {
                    PlayerHealthController.instance.SpendStamina(5);
                    canDodge = false;
                    dodging = true;
                }
                if (!canDash)
                {
                    canDashCooldown -= Time.deltaTime;
                }
                if (!canDodge)
                {
                    canDodgeCooldown -= Time.deltaTime;
                }
                if (dashing)
                {
                    //if (PlayerHealthController.instance.notMoving)
                    //{
                    //charCon.Move(-transform.forward * 20 * dashSpeed * Time.deltaTime);
                    //}
                    //else
                    //{
                    charCon.Move(moveInput * agility /** dashSpeed*/ * Time.deltaTime);//needs fix with high agility values, higher agility means faster and longer distance
                    dashingDuration -= Time.deltaTime;
                    //}
                }
                if (dodging)
                {
                    charCon.Move(moveInput * agility * Time.deltaTime);
                    dodgingDuration -= Time.deltaTime;
                }
                //check time to stop dashing 
                if (dashingDuration <= 0)
                {
                    dashingTime = 0.3f - (agility / 100);
                    dashingDuration = dashingTime;
                    dashing = false;
                }
                if (dodgingDuration <= 0)
                {
                    dodgingTime = 0.1f - (agility / 100);
                    dodgingDuration = dodgingTime;
                    dodging = false;
                }
                //check to see if canDash again/ reset the cooldownTimer
                if (canDashCooldown <= 0)
                {
                    canDashCooldown = canDashCooldownTime;
                    canDash = true;
                }
                if (canDodgeCooldown <= 0)
                {
                    canDodgeCooldown = canDodgeCooldownTime;
                    canDodge = true;
                }
            }

            //handle crouching
            if (Input.GetKey(KeyCode.LeftControl))
            {
                crouching = true;
                this.transform.localScale = new Vector3(1f, 0.5f, 1f);
                moveInput = moveInput * crouchSpeed;
                running = false;
            }
            else
            {
                crouching = false;
                this.transform.localScale = new Vector3(1f, 1f, 1f);
            }

            //reapply the ystore back to the moveinput
            moveInput.y = yStore;

            //***********handle gravity*****************
            moveInput.y += Physics.gravity.y * gravityModifier * Time.deltaTime;//generate gravity force
            if (charCon.isGrounded)//check if character is grounded
            {
                moveInput.y = Physics.gravity.y * gravityModifier * Time.deltaTime;
            }

            //take damage when falling
            if (moveInput.y < -20 && Physics.OverlapSphere(groundCheckPoint.position, 1f, whatIsGround).Length > 0)
            {
                PlayerHealthController.instance.DamagePlayer(-Mathf.RoundToInt(moveInput.y * fallDamageModifier));//replace -20 with yDamageVelocity
            }

            //**********************************************************************HANDLE JUMPING**********************************************************************************
            #region Jumping
            //set jump power
            if (running)//jump power is bigger when running, maybe add to be slightly bigger when walking too
            {
                jumpPower = ((agility / 2) + (strength / 2)) * 1.5f;
            }
            else if (!PlayerHealthController.instance.notMoving)
            {
                jumpPower = ((agility / 2) + (strength / 2)) * 1.25f;
            }
            else
            {
                jumpPower = (agility / 2) + (strength / 2);
            }
            //casts imaginary sphere to check if it touches the whatIsGround layers, set canJump bool
            canJump = Physics.OverlapSphere(groundCheckPoint.position, .25f, whatIsGround).Length > 0;
            //jumping
            if (PlayerHealthController.instance.currentStamina >= 5)
            {
                if (Input.GetKeyDown(KeyCode.Space) && canJump)//this is problematic because canJump turns to false as soon as the OverlapSphere stops colliding with the "Ground"
                {
                    if (abilityCanDoubleJump)
                    {
                        canDoubleJump = true;
                    }
                    PlayerHealthController.instance.notMoving = false;

                    moveInput.y = jumpPower;

                    PlayerHealthController.instance.SpendStamina(5);
                  
                    //AudioManager.instance.PlaySFX(8);
                }
                else if (canDoubleJump && Input.GetKeyDown(KeyCode.Space))
                {
                    PlayerHealthController.instance.SpendStamina(5);
                    moveInput.y = jumpPower;

                    canDoubleJump = false;
                    PlayerHealthController.instance.notMoving = false;
                    //AudioManager.instance.PlaySFX(8);
                }
            }

            //handle bounce
            if (bounce)
            {
                bounce = false;
                moveInput.y = bounceAmount;

                canDoubleJump = true;
            }
            #endregion

            charCon.Move(moveInput * Time.deltaTime);//moves the player

            //***************************************HANDLE CAMERA****************************************
            //control camera rotation from mouse input
            Vector2 mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * mouseSensitivity;

            //control camera inversion
            if (invertX)
            {
                mouseInput.x = -mouseInput.x;
            }
            if (invertY)
            {
                mouseInput.y = -mouseInput.y;
            }
            //move camera 
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + mouseInput.x, transform.rotation.eulerAngles.z);
            camTrans.rotation = Quaternion.Euler(camTrans.rotation.eulerAngles + new Vector3(-mouseInput.y, 0f, 0f));

            //***********************************************************************HANDLE SHOOTING********************************************************************************

            if (enableThrowingSystem == true)
            {
                throwingHolderSystem.SetActive(true);
                meleeHolderSystem.SetActive(false);
                {
                    if (Input.GetMouseButtonUp(0) && activeThrWeapon.fireCounter <= 0)
                    {
                        Shoot();
                    }
                }
            }
            else
            {
                throwingHolderSystem.SetActive(false);
                meleeHolderSystem.SetActive(true);
            }
            //handle switch weapon systems
            if (Input.GetKeyDown(KeyCode.Q))
            {
                SwitchWeaponSystems();

            }

            //animations
            anim.SetFloat("moveSpeed", moveInput.magnitude);//if moveSpeed/moveInput.magnitude > 1 trigger Player_Walk animation
            anim.SetBool("onGround", canJump);
            anim.SetFloat("runSpeed", moveInput.magnitude);
            anim.SetBool("hasShuriken", throwingHolderSystem.activeInHierarchy);

            //if (!crouching)
            //{

            //}
            //else
            //{
            //    moveInput = moveInput * crouchSpeed;
            //    running = false;
            //}

            //if (!wallRunning)
            //{
            //handle camera tilt fix
            //rotationVector = transform.rotation.eulerAngles;//not sure if needed
            //rotationVector.z = 0f;
            //transform.rotation = Quaternion.Euler(rotationVector);

            // camTrans.rotation = Quaternion.Euler( new Vector3((-mouseInput.y + camTrans.rotation.eulerAngles.x), 0f, 0f));

            //camTrans.rotation.eulerAngles.z = 0;


            //}
            //else//for wall running
            //{
            //    moveInput.y = jumpPower / 2; 
            //}


            //muted to fix double jump bug
            //if (canJump)
            // {
            //canDoubleJump = false;
            // }


            //}
            


            //camTrans.rotation = Quaternion.Euler(new Vector3((-mouseInput.y + camTrans.rotation.eulerAngles.x), 0f, 0f));

            if (camTrans.rotation.eulerAngles.x > maxViewAngle && camTrans.rotation.eulerAngles.x < 180f)
            {
                camTrans.rotation = Quaternion.Euler(maxViewAngle, camTrans.rotation.eulerAngles.y, camTrans.rotation.eulerAngles.z);
            }
            else if (camTrans.rotation.eulerAngles.x > 180f && camTrans.rotation.eulerAngles.x < 360f - maxViewAngle)
            {
                camTrans.rotation = Quaternion.Euler(-maxViewAngle, camTrans.rotation.eulerAngles.y, camTrans.rotation.eulerAngles.z);
            }

            //muzzleFlash.SetActive(false);

            #region Walking on Moving Platforms
            /* If Player is touching a MovingPlatform 
             * 'lock' players movement vector to the MovingPlatform 
             */
            #endregion


            //Debug.Log("fHitRight " + fHitRight + "\n " + "fHitLeft " + fHitLeft + "   " + "fHitForward " + fHitForward);

            //handle wall running
            //shooting raycasts
            //if (running && cantWallRun == false)
            //{
            //    //rayWallLeft = Physics.Raycast(camTrans.position, -PlayerController.instance.transform.right, out hitLeft, 2f, wallRunClimbLayerMask);
            //    //rayWallRight = Physics.Raycast(camTrans.position, PlayerController.instance.transform.right, out hitRight, 2f, wallRunClimbLayerMask);
            //    //rayWallForward = Physics.Raycast(camTrans.position, PlayerController.instance.transform.forward, out hitForward, 2f, wallRunClimbLayerMask);
            //    //rayWallForLeft = Physics.Raycast(camTrans.position, (PlayerController.instance.transform.forward - PlayerController.instance.transform.right) * Mathf.Sqrt(8) / 2, out hitForLeft, 2f, wallRunClimbLayerMask);
            //    //rayWallForRight = Physics.Raycast(camTrans.position, (PlayerController.instance.transform.forward + PlayerController.instance.transform.right) * Mathf.Sqrt(8) / 2, out hitForRight, 2f, wallRunClimbLayerMask);

            //    if (rayWallLeft)
            //    {

            //        Debug.Log("Left Raycast hit wall");
            //        fHitLeft = Vector3.Distance(PlayerController.instance.transform.position, hitLeft.point);
            //        wallRunning = true;
            //        wallLeft = true;



            //    }
            //    else
            //    {
            //        wallLeft = false;
            //    }



            //    if (rayWallRight)
            //    {
            //        Debug.Log("Right Raycast hit wall");
            //        fHitRight = Vector3.Distance(PlayerController.instance.transform.position, hitRight.point);
            //        wallRunning = true;
            //        wallRight = true;

            //    }
            //    else
            //    {
            //        wallRight = false;
            //    }



            //    if (rayWallForward )
            //    {
            //        Debug.Log("Forward Raycast hit wall");
            //        fHitForward = Vector3.Distance(PlayerController.instance.transform.position, hitForward.point);
            //        wallRunning = true;
            //        wallForward = true;

            //    }
            //    else
            //    {
            //        wallForward = false;
            //    }




            //    //if (rayWallForRight)
            //    //{
            //    //    Debug.Log("ForRight Raycast hit wall");
            //    //    fHitForRight = Vector3.Distance(PlayerController.instance.transform.position, hitForRight.point);
            //    //}




            //    //if (rayWallForLeft)
            //    //{
            //    //    Debug.Log("ForLeft Raycast hit wall");

            //    //    fHitForLeft = Vector3.Distance(PlayerController.instance.transform.position, hitForLeft.point);
            //    //}

            //    //if (rayWallRight || rayWallLeft || rayWallForward || rayWallForRight || rayWallForLeft)
            //    //{
            //    //    float fHitMin = Mathf.Min(fHitRight, fHitLeft, fHitForLeft, fHitForRight, fHitForward);


            //    //    if (fHitMin == fHitLeft)
            //    //    {
            //    //        wallRight = false;
            //    //        wallLeft = true;
            //    //        wallForward = false;
            //    //        wallForRight = false;
            //    //        wallForLeft = false;
            //    //        wallRunning = true;
            //    //    }
            //    //    else if (fHitMin == fHitForRight)
            //    //    {
            //    //        wallRight = false;
            //    //        wallLeft = false;
            //    //        wallForward = false;
            //    //        wallForRight = true;
            //    //        wallForLeft = false;
            //    //        wallRunning = true;
            //    //    }
            //    //    else if (fHitMin == fHitForLeft)
            //    //    {
            //    //        wallRight = false;
            //    //        wallLeft = false;
            //    //        wallForward = false;
            //    //        wallForRight = false;
            //    //        wallForLeft = true;
            //    //        wallRunning = true;
            //    //    }
            //    //    else
            //    //    {
            //    //        wallRight = false;
            //    //        wallLeft = false;
            //    //        wallForward = true;
            //    //        wallForRight = false;
            //    //        wallForLeft = false;
            //    //        wallRunning = true;
            //    //    }
            //    //}


            //}
            //else
            //{
            //    //wallForLeft = false;
            //    //wallForRight = false;
            //    wallForward = false;
            //    wallLeft = false;
            //    wallRight = false;
            //    wallRunning = false;
            //    //rayWallForLeft = false;
            //    //rayWallForRight = false;
            //    rayWallForward = false;
            //    rayWallRight = false;
            //    rayWallLeft = false;
            //}

            //if (wallRunning )
            //{
            //    wallRunningRate -= Time.deltaTime;



            //    //if (wallForLeft)
            //    //{
            //    //    vForLeft = Vector3.Cross(hitForLeft.normal, Vector3.up);
            //    //    charCon.Move(vForLeft * runSpeed * Time.deltaTime);
            //    //   // moveInput.y
            //    //}
            //    //else if(wallForRight)
            //    //{
            //    //    vForRight = Vector3.Cross(hitForRight.normal, Vector3.up);
            //    //    charCon.Move(vForRight * runSpeed * Time.deltaTime);
            //    //}
            //    //else if (wallForward)
            //    //{
            //    //    vForward = Vector3.Cross(hitForward.normal, Vector3.up);
            //    //    charCon.Move(vForward * runSpeed * Time.deltaTime);
            //    //}
            //    if (wallLeft)
            //    {
            //        vLeft = Vector3.Cross(hitLeft.normal, Vector3.up);
            //        charCon.Move(vLeft * runSpeed * Time.deltaTime);

            //        //rotationVector = transform.rotation.eulerAngles;
            //        //rotationVector.z = -15 ;
            //        //transform.rotation = Quaternion.Euler(rotationVector);

            //        //targetRotation *= Quaternion.AngleAxis(50, Vector3.forward);
            //        //transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 10*smooth*Time.deltaTime);
            //        float angle = rotateSpeed * Time.deltaTime *2;
            //        transform.rotation *= Quaternion.AngleAxis(angle, Vector3.forward);



            //    }
            //    else if(wallRight)
            //    {
            //        vRight = Vector3.Cross(hitRight.normal, Vector3.up);
            //        charCon.Move(-vRight * runSpeed * Time.deltaTime);
            //    }
            //    else
            //    {
            //        vForward = Vector3.Cross(hitForward.normal, Vector3.up);
            //        charCon.Move(vForward * runSpeed * Time.deltaTime);


            //    }


            //}

            //if (wallRunningRate <= 0)
            //{
            //    wallRunningRate = wallRunningCounter;
            //    wallRunning = false;
            //    cantWallRun = true;

            //    //rotationVector = transform.rotation.eulerAngles;//not sure if needed
            //    //rotationVector.z = 0f;
            //    //transform.rotation = Quaternion.Euler(rotationVector);

            //    //camRotationVector.z = 0f;
            //    //camRotationVector.y = 0f;
            //    //camTrans.rotation = Quaternion.Euler(camRotationVector);
            //    //camTrans.rotation = Quaternion.Euler(camTrans.rotation.eulerAngles + new Vector3(-mouseInput.y, 0f, 0f));




            //}

            //if (cantWallRun)
            //{
            //    wallRunRate -= Time.deltaTime;
            //}


            //if (wallRunRate <= 0)
            //{
            //    wallRunRate = wallRunCounter;
            //    cantWallRun = false;
            //}


            // hitRight.normal

            //Handle Shooting
            //single shots
            //if (enableGunSystem == true)
            //{
            //    gunHolderSystem.SetActive(true);
            //    if (Input.GetMouseButtonDown(0) && activeGun.fireCounter <= 0 && activeGun.canAutoFire == false)
            //    {
            //        RaycastHit hit;
            //        if (Physics.Raycast(camTrans.position, camTrans.forward, out hit, 50f/*, layerMask*/))
            //        {

            //            if (Vector3.Distance(camTrans.position, hit.point) > 1f)
            //            {
            //                firePoint.LookAt(hit.point);
            //            }

            //        }

            //        else
            //        {
            //            firePoint.LookAt(camTrans.position + (camTrans.forward * 30f));
            //        }


            //        //Instantiate(bullet, firePoint.position, firePoint.rotation);
            //        FireShot();
            //    }

            //    //repeating shots
            //    if (Input.GetMouseButton(0) && activeGun.canAutoFire)
            //    {
            //        if (activeGun.fireCounter <= 0)
            //        {
            //            RaycastHit hit;
            //            if (Physics.Raycast(camTrans.position, camTrans.forward, out hit, 50f/*, layerMask*/))
            //            {

            //                if (Vector3.Distance(camTrans.position, hit.point) > 2f)
            //                {
            //                    firePoint.LookAt(hit.point);
            //                }
            //                else
            //                {
            //                    firePoint.LookAt(camTrans.position + (camTrans.forward * 2f));
            //                    Debug.Log("ray cast targetting innactive");
            //                }

            //            }

            //            else
            //            {
            //                firePoint.LookAt(camTrans.position + (camTrans.forward * 30f));
            //                //Debug.Log("HEYYYYYYYYYYYYYYYYYYY");
            //            }


            //            FireShot();
            //        }
            //    }

            //    //reload
            //    if (Input.GetKey(KeyCode.R))
            //    {
            //        activeGun.Reload();
            //    }

            //    //gun swapping this needs fix
            //    if (Input.GetKeyDown(KeyCode.Tab) && !Input.GetMouseButton(1))
            //    {

            //        SwitchGun();

            //    }

            //    //gun swapping with 1 2 3 4
            //    if (Input.GetKeyDown("1"))
            //    {
            //        SwitchWeapon1();
            //    }

            //    if (Input.GetKeyDown("2"))
            //    {
            //        SwitchWeapon2();
            //    }

            //    if (Input.GetKeyDown("3"))
            //    {
            //        SwitchWeapon3();
            //    }

            //    if (Input.GetKeyDown("4"))
            //    {
            //        SwitchWeapon4();
            //    }


            //    if (Input.GetMouseButtonDown(1))
            //    {
            //        CameraController.instance.ZoomIn(activeGun.zoomAmount);
            //    }

            //    if (Input.GetMouseButton(1))
            //    {
            //        gunHolder.position = Vector3.MoveTowards(gunHolder.position, adsPoint.position, adsSpeed * Time.deltaTime);
            //    }
            //    else
            //    {
            //        gunHolder.localPosition = Vector3.MoveTowards(gunHolder.localPosition, gunStartPos, adsSpeed * Time.deltaTime);
            //    }

            //    if (Input.GetMouseButtonUp(1))
            //    {
            //        CameraController.instance.ZoomOut();
            //    }
            //}
            //else
            //{
            //    gunHolderSystem.SetActive(false);
            //}





            //}
            //else
            //{
            //    footstepsFast.SetActive(false);
            //    footstepsSlow.SetActive(false);
            //}

        } 

    }

    #region Functions
    

    //public void Shoot()
    //{
    //    if (activeThrWeapon.currentAmmo > 0)
    //    {
    //        if (activeThrWeapon.clipCurrent > 0)
    //        {
    //            activeThrWeapon.currentAmmo--;
    //            activeThrWeapon.clipCurrent--;

    //            Instantiate(activeThrWeapon., firePoint.position, firePoint.rotation);

    //            activeThrWeapon.fireCounter = activeThrWeapon.fireRate;

    //            UIController.instance.ammoText.text = activeThrWeapon.clipCurrent + "/" + activeThrWeapon.remainAmmo + "/" + activeThrWeapon.clips;

    //            //muzzleFlash.SetActive(true);

    //        }

    //    }

    //}

    public void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(camTrans.position, camTrans.forward, out hit, 50f)) //add layerMask
        {
            if (Vector3.Distance(camTrans.position, hit.point) > 2f)
            {
                firePoint.LookAt(hit.point);
            }
            firePoint.LookAt(hit.point);
        }
        else
        {
            firePoint.LookAt(camTrans.position + camTrans.forward * 30f);
        }
        if (activeThrWeapon.currentAmmo > 0)
        {
            activeThrWeapon.currentAmmo--;
            Instantiate(activeThrWeapon.shuriken, firePoint.position, firePoint.rotation);
            activeThrWeapon.fireCounter = activeThrWeapon.fireRate;

            UIController.instance.ammoText.text = "SHURIKEN: " + activeThrWeapon.currentAmmo;
        }
    }

    //public void SwitchGun()
    //{
    //    activeGun.gameObject.SetActive(false);

    //    currentGun++;

    //    if (currentGun >= allGuns.Count)
    //    {
    //        currentGun = 0;
    //    }

    //    activeGun = allGuns[currentGun];
    //    activeGun.gameObject.SetActive(true);

    //    UIController.instance.ammoText.text = activeGun.clipCurrent + "/" + activeGun.remainAmmo + "/" + activeGun.clips;

    //    firePoint.position = activeGun.firepoint.position;
    //}

    ////Switch Weapons with 1 2 3 4 Functions, need fix
    //public void SwitchWeapon1()
    //{
    //    activeGun.gameObject.SetActive(false);

    //    activeGun = allGuns[0];
    //    activeGun.gameObject.SetActive(true);

    //    UIController.instance.ammoText.text = activeGun.clipCurrent + "/" + activeGun.remainAmmo + "/" + activeGun.clips;

    //    firePoint.position = activeGun.firepoint.position;
    //}

    //public void SwitchWeapon2()
    //{
    //    activeGun.gameObject.SetActive(false);

    //    activeGun = allGuns[1];
    //    activeGun.gameObject.SetActive(true);

    //    UIController.instance.ammoText.text = activeGun.clipCurrent + "/" + activeGun.remainAmmo + "/" + activeGun.clips;

    //    firePoint.position = activeGun.firepoint.position;
    //}

    //public void SwitchWeapon3()
    //{
    //    activeGun.gameObject.SetActive(false);

    //    activeGun = allGuns[2];
    //    activeGun.gameObject.SetActive(true);

    //    UIController.instance.ammoText.text = activeGun.clipCurrent + "/" + activeGun.remainAmmo + "/" + activeGun.clips;

    //    firePoint.position = activeGun.firepoint.position;
    //}

    //public void SwitchWeapon4()
    //{
    //    activeGun.gameObject.SetActive(false);

    //    activeGun = allGuns[3];
    //    activeGun.gameObject.SetActive(true);

    //    UIController.instance.ammoText.text = activeGun.clipCurrent + "/" + activeGun.remainAmmo + "/" + activeGun.clips;

    //    firePoint.position = activeGun.firepoint.position;
    //}

    //public void AddGun(string gunToAdd)
    //{
    //    //Debug.Log("Adding " + gunToAdd);
    //    bool gunUnlocked = false;

    //    if (unlockableGuns.Count > 0)
    //    {
    //        for (int i = 0; i < unlockableGuns.Count; i++)
    //        {
    //            if (unlockableGuns[i].gunName == gunToAdd)
    //            {
    //                gunUnlocked = true;

    //                allGuns.Add(unlockableGuns[i]);

    //                unlockableGuns.RemoveAt(i);

    //                i = unlockableGuns.Count;
    //            }
    //        }

    //    }

    //    if (gunUnlocked)
    //    {
    //        currentGun = allGuns.Count - 2;
    //        SwitchGun();
    //    }
    //}

    public void SwitchWeaponSystems()
    {
        if (enableThrowingSystem == true)
        {
            enableThrowingSystem = false;
            meleeHolderSystem.SetActive(true);
            offhandMeleeHolderSystem.SetActive(true);
        }
        else
        {
            enableThrowingSystem = true;
            meleeHolderSystem.SetActive(false);
            offhandMeleeHolderSystem.SetActive(false);
        }
    }

    public void Bounce(float bounceForce)
    {
        bounceAmount = bounceForce;
        bounce = true;
    }

    //handle sword collider while attacking
    public void DisableSwordCollider()
    {
       meleeCollider.isTrigger = false;
    }
    //set-trigger-to-true function to use as an animation event 

    public void SetAttackingFalse()
    {
        anim.SetBool("attacking", false);
    }
    public void SetAttacking2False()
    {
        anim.SetBool("attacking2", false);
    }
    public void SetAttacking3False()
    {
        anim.SetBool("attacking3", false);
    }
    public void SetAttacking4False()
    {
        anim.SetBool("attacking4", false);
    }

    public void SetAtk1ToFalse()
    {
        attack1 = false;
    }
    public void SetAtk2ToFalse()
    {
        attack2 = false;
    }
    public void SetAtk3ToFalse()
    {
        attack3 = false;
    }
    public void SetAtk4ToFalse()
    {
        attack4 = false;
    }

   

    #endregion
}
