using System.Collections.Generic;
using Data;

namespace DefaultNamespace
{
    public static class GoogleSpreadsheetAdapter
    {
        //TODO Optimize
        public static BatchRequestBody GetBatchRequestBody(GoogleSpreadsheet googleSpreadsheet)
        {
            var requestData = new BatchRequestBody();

            foreach (var googleSheet in googleSpreadsheet)
            {
                var valueRange = new ValueRange();
                valueRange.range = googleSheet.Name + "!A1:Z1000";
                
                foreach (var googleSheetRow in googleSheet.GoogleSheetRows)
                {
                    var row = new List<object>();
                    
                    foreach (var value in googleSheetRow)
                    {
                        row.Add(value);
                    }
                    
                    valueRange.Add(row);
                }
                
                requestData.Add(valueRange);
            }
            
            return requestData;
        }
    }
}