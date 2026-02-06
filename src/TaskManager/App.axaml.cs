using System;
using System.IO;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia.Markup.Xaml;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using SukiUI.Toasts;
using TaskManager.Presentation.ViewModels;
using TaskManager.Presentation.Views;
using TaskManager.Services.TaskRepository;
using TaskManager.Services.TaskRepository.Ef;

namespace TaskManager;

public partial class App : Application
{
  public override void Initialize()
  {
    AvaloniaXamlLoader.Load(this);
  }

  public override void OnFrameworkInitializationCompleted()
  {
    var builder = Host.CreateDefaultBuilder()
      .ConfigureAppConfiguration(configHost =>
      {
        configHost.SetBasePath(Directory.GetCurrentDirectory());
        configHost.AddJsonFile("appsettings.json", false);
      });

    builder.ConfigureServices((hostContext, services) =>
    {
      services.AddSerilog((provider, configuration) =>
      {
        configuration
          .ReadFrom.Configuration(hostContext.Configuration)
          .Enrich.FromLogContext()
          .WriteTo.Console();
      });
      
      services.AddDbContextFactory<TasksContext>(opt => 
        opt.UseNpgsql(hostContext.Configuration.GetConnectionString("TasksDatabase")));
      
      services.AddScoped<ITaskRepository, EntityFrameworkTaskRepository>();

      services.AddSingleton<ISukiToastManager, SukiToastManager>();
      services.AddSingleton<IDesktopServices, SukiDesktopServices>();
      
      services.AddScoped<MainWindowViewModel>();
      services.AddScoped<ConnectionViewModel>();
      services.AddScoped<TaskListViewModel>();
    });

    var host = builder.Build();

    host.Start();
    
    
    if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
    {
      // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
      // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
      DisableAvaloniaDataAnnotationValidation();

      var mainVm = host.Services.GetRequiredService<MainWindowViewModel>();
      var connectionVm = host.Services.GetRequiredService<ConnectionViewModel>();
      var taskVm = host.Services.GetRequiredService<TaskListViewModel>();

      connectionVm.Connect.Subscribe(_ => mainVm.Content = taskVm);
      connectionVm.Connect.Execute().Catch(Observable.Empty<Unit>()).Subscribe();

      mainVm.Content = connectionVm;
      
      desktop.MainWindow = new MainWindow
      {
        DataContext = mainVm,
      };
    }

    base.OnFrameworkInitializationCompleted();
  }

  private void DisableAvaloniaDataAnnotationValidation()
  {
    // Get an array of plugins to remove
    var dataValidationPluginsToRemove =
      BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

    // remove each entry found
    foreach (var plugin in dataValidationPluginsToRemove)
    {
      BindingPlugins.DataValidators.Remove(plugin);
    }
  }
}