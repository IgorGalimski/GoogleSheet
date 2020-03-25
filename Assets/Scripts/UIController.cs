using System;
using DefaultNamespace;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] 
    private RectTransform _loadingIndicator;

    [SerializeField] 
    private TextMeshProUGUI _logText;

    private readonly GoogleDataProvider _googleDataProvider = new GoogleDataProvider();
    
    public async void LoadSpreadSheets()
    {
        _loadingIndicator.gameObject.SetActive(true);
        
        await _googleDataProvider.LoadSpreadsheets();
        
        _loadingIndicator.gameObject.SetActive(false);

        _logText.text += "SpreadSpreadshets loaded" + Environment.NewLine;
    }
}
