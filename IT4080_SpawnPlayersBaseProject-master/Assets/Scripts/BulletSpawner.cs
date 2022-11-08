using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BulletSpawner : NetworkBehaviour
{
    public Rigidbody bullet;
    private float bulletSpeed = 20f;

    [ServerRpc]
    public void FireServerRpc(ServerRpcParams rpcParams = default) {
        Rigidbody newBullet = Instantiate(bullet, transform.position, transform.rotation);
        newBullet.velocity = transform.forward * bulletSpeed;
        newBullet.gameObject.GetComponent<NetworkObject>().SpawnWithOwnership(rpcParams.Receive.SenderClientId);
        Destroy(newBullet.gameObject, 3);
    }
}
