using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using Unity.Netcode.Transports.UTP;


public class Main : NetworkBehaviour {

    public Button btnHost;
    public Button btnClient;
    public TMPro.TMP_Text txtStatus;
    public TMPro.TMP_InputField inputIP;
    public TMPro.TMP_InputField inputPort;
    public UnityTransport transport;


    public void Start() {
        btnHost.onClick.AddListener(OnHostClicked);
        btnClient.onClick.AddListener(OnClientClicked);
        NetworkManager.Singleton.OnClientDisconnectCallback += OnDisconnect;
    }

    private void StartHost() {
        NetworkManager.Singleton.StartHost();
        NetworkManager.SceneManager.LoadScene("Lobby", UnityEngine.SceneManagement.LoadSceneMode.Single);


        ushort result;
        transport.ConnectionData.Address = inputIP.text;
        ushort.TryParse(inputPort.text, out result);
        transport.ConnectionData.Port = result;
    }

    private void StartClient()
    {
        NetworkManager.Singleton.StartClient();

        ushort result;
        transport.ConnectionData.Address = inputIP.text;
        ushort.TryParse(inputPort.text, out result);
        transport.ConnectionData.Port = result;
    }



    private void OnHostClicked() {
        btnClient.gameObject.SetActive(false);
        btnHost.gameObject.SetActive(false);
        inputIP.enabled = false;
        inputPort.enabled = false;
        txtStatus.text = "Starting Host";
        StartHost();
    }

    private void OnClientClicked() {
        btnClient.gameObject.SetActive(false);
        btnHost.gameObject.SetActive(false);
        inputIP.enabled = false;
        inputPort.enabled = false;
        txtStatus.text = "Waiting on Host";
        StartClient();
    }

    private void OnDisconnect(ulong clientId)
    {
        txtStatus.text = "Failed to connect to server";

        btnHost.gameObject.SetActive(true);
        btnClient.gameObject.SetActive(true);
    }
}
