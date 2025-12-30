using Spectre.Console.Cli;
using ReverseGeocode.Commands;

var app = new CommandApp<LoadGeocodeDataCommand>();

app.Run(args);
