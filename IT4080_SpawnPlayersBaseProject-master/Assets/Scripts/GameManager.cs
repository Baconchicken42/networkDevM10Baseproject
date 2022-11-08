using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

public class GameManager : NetworkBehaviour {
    public Player playerPrefab;
    public GameObject spawnPoints;

    private int spawnIndex = 0;
    private List<Transform> availableSpawnPositions = new List<Transform>();

    public void Awake()
    {
        refreshSpawnPoints();
    }

    public override void OnNetworkSpawn()
    {
        if (IsHost)
        {
            SpawnPlayers();
        }
    }

    private void refreshSpawnPoints()
    {
        Transform[] allpoints = spawnPoints.GetComponentsInChildren<Transform>();
        availableSpawnPositions.Clear();
        foreach (Transform trans in allpoints)
        {
            if (trans != spawnPoints.transform)
            {
                availableSpawnPositions.Add(trans);
            }
        }
    }

    public Vector3 GetNextSpawnLocation()
    {
        var newPosition = availableSpawnPositions[spawnIndex].position;
        newPosition.y = 1.5f;
        spawnIndex += 1;

        if (spawnIndex > availableSpawnPositions.Count - 1)
        {
            spawnIndex = 0;
        }
        return newPosition;
    }

    private void SpawnPlayers()
    {
        foreach (PlayerInfo pi in GameData.Instance.allPlayers)
        {
            Player playerInst = Instantiate(playerPrefab, GetNextSpawnLocation(), Quaternion.identity);
            playerInst.GetComponent<NetworkObject>().SpawnWithOwnership(pi.clientId);
            playerInst.PlayerColor.Value = pi.color;
        }
    }
}