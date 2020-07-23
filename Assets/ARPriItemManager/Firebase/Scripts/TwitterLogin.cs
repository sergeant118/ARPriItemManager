

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using TwitterKit.Unity;

public class TwitterLogin : MonoBehaviour
{

    private string _AccessToken;
    private string _Secret;

    void Start()
    {
        Twitter.Init();
        this.TwitterAuth();
    }

    void Update() { }

    public void TwitterAuth()
    {
        UnityEngine.Debug.Log("[Info] : start login");
        DebugText.Instance.ADDText("[Info] : start login");

        TwitterSession session = Twitter.Session;
        if (session == null)
        {
            Twitter.LogIn(LoginComplete, LoginFailure);
        }
        else
        {
            LoginComplete(session);
        }
    }

    public void LoginComplete(TwitterSession session)
    {
        DebugText.Instance.ADDText("[Info] : Login success. " + session.authToken);

        _AccessToken = session.authToken.token;
        _Secret = session.authToken.secret;

        //DebugText.Instance.ADDText(session.id + "," + session.userName);
        //DebugText.Instance.ADDText(_AccessToken);
        //DebugText.Instance.ADDText(_Secret);

        //DebugText.Instance.ADDText("アプリIDを更新したぞ！");

        FirebaseLoginEditor();
        FirebaseLoginAndroid(session.authToken.token, session.authToken.secret, session.userName);
    }

    public void LoginFailure(ApiError error)
    {
        DebugText.Instance.ADDText("[Error ] : Login faild code =" + error.code + " msg =" + error.message);
    }

    [Conditional("UNITY_EDITOR")]
    public void FirebaseLoginEditor()
    {
        PUFirebaseTwitterLogin.Instance.FirebaseLoginEditor();
    }

    [Conditional("UNITY_ANDROID")]
    public void FirebaseLoginAndroid(string token, string secret, string userName)
    {
        PUFirebaseTwitterLogin.Instance.FirebaseLogin(token, secret, userName);
    }
}
