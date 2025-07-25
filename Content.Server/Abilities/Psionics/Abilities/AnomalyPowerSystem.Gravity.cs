// SPDX-FileCopyrightText: 2023 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 qwerltaz <69696513+qwerltaz@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Ed <96445749+TheShuEd@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2024 VMSolidus <evilexecutive@gmail.com>
// SPDX-FileCopyrightText: 2025 sleepyyapril <123355664+sleepyyapril@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later AND MIT

using Content.Shared.Abilities.Psionics;
using Content.Shared.Actions.Events;
using Robust.Shared.Physics.Components;
using Content.Shared.Physics;
using System.Linq;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;

namespace Content.Server.Abilities.Psionics;

public sealed partial class AnomalyPowerSystem
{
    private void DoGravityAnomalyEffects(EntityUid uid, PsionicComponent component, AnomalyPowerActionEvent args, bool overcharged = false)
    {
        if (args.Gravity is null)
            return;

        if (overcharged)
            GravitySupercrit(uid, component, args);
        else GravityPulse(uid, component, args);
    }

    private void GravitySupercrit(EntityUid uid, PsionicComponent component, AnomalyPowerActionEvent args)
    {
        var xform = Transform(uid);
        if (!TryComp(xform.GridUid, out MapGridComponent? grid))
            return;

        var gravity = args.Gravity!.Value;
        var worldPos = _xform.GetWorldPosition(xform);
        var tileref = _mapSystem.GetTilesIntersecting(
                xform.GridUid.Value,
                grid,
                new Circle(worldPos, gravity.SpaceRange))
            .ToArray();

        var tiles = tileref.Select(t => (t.GridIndices, Tile.Empty)).ToList();
        _mapSystem.SetTiles(xform.GridUid.Value, grid, tiles);

        var range = gravity.MaxThrowRange * component.CurrentDampening;
        var strength = gravity.MaxThrowStrength * component.CurrentAmplification;
        var lookup = _lookup.GetEntitiesInRange(uid, range, LookupFlags.Dynamic | LookupFlags.Sundries);
        var xformQuery = GetEntityQuery<TransformComponent>();
        var physQuery = GetEntityQuery<PhysicsComponent>();

        foreach (var ent in lookup)
        {
            if (physQuery.TryGetComponent(ent, out var phys)
                && (phys.CollisionMask & (int) CollisionGroup.GhostImpassable) != 0)
                continue;

            var foo = _xform.GetWorldPosition(ent, xformQuery) - worldPos;
            _throwing.TryThrow(ent, foo * 5, strength, uid, 0);
        }
    }

    private void GravityPulse(EntityUid uid, PsionicComponent component, AnomalyPowerActionEvent args)
    {
        var gravity = args.Gravity!.Value;
        var xform = Transform(uid);
        var range = gravity.MaxThrowRange * component.CurrentDampening;
        var strength = gravity.MaxThrowStrength * component.CurrentAmplification;
        var lookup = _lookup.GetEntitiesInRange(uid, range, LookupFlags.Dynamic | LookupFlags.Sundries);
        var xformQuery = GetEntityQuery<TransformComponent>();
        var worldPos = _xform.GetWorldPosition(xform, xformQuery);
        var physQuery = GetEntityQuery<PhysicsComponent>();

        foreach (var ent in lookup)
        {
            if (physQuery.TryGetComponent(ent, out var phys)
                && (phys.CollisionMask & (int) CollisionGroup.GhostImpassable) != 0)
                continue;

            var foo = _xform.GetWorldPosition(ent, xformQuery) - worldPos;
            _throwing.TryThrow(ent, foo * 10, strength, uid, 0);
        }
    }
}