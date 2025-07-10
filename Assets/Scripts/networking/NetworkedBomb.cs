using UnityEngine;
using Mirror;

public class NetworkedBomb : NetworkBehaviour
{
    [SerializeField] private SphereCollider sphereCollider;
    [SerializeField] private static AudioSource beepingSource;

    [SerializeField] private float blastDuration = 5f;
    [SerializeField] private float maxRadius = 600f;

    private float blastTimer = 0f;

    [SyncVar(hook = nameof(OnBlastChanged))]
    private static bool blastem = false;

    private bool done = false;

    private void FixedUpdate()
    {
        if (!blastem) return;

        if (!done)
        {
            sphereCollider.gameObject.SetActive(true);
            done = true;
        }

        blastTimer += Time.fixedDeltaTime;
        float t = Mathf.Clamp01(blastTimer / blastDuration);
        float eased = 1f - Mathf.Pow(1f - t, 3f);

        float scaleFactor = transform.lossyScale.x;
        sphereCollider.radius = (eased * maxRadius) / scaleFactor;

        if (t >= 1f)
        {
            if (isServer)
            {
                blastem = false;
            }
        }
    }

    private void OnBlastChanged(bool oldVal, bool newVal)
    {
        if (!newVal)
        {
            blastTimer = 0f;
            sphereCollider.radius = 0f;
            sphereCollider.gameObject.SetActive(false);
            done = false;
        }
    }

    // Call this from a client to trigger the blast
    public static void RequestBlast()
    {
        // Any client can request it
        if (NetworkClient.active)
        {
            CmdActivateBlast();
        }
    }

    [Command(requiresAuthority = false)]
    private static void CmdActivateBlast(NetworkConnectionToClient sender = null)
    {
        Debug.Log($"[Server] Blast triggered by connection {sender?.connectionId}");

        blastem = true;
        RpcPlayBeep();

        // Optional: track who triggered the blast
        GameManager.IsBombActive = true;
    }

    [ClientRpc]
    private static void RpcPlayBeep()
    {
        if (beepingSource != null)
        {
            beepingSource.Play();
        }
    }
}
