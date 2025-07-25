// SPDX-FileCopyrightText: 2023 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Pieter-Jan Briers <pieterjan.briers@gmail.com>
// SPDX-FileCopyrightText: 2023 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 sleepyyapril <123355664+sleepyyapril@users.noreply.github.com>
//
// SPDX-License-Identifier: MIT

using Content.Server.Hands.Systems;
using Content.Server.Popups;
using Content.Shared.IdentityManagement;
using Content.Shared.Popups;
using Content.Shared.Tag;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;

namespace Content.Server.Weapons.Melee.Balloon;

/// <summary>
/// This handles popping ballons when attacked with <see cref="BalloonPopperComponent"/>
/// </summary>
public sealed class BalloonPopperSystem : EntitySystem
{
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly HandsSystem _hands = default!;
    [Dependency] private readonly PopupSystem _popup = default!;
    [Dependency] private readonly TagSystem _tag = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<BalloonPopperComponent, MeleeHitEvent>(OnMeleeHit);
    }

    private void OnMeleeHit(EntityUid uid, BalloonPopperComponent component, MeleeHitEvent args)
    {
        foreach (var entity in args.HitEntities)
        {
            foreach (var held in _hands.EnumerateHeld(entity))
            {
                if (_tag.HasTag(held, component.BalloonTag))
                    PopBallooon(uid, held, component);
            }

            if (_tag.HasTag(entity, component.BalloonTag))
                PopBallooon(uid, entity, component);
        }
    }

    /// <summary>
    /// Pops a target balloon, making a popup and playing a sound.
    /// </summary>
    public void PopBallooon(EntityUid popper, EntityUid balloon, BalloonPopperComponent? component = null)
    {
        if (!Resolve(popper, ref component))
            return;

        _audio.PlayPvs(component.PopSound, balloon);
        _popup.PopupCoordinates(Loc.GetString("melee-balloon-pop",
            ("balloon", Identity.Entity(balloon, EntityManager))), Transform(balloon).Coordinates, PopupType.Large);
        QueueDel(balloon);
    }
}
