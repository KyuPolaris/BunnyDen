// SPDX-FileCopyrightText: 2025 Eris <eris@erisws.com>
// SPDX-FileCopyrightText: 2025 sleepyyapril <123355664+sleepyyapril@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later AND MIT

using Content.Shared._Lavaland.Procedural.Prototypes;
using Robust.Shared.Prototypes;

namespace Content.Server._Lavaland.Procedural.Components;

[RegisterComponent]
public sealed partial class LavalandMapComponent : Component
{
    [ViewVariables]
    public EntityUid Outpost;

    [ViewVariables]
    public int Seed;

    [ViewVariables]
    public ProtoId<LavalandMapPrototype>? PrototypeId;

    /// <summary>
    /// Chunks in this area are always loaded
    /// </summary>
    [ViewVariables]
    public Box2 LoadArea;

    /// <summary>
    /// Currently active chunks
    /// </summary>
    [DataField("loadedChunks")]
    public HashSet<Vector2i> LoadedChunks = new();
}
