using System.Collections;
using Firebase;
using FireBase;
using UnityEngine;

namespace Clicker
{
    public class Clicker : MonoBehaviour
    {
        [SerializeField] private FBAuthManager fbAuthManager;
        [SerializeField] private float saveInterval;

        private void Start()
        {
            fbAuthManager.OnAuthComplete += StartSaving;
        }

        private void StartSaving()
        {
            StartCoroutine(SaveDataCoroutine());
        }

        private void OnMouseDown()
        {
            if (FBAuthManager.CurrentUserData != null)
                FBAuthManager.CurrentUserData.Clicks++;
        }

        private IEnumerator SaveDataCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(saveInterval);
                fbAuthManager.SaveUserData();
            }
        }
    }
}