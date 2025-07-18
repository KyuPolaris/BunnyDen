// SPDX-FileCopyrightText: 2023 Debug <49997488+DebugOk@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Kara <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2023 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Pieter-Jan Briers <pieterjan.briers@gmail.com>
// SPDX-FileCopyrightText: 2023 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 sleepyyapril <123355664+sleepyyapril@users.noreply.github.com>
//
// SPDX-License-Identifier: MIT

using System.Linq;
using Content.Shared.FixedPoint;
using Content.Shared.Points;
using JetBrains.Annotations;
using Robust.Server.GameStates;
using Robust.Server.Player;
using Robust.Shared.Player;
using Robust.Shared.Utility;

namespace Content.Server.Points;

/// <inheritdoc/>
public sealed class PointSystem : SharedPointSystem
{
    [Dependency] private readonly IPlayerManager _player = default!;
    [Dependency] private readonly PvsOverrideSystem _pvsOverride = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<PointManagerComponent, ComponentStartup>(OnStartup);
    }

    private void OnStartup(EntityUid uid, PointManagerComponent component, ComponentStartup args)
    {
        _pvsOverride.AddGlobalOverride(GetNetEntity(uid));
    }

    /// <summary>
    /// Adds the specified point value to a player.
    /// </summary>
    [PublicAPI]
    public void AdjustPointValue(EntityUid user, FixedPoint2 value, EntityUid uid, PointManagerComponent? component, ActorComponent? actor = null)
    {
        if (!Resolve(uid, ref component) || !Resolve(user, ref actor, false))
            return;
        AdjustPointValue(actor.PlayerSession.UserId, value, uid, component);
    }

    /// <summary>
    /// Sets the amount of points for a player
    /// </summary>
    [PublicAPI]
    public void SetPointValue(EntityUid user, FixedPoint2 value, EntityUid uid, PointManagerComponent? component, ActorComponent? actor = null)
    {
        if (!Resolve(uid, ref component) || !Resolve(user, ref actor, false))
            return;
        SetPointValue(actor.PlayerSession.UserId, value, uid, component);
    }

    /// <summary>
    /// Gets the amount of points for a given player
    /// </summary>
    [PublicAPI]
    public FixedPoint2 GetPointValue(EntityUid user, EntityUid uid, PointManagerComponent? component, ActorComponent? actor = null)
    {
        if (!Resolve(uid, ref component) || !Resolve(user, ref actor, false))
            return FixedPoint2.Zero;
        return GetPointValue(actor.PlayerSession.UserId, uid, component);
    }

    /// <inheritdoc/>
    public override FormattedMessage GetScoreboard(EntityUid uid, PointManagerComponent? component = null)
    {
        var msg = new FormattedMessage();

        if (!Resolve(uid, ref component))
            return msg;

        var orderedPlayers = component.Points.OrderByDescending(p => p.Value).ToList();
        var place = 1;
        foreach (var (id, points) in orderedPlayers)
        {
            if (!_player.TryGetPlayerData(id, out var data))
                continue;

            msg.AddMarkup(Loc.GetString("point-scoreboard-list",
                ("place", place),
                ("name", data.UserName),
                ("points", points.Int())));
            msg.PushNewline();
            place++;
        }

        return msg;
    }
}
