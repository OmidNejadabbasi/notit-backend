using System.ComponentModel;
using Microsoft.EntityFrameworkCore.ChangeTracking;

public static class NoteEndpoints
{
    public static void MapNoteEndpoints(this WebApplication app)
    {
        var noteEndpointsGroup = app.MapGroup("/notes").RequireAuthorization();
        noteEndpointsGroup.MapGet("/", GetNotes);
        noteEndpointsGroup.MapPost("/", CreateNote);
    }

    private static async Task<List<Note>> GetNotes(NoteService noteService)
    {
        return noteService.GetAllNotes();
    }

    private static async Task<Note> CreateNote(NoteService noteService, Note note){
        return noteService.CreateNote(note);
    }
}
