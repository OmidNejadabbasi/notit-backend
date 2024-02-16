using System.Security.Claims;

public static class NoteEndpoints
{
    public static void MapNoteEndpoints(this WebApplication app)
    {
        var noteEndpointsGroup = app.MapGroup("/notes").RequireAuthorization();
        noteEndpointsGroup.MapGet("/", GetNotes);
        noteEndpointsGroup.MapPost("/", CreateNote);
        noteEndpointsGroup.MapPut("/{id}", EditNote);
        noteEndpointsGroup.MapDelete("/{id}", DeleteNote);
    }

    private static async Task DeleteNote(NoteService noteService, int id, IHttpContextAccessor accessor)
    {
        await noteService.DeleteNote(
            id,
            accessor.HttpContext!.User.FindFirstValue(ClaimTypes.Sid)!
        );
    }

    private static async Task<Note> EditNote(
        int id,
        IHttpContextAccessor accessor,
        PartialNote partialNote,
        NoteService noteService
    )
    {
        return await noteService.EditNote(
            id,
            partialNote,
            accessor.HttpContext!.User.FindFirstValue(ClaimTypes.Sid)!
        );
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
