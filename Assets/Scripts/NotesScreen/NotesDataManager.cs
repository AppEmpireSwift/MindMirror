using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace NotesScreen
{
    public static class NotesDataManager
    {
        private const string NotesFileName = "health_notes.json";
        private static string FilePath => Path.Combine(Application.persistentDataPath, NotesFileName);

        public static bool SaveNotes(List<NoteData> notes)
        {
            try
            {
                string jsonData = JsonConvert.SerializeObject(notes, Formatting.Indented);
                File.WriteAllText(FilePath, jsonData);
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return false;
            }
        }

        public static List<NoteData> LoadNotes()
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    string jsonData = File.ReadAllText(FilePath);
                    var notes = JsonConvert.DeserializeObject<List<NoteData>>(jsonData);
                    return notes ?? new List<NoteData>();
                }
                else
                {
                    return new List<NoteData>();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return new List<NoteData>();
            }
        }

        public static bool AddNote(NoteData note)
        {
            var notes = LoadNotes();
            notes.Add(note);
            return SaveNotes(notes);
        }

        public static bool UpdateNote(NoteData oldNote, NoteData updatedNote)
        {
            var notes = LoadNotes();

            for (int i = 0; i < notes.Count; i++)
            {
                if (notes[i].Title == oldNote.Title &&
                    notes[i].Description == oldNote.Description)
                {
                    notes[i] = updatedNote;
                    return SaveNotes(notes);
                }
            }

            return false;
        }

        public static bool DeleteNote(NoteData note)
        {
            var notes = LoadNotes();

            for (int i = 0; i < notes.Count; i++)
            {
                if (notes[i].Title == note.Title &&
                    notes[i].Description == note.Description)
                {
                    notes.RemoveAt(i);
                    return SaveNotes(notes);
                }
            }

            return false;
        }
    }
}