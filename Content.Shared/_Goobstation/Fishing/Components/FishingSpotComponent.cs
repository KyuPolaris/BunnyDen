// SPDX-FileCopyrightText: 2025 sleepyyapril <123355664+sleepyyapril@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later AND MIT

using Content.Shared.EntityTable.EntitySelectors;

namespace Content.Shared._Goobstation.Fishing.Components;

[RegisterComponent]
public sealed partial class FishingSpotComponent : Component
{
    /// <summary>
    /// All possible fishes to catch here
    /// </summary>
    [DataField(required: true)]
    public EntityTableSelector FishList;

    /// <summary>
    /// Default time for fish to occur
    /// </summary>
    [DataField]
    public float FishDefaultTimer;

    /// <summary>
    /// Variety number that FishDefaultTimer can go up or down to randomly
    /// </summary>
    [DataField]
    public float FishTimerVariety;
}
