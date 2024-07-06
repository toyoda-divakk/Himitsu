using BliFunc.Models;
using ConsoleAppFramework;
using Himitsu.Commands;
using System;

var app = ConsoleApp.Create();
app.Add<MyCommands>();
app.Add<WorkCommands>();
app.Run(args);
