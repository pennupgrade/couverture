using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStateMachine : MonoBehaviour
{
    // Basic State Information
    private State currentState;
    private Attack currentAttack;

    // Other Parameters
    private double timer;
    private GameObject player;
    [SerializeField] private Boss boss;

    // Constants
    private double timeUntilAttack = 5;
    private const double MELEE_DISTANCE = 4f;
    private const double TIME_BETWEEN_BULLETS = 0.5f;

    // Attack Specific Parameters
    private int numBulletsShot;


    private void Awake()
    {
        currentState = State.Idle;
        currentAttack = Attack.None;
        timer = 0;

        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.Log("Could not find player");
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (currentState == State.Idle)
        {
            if (timer > timeUntilAttack)
            {
                switchToState(State.Attacking);
            }
            doIdleState();
        }
        else if (currentState == State.Attacking)
        {
            doAttackState();
        }
    }

    private void switchToState(State newState)
    {
        currentState = newState;
        if (newState == State.Idle)
        {
            Debug.Log("Switching To Idle State");
            timer = 0;
            currentAttack = Attack.None;
            return;
        }
        if (newState == State.Attacking)
        {
            Debug.Log("Switching To Attack State");
            if (Vector3.Distance(player.transform.position, transform.position) < MELEE_DISTANCE)
            {
                // Chose between shotgun and melee
                currentAttack = Attack.Shotgun;
            }
            else
            {
                // Choose between summon, shoot, and charge
                currentAttack = Attack.Shoot;
                timer = TIME_BETWEEN_BULLETS;
                numBulletsShot = 0;
            }
        }
    }

    private void doIdleState()
    {
        // Track Onto Player
    }

    private void doAttackState()
    {
        switch (currentAttack)
        {
            // Shoots 3 shots in rapid succession. When done, switches back to idle.
            case Attack.Shoot:
                if (timer > TIME_BETWEEN_BULLETS)
                {
                    timer = 0;
                    boss.shootBullet();
                    numBulletsShot++;
                    if (numBulletsShot >= 3)
                    {
                        switchToState(State.Idle);
                    }
                }
                break;
            case Attack.Shotgun:
                boss.shootShotgun();
                switchToState(State.Idle);
                
                break;
        }
    }


    private enum State
    {
        Idle,
        Attacking
    }

    private enum Attack
    {
        Shotgun,
        Shoot,
        Charge,
        Summon,
        Melee,
        None
    }
}
