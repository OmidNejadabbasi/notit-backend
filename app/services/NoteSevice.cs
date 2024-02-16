using Microsoft.EntityFrameworkCore;

public class NoteService
{
    private readonly MyDBContext db;

    public NoteService(MyDBContext db)
    {
        this.db = db;
    }

    public async Task<List<NoteDto>> GetAllNotes()
    {
        return await db.Notes
            .Select(
                n =>
                    new NoteDto
                    {
                        Id = n.ID,
                        Content = n.Content,
                        CreatedAt = n.CreatedAt,
                        UpdatedAt = n.UpdatedAt,
                        Title = n.Title,
                        NoteType = n.NoteType,
                        ViewCount = n.ViewCount,
                        UserId = n.UserId,
                    }
            )
            .ToListAsync();
    }

    public async Task<Note> CreateNote(Note note)
    {
        await db.Notes.AddAsync(note);
        await db.SaveChangesAsync();
        return note;
    }

    public async Task<Note> EditNote(int noteId, PartialNote partialNote, string userId)
    {
        var note = await db.Notes.FindAsync(noteId);
        if (note == null || note.UserId != userId)
        {
            throw new Exception("Note not found or not authorized");
        }

        note.Title = partialNote.Title ?? note.Title;
        note.Content = partialNote.Content ?? note.Content;
        note.NoteType = partialNote.NoteType ?? note.NoteType;
        note.ViewCount = partialNote.ViewCount ?? note.ViewCount;
        db.Notes.Update(note);
        db.SaveChanges();

        return note;
    }

    internal async Task DeleteNote(int id, string userId)
    {
        var note = await db.Notes.FindAsync(id);
        if (note == null || note.UserId != userId)
        {
            throw new Exception("Note not found or not authorized");
        }

        db.Notes.Remove(note);
        await db.SaveChangesAsync();
    }
}

public class NoteDto
{
    public int Id { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string Title { get; set; }
    public NoteType NoteType { get; set; }
    public int? ViewCount { get; set; }
    public string UserId { get; set; }
    // Include other necessary properties from the User object
}
