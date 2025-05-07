using ShelfScore.Server.Data;
using ShelfScore.Server.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSqlite<ShelfScoreContext>("Data Source=ShelfScore.db");

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseHttpsRedirection();

app.MapAuthorEndpoints();
app.MapBookEndpoints();
app.MapUserEndpoints();
app.MapRatingEndpoints(); 

app.Run();