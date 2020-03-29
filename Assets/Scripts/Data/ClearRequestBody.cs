using System.Collections.Generic;

namespace Data
{
    public class ClearRequestBody
    {
        public List<string> ranges = new List<string>();
    }

    public static class ClearRequestBodyAdapter
    {
        public static ClearRequestBody GetClearRequestBody(ICollection<Sheet> googleSheets)
        {
            var clearRequestBody = new ClearRequestBody();

            foreach (var googleSheet in googleSheets)
            {
                clearRequestBody.ranges.Add(googleSheet.Name);
            }
            
            return clearRequestBody;
        }
    }
}