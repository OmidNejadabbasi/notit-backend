using System.Security.Claims;

public static class NoteEndpoints
{
    public static void MapNoteEndpoints(this WebApplication app)
    {
        var noteEndpointsGroup = app.MapGroup("/notes").RequireAuthorization();
        noteEndpointsGroup.MapGet("/", GetNotes);
        noteEndpointsGroup.MapPost("/", CreateNote);
    }

    private static async Task<List<NoteDto>> GetNotes(NoteService noteService)
    {
        return await noteService.GetAllNotes();
    }

    private static async Task<Note> CreateNote(
        NoteService noteService,
        Note note,
        IHttpContextAccessor accessor
    )
    {
        note.UserId = accessor.HttpContext!.User.FindFirstValue(ClaimTypes.Sid)!;
        return await noteService.CreateNote(note);
    }
}
