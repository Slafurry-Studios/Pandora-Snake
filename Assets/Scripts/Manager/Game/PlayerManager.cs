using System.Collections;
using System.Collections.Generic;
using Game.Player;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    private GameObject playerPrefab;
    public static Transform PlayerTransform;

    public GameObject StatHUD;
    public GameObject ChatHUD;
    public GameObject ObjectiveHUD;
    public GameObject ThreatHUD;
    public GameObject PauseHUD;
    public GameObject DonationHUD;

    private void Awake()
    {
        Instance = this;
        playerPrefab = GameObject.FindGameObjectWithTag("Player");
        PlayerTransform = playerPrefab.transform;
    }

    public void Pause()
    {
        playerPrefab.GetComponentInChildren<PlayerShoot>().enabled = false;
        playerPrefab.GetComponentInChildren<PlayerAim>().enabled = false;

    }

    public void Resume()
    {
        playerPrefab.GetComponentInChildren<PlayerShoot>().enabled = true;
        playerPrefab.GetComponentInChildren<PlayerAim>().enabled = true;

    }
}
