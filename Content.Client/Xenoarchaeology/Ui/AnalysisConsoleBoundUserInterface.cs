// SPDX-FileCopyrightText: 2023 Kara <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2023 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 TemporalOroboros <TemporalOroboros@gmail.com>
// SPDX-FileCopyrightText: 2023 deltanedas <39013340+deltanedas@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 DEATHB4DEFEAT <77995199+DEATHB4DEFEAT@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 sleepyyapril <123355664+sleepyyapril@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later AND MIT

using Content.Shared.Xenoarchaeology.Equipment;
using JetBrains.Annotations;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;

namespace Content.Client.Xenoarchaeology.Ui;

[UsedImplicitly]
public sealed class AnalysisConsoleBoundUserInterface : BoundUserInterface
{
    [ViewVariables]
    private AnalysisConsoleMenu? _consoleMenu;

    public AnalysisConsoleBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
    }

    protected override void Open()
    {
        base.Open();

        _consoleMenu = this.CreateWindow<AnalysisConsoleMenu>();

        _consoleMenu.OnServerSelectionButtonPressed += () =>
        {
            SendMessage(new AnalysisConsoleServerSelectionMessage());
        };
        _consoleMenu.OnScanButtonPressed += () =>
        {
            SendMessage(new AnalysisConsoleScanButtonPressedMessage());
        };
        _consoleMenu.OnPrintButtonPressed += () =>
        {
            SendMessage(new AnalysisConsolePrintButtonPressedMessage());
        };
        _consoleMenu.OnExtractButtonPressed += () =>
        {
            SendMessage(new AnalysisConsoleExtractButtonPressedMessage());
        };
        _consoleMenu.OnUpBiasButtonPressed += () =>
        {
            SendMessage(new AnalysisConsoleBiasButtonPressedMessage(false));
        };
        _consoleMenu.OnDownBiasButtonPressed += () =>
        {
            SendMessage(new AnalysisConsoleBiasButtonPressedMessage(true));
        };
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        switch (state)
        {
            case AnalysisConsoleUpdateState msg:
                _consoleMenu?.SetButtonsDisabled(msg);
                _consoleMenu?.UpdateInformationDisplay(msg);
                _consoleMenu?.UpdateProgressBar(msg);
                break;
        }
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (!disposing)
            return;

        _consoleMenu?.Dispose();
    }
}

