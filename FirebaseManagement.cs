using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;

public class FirebaseManagement : MonoBehaviour
{
    // Firebase components
    private FirebaseAuth auth;
    private DatabaseReference databaseReference;

    // Firebase user
    private FirebaseUser currentUser;

    private void Awake()
    {
        // Initialize Firebase components
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp.
                FirebaseApp app = FirebaseApp.DefaultInstance;
                // Set a flag here to indicate whether Firebase is ready to use by your app.
            }
            else
            {
                Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }

    // Register user with email, password, and username
    public void RegisterUser(string email, string password, string username)
    {
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError("Failed to create user: " + task.Exception);
                return;
            }

            FirebaseUser newUser = task.Result.User;
            Debug.Log("User registered successfully: " + newUser.UserId);

            // Create user data in the database
            CreateUserInDatabase(newUser.UserId, username, email);
        });
    }

    // Login user with email and password
    public void LoginUser(string email, string password)
    {
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError("Failed to sign in: " + task.Exception);
                return;
            }

            currentUser = task.Result.User;
            Debug.Log("User logged in successfully: " + currentUser.UserId);

            // Handle successful login, such as loading the main game scene
        });
    }

    // Create user data in the database
    private void CreateUserInDatabase(string userId, string username, string email)
    {
        UserData userData = new UserData(username, email);
        string json = JsonUtility.ToJson(userData);

        databaseReference.Child("users").Child(userId).SetRawJsonValueAsync(json)
            .ContinueWith(task =>
            {
                if (task.Exception != null)
                {
                    Debug.LogError("Failed to save user data: " + task.Exception);
                    return;
                }

                Debug.Log("User data saved successfully.");
            });
    }

    // Update user data in the database
    public void UpdateUserData(string newUsername)
    {
        if (currentUser != null)
        {
            UserData updatedData = new UserData(newUsername, currentUser.Email);
            string json = JsonUtility.ToJson(updatedData);

            databaseReference.Child("users").Child(currentUser.UserId).SetRawJsonValueAsync(json)
                .ContinueWith(task =>
                {
                    if (task.Exception != null)
                    {
                        Debug.LogError("Failed to update user data: " + task.Exception);
                        return;
                    }

                    Debug.Log("User data updated successfully.");
                });
        }
    }

    // Fetch user data from the database
    public void FetchUserData(System.Action<UserData> callback)
    {
        if (currentUser != null)
        {
            databaseReference.Child("users").Child(currentUser.UserId).GetValueAsync()
                .ContinueWith(task =>
                {
                    if (task.Exception != null)
                    {
                        Debug.LogError("Failed to fetch user data: " + task.Exception);
                        return;
                    }

                    DataSnapshot snapshot = task.Result;
                    if (snapshot.Exists)
                    {
                        string json = snapshot.GetRawJsonValue();
                        UserData userData = JsonUtility.FromJson<UserData>(json);
                        callback?.Invoke(userData);
                    }
                    else
                    {
                        Debug.LogWarning("User data not found.");
                    }
                });
        }
    }

    // Sign out the current user
    public void SignOutUser()
    {
        auth.SignOut();
        currentUser = null;
    }

    [System.Serializable]
    public class UserData
    {
        public string username;
        public string email;

        public UserData(string username, string email)
        {
            this.username = username;
            this.email = email;
        }
    }
}
