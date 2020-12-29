﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyRangedController : MonoBehaviour
{
    #region Variables
    public Transform player;

    
    Animator anim;
    NavMeshAgent agent;

    float rotationSpeed = 2.0f;
    
    [Header("LINE OF SIGHT")]
    public float visDist = 20.0f;
    public float visAngle = 30.0f;
    public float shootDist = 15.0f;

    

    bool isIdle = true;
    bool isMoving = false;
    bool isAttacking = false;

    //change to enum 
    string state = "IDLE";

    //patrol
    Transform goal;
    float patrolAccuracy = 1.0f;
    public GameObject wpManager;
    GameObject[] wps;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();

        anim = GetComponentInChildren<Animator>();
        state = "IDLE";

        wps = wpManager.GetComponent<WPManager>().waypoints;
    }

    // Update is called once per frame
    void Update()
    {
        //calculate direction from npc to player
        Vector3 direction = player.position - this.transform.position;

        //calculate the angle from the direction (of npc to player) and the 'facing forwrd' of npc
        float angle = Vector3.Angle(direction, this.transform.forward);

        //test if npc sees player
        if (direction.magnitude < visDist && angle < visAngle)
        {
            //set direction y to zero (so npc wont tilt)
            direction.y = 0;

            //turn npc to player
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);

            //test if in range to shoot
            if (direction.magnitude > shootDist)
            {
                if (state != "RUNNING")
                {
                    state = "RUNNING";
                    isIdle = false;
                    isAttacking = false;
                    isMoving = true;

                }
            }
            else
            {
                if (state != "SHOOTING")
                {
                    state = "SHOOTING";
                    isIdle = false;
                    isMoving = false;
                    isAttacking = true;
                }
            }
        }
        else
        {
            if (state != "IDLE")
            {
                state = "IDLE";
                isMoving = false;
                isAttacking = false;
                isIdle = true;
            }

        }

        //running state
        if (state == "RUNNING")
        {
            //this.transform.Translate(0, 0, Time.deltaTime * speed);
            agent.SetDestination(player.transform.position);
        }
        else 
        { 
            agent.SetDestination(this.transform.position); 
        }

        //handle animations
        anim.SetBool("isIdle", isIdle);
        anim.SetBool("isMoving", isMoving);        
        anim.SetBool("isAttacking", isAttacking);
    }

    //FUNCTIONS
    void TriggerAnimParameter(bool isDoing)
    {
        bool[] anims = {isIdle, isMoving, isAttacking};
        string isDoingStr = isDoing.ToString();
        for (int i = 0; i < anims.Length; i++)
        {
            string tempBool = anims[i].ToString();
            if (tempBool != isDoingStr)
                anims[i] = false;
        }

        isDoing = true;
    }
}
