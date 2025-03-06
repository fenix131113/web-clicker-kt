using FireBase;
using TMPro;
using UnityEngine;

namespace Clicker.View
{
    public class ClickerCounter : MonoBehaviour
    {
        [SerializeField] private TMP_Text counterLabel;

        private void Update()
        {
            if (FBAuthManager.CurrentUserData != null)
                counterLabel.text = FBAuthManager.CurrentUserData.Clicks.ToString();
        }
    }
}