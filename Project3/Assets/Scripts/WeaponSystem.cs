﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSystem : MonoBehaviour
{
    public GameObject rocketPrefab;
    public GameObject ImpactEffect;
    public AudioClip ImpactNoise;

    public GameObject targeter;

    //range of the weapon
    //used externally for targeting reticule
    public static int Range = 300;
    public int Damage = 10;

    //weapon vars
    private LineRenderer _gunLine;
    private AudioSource _gunAudio;
    private Light _gunLight;

    //utils
    private float _timer;
    private float _rocketTimer;
    private float _timeBetweenShots = 0.1f;
    private float _timeBetweenRockets = 2.0f;
    private float _effectDisplayTime = 0.2f;

    private GameObject target;

    public Image OnScreenSprite;
    public Image OffScreenSprite;

    private Image[] onScreenSprites;

    private Image[] offScreenSprites;
    //public List<GameObject> objects;

    //public Vector3 objectPoolPos;
    //Vector3 screenCenter = new Vector3(Screen.width,Screen.height,0)*.5f;

    void Start()
    {
//		onScreenSprites = new Image[objects.Count];
//		offScreenSprites = new Image[objects.Count];
//		Debug.Log ("Center: " + screenCenter);
//		for(var i = 0; i < objects.Count; i++)
//		{
//			onScreenSprites[i] = Instantiate(OnScreenSprite,objectPoolPos,transform.rotation);
//			offScreenSprites[i] = Instantiate(OffScreenSprite,objectPoolPos, transform.rotation);
//		}
    }

    private void Awake()
    {
        _gunLine = GetComponent<LineRenderer>();
        _gunAudio = GetComponent<AudioSource>();
        _gunLight = GetComponent<Light>();
    }

    void Update()
    {
        PlaceIndicators();
        _timer += Time.deltaTime;
        _rocketTimer += Time.deltaTime;
        if (Input.GetButton("Fire1") && _timer >= _timeBetweenShots && !PauseController.isGamePaused)
        {
            Shoot();
        }

        if (Input.GetButton("Fire2") && _rocketTimer >= _timeBetweenRockets && !PauseController.isGamePaused)
        {
            ShootRocket();
        }

        if (_timer >= _timeBetweenShots * _effectDisplayTime)
        {
            DisableEffects();
        }
    }

    void PlaceIndicators()
    {
        var objects = FindObjectsOfType(typeof(Targetable)) as GameObject[];

        Debug.Log(objects);

//        foreach (var go in objects)
//        {
//            Vector3 screenpos = Camera.main.WorldToScreenPoint(go.transform.position);
//
//            //if onscreen
//            if (screenpos.z > 0 && screenpos.x < Screen.width && screenpos.x > 0 && screenpos.y < Screen.height &&
//                screenpos.y > 0)
//            {
//                OnScreenSprite.rectTransform.position = screenpos;
//                //Debug.Log("OnScreen: " + screenpos);
//            }
//            else
//            {
//                PlaceOffscreen(screenpos);
//            }
//        }
    }


    void PlaceOffscreen(Vector3 screenpos)
    {
        float x = screenpos.x;
        float y = screenpos.y;
        float offset = 10;

        if (screenpos.z < 0)
        {
            screenpos = -screenpos;
        }

        if (screenpos.x > Screen.width)
        {
            x = Screen.width - offset;
        }

        if (screenpos.x < 0)
        {
            x = offset;
        }

        if (screenpos.y > Screen.height)
        {
            y = Screen.height - offset;
        }

        if (screenpos.y < 0)
        {
            y = offset;
        }

        OffScreenSprite.rectTransform.position = new Vector3(x, y, 0);
    }

    public void SwitchWeapons()
    {
        //stub
    }

    public void Shoot()
    {
        _timer = 0f;

        Ray shootRay = new Ray();
        RaycastHit hit = new RaycastHit();


        _gunLight.enabled = true;
        _gunLine.enabled = true;

        _gunAudio.Play();

        _gunLine.SetPosition(0, transform.position);

        shootRay.origin = transform.position;
        shootRay.direction = transform.forward;

        //https://docs.unity3d.com/ScriptReference/Physics.BoxCast.html
        //https://docs.unity3d.com/ScriptReference/Renderer-bounds.html
    }

    public void ShootRocket() {
        _rocketTimer = 0f;

        Debug.Log(transform.forward);

        Quaternion q = Quaternion.FromToRotation(transform.position, targeter.transform.position);

        GameObject Rocket = GameObject.Instantiate(rocketPrefab, transform.position, q);
        Rocket.GetComponent<GuidedRocket>().target = this.target;
    }

    public void ApplyHit(RaycastHit hit, int damage)
    {
        Instantiate(ImpactEffect, hit.point, Quaternion.identity);
        AudioSource.PlayClipAtPoint(ImpactNoise, hit.point);
        var t = hit.collider.gameObject.GetComponent<Targetable>();
        if (t != null)
            t.Damage(damage);
    }

    public void DisableEffects()
    {
        _gunLight.enabled = false;
        _gunLine.enabled = false;
        _gunLine.SetPosition(1, transform.position + transform.forward * Range);
    }
}