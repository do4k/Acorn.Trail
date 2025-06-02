using Acorn.Trail.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = new HostApplicationBuilder(args);

builder.Services.AddAcornTrailServices();
builder.Services.AddSingleton<Microsoft.Xna.Framework.Game, Acorn.Trail.MapComponent>();

var app = builder.Build();

using var game = app.Services.GetRequiredService<Microsoft.Xna.Framework.Game>();
game.Run();