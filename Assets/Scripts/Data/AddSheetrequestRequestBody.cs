using System;
using System.Collections.Generic;

namespace Data
{
    [Serializable]
    public class AddSheetTitleProperty
    {
        public string title;

        public AddSheetTitleProperty(string name)
        {
            title = name;
        }
    }

    [Serializable]
    public class AddSheetProperty
    {
        public AddSheetTitleProperty properties;

        public AddSheetProperty(string name)
        {
            properties = new AddSheetTitleProperty(name);
        }
    }

    [Serializable]
    public class AddSheetRequest
    {
        public AddSheetProperty addSheet;

        public AddSheetRequest(string name)
        {
            addSheet = new AddSheetProperty(name);
        }
    }
    
    [Serializable]
    public class AddSheetRequestBody
    {
        public List<AddSheetRequest> requests = new List<AddSheetRequest>();

        public void Add(string name)
        {
            requests.Add(new AddSheetRequest(name));
        }
    }

    public static class AddSheetRequestBodyAdapter
    {
        public static AddSheetRequestBody GetAddSheetRequestBody(ICollection<string> names)
        {
            var addSheetRequestBody = new AddSheetRequestBody();

            foreach (var name in names)
            {
                addSheetRequestBody.Add(name);
            }

            return addSheetRequestBody;
        }
    }
}