using BliFunc.Models;
using ConsoleAppFramework;
using Himitsu.Commands;
using Himitsu.Dependency.Services;
using Microsoft.Extensions.DependencyInjection;
using System;

// DI
var services = new ServiceCollection();
services.AddTransient<HelperService>();

using var serviceProvider = services.BuildServiceProvider();
ConsoleApp.ServiceProvider = serviceProvider;

// 本体
var app = ConsoleApp.Create();
app.Add<WorkCommands>();
app.Add<TaskCommands>();
app.Add<AiCommands>();
app.Run(args);

