using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Extensions;
using Firebase.Auth;
using UnityEngine.UI;
using Google;
using System.Net.Http;
using System.Threading.Tasks;
using TMPro;

public class FirebaseGoogleLogin : MonoBehaviour {

    public string GoogleWebAPI = "44721014930-jepi4vrb6asgu2sbig42fr5ijbf0fos4.apps.googleusercontent.com";

    private GoogleSignInConfiguration configuration;

    DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;
    FirebaseAuth auth;
    FirebaseUser user;

    public TMP_Text UsernameTxt,UserEmailTxt;
    public Image UserProfilePic;
    public string imageUrl;
    public GameObject LoginScreen, ProfileScreen;

    private void Awake() {
        configuration = new GoogleSignInConfiguration {
            WebClientId = GoogleWebAPI,
            RequestIdToken = true
        };
    }

    private void Start() {
        InitFirebase();
    }

    public void InitFirebase() {
        auth = FirebaseAuth.DefaultInstance;
    }

    public void GoogleSignInClick() {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        GoogleSignIn.Configuration.RequestEmail = true;

        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnGoogleAuthenticatedFinished);
    }

    public void OnGoogleAuthenticatedFinished(Task<GoogleSignInUser> task) {
        if(task.IsFaulted) {
            Debug.LogError("Fault");
        } else if(task.IsCanceled) {
            Debug.LogError("Login Cancel");
        } else {
            Credential credential = GoogleAuthProvider.GetCredential(task.Result.IdToken, null);

            auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task => {
                if(task.IsCanceled) {
                    Debug.LogError("SignInWithCredentialAsync was canceled.");
                        return;
                }
                if(task.IsFaulted) {
                    Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                        return;
                }

                user =auth.CurrentUser;

                UsernameTxt.text = user.DisplayName;
                UserEmailTxt.text = user.Email;

                LoginScreen.SetActive(false);
                ProfileScreen.SetActive(true);

                StartCoroutine(LoadImage(CheckImageUrl(user.PhotoUrl.ToString())));
            });
        }
    }

    private string CheckImageUrl(string url) {
        if(!string.IsNullOrEmpty(url)) {
            return url;
        }

        return imageUrl;
    }

    IEnumerator LoadImage(string imageUrl) {
        WWW www = new WWW(imageUrl);
        yield return www;

        UserProfilePic.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));
    }
}