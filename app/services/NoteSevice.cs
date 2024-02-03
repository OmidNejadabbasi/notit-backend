public class NoteService {
    private readonly MyDBContext db;
    public NoteService(MyDBContext db){
        this.db  =db;
    }

    public List<Note> GetAllNotes(){
        return db.Notes.ToList();
    }
}