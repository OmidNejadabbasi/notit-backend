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
                        NoteId = n.ID,
                        Content = n.Content,
                        CreatedAt = n.CreatedAt,
                        UpdatedAt = n.UpdatedAt,
                        Title = n.Title,
                        NoteType = n.NoteType,
                        ViewCount = n.ViewCount,
                        UserId = n.UserId,
                        UserName = n.User.UserName,
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
}

public class NoteDto
{
    public int NoteId { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string Title { get; set; }
    public NoteType NoteType { get; set; }
    public int? ViewCount { get; set; }
    public string UserId { get; set; }
    public string UserName { get; set; }
    // Include other necessary properties from the User object
}
