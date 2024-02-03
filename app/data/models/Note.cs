public class Note
{
    public int ID { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public NoteType NoteType { get; set; }

    public User user { get; set; }
    public Tag[]? Tags { get; set; }

    // shows how many times the note has been viewed
    public int? ViewCount { get; set; }
}

public enum NoteType
{
    NOTE,
    FLASHCARD,
}
