using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using KS.RustAnalyzer.Cargo;
using KS.RustAnalyzer.Common;
using Microsoft.VisualStudio.Workspace;

namespace KS.RustAnalyzer.VS;

public class FileContextActionProvider : IFileContextActionProvider
{
    private readonly IWorkspace _workspace;
    private readonly IOutputWindowPane _outputPane;
    private readonly ITelemetryService _telemetryService;
    private readonly ILogger _logger;
    private readonly Func<string, Task> _showMessageBox;

    public FileContextActionProvider(IWorkspace workspace, IOutputWindowPane outputPane, ITelemetryService telemetryService, ILogger logger)
    {
        _workspace = workspace;
        _outputPane = outputPane;
        _telemetryService = telemetryService;
        _logger = logger;
        _showMessageBox = ShowMessageBoxAsync;
    }

    public async Task<IReadOnlyList<IFileContextAction>> GetActionsAsync(string filePath, FileContext fileContext, CancellationToken cancellationToken)
    {
        var actions = new List<IFileContextAction>();
        if (!Manifest.IsManifest(filePath))
        {
            return actions;
        }

        await _workspace.JTF.SwitchToMainThreadAsync();

        _outputPane.Initialize();
        actions.Add(new BuildFileContextAction(filePath, fileContext, _outputPane, _telemetryService, _showMessageBox, _logger));

        return actions;
    }

    private async Task ShowMessageBoxAsync(string message)
    {
        await _workspace.JTF.SwitchToMainThreadAsync();

        MessageBox.Show(message, "rust-analyzer", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }
}
