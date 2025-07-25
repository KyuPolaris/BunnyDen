// SPDX-FileCopyrightText: 2023 Ed <96445749+theshued@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 sleepyyapril <123355664+sleepyyapril@users.noreply.github.com>
//
// SPDX-License-Identifier: MIT

using Content.Server.Objectives.Systems;
using Content.Shared.Humanoid.Prototypes;
using Robust.Shared.Prototypes;

namespace Content.Server.Objectives.Components;

/// <summary>
/// Requires that the player's species matches a whitelist.
/// </summary>
[RegisterComponent, Access(typeof(SpeciesRequirementSystem))]
public sealed partial class SpeciesRequirementComponent : Component
{
    [DataField(required: true), ViewVariables(VVAccess.ReadWrite)]
    public List<ProtoId<SpeciesPrototype>> AllowedSpecies = new();
}
