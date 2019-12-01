using DefaultNamespace;
using UnityEngine;

[CreateAssetMenu(fileName = "GoogleDataStorage", menuName = "GoogleDataStorage")]
public class GoogleDataStorage : ScriptableObject
{
    [SerializeField]
    private string _apiKey;

    [SerializeField] 
    private string _clientId;
    
    [SerializeField]
    private string _clientSecret;

    [Space(15)]

    [SerializeField]
    private string _accessCode;

    [SerializeField] 
    private string _accessToken;

    [SerializeField] 
    private string _refreshToken;

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
}
