using System;
using System.Collections.Generic;
using DefaultNamespace;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] 
    private Button _testButton;

    [SerializeField] 
    private TextMeshProUGUI _logText;

    private readonly GoogleDataProvider _googleDataProvider = new GoogleDataProvider();

    public void OnEnable()
    {
        Application.logMessageReceived += OnLogMessage;
    }

    public void OnDisable()
    {
        Application.logMessageReceived -= OnLogMessage;
    }

    public async void Test()
    {
        _logText.text = string.Empty;
        
        _testButton.enabled = false;

        var newSpreadsheetName = "NewTestSpreadsheet";
        
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

        _logText.text += "Finish";
        
        _testButton.enabled = true;
    }

    private void OnLogMessage(string condition, string stackTrace, LogType type)
    {
        _logText.text += DateTime.Now.ToLongDateString() + "\t" + condition + Environment.NewLine;
    }
}
