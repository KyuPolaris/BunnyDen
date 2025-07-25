// SPDX-FileCopyrightText: 2022 Eoin Mcloughlin <helloworld@eoinrul.es>
// SPDX-FileCopyrightText: 2022 Flipp Syder <76629141+vulppine@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 Vera Aguilera Puerto <6766154+Zumorica@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 eoineoineoin <eoin.mcloughlin+gh@gmail.com>
// SPDX-FileCopyrightText: 2022 mirrorcult <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2022 vulppine <vulppine@gmail.com>
// SPDX-FileCopyrightText: 2023 Ilya246 <57039557+Ilya246@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 TemporalOroboros <TemporalOroboros@gmail.com>
// SPDX-FileCopyrightText: 2023 c4llv07e <38111072+c4llv07e@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 sleepyyapril <123355664+sleepyyapril@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later AND MIT

using Content.Shared.Atmos;
using Content.Shared.Atmos.Monitor;
using Content.Shared.Atmos.Monitor.Components;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Log;

namespace Content.Client.Atmos.Monitor.UI;

public sealed class AirAlarmBoundUserInterface : BoundUserInterface
{
    private AirAlarmWindow? _window;

    public AirAlarmBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {
    }

    protected override void Open()
    {
        base.Open();

        _window = this.CreateWindow<AirAlarmWindow>();
        _window.SetEntity(Owner);

        _window.AtmosDeviceDataChanged += OnDeviceDataChanged;
		_window.AtmosDeviceDataCopied += OnDeviceDataCopied;
        _window.AtmosAlarmThresholdChanged += OnThresholdChanged;
        _window.AirAlarmModeChanged += OnAirAlarmModeChanged;
        _window.AutoModeChanged += OnAutoModeChanged;
        _window.ResyncAllRequested += ResyncAllDevices;
        _window.AirAlarmTabChange += OnTabChanged;
    }

    private void ResyncAllDevices()
    {
        SendMessage(new AirAlarmResyncAllDevicesMessage());
    }

    private void OnDeviceDataChanged(string address, IAtmosDeviceData data)
    {
        SendMessage(new AirAlarmUpdateDeviceDataMessage(address, data));
    }

	private void OnDeviceDataCopied(IAtmosDeviceData data)
    {
        SendMessage(new AirAlarmCopyDeviceDataMessage(data));
    }

    private void OnAirAlarmModeChanged(AirAlarmMode mode)
    {
        SendMessage(new AirAlarmUpdateAlarmModeMessage(mode));
    }

    private void OnAutoModeChanged(bool enabled)
    {
        SendMessage(new AirAlarmUpdateAutoModeMessage(enabled));
    }

    private void OnThresholdChanged(string address, AtmosMonitorThresholdType type, AtmosAlarmThreshold threshold, Gas? gas = null)
    {
        SendMessage(new AirAlarmUpdateAlarmThresholdMessage(address, type, threshold, gas));
    }

    private void OnTabChanged(AirAlarmTab tab)
    {
        SendMessage(new AirAlarmTabSetMessage(tab));
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        if (state is not AirAlarmUIState cast || _window == null)
        {
            return;
        }

        _window.UpdateState(cast);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing) _window?.Dispose();
    }
}
