using System;

namespace NotesScreen
{
    [Serializable]
    public class NoteData
    {
        public TagType TagType;
        public byte[] Photo;
        public string Title;
        public string Description;
    }
}