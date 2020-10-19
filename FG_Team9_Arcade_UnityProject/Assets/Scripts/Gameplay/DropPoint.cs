﻿using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[RequireComponent(typeof(BoxCollider),typeof(CapsuleCollider))]
public class DropPoint : MonoBehaviour
{
    public float radiusOfDeliveryBullseye = 10f;
    [Tooltip("Should be set to the visual cylinder that is the beacon of the point")]
    public Transform visualBeacon;

    public bool isPickupPoint = false;
    public bool isDeliveryPoint = false;

    [Header("Debug")]
    
    public bool debugRadius = false;
    public Color debugColor = Color.red;

    [NonSerialized] public bool currentlyActiveDeliveryPoint = false;
    [NonSerialized] public bool currentlyActivePickupPoint = false;

    PickupManager _pickupManager;
    HealthSystem _healthSystem;
    Vector2 _position2D;
    CapsuleCollider _capsuleCollider;
    BoxCollider _boxCollider;

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (debugRadius)
        {
            Handles.color = debugColor;
            Handles.DrawWireDisc(transform.position, Vector3.up, radiusOfDeliveryBullseye);
        }
    }
#endif

    void Awake()
    {
        _pickupManager = PickupManager.Instance;
        _healthSystem = GameManager.Player.GetComponentInChildren<HealthSystem>();
        
        _position2D = new Vector2(transform.position.x, transform.position.z);
    }
    

    void OnValidate()
    {
        if (_pickupManager == null)
        {
            _pickupManager = PickupManager.Instance;
        }
        
        if (!isDeliveryPoint && _pickupManager.deliveryPoints.Contains(this))
        {
            _pickupManager.deliveryPoints.Remove(this);
        }
        
        if (!isPickupPoint && _pickupManager.pickupPoints.Contains(this))
        {
            _pickupManager.pickupPoints.Remove(this);
        }

        if (isDeliveryPoint && !_pickupManager.deliveryPoints.Contains(this))
        {
            _pickupManager.deliveryPoints.Add(this);
        }
        
        if (isPickupPoint && !_pickupManager.pickupPoints.Contains(this))
        {
            _pickupManager.pickupPoints.Add(this);
        }
        
        if (_capsuleCollider == null)
        {
            _capsuleCollider = GetComponent<CapsuleCollider>();
        }

        if (_boxCollider == null)
        {
            _boxCollider = GetComponent<BoxCollider>();
        }

        _capsuleCollider.radius = radiusOfDeliveryBullseye;
        _boxCollider.size = new Vector3(radiusOfDeliveryBullseye * 2f, _boxCollider.size.y, radiusOfDeliveryBullseye * 2f);
        visualBeacon.localScale = new Vector3(radiusOfDeliveryBullseye * 2f, visualBeacon.localScale.y,
            radiusOfDeliveryBullseye * 2f);
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("Pickup") && currentlyActiveDeliveryPoint) // if pickup contacts with base plate collider
        {
            Vector3 pickupPos = other.transform.position;
            Vector2 pickupPos2D = new Vector2(pickupPos.x, pickupPos.z);

            float distance = Vector2.Distance(pickupPos2D, _position2D);

            float bullsEyePoints = Mathf.InverseLerp(radiusOfDeliveryBullseye, 0f, distance);
            
            _healthSystem.enabled = false;
            
            if (_healthSystem.currentHealth <= 0f)
            {
                _pickupManager.FailDelivery();
            }
            else
            {
                _pickupManager.SuccessfulDelivery(bullsEyePoints);
            }
        }
    }
}