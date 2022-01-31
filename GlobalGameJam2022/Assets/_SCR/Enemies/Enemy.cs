using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OliverLoescher;

public class Enemy : MonoBehaviour
{
    public Health health = null;
    [SerializeField] private EnemyMovement movement = null;
    [SerializeField] private EnemyAnimationController animationController = null;

    [Header("Death")]
    [SerializeField] private ParticleSystem deathFX = null;

    private void Awake() 
    {
        animationController.Initialize(this, movement);
        health.onValueOut.AddListener(OnHealthOut);
    }

    private void OnHealthOut()
    {
        deathFX.Play();
        deathFX.transform.SetParent(null);
        Destroy(gameObject);
    }
}
