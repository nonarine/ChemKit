using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MudBlazor;
using Microsoft.Extensions.Logging;

using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading.Tasks;

using ChemKit.Components;

namespace ChemKit;

public class LoadingBarService
{
    public IDialogService? DialogService { get; set; }
    private readonly ILogger<LoadingBarService> _logger;
    private IDialogReference _dialogReference;

    public bool IsVisible { get; private set; }
    public double ProgressPercentage { get; private set; }
    public event Action OnStateChanged;

    public LoadingBarService(ILogger<LoadingBarService> logger)
    {
        _logger = logger;
    }

    public async Task<List<TResult>> WhenAll<TResult>(IEnumerable<Task<TResult>> tasks)
    {
        IsVisible = true;
        ProgressPercentage = 0;
        OnStateChanged?.Invoke();

        if(DialogService is null) throw new ArgumentNullException(nameof(DialogService));
        
        _logger.LogInformation("Showing loading bar");

        var options = new DialogOptions
        {
            CloseButton = false,
            MaxWidth = MaxWidth.Large,
            FullWidth = true,
            BackdropClick = false
        };

        _dialogReference = DialogService.Show<LoadingBar>("Loading...", options = options);

        var taskList = tasks.ToList();
        int totalTasks = taskList.Count;
        int completedTasks = 0;

        foreach (var task in taskList)
        {
            task.ContinueWith(t =>
            {
                completedTasks++;
                ProgressPercentage = (double)completedTasks / totalTasks * 100;
                _logger.LogInformation($"Progress updated: {ProgressPercentage}%");
                OnStateChanged?.Invoke();
            });
        }

        var results = await Task.WhenAll(taskList);

        await Task.Delay(1000);

        IsVisible = false;
        OnStateChanged?.Invoke();
        _dialogReference.Close();
        _logger.LogInformation("Closing loading bar");

        return results.ToList();
    }
}
