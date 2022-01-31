using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerDeath : MonoBehaviour
{
    [SerializeField] private OliverLoescher.Health health = null;
    [SerializeField] private GameObject player = null;
    [SerializeField] private OliverLoescher.Health trainHealth = null;
    [SerializeField] private ParticleSystem deathFX = null;
    [SerializeField] private TMP_Text text = null;

    private void Awake() 
    {
        text.gameObject.SetActive(false);
    }

    private void Start() 
    {
        health.onValueOut.AddListener(OnHealthOut);
        trainHealth.onValueOut.AddListener(OnHealthOutTrain);
    }

    private void OnHealthOutTrain()
    {
        health.onValueOut.RemoveListener(OnHealthOut);
        trainHealth.onValueOut.RemoveListener(OnHealthOutTrain);

        text.text = "The Train was Lost";
        text.gameObject.SetActive(true);
        Invoke(nameof(ResetLevel), 15.0f);
    }

    private void OnHealthOut()
    {
        health.onValueOut.RemoveListener(OnHealthOut);
        trainHealth.onValueOut.RemoveListener(OnHealthOutTrain);
        
        text.text = "You Died";
        text.gameObject.SetActive(true);

        deathFX.transform.SetParent(null);
        deathFX.Play();

        Destroy(player);

        Invoke(nameof(ResetLevel), 12.0f);
    }

    private void ResetLevel()
    {
        SceneManager.LoadScene(0);
    }
}
