using System;
using System.Threading.Tasks;
using labo.signalr.api.Data;
using labo.signalr.api.Models;
using Microsoft.AspNetCore.SignalR;


public class TaskHub : Hub
    {
    public static class UserHandler
    {
        public static HashSet<string> ConnectedIds = new HashSet<string>();
    }
    ApplicationDbContext _context;

    public TaskHub(ApplicationDbContext context)
    {
        _context = context;
    }
    public override async Task OnConnectedAsync() {
         await base.OnConnectedAsync();
        UserHandler.ConnectedIds.Add(Context.ConnectionId);
        await Clients.All.SendAsync("UserCount", UserHandler.ConnectedIds.Count);
        await Clients.Caller.SendAsync("TaskList", _context.UselessTasks.ToList());
    }
    public async Task AddTask(string task) {

        _context.UselessTasks.Add(new UselessTask { Text = task });
       await _context.SaveChangesAsync();
        await Clients.All.SendAsync("TaskList", _context.UselessTasks.ToList());
    
    }
    public async Task CompleteTask(int taskId) {
        var task = _context.UselessTasks.Single(t => t.Id == taskId);
        task.Completed = true;
        await _context.SaveChangesAsync();
        await Clients.All.SendAsync("TaskList", _context.UselessTasks.ToList());
    }
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
    
        // TODO: Ajouter votre logique
        UserHandler.ConnectedIds.Remove(Context.ConnectionId);
        await Clients.All.SendAsync("UserCount", UserHandler.ConnectedIds.Count);

        await base.OnDisconnectedAsync(exception);
    }

}

