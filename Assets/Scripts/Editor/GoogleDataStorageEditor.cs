using UnityEditor;
using UnityEngine;

namespace DefaultNamespace.Editor
{
    [CustomEditor(typeof(GoogleDataStorage))]
    public class GoogleDataStorageEditor : UnityEditor.Editor
    {
        private GoogleDataStorage _googleDataStorage;
        
        public void OnEnable()
        {
            _googleDataStorage = (GoogleDataStorage)target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (_googleDataStorage.IsAuthStarted)
            {
                if(GUILayout.Button("Finish auth"))
                {
                    _googleDataStorage.FinishAuth();
                }
            }
            else
            {
                if (_googleDataStorage.IsAuthFinished)
                {
                    if(GUILayout.Button("Clear"))
                    {
                        _googleDataStorage.Clear();
                    }
                }
                else
                {
                    if(GUILayout.Button("Start auth"))
                    {
                        _googleDataStorage.StartAuth();
                    }
                }
            }
        }
    }
}