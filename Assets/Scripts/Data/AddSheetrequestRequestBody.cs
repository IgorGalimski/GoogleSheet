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

    public class DeleteSheetProperty
    {
        public string sheetId;

        public DeleteSheetProperty(string sheetId)
        {
            this.sheetId = sheetId;
        }
    }
    
    public class DeleteSheetRequest
    {
        public DeleteSheetProperty deleteSheet;
        
        public DeleteSheetRequest(string sheetId)
        {
            deleteSheet = new DeleteSheetProperty(sheetId);
        }
    }

    [Serializable]
    public class AddSheetRequestBody
    {
        public List<AddSheetRequest> requests = new List<AddSheetRequest>();

        public void AddSheetRequest(string name)
        {
            requests.Add(new AddSheetRequest(name));
        }
    }

    [Serializable]
    public class DeleteSheetRequestBody
    {
        public List<DeleteSheetRequest> requests = new List<DeleteSheetRequest>();

        public void Add(string id)
        {
            requests.Add(new DeleteSheetRequest(id));
        }
    }

    public static class AddSheetRequestBodyAdapter
    {
        public static AddSheetRequestBody GetAddSheetRequestBody(ICollection<string> names)
        {
            var addSheetRequestBody = new AddSheetRequestBody();

            foreach (var name in names)
            {
                addSheetRequestBody.AddSheetRequest(name);
            }

            return addSheetRequestBody;
        }

        public static DeleteSheetRequestBody GetDeleteSheetRequestBody(ICollection<int> ids)
        {
            var deleteSheetRequestBody = new DeleteSheetRequestBody();

            foreach (var id in ids)
            {
                deleteSheetRequestBody.Add(id.ToString());
            }

            return deleteSheetRequestBody;
        }
    }
}