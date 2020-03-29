using System;
using System.Collections.Generic;
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

    public async void Start()
    {
        var newSpreadsheetName = "NewSpreadsheet: " + DateTime.Now.ToLongTimeString();
        
        await _googleDataProvider.Init();

        await _googleDataProvider.CreateSpreadsheet(newSpreadsheetName);

        var spreadSheet = _googleDataProvider[newSpreadsheetName];

        var sheetName = "test";

        await spreadSheet.CreateSheets(new List<string>
        {
            sheetName
        });

        var sheet = spreadSheet[sheetName];
        sheet["A1"].Value = "TestCell";

        await spreadSheet.Save();
    }

    public async void LoadSpreadSheets()
    {
        _loadingIndicator.gameObject.SetActive(true);
        
        await _googleDataProvider.LoadSpreadsheets();
        
        _loadingIndicator.gameObject.SetActive(false);

        _logText.text += "SpreadSpreadshets loaded" + Environment.NewLine;
    }
}
