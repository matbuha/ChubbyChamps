using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth;
using System;
using Firebase;
using System.Threading.Tasks;
using Firebase.Extensions;

public class FirebaseController : MonoBehaviour {


    private FirebaseUser user;
    private FirebaseAuth auth;
    public GameObject loginPanel, signupPanel, mainPagePanel, forgetPasswordPanel, notificationPanel;
    public TMP_InputField loginEmail, loginPassword, signupEmail, signupPassword,signupConfirmPassword, signupUserName,forgetPassEmail;
    public TMP_Text errorTitleText, errorMessage, profileUserName_Text, profileUserEmail_Text;
    public Toggle rememberMe;
    bool isSignIn = false;



    void Start() {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available) {
                    // Create and hold a reference to your FirebaseApp,
                    // where app is a Firebase.FirebaseApp property of your application class.
                    InitializeFirebase();

                    // Set a flag here to indicate whether Firebase is ready to use by your app.
            } else {
                    Debug.LogError(string.Format(
                    "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                    // Firebase Unity SDK is not safe to use here.
            }
        });
    }


    public void OpenLoginPanel () {
        loginPanel.SetActive(true);
        signupPanel.SetActive(false);
        mainPagePanel.SetActive(false);
        forgetPasswordPanel.SetActive(false);
    }

    public void OpenSignUpPanel () {
        loginPanel.SetActive(false);
        signupPanel.SetActive(true);
        mainPagePanel.SetActive(false);
        forgetPasswordPanel.SetActive(false);
    }

    public void OpenProfilePagePanel () {
        loginPanel.SetActive(false);
        signupPanel.SetActive(false);
        mainPagePanel.SetActive(true);
        forgetPasswordPanel.SetActive(false);
    }

    public void OpenforgetPassPanel () {
        loginPanel.SetActive(false);
        signupPanel.SetActive(false);
        mainPagePanel.SetActive(false);
        forgetPasswordPanel.SetActive(true);
    }

    public void LogInUser() {
        if (string.IsNullOrEmpty(loginEmail.text)&&string.IsNullOrEmpty(loginPassword.text)) {

            showNotifactionMessage("Error", "Please Fill All Necessary Fields");
            return;
        }

        SignInUser(loginEmail.text, loginPassword.text);
    }

    public void SignUpUser() {
        if (string.IsNullOrEmpty(signupEmail.text)&&string.IsNullOrEmpty(signupPassword.text)
        &&string.IsNullOrEmpty(signupConfirmPassword.text)&&string.IsNullOrEmpty(signupUserName.text)) {

            showNotifactionMessage("Error", "Please Fill All Necessary Fields");
            return;
        }

        CreateUser(signupEmail.text, signupPassword.text, signupUserName.text);
    }

    public void forgetPass() {
        if (string.IsNullOrEmpty(forgetPassEmail.text)) {

            showNotifactionMessage("Error", "Please Fill All Necessary Fields");
            return;
        }

        forgetPasswordSubmit(forgetPassEmail.text);
    }

    private void showNotifactionMessage(string title, string message) {
        errorTitleText.text = "" + title;
        errorMessage.text = "" + message;

        notificationPanel.SetActive(true);
    }

    public void closeNotifactionMessage() {
        errorTitleText.text = "";
        errorMessage.text = "";

        notificationPanel.SetActive(false);
    }

    public void Logout() {
        auth.SignOut();
        profileUserEmail_Text.text = "";
        profileUserName_Text.text = "";
        
        OpenLoginPanel();
    }


    void CreateUser(string email, string password, string UserName) {
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task => {
            if (task.IsCanceled) {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted) {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);

                foreach (Exception exception in task.Exception.Flatten().InnerExceptions) {
                    FirebaseException firebaseEx = exception as FirebaseException;
                        if (firebaseEx != null) {
                            var errorCode = (AuthError)firebaseEx.ErrorCode;
                            showNotifactionMessage("Error", GetErrorMessage(errorCode));
                        }
                }

                return;
            }


            // Firebase user has been created.
            AuthResult result = task.Result;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                result.User.DisplayName, result.User.UserId);

                UpdateUserProfile(UserName);
        });
    }

    public void SignInUser (string email, string password) {
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task => {
            if (task.IsCanceled) {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted) {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);

                foreach (Exception exception in task.Exception.Flatten().InnerExceptions) {
                    FirebaseException firebaseEx = exception as FirebaseException;
                        if (firebaseEx != null) {
                            var errorCode = (AuthError)firebaseEx.ErrorCode;
                            showNotifactionMessage("Error", GetErrorMessage(errorCode));
                        }
                }

                return;
            }

            AuthResult newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                newUser.User.DisplayName, newUser.User.UserId);
                
            profileUserName_Text.text = "" + newUser.User.DisplayName;
            profileUserEmail_Text.text  = "" + newUser.User.Email;
            OpenProfilePagePanel();
        });
    }

    void InitializeFirebase() {
        Debug.Log("Setting up Firebase Auth");
        auth = FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    void AuthStateChanged(object sender, EventArgs eventArgs) {
        if (auth.CurrentUser != user) {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null
                && auth.CurrentUser.IsValid();
            if (!signedIn && user != null) {
                Debug.Log("Signed out " + user.UserId);
            }
            user = auth.CurrentUser;
            if (signedIn) {
                Debug.Log("Signed in " + user.UserId);
                isSignIn = true;
            }
        }
    }

    void OnDestroy() {
        auth.StateChanged -= AuthStateChanged;
        auth = null;
    }

    void UpdateUserProfile(string UserName) {
        FirebaseUser user = auth.CurrentUser;
        if (user != null) {
            UserProfile profile = new UserProfile {
                DisplayName = UserName,
                PhotoUrl = new Uri("https://via.placeholder.com/150"),
            };
            user.UpdateUserProfileAsync(profile).ContinueWith(task => {
                if (task.IsCanceled) {
                    Debug.LogError("UpdateUserProfileAsync was canceled.");
                    return;
                }
                if (task.IsFaulted) {
                    Debug.LogError("UpdateUserProfileAsync encountered an error: " + task.Exception);
                    return;
                }

                Debug.Log("User profile updated successfully.");

                showNotifactionMessage("Alert", "Account Successfully Created");
            });
        }
    }

    bool isSigned = false;

    void Update() {
        if (isSignIn) {
            if (isSigned) {
                isSigned = true;
                profileUserName_Text.text = "" + user.DisplayName;
                profileUserEmail_Text.text  = "" + user.Email;
                OpenProfilePagePanel();
            }
        }
    }

    private static string GetErrorMessage(AuthError errorCode) {
        var message = "";
        switch (errorCode)
        {
            case AuthError.AccountExistsWithDifferentCredentials:
                message = "The account already exists with different credentials";
                break;
            case AuthError.MissingPassword:
                message = "Password is needed";
                break;
            case AuthError.WeakPassword:
                message = "The password is weak";
                break;
            case AuthError.WrongPassword:
                message = "The password is incorrect";
                break;
            case AuthError.EmailAlreadyInUse:
                message = "The account with that email already exists";
                break;
            case AuthError.InvalidEmail:
                message = "invalid email";
                break;
            case AuthError.MissingEmail:
                message = "Email is needed";
                break;
            default:
                message = "An error occurred";
                break;
        }
        return message;
    }

    void forgetPasswordSubmit(string forgetPasswordEmail) {
        auth.SendPasswordResetEmailAsync(forgetPasswordEmail).ContinueWithOnMainThread(task=>{
            if(task.IsCanceled) {
                Debug.LogError("SendPasswordResetEmailAsyn was canceled");
            }

            if(task.IsFaulted) {
                foreach (Exception exception in task.Exception.Flatten().InnerExceptions) {
                    FirebaseException firebaseEx = exception as FirebaseException;
                        if (firebaseEx != null) {
                            var errorCode = (AuthError)firebaseEx.ErrorCode;
                            showNotifactionMessage("Error", GetErrorMessage(errorCode));
                        }
                }
            }

            showNotifactionMessage("Alert", "Check Your Email For Further Instructions");
        });
    }

    public void SaveCredentials(string email, string password) {
        if (Application.platform == RuntimePlatform.Android) {
            using (var javaClass = new AndroidJavaClass("com.yourcompany.plugin.SharedPreferencesManager")) {
                using (var unityActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
                    var context = unityActivity.GetStatic<AndroidJavaObject>("currentActivity");
                    javaClass.CallStatic("setRememberMe", context, email, password);
                }
            }
        }
    }

    public void LoadCredentials() {
        if (Application.platform == RuntimePlatform.Android) {
            string email = "", password = "";
            using (var javaClass = new AndroidJavaClass("com.arielbz.ChubbyChampsANDROID.plugin.SharedPreferencesManager")) {
                using (var unityActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
                    var context = unityActivity.GetStatic<AndroidJavaObject>("currentActivity");
                    email = javaClass.CallStatic<string>("getUserEmail", context);
                    password = javaClass.CallStatic<string>("getUserPassword", context);
                }
            }

            // Now you have your email and password, handle them accordingly
            Debug.Log("Email: " + email + ", Password: " + password);
            // Autofill or whatever you need to do with them
        }
    }
}