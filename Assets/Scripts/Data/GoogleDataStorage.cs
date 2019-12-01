using System.Net.Http;
using System.Threading.Tasks;
using DefaultNamespace;
using Google.GData.Client;
using Newtonsoft.Json.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "GoogleDataStorage", menuName = "GoogleDataStorage")]
public class GoogleDataStorage : ScriptableObject
{
    //todo move
    private const string CHECK_TOKEN_EXPIRES = "https://www.googleapis.com/oauth2/v1/tokeninfo?access_token=ACCESS_TOKEN";
    private const string UPDATE_ACCESS_TOKEN =
        "https://www.googleapis.com/oauth2/v4/token?client_id=CLIENT_ID&client_secret=CLIENT_SECRET&refresh_token=REFRESH_TOKEN&grant_type=refresh_token";
    
    private const int REMAINING_TIME_TO_REFRESH_ACCESS_TOKEN = 30;
    
    [SerializeField] private string _apiKey;

    [SerializeField] private string _clientId;

    [SerializeField] private string _clientSecret;

    [Space(15)] [SerializeField] private string _accessCode;

    [SerializeField] private string _accessToken;

    [SerializeField] private string _refreshToken;
    
    private readonly HttpClient _httpClient = new HttpClient(new HttpClientHandler(), false);

#if UNITY_EDITOR

    private GoogleApi _googleApi;
    private GoogleApi GoogleApi => _googleApi ?? (_googleApi = new GoogleApi(_clientId, _clientSecret));

    public bool IsAuthStarted => !string.IsNullOrEmpty(_accessCode);

    public bool IsAuthFinished => !string.IsNullOrEmpty(_accessToken) && !string.IsNullOrEmpty(_refreshToken);

    public void StartAuth()
    {
        GoogleApi.GenerateAccessCode();
    }

    public void FinishAuth()
    {
        var credentials = GoogleApi.GenerateAccessToken(_accessCode);

        _accessToken = credentials.accessToken;
        _refreshToken = credentials.refreshToken;

        _accessCode = null;

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

    public async Task RefreshAccessTokenIfExpires()
    {
        var url = new URLBuilder(CHECK_TOKEN_EXPIRES);
        url.AddAccessToken(_accessToken);

        using (var response = await _httpClient.GetAsync(url.GetURL()))
        {
            var content = await response.Content.ReadAsStringAsync();

            var jObjectResponse = JObject.Parse(content);

            int.TryParse((string) jObjectResponse["expires_in"], out var expiresTime);

            if (expiresTime <= REMAINING_TIME_TO_REFRESH_ACCESS_TOKEN)
            {
                await RefreshAccessToken();
            }
        }
    }

    private async Task RefreshAccessToken()
    {
        var url = new URLBuilder(UPDATE_ACCESS_TOKEN);
        url.AddCliendId(_clientId);
        url.AddCliendSecret(_clientSecret);
        url.AddRefreshToken(_refreshToken);

        using (var response = await _httpClient.PostAsync(url.GetURL(), null))
        {
            var content = await response.Content.ReadAsStringAsync();
            
            var jObjectResponse = JObject.Parse(content);

            _accessToken = (string) jObjectResponse["access_token"];
        }
    }
}
