using System;
using UnityEngine;

[RequireComponent(typeof(HelicopterMovement), typeof(MouseMovement))]
public class PlayerMovement : MonoBehaviour
{

    [Header("Setup")] 
    public Transform transformToYaw;
    public Transform transformToPitch;
    public Transform bodyTransform;

    public Transform targetPivot = default;

    public LayerMask mouseControlMask;
    
    MouseMovement _mouseMovement;
    HelicopterMovement _helicopterMovement;
    
    public static float CurrentForwardSpeed = 0;
    protected static float CurrentUpWardsSpeed = 0;
    protected static float CurrentDownWardsSpeed = 0;
    protected static float CurrentRightSpeed = 0;

    protected Camera _cam;
    protected Rigidbody _rigidbody;
    
    public bool _usingPlaneMovement = true;

    [NonSerialized] public bool resetSwapTimer = false;


    protected Quaternion _originalBodyRotation;
    
    public void ResetSwapTimer() 
    {
        if (_usingPlaneMovement)
        {
            _mouseMovement.ResetSwapTimer1();
        }
        else
        {
            _helicopterMovement.ResetSwapTimer1();
        }
    }
    
    void Awake()
    {
        _cam = Camera.main;
        
        _rigidbody = GetComponent<Rigidbody>();
        
        // movement
        _helicopterMovement = GetComponent<HelicopterMovement>();
        _mouseMovement = GetComponent<MouseMovement>();
        //
        _originalBodyRotation = bodyTransform.rotation;
    }

    public float MinForwardSpeed()
    {
        return _usingPlaneMovement ? _mouseMovement.minForwardSpeed : 0f;
    }
    
    public float MaxForwardSpeed()
    {
        return _usingPlaneMovement ? _mouseMovement.maxForwardSpeed : _helicopterMovement.maxForwardSpeed;
    }
    
    void RotateShip(float xAxisInput)
    {
        if (_usingPlaneMovement)
        {
            _mouseMovement.Rotate(xAxisInput);
        }
        else
        {
            _helicopterMovement.Rotate();
        }
    }

    
    public void MoveShip(float xAxisInput, float yAxisInput, float zAxisInput, bool useMouseMovement)
    {
        _usingPlaneMovement = useMouseMovement;
        
        RotateShip(xAxisInput);
        
        if (useMouseMovement)
        {
            _mouseMovement.MoveShip(xAxisInput, yAxisInput, zAxisInput);
        }
        else
        {
            _helicopterMovement.MoveShip(xAxisInput, yAxisInput, zAxisInput);
        }
    }
}
