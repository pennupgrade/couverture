using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    private State currentState;
    private double timer;
    private double timeUntilAttack;
    private Attack currentAttack;
    private const double MELEE_DISTANCE = 1f;
    private GameObject player;


    private void Awake()
    {
        currentState = State.Idle;
        currentAttack = Attack.None;
        timer = 0;
        timeUntilAttack = 5;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.Log("Could not find player");
        }
    }

    private void Update()
    {
        if (currentState == State.Idle)
        {
            timer += Time.deltaTime;
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
        if (newState == State.Idle)
        {
            timer = 0;
            return;
        }
        if (newState == State.Attacking)
        {
            if (Vector3.Distance(player.transform.position, transform.position) > MELEE_DISTANCE)
            {
                // Choose between Shotgun and Melee
            }
            else
            {
                // Choose between summon, shoot, and charge
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
