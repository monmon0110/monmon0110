using DevionGames.UIWidgets;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DevionGames.LoginSystem
{
    public class LoginWindow : UIWidget
    {
        [Header("Reference")]
        [SerializeField]
        protected InputField username;
        [SerializeField]
        protected InputField password;
        [SerializeField]
        protected Toggle rememberMe;
        [SerializeField]
        protected Button loginButton;
        [SerializeField]
        protected GameObject loadingIndicator;

        // Use Awake for initialization
        private void Awake()
        {
            username.text = PlayerPrefs.GetString("username", string.Empty);
            password.text = PlayerPrefs.GetString("password", string.Empty);

            if (rememberMe != null)
            {
                rememberMe.isOn = string.IsNullOrEmpty(username.text) ? false : true;
            }

            if (loadingIndicator != null)
            {
                loadingIndicator.SetActive(false);
            }

            // Remove or comment out event registrations related to authentication
             EventHandler.Register("OnLogin", OnLogin);
             EventHandler.Register("OnFailedToLogin", OnFailedToLogin);

            // Attach the LoginUsingFields method to the button click event
            loginButton.onClick.AddListener(LoginUsingFields);
        }

        public void LoginUsingFields()
        {
            // Remove or comment out any authentication-related logic
             LoginManager.LoginAccount(username.text, password.text);
            loginButton.interactable = false;
            if (loadingIndicator != null)
            {
                loadingIndicator.SetActive(true);
            }
        }

        // Remove or comment out the authentication-related event handlers

         private void OnLogin()
         {
             if (rememberMe != null && rememberMe.isOn)
             {
                PlayerPrefs.SetString("username", username.text);
                 PlayerPrefs.SetString("password", password.text);
             }
            else
            {
                PlayerPrefs.DeleteKey("username");
                 PlayerPrefs.DeleteKey("password");
             }
             Execute("OnLogin", new CallbackEventData());
             if (LoginManager.DefaultSettings.loadSceneOnLogin)
             {
                 UnityEngine.SceneManagement.SceneManager.LoadScene(LoginManager.DefaultSettings.sceneToLoad);
             }
         }

         private void OnFailedToLogin()
         {
             Execute("OnFailedToLogin", new CallbackEventData());
             username.text = "";
             password.text = "";
            LoginManager.Notifications.loginFailed.Show(delegate(int result) { Show(); }, "OK");
             loginButton.interactable = true;
             if (loadingIndicator != null)
             {
                 loadingIndicator.SetActive(false);
             }
             Close();
         }
    }
}
