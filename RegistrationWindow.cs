using DevionGames.UIWidgets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DevionGames.LoginSystem
{
    public class RegistrationWindow : UIWidget
    {
        public override string[] Callbacks
        {
            get
            {
                List<string> callbacks = new List<string>(base.Callbacks);
                callbacks.Add("OnAccountCreated");
                callbacks.Add("OnFailedToCreateAccount");
                return callbacks.ToArray();
            }
        }

        [Header("Reference")]
        /// <summary>
        /// Referenced UI field
        /// </summary>
        [SerializeField]
        protected InputField username;
        /// <summary>
        /// Referenced UI field
        /// </summary>
        [SerializeField]
        protected InputField password;
        /// <summary>
        /// Referenced UI field
        /// </summary>
        [SerializeField]
        protected InputField confirmPassword;
        /// <summary>
        /// Referenced UI field
        /// </summary>
        [SerializeField]
        protected InputField email;
        /// <summary>
        /// Referenced UI field
        /// </summary>
        [SerializeField]
        protected Toggle termsOfUse;
        /// <summary>
        /// Referenced UI field
        /// </summary>
        [SerializeField]
        protected Button registerButton;

        [SerializeField]
        protected GameObject loadingIndicator;

        protected override void OnStart()
                {
                    base.OnStart();
                    if (loadingIndicator != null)
                    {
                        loadingIndicator.SetActive(false);
                    }

                    EventHandler.Register("OnAccountCreated", OnAccountCreated);
                    EventHandler.Register("OnFailedToCreateAccount", OnFailedToCreateAccount);

                    // Update this line to call CreateFirebaseAccountUsingFields
                    registerButton.onClick.AddListener(CreateAccountUsingFields);
                }


        /// <summary>
        /// Creates the account using data from referenced fields.
        /// </summary>
 [SerializeField] // This makes the method public and visible in the inspector.
        private void CreateAccountUsingFields()
{
    if (string.IsNullOrEmpty(email.text) ||
        string.IsNullOrEmpty(password.text) ||
        string.IsNullOrEmpty(confirmPassword.text) ||
        string.IsNullOrEmpty(email.text))
    {
        FirebaseManagement.Notifications.emptyField.Show(delegate (int result) { Show(); }, "OK");
        Close();
        return;
    }

    if (password.text != confirmPassword.text)
    {
        password.text = "";
        confirmPassword.text = "";
        FirebaseManagement.Notifications.passwordMatch.Show(delegate (int result) { Show(); }, "OK");
        Close();
        return;
    }

    if (!ValidateEmail(email.text))
    {
        email.text = "";
        FirebaseManagement.Notifications.invalidEmail.Show(delegate (int result) { Show(); }, "OK");
        Close();
        return;
    }

    if (!termsOfUse.isOn)
    {
        FirebaseManagement.Notifications.termsOfUse.Show(delegate (int result) { Show(); }, "OK");
        Close();
        return;
    }

    // Call the RegisterUser method with the necessary parameters
    string userEmail = email.text;
    string userPassword = password.text;
    string userUsername = username.text;
    
    RegisterUser(userEmail, userPassword, userUsername);
}
[SerializeField]
private void RegisterUser(string userEmail, string userPassword, string userUsername)
{
    // Check if any of the fields are empty (you can add more validation as needed)
    if (string.IsNullOrEmpty(userEmail) || string.IsNullOrEmpty(userPassword) || string.IsNullOrEmpty(userUsername))
    {
        // Handle empty fields, show a message, or prevent registration
        Debug.LogWarning("Please fill in all fields.");
        return;
    }

    // Call the FirebaseManager to register the user
    FirebaseManagement.Instance.RegisterUser(userEmail, userPassword, userUsername);

    // Disable the register button and show loading indicator
    registerButton.interactable = false;
    if (loadingIndicator != null)
    {
        loadingIndicator.SetActive(true);
    }
}
[SerializeField]
        private void OnAccountCreated()
        {
            Execute("OnAccountCreated", new CallbackEventData());
            // Commented out notification and navigation
            LoginManager.Notifications.accountCreated.Show(delegate (int result) { LoginManager.UI.loginWindow.Show(); }, "OK");
            registerButton.interactable = true;
            if (loadingIndicator != null)
            {
                loadingIndicator.SetActive(false);
            }
            Close();
        }
[SerializeField]
        private void OnFailedToCreateAccount()
        {
            Execute("OnFailedToCreateAccount", new CallbackEventData());
            username.text = "";
            // Commented out notification
            LoginManager.Notifications.userExists.Show(delegate (int result) { Show(); }, "OK");
            registerButton.interactable = true;
            if (loadingIndicator != null)
            {
                loadingIndicator.SetActive(false);
            }
            Close();
        }
[SerializeField]
      private void CreateFirebaseAccountUsingFields() // Pangalawang method
{
    string userEmail = email.text;
    string userPassword = password.text;
    string userUsername = username.text;

    // Check if any of the fields are empty (you can add more validation as needed)
    if (string.IsNullOrEmpty(userEmail) || string.IsNullOrEmpty(userPassword) || string.IsNullOrEmpty(userUsername))
    {
        // Handle empty fields, show a message, or prevent registration
        Debug.LogWarning("Please fill in all fields.");
        return;
    }

    // Call the FirebaseManager to register the user
    FirebaseManagement.Instance.RegisterUser(userEmail, userPassword, userUsername);
}
[SerializeField]
public void OnRegisterButtonClicked()
{
    string userEmail = email.text;
    string userPassword = password.text;
    string userUsername = username.text;

    // Check if any of the fields are empty (you can add more validation as needed)
    if (string.IsNullOrEmpty(userEmail) || string.IsNullOrEmpty(userPassword) || string.IsNullOrEmpty(userUsername))
    {
        // Handle empty fields, show a message, or prevent registration
        Debug.LogWarning("Please fill in all fields.");
        return;
    }

    // Call the FirebaseManager to register the user
    FirebaseManagement.Instance.RegisterUser(userEmail, userPassword, userUsername);
}



        // You can implement your own ValidateEmail function here
        private bool ValidateEmail(string email)
        {
            // Implement your email validation logic
            return true; // Return true for demonstration purposes
        }
    }
}
