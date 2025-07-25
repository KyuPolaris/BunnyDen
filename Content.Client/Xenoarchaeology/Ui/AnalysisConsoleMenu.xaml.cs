// SPDX-FileCopyrightText: 2023 Guillaume E <262623+quatre@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Kara <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2023 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Visne <39844191+Visne@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 deltanedas <39013340+deltanedas@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 DEATHB4DEFEAT <77995199+DEATHB4DEFEAT@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 sleepyyapril <123355664+sleepyyapril@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later AND MIT

using Content.Client.Stylesheets;
using Content.Client.UserInterface.Controls;
using Content.Shared.Xenoarchaeology.Equipment;
using Microsoft.VisualBasic;
using Robust.Client.AutoGenerated;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Timing;
using Robust.Shared.Utility;

namespace Content.Client.Xenoarchaeology.Ui;

[GenerateTypedNameReferences]
public sealed partial class AnalysisConsoleMenu : FancyWindow
{
    [Dependency] private readonly IEntityManager _ent = default!;
    [Dependency] private readonly IGameTiming _timing = default!;

    public event Action? OnServerSelectionButtonPressed;
    public event Action? OnScanButtonPressed;
    public event Action? OnPrintButtonPressed;
    public event Action? OnExtractButtonPressed;
    public event Action? OnUpBiasButtonPressed;
    public event Action? OnDownBiasButtonPressed;

    // For rendering the progress bar, updated from BUI state
    private TimeSpan? _startTime;
    private TimeSpan? _totalTime;
    private TimeSpan? _accumulatedRunTime;

    private bool _paused;

    public AnalysisConsoleMenu()
    {
        RobustXamlLoader.Load(this);
        IoCManager.InjectDependencies(this);

        ServerSelectionButton.OnPressed += _ => OnServerSelectionButtonPressed?.Invoke();
        ScanButton.OnPressed += _ => OnScanButtonPressed?.Invoke();
        PrintButton.OnPressed += _ => OnPrintButtonPressed?.Invoke();
        ExtractButton.OnPressed += _ => OnExtractButtonPressed?.Invoke();
        UpBiasButton.OnPressed += _ => OnUpBiasButtonPressed?.Invoke();
        DownBiasButton.OnPressed += _ => OnDownBiasButtonPressed?.Invoke();

        var buttonGroup = new ButtonGroup(false);
        UpBiasButton.Group = buttonGroup;
        DownBiasButton.Group = buttonGroup;
    }

    protected override void FrameUpdate(FrameEventArgs args)
    {
        base.FrameUpdate(args);

        if (_startTime is not { } start || _totalTime is not { } total || _accumulatedRunTime is not { } accumulated)
            return;

        var remaining = total - accumulated;
        if (!_paused)
        {
            // If the analyzer is running, its remaining time is further discounted by the time it's been running for.
            remaining += start - _timing.CurTime;
        }
        var secsText = Math.Max((int) remaining.TotalSeconds, 0);

        ProgressLabel.Text = Loc.GetString("analysis-console-progress-text",
            ("seconds", secsText));

        // 1.0 - div because we want it to tick up not down
        ProgressBar.Value = Math.Clamp(1.0f - (float) remaining.Divide(total), 0.0f, 1.0f);
    }

    public void SetButtonsDisabled(AnalysisConsoleUpdateState state)
    {
        ScanButton.Disabled = !state.CanScan;
        PrintButton.Disabled = !state.CanPrint;
        if (state.IsTraversalDown)
            DownBiasButton.Pressed = true;
        else
            UpBiasButton.Pressed = true;

        ExtractButton.Disabled = false;
        if (!state.ServerConnected)
        {
            ExtractButton.Disabled = true;
            ExtractButton.ToolTip = Loc.GetString("analysis-console-no-server-connected");
        }
        else if (!state.CanScan)
        {
            ExtractButton.Disabled = true;

            // CanScan can be false if either there's no analyzer connected or if there's
            // no entity on the scanner. The `Information` text will always tell the user
            // of the former case, but in the latter, it'll only show a message if a scan
            // has never been performed, so add a tooltip to indicate that the artifact
            // is gone.
            if (state.AnalyzerConnected)
            {
                ExtractButton.ToolTip = Loc.GetString("analysis-console-no-artifact-placed");
            }
            else
            {
                ExtractButton.ToolTip = null;
            }
        }
        else if (state.PointAmount <= 0)
        {
            ExtractButton.Disabled = true;
            ExtractButton.ToolTip = Loc.GetString("analysis-console-no-points-to-extract");
        }

        if (ExtractButton.Disabled)
        {
            ExtractButton.RemoveStyleClass("ButtonColorGreen");
        }
        else
        {
            ExtractButton.AddStyleClass("ButtonColorGreen");
            ExtractButton.ToolTip = null;
        }
    }
    private void UpdateArtifactIcon(EntityUid? uid)
    {
        if (uid == null)
        {
            ArtifactDisplay.Visible = false;
            return;
        }

        ArtifactDisplay.Visible = true;
        ArtifactDisplay.SetEntity(uid);
    }

    public void UpdateInformationDisplay(AnalysisConsoleUpdateState state)
    {
        var message = new FormattedMessage();

        if (state.Scanning)
        {
            if (state.Paused)
            {
                message.AddMarkup(Loc.GetString("analysis-console-info-scanner-paused"));
            }
            else
            {
                message.AddMarkup(Loc.GetString("analysis-console-info-scanner"));
            }
            Information.SetMessage(message);
            UpdateArtifactIcon(null); //set it to blank
            return;
        }

        UpdateArtifactIcon(_ent.GetEntity(state.Artifact));

        if (state.ScanReport == null)
        {
            if (!state.AnalyzerConnected) //no analyzer connected
                message.AddMarkup(Loc.GetString("analysis-console-info-no-scanner"));
            else if (!state.CanScan) //no artifact
                message.AddMarkup(Loc.GetString("analysis-console-info-no-artifact"));
            else if (state.Artifact == null) //ready to go
                message.AddMarkup(Loc.GetString("analysis-console-info-ready"));
        }
        else
        {
            message.AddMessage(state.ScanReport);
        }

        Information.SetMessage(message);
    }

    public void UpdateProgressBar(AnalysisConsoleUpdateState state)
    {
        ProgressBar.Visible = state.Scanning;
        ProgressLabel.Visible = state.Scanning;

        _startTime = state.StartTime;
        _totalTime = state.TotalTime;
        _accumulatedRunTime = state.AccumulatedRunTime;
        _paused = state.Paused;
    }
}

