using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TankCharacterController
{
    private Tank tank;
    private float gravity = -9.81f;
    private float gravityMultiplier = 0.01f;
    private float velocity = 0.0f;

    public TankCharacterController(Tank tank)
    {
        this.tank = tank;
    }

    public void RayCastTank()
    {
        RaycastHit hit;
        Vector3 origin = tank.transform.position;
        Vector3 direction = -tank.transform.up;

        Physics.Raycast(origin, direction, out hit, tank.wheelMaxDist);
        int layer = hit.transform.gameObject.layer;

        if (hit.collider == null || layer  >= 7)
        {
            return;
        }

        Vector3 normal = hit.normal;
        Vector3 axis = Vector3.Cross(Vector3.up, normal);

        if (axis == Vector3.zero)
        {
            axis = tank.Body.transform.forward;
        }

        Quaternion targetRotation = Quaternion.LookRotation(Vector3.Normalize(axis), normal);

        tank.Body.transform.rotation = Quaternion.Slerp(tank.Body.transform.rotation, targetRotation, Time.deltaTime * 20.0f);
    }

    // Source: https://www.youtube.com/watch?v=2UhJRoUMfMAv
    public void GravityFall()
    {
        if (tank.characterController.isGrounded && velocity < 0.0f)
        {
            velocity = -1.0f * Time.deltaTime;
        } else
        {
            velocity += gravity * gravityMultiplier * Time.deltaTime;
        }

        tank.characterController.Move(Vector3.up * velocity);
    }

    public void MoveTank(Vector2 dir)
    {
        Vector3 playerInput = dir.x * tank.forward + -dir.y * tank.right;
        Vector3 direction = Vector3.Normalize(playerInput);

        // insert zach's code to rotate the body thing

        tank.characterController.Move(tank.moveSpeed * direction * Time.deltaTime);
    }
}
