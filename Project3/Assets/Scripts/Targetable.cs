﻿using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


public class Targetable : MonoBehaviour {

    public float Health;

    public ParticleSystem Explosion; 
    public AudioClip ExplosionNoise;
    
    public GameObject TargetPrefab;
    public GameObject TargetLockPrefab;
    public int TargetRangeFromPlayer = 250;
    private Image _targetBox;
    public Image targetLock;

    private static List<Targetable> _possibleTargetLocks = new List<Targetable>();
    public static Targetable closestTarget = null;

    private void Start()
    {
        _targetBox = Instantiate(TargetPrefab, FindObjectOfType<Canvas>().transform).GetComponent<Image>();
        targetLock = Instantiate(TargetLockPrefab, FindObjectOfType<Canvas>().transform).GetComponent<Image>();
        targetLock.enabled = false;
        _targetBox.enabled = false;
    }

    private void Update()
    {
        if (IsDead()) {
            StartCoroutine("Death");
            return;
        }
        Target();
        FindPossibleTargetLock();
    }

    private bool PreconditionTarget()
    {    
        var pos =  Camera.main.WorldToScreenPoint(transform.position);
        if (Vector3.Distance(transform.position, Player.PlayerPosition) > TargetRangeFromPlayer) return false;
        return pos.z > 0 && !IsDead();
    }

    public static void TargetLock()
    {
        var distance = 99999f;
        if (closestTarget != null) closestTarget.targetLock.enabled = false;
        foreach (var possibleTargetLock in _possibleTargetLocks)
        {
            if (possibleTargetLock == null) continue;
            var distToPlayer = Vector3.Distance(possibleTargetLock.transform.position, Player.PlayerPosition);
            if (distToPlayer < distance)
            {
                closestTarget = possibleTargetLock;
            }
        }
        
        if (closestTarget == null) return;
        var pos =  Camera.main.WorldToScreenPoint(closestTarget.transform.position);
        if (pos.z < 0) return;
        
        closestTarget.targetLock.enabled = true;
        closestTarget.targetLock.transform.position = pos;
        _possibleTargetLocks  = new List<Targetable>();
    }

    public void FindPossibleTargetLock()
    {
        if (PreconditionTarget() == false) return;
        var pos = Camera.main.WorldToScreenPoint(transform.position);
        var mousePosDistance = Vector2.Distance(pos, Input.mousePosition);
        if (mousePosDistance > 50) return;
        _possibleTargetLocks.Add(this);
    }

    public void Target()
    {
        _targetBox.enabled = false;
        if (PreconditionTarget() == false) return;
        _targetBox.enabled = true;
        _targetBox.transform.position = Camera.main.WorldToScreenPoint(transform.position);
    }

    private void OnCollisionEnter(Collision collision) {

        if (collision.gameObject.layer == gameObject.layer)
            return;
        var magnitude = GetComponent<Rigidbody>().velocity.magnitude;
        Damage(magnitude * 3);
        //GameObject.Instantiate(explosion, entity.transform.position, Quaternion.identity);
    }


    public void Damage(float amount) {
        Health -= amount;
        if (Health <= 0) StartCoroutine(Death());
    }



    private IEnumerator Death() {
        //if (_targetBox != null) _targetBox.enabled = false;
        
        //playDeathAnimation();
        _targetBox.enabled = false;
        targetLock.enabled = false;
        Destroy(gameObject);
        yield return null;
    }
    
    public bool IsDead()
    {
        
        return Health <= 0;
    }

//    void playDeathAnimation() {
//        GameObject.Instantiate(explosion, transform.position, Quaternion.identity);
//        AudioSource.PlayClipAtPoint(explNoise, transform.position,1f);
//    }
}
