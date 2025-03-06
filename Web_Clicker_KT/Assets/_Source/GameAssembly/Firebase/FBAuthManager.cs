using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace FireBase
{
    public class FBAuthManager : MonoBehaviour
    {
        private const string DB_URL = "https://web-clicker-kt-default-rtdb.europe-west1.firebasedatabase.app/";
        private const string API_KEY = "AIzaSyA9yApSyW1moCc0g8m95w8K-mghCmcyD4E";

        private string idToken;
        private string currentUserId;

        public static User CurrentUserData;

        public event Action OnAuthComplete;

        public void RegisterUser(string email, string password, string userName)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(userName))
            {
                Debug.Log("Поля пустые");
                return;
            }

            StartCoroutine(SetupUserCoroutine(email, password, false, userName));
        }

        public void LoginUser(string email, string password)
        {
            StartCoroutine(SetupUserCoroutine(email, password, true));
        }

        public void SaveUserData()
        {
            StartCoroutine(SetData($"users/{currentUserId}", JsonUtility.ToJson(CurrentUserData)));
        }

        public User[] UpdateLeaderBoard()
        {
            /*try
            {
                if (FirebaseAuth.DefaultInstance.CurrentUser == null)
                {
                    Debug.LogError("User is not authenticated!");
                    return null;
                }

                Debug.Log("Fetching users from Firebase...");

                    var users = new List<User>();
                    foreach (var childSnapshot in snapshot.Children)
                    {
                        var json = childSnapshot.GetRawJsonValue();
                        var user = JsonUtility.FromJson<User>(json);
                        users.Add(new User(user.UserName, user.Clicks));
                        Debug.Log($"User ID: {childSnapshot.Key}, Name: {user.UserName}, Clicks: {user.Clicks}");
                    }

                    users = users.OrderByDescending(x => x.Clicks).ToList();
                    return new[] { users[0], users[1], users[2] };

                Debug.Log("No users found in the database.");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error retrieving users: {e.Message}\nStack Trace: {e.StackTrace}");
            }
            */

            return null;
        }

        private IEnumerator SetData(string path, string jsonData, bool needToUpdateUser = false)
        {
            var url = DB_URL + path + ".json?auth=" + idToken;

            using var request = new UnityWebRequest(url, "PUT");
            var bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Success data PUT!");
                if (needToUpdateUser)
                    StartCoroutine(GetCurrentUser());
            }
            else
                Debug.Log("Error data PUT! " + request.error);
        }

        private IEnumerator GetCurrentUser()
        {
            if (string.IsNullOrEmpty(currentUserId))
            {
                Debug.Log("User not selected!");
                yield break;
            }

            var url = DB_URL + $"users/{currentUserId}" + ".json?auth=" + idToken;
            using var request = UnityWebRequest.Get(url);

            yield return request.SendWebRequest();
            var jsonResponse = request.downloadHandler.text;

            if (jsonResponse == null)
                Debug.Log("No user found in the database.");
            else
            {
                Debug.Log(jsonResponse);
                CurrentUserData = JsonUtility.FromJson<User>(jsonResponse);
            }

            if (request.result == UnityWebRequest.Result.Success)
            {
                OnAuthComplete?.Invoke();
            }
            else
            {
                Debug.Log("Error data GET! " + request.error);
            }
        }

        private IEnumerator SetupUserCoroutine(string email, string password, bool login, string userName = "")
        {
            var methodText = login ? "InWithPassword" : "Up";
            var url = $"https://identitytoolkit.googleapis.com/v1/accounts:sign{methodText}?key={API_KEY}";

            var jsonData = $"{{\"email\":\"{email}\",\"password\":\"{password}\", \"returnSecureToken\":true}}";

            using var request = new UnityWebRequest(url, "POST");

            var bodyRaw = Encoding.UTF8.GetBytes(jsonData);

            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var response = request.downloadHandler.text;
                var authResponse = JsonUtility.FromJson<FirebaseAuthResponse>(response);

                idToken = authResponse.idToken;
                currentUserId = authResponse.localId;

                Debug.Log(authResponse.localId + " | Register successful! " + response);

                if (!login)
                {
                    CurrentUserData = new User(userName, 0);
                    StartCoroutine(SetData($"users/{currentUserId}", JsonUtility.ToJson(CurrentUserData)));
                    OnAuthComplete?.Invoke();
                }
                else
                    StartCoroutine(GetCurrentUser());
            }
            else
                Debug.Log($"Register error: {request.error}");
        }
    }
}

[Serializable]
public class User
{
    public string UserName;
    public int Clicks;

    public User(string userName, int clicks)
    {
        UserName = userName;
        Clicks = clicks;
    }
}

[Serializable]
public class FirebaseAuthResponse
{
    public string idToken;
    public string email;
    public string refreshToken;
    public string expiresIn;
    public string localId;
}