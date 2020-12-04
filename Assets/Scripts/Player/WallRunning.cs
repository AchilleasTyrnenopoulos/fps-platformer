using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class WallRunning : MonoBehaviour
{
    public bool canWallRun = true;

    public bool wallRunning = false;

    public float wallRunForce;
    public Transform orientation;

    [SerializeField] private LayerMask wallRunClimbLayerMask = new LayerMask();

    bool isWallLeft = false;
    bool isWallRight = false;

    float mass = 3.0f; // defines the character mass
    Vector3 impact = Vector3.zero;

    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        //anim = this.GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {


        CheckForWalls();

        WallRunInput();

        //if (wallRunning)
        //    anim.enabled = true;

        

        //end of update

    }


    #region Functions

    private void WallRunInput()
    {
        if (PlayerController.instance.running && canWallRun)
        {
            if (isWallLeft || isWallRight )
            {
                EnterWallRun();
                //PlayerController.instance.anim.enabled = false;
                //anim.enabled = true;
                
            }
        }
    }

    private void EnterWallRun()
    {        

        PlayerController.instance.gravityModifier = 0.3f;
        wallRunning = true;
        //anim.enabled = true;

        AddImpact(orientation.forward, wallRunForce * Time.deltaTime);

        if (isWallLeft)
        {
            AddImpact(-orientation.right, wallRunForce / 5 * Time.deltaTime);
            anim.SetBool("leftWallRun", true);
        }
        else if (isWallRight)
        {
            AddImpact(orientation.right, wallRunForce / 5 * Time.deltaTime);
            anim.SetBool("rightWallRun", true);
        }
    }

    private void ExitWallRun()
    {
        if (SpellsController.instance.graved)
            PlayerController.instance.gravityModifier = 2 / SpellsController.instance.gravReduction;
        else
            PlayerController.instance.gravityModifier = 2;//problem with antigravity spell
    
        wallRunning = false;
        anim.SetBool("leftWallRun", false);
        anim.SetBool("rightWallRun", false);
        Invoke("DisableAnim", 1);
    }

    private void CheckForWalls()
    {
       isWallLeft = Physics.Raycast(PlayerController.instance.camTrans.position, -PlayerController.instance.transform.right, 2f, wallRunClimbLayerMask);
       isWallRight = Physics.Raycast(PlayerController.instance.camTrans.position, PlayerController.instance.transform.right, 2f, wallRunClimbLayerMask);      

        //leave wall run
        if ((!isWallLeft && !isWallRight) || !Input.GetKey(KeyCode.W))
            ExitWallRun();
        //reset double jump
    }

    // call this function to add an impact force:
    public void AddImpact(Vector3 dir, float force)
    {
        dir.Normalize();
        if (dir.y < 0) dir.y = -dir.y; // reflect down force on the ground
        impact += dir.normalized * force / mass;
    }

    public void DisableAnim()
    {
        anim.enabled = false;
        PlayerController.instance.anim.enabled = true;
    }
    
    #endregion
}