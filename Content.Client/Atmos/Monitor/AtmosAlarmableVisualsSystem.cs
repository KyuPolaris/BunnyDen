// SPDX-FileCopyrightText: 2022 vulppine <vulppine@gmail.com>
// SPDX-FileCopyrightText: 2025 sleepyyapril <123355664+sleepyyapril@users.noreply.github.com>
//
// SPDX-License-Identifier: MIT

using System.Collections.Generic;
using Content.Shared.Atmos.Monitor;
using Content.Shared.Power;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Client.Atmos.Monitor;

public sealed class AtmosAlarmableVisualsSystem : VisualizerSystem<AtmosAlarmableVisualsComponent>
{
    protected override void OnAppearanceChange(EntityUid uid, AtmosAlarmableVisualsComponent component, ref AppearanceChangeEvent args)
    {
        if (args.Sprite == null || !args.Sprite.LayerMapTryGet(component.LayerMap, out var layer))
            return;

        if (!args.AppearanceData.TryGetValue(PowerDeviceVisuals.Powered, out var poweredObject) ||
            poweredObject is not bool powered)
        {
            return;
        }

        if (component.HideOnDepowered != null)
        {
            foreach (var visLayer in component.HideOnDepowered)
            {
                if (args.Sprite.LayerMapTryGet(visLayer, out int powerVisibilityLayer))
                    args.Sprite.LayerSetVisible(powerVisibilityLayer, powered);
            }
        }

        if (component.SetOnDepowered != null && !powered)
        {
            foreach (var (setLayer, powerState) in component.SetOnDepowered)
            {
                if (args.Sprite.LayerMapTryGet(setLayer, out int setStateLayer))
                    args.Sprite.LayerSetState(setStateLayer, new RSI.StateId(powerState));
            }
        }

        if (args.AppearanceData.TryGetValue(AtmosMonitorVisuals.AlarmType, out var alarmTypeObject)
            && alarmTypeObject is AtmosAlarmType alarmType
            && powered
            && component.AlarmStates.TryGetValue(alarmType, out var state))
        {
            args.Sprite.LayerSetState(layer, new RSI.StateId(state));
        }
    }
}
