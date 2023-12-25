using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FirebaseController : MonoBehaviour {


    public GameObject loginPanel, signupPanel, mainPagePanel, forgetPasswordPanel, notificationPanel;
    public TMP_InputField loginEmail, loginPassword, signupEmail, signupPassword,signupConfirmPassword, signupUserName,forgetPassEmail;
    public TMP_Text titleText, errorMessage, profileUserName_Text, profileUserEmail_Text;
    public Toggle rememberMe;


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

    public void OpenMainPagePanel () {
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
    }

    public void SignUpUser() {
        if (string.IsNullOrEmpty(signupEmail.text)&&string.IsNullOrEmpty(signupPassword.text)
        &&string.IsNullOrEmpty(signupConfirmPassword.text)&&string.IsNullOrEmpty(signupUserName.text)) {

            showNotifactionMessage("Error", "Please Fill All Necessary Fields");
            return;
        }
    }

    public void forgetPass() {
        if (string.IsNullOrEmpty(forgetPassEmail.text)) {

            showNotifactionMessage("Error", "Please Fill All Necessary Fields");
            return;
        }
    }

    private void showNotifactionMessage(string title, string message) {
        titleText.text = "" + title;
        errorMessage.text = "" + message;

        notificationPanel.SetActive(true);
    }

    public void closeNotifactionMessage() {
        titleText.text = "";
        errorMessage.text = "";

        notificationPanel.SetActive(false);
    }

    public void Logout() {
        profileUserEmail_Text.text = "";
        profileUserName_Text.text = "";
        
        OpenLoginPanel();
    }
}