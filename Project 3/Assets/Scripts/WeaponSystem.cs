﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSystem : MonoBehaviour {
	
	public GameObject ship;

    public GameObject impactEffect;
    public AudioClip impactNoise;
	
	//range of the weapon
	//used externally for targeting reticule
	public static int range = 300;
	public int damage = 10;

    //weapon vars
    LineRenderer gunLine;
    AudioSource gunAudio;
    Light gunLight;

    //utils
    float timer;
    float timeBetweenShots = 0.1f;
    float effectDisplayTime = 0.2f;

    private void Awake() {

        gunLine = GetComponent<LineRenderer>();
        gunAudio = GetComponent<AudioSource>();
        gunLight = GetComponent<Light>();

    }

    void Update () {
        timer += Time.deltaTime;

		if(Input.GetButton("Fire1") && timer >= timeBetweenShots && !PauseController.isGamePaused){
			Shoot();
		}

        if (timer >= timeBetweenShots * effectDisplayTime) {
            DisableEffects();
        }
	}
	
	
	///////////
	//UTILITY//
	///////////
	
	public bool IsObjectInRange() {
		//stub for hit/range detection - implement later
		return true;
	}

    public void SwitchWeapons() {
        //stub
    }


    public void Shoot() {
        timer = 0f;

        Ray shootRay = new Ray();
        RaycastHit hit = new RaycastHit();


        gunLight.enabled = true;
        gunLine.enabled = true;

        gunAudio.Play();

        gunLine.SetPosition(0, transform.position);

        shootRay.origin = transform.position;
        shootRay.direction = transform.forward;
		
	    //https://docs.unity3d.com/ScriptReference/Physics.BoxCast.html
	    //https://docs.unity3d.com/ScriptReference/Renderer-bounds.html
	    
	    Debug.Log("Shoot");

	    var boxCast = Physics.BoxCast(transform.position, transform.localScale, transform.forward, out hit, transform.rotation, 300f);
	    if (boxCast)
	    {
		    Debug.Log("Hit : " + hit.collider.name);
	    }


	    if (Physics.Raycast(shootRay, out hit, range)) {
            gunLine.SetPosition(1, hit.point);
            ApplyHit(hit, damage);
        }

        else {
            gunLine.SetPosition(1, shootRay.origin + shootRay.direction * range);
        }
	}

	public void ApplyHit(RaycastHit hit, int damage) {
        GameObject.Instantiate(impactEffect, hit.point, Quaternion.identity);
        AudioSource.PlayClipAtPoint(impactNoise, hit.point);
        Targetable t = hit.collider.gameObject.GetComponent<Targetable>();
        if (t != null)
            t.Damage(damage);
	}

    public void DisableEffects() {
        gunLight.enabled = false;
        gunLine.enabled = false;
        gunLine.SetPosition(1, transform.position + transform.forward * range); 
    }
}
