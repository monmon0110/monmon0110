using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;


public class GameManager : MonoBehaviour
{
    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                // Firebase has been successfully initialized.
            }
            else
            {
                Debug.LogError($"Firebase initialization failed with status: {dependencyStatus}");
            }
        });
    }
}