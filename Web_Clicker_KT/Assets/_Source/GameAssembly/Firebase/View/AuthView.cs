using FireBase;
using Firebase.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Firebase.View
{
    public class AuthView : MonoBehaviour
    {
        [SerializeField] private GameObject enterBlocker;
        [SerializeField] private TMP_InputField passwordInput;
        [SerializeField] private TMP_InputField emailInput;
        [SerializeField] private TMP_InputField usernameInput;
        [SerializeField] private TMP_Text enterButtonLabel;
        [SerializeField] private TMP_Text changeEnterTypeLabel;
        [SerializeField] private Button enterButton;
        [SerializeField] private Button switchEnterTypeButton;
        [SerializeField] private FBAuthManager fbAuthManager;

        private EnterType _currentEnterType = EnterType.REGISTRATION;

        private void Start() => Bind();

        private void OnDestroy() => Expose();

        public void OnEnterButtonClick()
        {
            if (_currentEnterType == EnterType.REGISTRATION)
                fbAuthManager.RegisterUser(emailInput.text, passwordInput.text, usernameInput.text);
            else
                fbAuthManager.LoginUser(emailInput.text, passwordInput.text);
        }

        private void SwitchEnterType()
        {
            _currentEnterType = _currentEnterType == EnterType.REGISTRATION ? EnterType.LOGIN : EnterType.REGISTRATION;
            enterButtonLabel.text = _currentEnterType == EnterType.REGISTRATION ? "Register" : "Login";
            changeEnterTypeLabel.text = _currentEnterType == EnterType.REGISTRATION
                ? "Already have an account?"
                : "Don't have an account?";
            
            usernameInput.gameObject.SetActive(_currentEnterType == EnterType.REGISTRATION);
        }

        private void CloseAuthPanel()
        {
            enterBlocker.SetActive(false);
            Expose();
        }

        private void Bind()
        {
            enterButton.onClick.AddListener(OnEnterButtonClick);
            switchEnterTypeButton.onClick.AddListener(SwitchEnterType);
            fbAuthManager.OnAuthComplete += CloseAuthPanel;
        }

        private void Expose()
        {
            enterButton.onClick.RemoveAllListeners();
            switchEnterTypeButton.onClick.RemoveAllListeners();
            fbAuthManager.OnAuthComplete -= CloseAuthPanel;
        }
    }
}