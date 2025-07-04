// SPDX-FileCopyrightText: 2024 Timemaster99 <57200767+Timemaster99@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 sleepyyapril <123355664+sleepyyapril@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later AND MIT

using System.Numerics;

namespace Content.Server.Power.Components;

[RegisterComponent]
public sealed partial class RandomBatteryChargeComponent : Component
{
    /// <summary>
    ///     The minimum and maximum max charge the battery can have.
    /// </summary>
    [DataField]
    public Vector2 BatteryMaxMinMax = new(0.85f, 1.15f);

    /// <summary>
    ///     The minimum and maximum current charge the battery can have.
    /// </summary>
    [DataField]
    public Vector2 BatteryChargeMinMax = new(1f, 1f);

    /// <summary>
    ///     False if the randomized charge of the battery should be a multiple of the preexisting current charge of the battery.
    ///     True if the randomized charge of the battery should be a multiple of the max charge of the battery post max charge randomization.
    /// </summary>
    [DataField]
    public bool BasedOnMaxCharge = true;
}
