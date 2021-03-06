using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ZL.Saving
{
    public class SavingSystem : MonoBehaviour
    {
        public IEnumerator LoadLastScene(string saveFile)
        {
            Dictionary<string, object> state = LoadFile(saveFile);
            if (state.ContainsKey("lastSceneBuildIndex"))
            {
                int buildIndex = (int)state["lastSceneBuildIndex"];

                if (buildIndex != SceneManager.GetActiveScene().buildIndex)
                    yield return SceneManager.LoadSceneAsync(buildIndex);
            }
            yield return null;
            RestoreState(state);
        }

        public void Save(string saveFile)
        {
            Dictionary<string, object> state = LoadFile(saveFile);
            CaptureState(state);
            SaveFile(saveFile, state);
        }

        public void Load(string saveFile)
        {
            RestoreState(LoadFile(saveFile));
        }

        private Dictionary<string, object> LoadFile(string saveFile)
        {
            string path = GetPathFromSaveFile(saveFile);

            if (!File.Exists(path))
                return new Dictionary<string, object>();

            using (FileStream stream = File.Open(path, FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return (Dictionary<string, object>)formatter.Deserialize(stream);
            }
        }

        private void SaveFile(string saveFile, object state)
        {
            string path = GetPathFromSaveFile(saveFile);
            print("Saving to " + path);

            using (FileStream stream = File.Open(path, FileMode.Create))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, state);
            }
        }

        private void RestoreState(Dictionary<string, object> state)
        {
            Dictionary<string, object> stateDict = state;

            foreach (SavableEntity savable in FindObjectsOfType<SavableEntity>())
            {
                string uniqueIdentifier = savable.GetUniqueIdentifier();
                if (stateDict.ContainsKey(uniqueIdentifier))
                    savable.RestoreState(stateDict[uniqueIdentifier]);
            }
        }

        private void CaptureState(Dictionary<string, object> state)
        {
            foreach (SavableEntity savable in FindObjectsOfType<SavableEntity>())
                state[savable.GetUniqueIdentifier()] = savable.CaptureState();

            state["lastSceneBuildIndex"] = SceneManager.GetActiveScene().buildIndex;
        }

        private string GetPathFromSaveFile(string saveFile)
        {
            string[] directoryPaths = { Application.persistentDataPath, "Saves" };
            string directoryPath = Path.Combine(directoryPaths);
            Directory.CreateDirectory(directoryPath);

            string[] paths = { Application.persistentDataPath, "Saves", saveFile + ".bin" };
            string fullPath = Path.Combine(paths);
            return fullPath;
        }
    }
}