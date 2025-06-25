using Mirror;
using UnityEngine;

public class NetworkedPlayer : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnNameChanged))]
    public string playerName;

    [SyncVar(hook = nameof(OnReadyChanged))]
    public bool isReady;
    private ReworkedLobbyNetworkUI lobby;

    public override void OnStartClient()
    {
        lobby = FindObjectOfType<ReworkedLobbyNetworkUI>();
        lobby.AddPlayer(this);
        
        if (isLocalPlayer)
        {
            string name = PlayerPrefs.GetString("PlayerName", $"Player {Random.Range(1000, 9999)}");
            CmdSetPlayerName(name);
        }
    }

    public override void OnStopClient()
    {
        lobby?.RemovePlayer(this);
    }

    [Command]
    public void CmdSetPlayerName(string name)
    {
        playerName = name;
    }

    [Command]
    public void CmdSetReady(bool value)
    {
        isReady = value;
    }

    private void OnNameChanged(string oldName, string newName)
    {
        lobby?.UpdatePlayerUI(this);
    }

    private void OnReadyChanged(bool oldReady, bool newReady)
    {
        lobby?.UpdatePlayerUI(this);
    }
}