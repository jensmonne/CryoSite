using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

namespace BNG
{
    public class UnityAuthInitializer : MonoBehaviour
    {
        public static bool IsAuthenticated { get; private set; } = false;

        private static bool isAuthenticating = false;

        private async void Awake()
        {
            await Authenticate();
        }

        public static async Task Authenticate()
        {
            if (IsAuthenticated || isAuthenticating) return;

            isAuthenticating = true;

            try
            {
                if (UnityServices.State != ServicesInitializationState.Initialized)
                {
                    await UnityServices.InitializeAsync();
                    Debug.Log("Unity Services Initialized");
                }

                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                    Debug.Log($"Signed in! PlayerID: {AuthenticationService.Instance.PlayerId}");
                }

                IsAuthenticated = true;
            }
            catch (Exception e)
            {
                Debug.LogError("Unity Services Auth Failed: " + e.Message);
            }
            finally
            {
                isAuthenticating = false;
            }
        }
    }
}