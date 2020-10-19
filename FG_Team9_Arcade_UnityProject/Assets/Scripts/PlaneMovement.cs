using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Win32;
using UnityEngine;

public class PlaneMovement : MonoBehaviour
{
    [Header("Setup")] 
    public Transform mouseTransform = default;
    public Transform cameraArm;
    public float mouseSpeed = 1f;
    
    [Header("Gas / Forward thrusters (local z-axis)")] 
    public float forwardAcceleration = 2f;
    public float maxForwardSpeed = 10f;
    public float deAcceleration = 1f;
    public float minimumSpeed = 0f;
    
    

    [Header("Aim slerp")] // todo optional: change from slerp to customizable animation curve
    public float mouseReticleMoveToFrontReticleValue = 3f;
    public float frontReticleMoveToMouseReticleValue = 5f;

    [Header("Constant forward speed (m/s)")]
    public float constantForwardSpeed = 5f;
    
    
    Vector3 _forwardVelocity = Vector3.zero;
    
    float _currentSpeed = 0;
    
    void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    /// <summary>
    /// Moves player forward with a velocity.
    /// zInput scales the velocity.
    /// </summary>
    /// <param name="zInput"></param>
    public void MoveShip(float zInput)
    {
        Vector3 forwardDir = transform.forward;
        
        if (zInput > 0f)
        {
            _currentSpeed = Mathf.MoveTowards(_currentSpeed, maxForwardSpeed, forwardAcceleration * Time.fixedDeltaTime);
        }
        else
        {
            _currentSpeed = Mathf.MoveTowards(_currentSpeed, minimumSpeed, deAcceleration * Time.fixedDeltaTime);
        }
        
        Vector3 newVelocity = forwardDir * _currentSpeed;
        transform.position += newVelocity * Time.fixedDeltaTime;
        
        Debug.DrawLine(transform.position, transform.position + newVelocity);
        
        
        //transform.Translate(newVelocity * Time.fixedDeltaTime);
        
        // calculate forward velocity
        // float forwardSpeed = zInput * maxForwardSpeed;
        // Vector3 desiredForwardVelocity = forwardDir * forwardSpeed;
        //
        // _forwardVelocity = Vector3.MoveTowards(_forwardVelocity, desiredForwardVelocity,
        //     maxForwardAcceleration * Time.fixedDeltaTime);
        //
        // print("Velocity: " + _forwardVelocity.ToString());
        //
        // // add constant speed
        // _forwardVelocity += forwardDir * constantForwardSpeed;
        //
        // // move player with velocity
        // Vector3 intendedVelocity = _forwardVelocity;
        //
        // transform.Translate(intendedVelocity * Time.fixedDeltaTime, Space.World);
        
        //Debug.DrawLine(transform.position, transform.position + intendedVelocity);
        
        
    }
    
    
    /// <summary>
    /// Rotates the "arm" that holds the mouse crosshair
    /// </summary>
    public void RotateMousePos(float deltaMouseX, float deltaMouseY)
    {
        // todo clamp rotation
        Vector3 rotation = new Vector3(-deltaMouseY * mouseSpeed, deltaMouseX * mouseSpeed, 0f);

        mouseTransform.Rotate(rotation, Space.Self);
    }

    /// <summary>
    /// Rotates the arm that holds the camera to point towards the mouse crosshair
    /// </summary>
    public void MoveCamera()
    {
        // todo optional: Smooth the camera
        
        cameraArm.rotation = Quaternion.LookRotation(mouseTransform.forward, Vector3.up);
    }

    
    public void RotateShip()
    {
        Vector3 mousePosTarget = mouseTransform.position + mouseTransform.forward * 10f;

        Vector3 targetInLocalSpace = transform.InverseTransformPoint(mousePosTarget);
        Vector3 targetDirection = targetInLocalSpace.normalized;


        Vector3 shipForward = transform.forward;
        Vector3 mouseDir = mouseTransform.forward;
        
        
        Vector3 newMouseDir = Vector3.Slerp(mouseDir, shipForward, mouseReticleMoveToFrontReticleValue * Time.deltaTime);
        mouseTransform.rotation = Quaternion.LookRotation(newMouseDir, Vector3.up);

        Vector3 newShipForward = Vector3.Slerp(shipForward, newMouseDir, frontReticleMoveToMouseReticleValue * Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(newShipForward, Vector3.up);
    }
} 
