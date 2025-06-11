using Mirror;

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

    void OnNameChanged(string oldName, string newName)
    {
        lobby?.UpdatePlayerUI(this);
    }

    void OnReadyChanged(bool oldReady, bool newReady)
    {
        lobby?.UpdatePlayerUI(this);
    }
}