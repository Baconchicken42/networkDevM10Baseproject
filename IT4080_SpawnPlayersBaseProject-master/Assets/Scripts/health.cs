using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class health : NetworkBehaviour
{
    public float rotationSpeed = 10;

    void Update()
    {
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + rotationSpeed * Time.deltaTime, transform.eulerAngles.z);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("player"))
        {
            if (IsHost)
            {
                Player player = collision.gameObject.GetComponent<Player>();
                player.RequestSetScoreServerRpc(player.score.Value + 10);
            }
            Destroy(this.gameObject);
        }
    }
}
