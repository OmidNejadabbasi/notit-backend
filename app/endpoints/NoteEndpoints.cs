using System.ComponentModel;
using Microsoft.EntityFrameworkCore.ChangeTracking;

public static class NoteEndpoints
{
    public static void MapNoteEndpoints(this WebApplication app)
    {
        var noteEndpointsGroup = app.MapGroup("/notes");
        noteEndpointsGroup.MapGet("/", GetNotes);
    }

    private static async Task<List<Note>> GetNotes(NoteService noteService)
    {
        return noteService.GetAllNotes();
    }
}
