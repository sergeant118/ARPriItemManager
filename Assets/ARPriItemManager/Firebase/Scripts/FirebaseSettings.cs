using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Setting/FirebaseSettings")]
public class FirebaseSettings : ScriptableObject
{
    public string FIREBASE_DATABASE_URL = "";
    public string FIREBASE_STORAGE_URL = "";

    public string TEST_EMAIL = "";

    public string TEST_PASSWORD = "";
}
