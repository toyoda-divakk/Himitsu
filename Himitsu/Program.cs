using BliFunc.Models;
using ConsoleAppFramework;
using Himitsu.Commands;
using Himitsu.Dependency.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text;

// DI
var services = new ServiceCollection();
services.AddTransient<HelperService>();

using var serviceProvider = services.BuildServiceProvider();
ConsoleApp.ServiceProvider = serviceProvider;

Console.InputEncoding = Encoding.UTF8;
Console.OutputEncoding = Encoding.UTF8;

// 本体
var app = ConsoleApp.Create();
app.Add<WorkCommands>();
app.Add<TaskCommands>();
app.Add<SingleAiCommands>();
app.Run(args);

