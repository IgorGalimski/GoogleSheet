using System;
using System.Collections.Generic;

namespace Data
{
    public enum ValueInputOption
    {
        RAW,
        USER_ENTERED
    }

    public enum MajorDimension
    {
        DIMENSION_UNSPECIFIED,
        ROWS,
        COLUMNS
    }
    
    [Serializable]
    public class BatchRequestBody
    {
        public ValueInputOption valueInputOption = ValueInputOption.USER_ENTERED;
        public List<ValueRange> data = new List<ValueRange>();

        public void Add(ValueRange data)
        {
            this.data.Add(data);
        }
    }

    [Serializable]
    public class ValueRange
    {
        public string range = "";
        public MajorDimension majorDimension = MajorDimension.ROWS;
        public List<List<object>> values = new List<List<object>>();

        public ValueRange()
        {
        }

        public ValueRange(List<List<object>> data)
        {
            values = data;
        }

        public ValueRange(List<object> data)
        {
            values.Add(data);
        }

        public ValueRange(string data)
        {
            values.Add(new List<object>()
            {
                data
            });
        }

        public void Add(List<object> data)
        {
            values.Add(data);
        }
    }
}