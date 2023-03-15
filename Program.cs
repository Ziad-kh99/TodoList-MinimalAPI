using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using TodoAPI.Data;
using TodoAPI.Models; 

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

//> Add Services to Container
builder.Services.AddDbContext<TodoDb>(opt =>
    opt.UseInMemoryDatabase("TodoList"));
//builder.Services.AddDatabaseDeveloperPageExceptionFilter();


WebApplication app = builder.Build();


//> Configure the HTTP Request Pipeline:
app.MapGet("/", () => "Hello world! - Todo App");

app.MapGet("/todoitems", async (TodoDb db) =>
    await db.Todos.Select(x => new TodoItemDTO(x)).ToListAsync());

app.MapGet("/todoitems/complete", async (TodoDb db) =>
{
    var completedItems = await db.Todos.Where(i => i.IsCompleted).ToListAsync();

    var completedItemsDTO = completedItems.Select(x => new TodoItemDTO(x));
    
    return Results.Ok(completedItemsDTO);
});




app.MapGet("/todoitems/{id}", async (int id, TodoDb db) =>
{
    var item = await db.Todos.FindAsync(id);

    return item is Todo ? Results.Ok(new TodoItemDTO(item)) : Results.NotFound();
});

app.MapPost("/todoitems", async (TodoItemDTO todoItemDTO, TodoDb db) =>
{
    var todoItem = new Todo
    {
        Name = todoItemDTO.Name,
        IsCompleted = todoItemDTO.IsCompleted
    };

    db.Todos.Add(todoItem);
    await db.SaveChangesAsync();

    return Results.Created($"todoitems/{todoItem.Id}", new TodoItemDTO(todoItem));
});

app.MapPut("/todoitems/{id}", async (int id, TodoItemDTO todoItemDTO, TodoDb db) =>
{
    var todoItem = await db.Todos.FindAsync(id);

    if (todoItem is null)
    {
        return Results.NotFound(); 
    }

    todoItem.Name = todoItemDTO.Name;
    todoItem.IsCompleted = todoItemDTO.IsCompleted; 

    await db.SaveChangesAsync(); 

    return Results.NoContent();
});

app.MapDelete("/todoitems/{id}", async (int id, TodoDb db) =>
{
    var todoItem = await db.Todos.FindAsync(id);

    if (todoItem is null)
        return Results.NotFound();

    db.Todos.Remove(todoItem);

    await db.SaveChangesAsync();

    return Results.NoContent();
});


app.Run();
