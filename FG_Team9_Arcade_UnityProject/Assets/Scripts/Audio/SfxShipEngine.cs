﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class SfxShipEngine : MonoBehaviour
{
    public bool isPlaneMovementActivated;
    public bool isAccelerationActivated;

    public AudioClip engineSFXNormalFlight;
    public AudioClip engineSFXHoverMode;
    public AudioClip engineSFXAfterburnerMode;

    private AudioSource _audioSource;
    public AudioSource hoveraudioSource;
    public Player player;
    public PlayerMovement movement;



    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        player = GetComponentInParent<Player>();
        movement = GetComponentInParent<PlayerMovement>();
    }


    // TO DO Change system based 
    private void FixedUpdate()
    {
        isAccelerationActivated = Input.GetKey(KeyCode.W);

        if (player.usePlaneMovement)
        {
            if (!isPlaneMovementActivated)
            {
                SetHoverFlightMode(false);
                isPlaneMovementActivated = true;
            }

            if (isAccelerationActivated)
            {
                ActivateAfterBurner(true);
            }
            else
            {
                ActivateAfterBurner(false);
            }
        }
        else
        {
            if (isPlaneMovementActivated)
            {
                SetHoverFlightMode(true);
                isPlaneMovementActivated = false;
            }
        }

       
    }

    public void ActivateAfterBurner(bool activate)
    {
        if (activate && _audioSource.clip != engineSFXAfterburnerMode)
        {
            _audioSource.clip = engineSFXAfterburnerMode;
            _audioSource.Play();
        }
        else if(!activate && _audioSource.clip != engineSFXNormalFlight)
        {
            _audioSource.clip = engineSFXNormalFlight;
            _audioSource.Play();
        }
    }

    public void SetHoverFlightMode(bool activate)
    {
        if (activate)
        {
            _audioSource.Stop();
            hoveraudioSource.clip = engineSFXHoverMode;
            hoveraudioSource.Play();
        }
        else
        {
            hoveraudioSource.Stop();
            _audioSource.clip = engineSFXNormalFlight;
            _audioSource.Play();
        }

    }
}