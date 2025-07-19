using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using UnityEngine.TextCore.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Reflection;

namespace BetterRagdoll2;

[BepInAutoPlugin]
public partial class Plugin : BaseUnityPlugin
{
    internal static ManualLogSource Log { get; private set; } = null!;

    private ConfigEntry<float>? forceConfig = null;

    private void Awake()
    {
        Log = Logger;
        Log.LogInfo($"Plugin {Name} is loaded!");

        this.forceConfig = base.Config.Bind<float>("Settings", "Ragdoll Force", 322f, "Force applied to the character when triggering ragdoll.");
    }

    private void Update()
    {
        if (!(Character.localCharacter == null) && Application.isFocused && this.forceConfig != null)
        {
            if (Input.GetKeyDown(KeyCode.F8))
            {
                this.forceConfig.Value -= 10f;
                Debug.Log(string.Format("Force decreased to {0}", this.forceConfig.Value));
            }
            if (Input.GetKeyDown(KeyCode.F9))
            {
                this.forceConfig.Value += 10f;
                Debug.Log(string.Format("Force increased to {0}", this.forceConfig.Value));
            }
            if (Input.GetKeyDown(KeyCode.X) || (Gamepad.current != null && Gamepad.current.leftShoulder.wasPressedThisFrame))
            {
                Character localCharacter = Character.localCharacter;
                if (localCharacter.data.fallSeconds > 0f)
                {
                    Debug.Log("Already in ragdoll – skipping.");
                    return;
                }
                Debug.Log("Ragdoll Key pressed");
                localCharacter.photonView.RPC("RPCA_Fall", Photon.Pun.RpcTarget.All, .5f);
                Vector3 avarageVelocity = localCharacter.data.avarageVelocity;
                if (avarageVelocity.magnitude < 0.1f)
                {
                    Debug.Log("Movement too small, no force applied.");
                    return;
                }
                Vector3 vector = avarageVelocity.normalized * this.forceConfig.Value;
                localCharacter.AddForce(vector);
                Debug.Log(string.Format("Force applied: {0}", vector));
            }
        }
    }
}
