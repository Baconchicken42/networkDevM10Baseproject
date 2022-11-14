using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;



public class LobbyPlayerPanel : MonoBehaviour
{
    [SerializeField] protected TMPro.TMP_Text txtName;
    [SerializeField] protected TMPro.TMP_Text txtReady;
    [SerializeField] protected GameObject pnlColor;
    [SerializeField] protected Button btnKick;

    public event UnityAction OnKickPlayer;
    private bool isReady = false;

    public void Start() {
        SetReady(isReady);
        btnKick.onClick.AddListener(OnKickPlayerClicked);
    }

    public void OnKickPlayerClicked()
    {
        OnKickPlayer.Invoke();
    }

    public void SetName(string newName) {
        txtName.text = newName;
    }

    public string GetName() {
        return txtName.text;
    }

    public void SetColor(Color c) {
        pnlColor.GetComponent<Image>().color = c;
    }

    public void SetReady(bool ready) {
        isReady = ready;
        if (isReady) {
            txtReady.text = "Ready";
        } else {
            txtReady.text = "Not Ready";
        }
    }
}
