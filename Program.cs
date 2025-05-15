using BackEnd.Exceptions;
using BackEnd.Extensions;
using BackEnd.Hubs;
using BackEnd.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var services = builder.Services;

services.AddControllers();
services.AddSignalR();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
services.AddDbContext<IChatroomContext>(options =>
    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

services.AddGoogleAuthentication(configuration);
services.AddJwtAuthentication(configuration);
services.AddApplicationServices();
services.AddCustomCors();
services.AddCustomRedis(configuration);


var app = builder.Build();

app.UseCors("AllowFrontend");
app.UseMiddleware<ExceptionMiddleware>();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.MapHub<ChatHub>("/chathub");
app.MapHub<FriendNotificationHub>("/FriendNotificationHub");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<IChatroomContext>();
    db.Database.Migrate();
}

app.Run();
