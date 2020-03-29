using System;
using System.Threading.Tasks;
using DefaultNamespace;
using Google.GData.Client;
using Newtonsoft.Json.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "GoogleDataStorage", menuName = "GoogleDataStorage")]
public class GoogleDataStorage : ScriptableObject
{
    private const int REMAINING_TIME_TO_REFRESH_ACCESS_TOKEN = 3500;

    private DateTime? _lastCheck;

    private static GoogleDataStorage _instance;

    public static GoogleDataStorage Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<GoogleDataStorage>(nameof(GoogleDataStorage));
            }

            return _instance;
        }
    }
    
    [SerializeField] 
    private string _apiKey;
    public string ApiKey => _apiKey;

    [SerializeField] private string _clientId;

    [SerializeField] private string _clientSecret;

    [Space(15)] [SerializeField] private string _accessCode;

    [SerializeField] 
    private string _accessToken;
    public string AccessToken => _accessToken;

    [SerializeField] private string _refreshToken;

#if UNITY_EDITOR

    private GoogleApiAuth googleApiAuth;
    private GoogleApiAuth GoogleApiAuth => googleApiAuth ?? (googleApiAuth = new GoogleApiAuth(_clientId, _clientSecret));

    public bool IsAuthStarted => !string.IsNullOrEmpty(_accessCode);

    public bool IsAuthFinished => !string.IsNullOrEmpty(_accessToken) && !string.IsNullOrEmpty(_refreshToken);

    public void StartAuth()
    {
        GoogleApiAuth.GenerateAccessCode();
    }

    public void FinishAuth()
    {
        var credentials = GoogleApiAuth.GenerateAccessToken(_accessCode);

        _accessToken = credentials.accessToken;
        _refreshToken = credentials.refreshToken;

        Save();
    }

    public void Clear()
    {
        _accessCode = null;
    }

    private void Save()
    {
        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.AssetDatabase.SaveAssets();
    }

#endif

    private async Task<bool> CheckAccessTokenExpire()
    {
        var urlBuilder = URLBuilder
            .CheckTokenExpires()
            .AddAccessToken(_accessToken);

        using (var response = await Utils.HttpClient.GetAsync(urlBuilder.GetURL()))
        {
            var content = await response.Content.ReadAsStringAsync();

            var jObjectResponse = JObject.Parse(content);

            if (int.TryParse((string) jObjectResponse["expires_in"], out var expired))
            {
                return expired <= 0;
            }
        }

        return true;
    }

    public async Task RefreshAccessToken()
    {
        if(_lastCheck != null && (DateTime.Now - _lastCheck.Value).Seconds < REMAINING_TIME_TO_REFRESH_ACCESS_TOKEN)
        {
            Debug.Log("Access token is valid");
            
            return;
        }
        
        var urlBuilder = URLBuilder.UpdateAccessToken().
            AddClientId(_clientId).
            AddClientSecret(_clientSecret).
            AddRefreshToken(_refreshToken).
            AddGrantType("refresh_token");

        using (var response = await Utils.HttpClient.PostAsync(urlBuilder.GetURL(), null))
        {
            var content = await response.Content.ReadAsStringAsync();
            
            var jObjectResponse = JObject.Parse(content);

            _accessToken = (string) jObjectResponse["access_token"];
            
            _lastCheck = DateTime.Now;
            
            Debug.Log("Update access token: " + _lastCheck);
        }
    }
}
