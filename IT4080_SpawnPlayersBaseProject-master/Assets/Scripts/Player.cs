using Unity.Netcode;
using UnityEngine;


public class Player : NetworkBehaviour {


    public NetworkVariable<Vector3> PositionChange = new NetworkVariable<Vector3>();
    public NetworkVariable<Vector3> RotationChange = new NetworkVariable<Vector3>();
    public NetworkVariable<Color> PlayerColor = new NetworkVariable<Color>(Color.red);
    public NetworkVariable<int> score = new NetworkVariable<int>(50);

    private GameManager _gameMgr;
    private Camera _camera;
    public float movementSpeed = .5f;
    private float rotationSpeed = 1f;
    private BulletSpawner _bulletSpawner;
    public TMPro.TMP_Text txtScoreDisplay;

    private void Start() {
        ApplyPlayerColor();
        PlayerColor.OnValueChanged += OnPlayerColorChanged;
        _bulletSpawner = transform.Find("RArm").transform.Find("BulletSpawner").GetComponent<BulletSpawner>();
    }

    public override void OnNetworkSpawn() {
        _camera = transform.Find("Camera").GetComponent<Camera>();
        _camera.enabled = IsOwner;

        txtScoreDisplay.text = score.Value.ToString();
        score.OnValueChanged += ClientOnScoreChanged;
    }

    private void ClientOnScoreChanged(int previous, int current)
    {
        txtScoreDisplay.text = current.ToString();
        if (score.Value == 0)
        {
            txtScoreDisplay.text = "DEAD";
            Destroy(this.gameObject);
        }
    }


    [ServerRpc]
    void RequestPositionForMovementServerRpc(Vector3 posChange, Vector3 rotChange) {
        if (!IsServer && !IsHost) return;

        PositionChange.Value = posChange;
        RotationChange.Value = rotChange;
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestSetScoreServerRpc(int value)
    {
        score.Value = value;
    }


    public void OnPlayerColorChanged(Color previous, Color current) {
        ApplyPlayerColor();
    }

    public void ApplyPlayerColor() {
        GetComponent<MeshRenderer>().material.color = PlayerColor.Value;
        transform.Find("LArm").GetComponent<MeshRenderer>().material.color = PlayerColor.Value;
        transform.Find("RArm").GetComponent<MeshRenderer>().material.color = PlayerColor.Value;
    }


    // horiz changes y rotation or x movement if shift down, vertical moves forward and back.
    private Vector3[] CalcMovement() {
        bool isShiftKeyDown = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        float x_move = 0.0f;
        float z_move = Input.GetAxis("Vertical");
        float y_rot = 0.0f;

        if (isShiftKeyDown) {
            x_move = Input.GetAxis("Horizontal");
        } else {
            y_rot = Input.GetAxis("Horizontal");
        }

        Vector3 moveVect = new Vector3(x_move, 0, z_move);
        moveVect *= movementSpeed;

        Vector3 rotVect = new Vector3(0, y_rot, 0);
        rotVect *= rotationSpeed;

        return new[] { moveVect, rotVect };
    }


    void Update() {
        if (IsOwner) {
            Vector3[] results = CalcMovement();
            RequestPositionForMovementServerRpc(results[0], results[1]);
            if (Input.GetButtonDown("Fire1")) {
                _bulletSpawner.FireServerRpc();
            }
        }

        if(!IsOwner || IsHost){
            transform.Translate(PositionChange.Value);
            transform.Rotate(RotationChange.Value);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (IsHost)
        {
            if (collision.gameObject.CompareTag("bullet"))
            {
                RequestSetScoreServerRpc(score.Value - 1);

                ulong ownerClientId = collision.gameObject.GetComponent<NetworkObject>().OwnerClientId;
                Player otherPlayer = NetworkManager.Singleton.ConnectedClients[ownerClientId].PlayerObject.GetComponent<Player>();
                otherPlayer.score.Value += 1;

                Destroy(collision.gameObject);
            }
            
        }
    }
}