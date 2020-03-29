using System.Collections.Generic;
using Data;

namespace DefaultNamespace
{
    public static class GoogleSpreadsheetAdapter
    {
        //TODO Optimize
        public static BatchRequestBody GetBatchRequestBody(Spreadsheet spreadsheet)
        {
            var requestData = new BatchRequestBody();

            foreach (var googleSheet in spreadsheet)
            {
                var valueRange = new ValueRange();
                valueRange.range = googleSheet.Name + "!A1:Z1000";
                
                foreach (var googleSheetRow in googleSheet.Rows)
                {
                    var row = new List<object>();
                    
                    foreach (var googleSheetCell in googleSheetRow)
                    {
                        row.Add(googleSheetCell.Value);
                    }
                    
                    valueRange.Add(row);
                }
                
                requestData.Add(valueRange);
            }
            
            return requestData;
        }
    }
}