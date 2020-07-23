using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TwitterKit.Unity;
using Firebase;
using Firebase.Database;
using Firebase.Storage;
using Firebase.Unity.Editor;
using MiniJSON;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Threading.Tasks;

public class PUFirebaseTwitterLogin : SingletonMonoBehaviour<PUFirebaseTwitterLogin>
{
    private string _UserName;
    private string _DisplayName;

    [SerializeField]
    private string FirebaseDatabaseUrl = "";

    [SerializeField]
    private string FirebaseStorageUrl = "";

    [SerializeField]
    private string test_email = "";

    [SerializeField]
    private string test_password = "";

    private const string FIREBASE_SETTINGS_ASSET_NAME = "FirebaseSettings";

    private DatabaseReference _FirebaseDB;
    private StorageReference _StorageReference;
    private Firebase.Auth.FirebaseUser _FirebaseUser;

    [SerializeField]
    private User userdata;

    private Dictionary<string, object> ItemPossesionDic = new Dictionary<string, object>();

    protected override void Awake()
    {
        var fb_setting = Resources.Load(FIREBASE_SETTINGS_ASSET_NAME) as FirebaseSettings;

        FirebaseDatabaseUrl = fb_setting.FIREBASE_DATABASE_URL;

        FirebaseStorageUrl = fb_setting.FIREBASE_STORAGE_URL;

        test_email = fb_setting.TEST_EMAIL;

        test_password = fb_setting.TEST_PASSWORD;

        DebugText.Instance.ADDText("Firebase Start!");

        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl(FirebaseDatabaseUrl);
        _FirebaseDB = FirebaseDatabase.DefaultInstance.GetReference("Users");

        _StorageReference = FirebaseStorage.DefaultInstance.GetReferenceFromUrl(FirebaseStorageUrl);

        DebugText.Instance.ADDText("Firebase Init!");

        base.Awake();

    }

    /// <summary>
    /// Twitterの認証が完了したら呼び出される
    /// </summary>
    /// <param name="session"></param>
    private void TwitterLoginComplete(TwitterSession session)
    {
        Debug.Log("[Info] : Login success. " + session.authToken);
        //DebugText.Instance.ADDText("[Info] : Login success. " + session.authToken);
    }

    /// <summary>
    /// Twitterの認証が失敗したら呼び出される
    /// </summary>
    /// <param name="error"></param>
    private void TwitterLoginFailure(ApiError error)
    {
        Debug.Log("[Error ] : Login faild code =" + error.code + " msg =" + error.message);
        //DebugText.Instance.ADDText("[Error ] : Login faild code =" + error.code + " msg =" + error.message);
    }

    /// <summary>
    /// Firebaseにログインする
    /// </summary>
    public void FirebaseLogin(string _AccessToken, string _Secret, string UserName)
    {
        CommunicationIcon.Instance.CommunicationOn();


        //DebugText.Instance.ADDText("FirebaseLogin.");

        _UserName = UserName;

        //DebugText.Instance.ADDText(_UserName);
        //DebugText.Instance.ADDText(_AccessToken);
        //DebugText.Instance.ADDText(_Secret);

        Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;

        //DebugText.Instance.ADDText("Firebase.Auth.FirebaseAuth.DefaultInstance");

        //Twitterの認証情報を元にfirebaseの認証を行う
        Firebase.Auth.Credential credential = Firebase.Auth.TwitterAuthProvider.GetCredential(_AccessToken, _Secret);

        //DebugText.Instance.ADDText("Firebase.Auth.TwitterAuthProvider");
        //DebugText.Instance.ADDText(_AccessToken);
        //DebugText.Instance.ADDText(_Secret);


        auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
        {

            if (task.IsCanceled)
            {
                Debug.LogError("[Info ] : SignInWithCredentialAsync canceled.");
                //DebugText.Instance.ADDText("[Info ] : SignInWithCredentialAsync canceled.");

                CommunicationIcon.Instance.CommunicationOff();

                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("[Error ] : SignInWithCredentialAsync fatal. an error: " + task.Exception);
                //DebugText.Instance.ADDText("[Error ] : SignInWithCredentialAsync fatal. an error: " + task.Exception);

                CommunicationIcon.Instance.CommunicationOff();

                return;
            }

            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);

            //DebugText.Instance.ADDText("User signed in successfully:" + newUser.DisplayName + "(" + _UserName + ")");
            //DebugText.Instance.ADDText(newUser.UserId);

            _FirebaseUser = task.Result;

            _DisplayName = newUser.DisplayName;

            // 認証完了したらデータを読み込む
            this.getUserData();
            this.getItemData();

            CommunicationIcon.Instance.CommunicationOff();
        });
    }

    /// <summary>
    /// Firebaseにログインする(Editor デバッグ用アカウント使用)
    /// </summary>
    public void FirebaseLoginEditor()
    {
        var email = test_email;
        var password = test_password;

        Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;

        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0}", newUser.UserId);

            //DebugText.Instance.ADDText("User signed in successfully:" + newUser.DisplayName + "(" + _UserName + ")");
            //DebugText.Instance.ADDText(newUser.UserId);

            _UserName = "Editor";
            _DisplayName = "Unity Editor";

            _FirebaseUser = task.Result;

            // 認証完了したらデータを読み込む
            //this.GetRankingData();
            this.getUserData();
            this.getItemData();

        });
    }

    /// <summary>
    /// ユーザーデータを取得する
    /// </summary>
    private void getUserData()
    {
        _FirebaseDB.Child(_FirebaseUser.UserId).Child("UserData").GetValueAsync().ContinueWith(task =>
        {

            if (task.IsFaulted)
            {
                Debug.LogError("失敗");
            }
            else if (task.IsCompleted)
            {
                Debug.Log("firebase接続成功");

                DataSnapshot snapshot = task.Result;

                string json = snapshot.GetRawJsonValue();

                Debug.Log("Raw User JSON :\n" + json);

                //まだユーザーデータがなかったら作成する
                if (json == null)
                {
                    //まだデータがなかったら作成する
                    Debug.Log("データがないので初期設定");

                    userdata = new User(_UserName, _DisplayName);
                    string user_json = JsonUtility.ToJson(userdata);

                    _FirebaseDB.Child(_FirebaseUser.UserId).Child("UserData").SetRawJsonValueAsync(user_json);

                    Debug.Log("Create JSON : " + userdata);
                }
                else
                {
                    Debug.Log("データ取得成功");
                    //ユーザーデータがあったらデリシアライズする
                    userdata = JsonUtility.FromJson<User>(json);

                    if (userdata.user_name != _UserName)
                    {
                        userdata.user_name = _UserName;

                        updateUserDataValue("user_name", _UserName);
                    }

                    if (userdata.display_name != _DisplayName)
                    {
                        userdata.display_name = _DisplayName;

                        updateUserDataValue("display_name", _DisplayName);
                    }

                    Debug.Log("load JSON : " + userdata);

                }

            }
        });
    }

    /// <summary>
    /// ユーザーデータを更新する
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    private void updateUserDataValue(string key, object value)
    {
        var update = new Dictionary<string, object> { { key, value } };
        _FirebaseDB.Child(_FirebaseUser.UserId).UpdateChildrenAsync(update);
    }

    /// <summary>
    /// アイテムデータを取得する
    /// </summary>
    private void getItemData()
    {
        _FirebaseDB.Child(_FirebaseUser.UserId).Child("Items").GetValueAsync().ContinueWith(task =>
        {

            if (task.IsFaulted)
            {
                Debug.LogError("失敗");
            }
            else if (task.IsCompleted)
            {
                Debug.Log("firebase接続成功");

                DataSnapshot snapshot = task.Result;

                string json = snapshot.GetRawJsonValue();

                if (json == null)
                {
                    Debug.Log("アイテムデータなし");

                    //ない場合はダミーデータを1つ挿入
                    updateItem("PCH1-01", 0);
                }
                else
                {
                    Debug.Log("データ取得成功");

                    ItemPossesionDic = Json.Deserialize(json) as Dictionary<string, object>;
                }

            }
        });
    }

    /// <summary>
    /// アイテムの所持情報を更新する
    /// </summary>
    /// <param name="item_id"></param>
    /// <param name="possesion_num"></param>
    public void updateItem(string item_id, object possesion_num)
    {
        //firebaseの更新
        updateItemDataValue(item_id, possesion_num);

        //Dictionaryの更新

        if (ItemPossesionDic.ContainsKey(item_id))
            ItemPossesionDic[item_id] = possesion_num;
        else
            ItemPossesionDic.Add(item_id, possesion_num);
    }

    /// <summary>
    /// Firebaseのアイテムデータを更新する
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    private void updateItemDataValue(string item_id, object possesion_num)
    {
        var update = new Dictionary<string, object> { { item_id, possesion_num } };
        _FirebaseDB.Child(_FirebaseUser.UserId).Child("Items").UpdateChildrenAsync(update);
    }

    /// <summary>
    /// アイテムの所持数を返す
    /// </summary>
    /// <param name="item_id"></param>
    /// <returns></returns>
    public int getItemPosessionNum(string item_id)
    {
        if (ItemPossesionDic.ContainsKey(item_id))
        {
            int posession_num = Convert.ToInt32(ItemPossesionDic[item_id]);

            return posession_num;
        }
        else
            return 0;
    }

    public void storageTest(ContentManager cm, string image_num)
    {
        var image_jpg = image_num + ".jpg";

        var imageRef = _StorageReference.Child(image_jpg);

        string local_url = Application.persistentDataPath + "/ItemImage/" + image_num + ".jpg";

        // Download to the local filesystem
        imageRef.GetFileAsync(local_url).ContinueWith(task =>
        {
            if (!task.IsFaulted && !task.IsCanceled)
            {
                cm.refleshImage();
            }
        });


    }
}


[Serializable]
public class User
{
    public string user_name;
    public string display_name;


    public User(string user_name, string display_name)
    {
        this.user_name = user_name;
        this.display_name = display_name;
    }

    public override string ToString()
    {
        return JsonUtility.ToJson(this);
    }
}

