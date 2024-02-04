public class NoteService
{
    private readonly MyDBContext db;

    public NoteService(MyDBContext db)
    {
        this.db = db;
    }

    public List<Note> GetAllNotes()
    {
        return db.Notes.ToList();
    }

    public Note CreateNote(Note note)
    {
        db.Notes.Add(note);
        db.SaveChanges();
        return note;
    }
}
