// SPDX-FileCopyrightText: 2024 beck-thompson <107373427+beck-thompson@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 CyberLanos <cyber.lanos00@gmail.com>
// SPDX-FileCopyrightText: 2025 sleepyyapril <123355664+sleepyyapril@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later AND MIT

using Content.Shared.DoAfter;
using Content.Shared.Inventory;
using Robust.Shared.Serialization;

namespace Content.Shared.Chemistry.Hypospray.Events;

public abstract partial class BeforeHyposprayInjectsTargetEvent : CancellableEntityEventArgs, IInventoryRelayEvent
{
    public SlotFlags TargetSlots { get; } = SlotFlags.WITHOUT_POCKET;
    public EntityUid EntityUsingHypospray;
    public readonly EntityUid Hypospray;
    public EntityUid TargetGettingInjected;
    public string? InjectMessageOverride;

    public BeforeHyposprayInjectsTargetEvent(EntityUid user, EntityUid hypospray, EntityUid target)
    {
        EntityUsingHypospray = user;
        Hypospray = hypospray;
        TargetGettingInjected = target;
        InjectMessageOverride = null;
    }
}

/// <summary>
///     This event is raised on the user using the hypospray before the hypospray is injected.
///     The event is triggered on the user and all their clothing.
/// </summary>
public sealed class SelfBeforeHyposprayInjectsEvent : BeforeHyposprayInjectsTargetEvent
{
    public SelfBeforeHyposprayInjectsEvent(EntityUid user, EntityUid hypospray, EntityUid target) : base(user, hypospray, target) { }
}

/// <summary>
///     This event is raised on the target before the hypospray is injected.
///     The event is triggered on the target itself and all its clothing.
/// </summary>
public sealed class TargetBeforeHyposprayInjectsEvent  : BeforeHyposprayInjectsTargetEvent
{
    public TargetBeforeHyposprayInjectsEvent (EntityUid user, EntityUid hypospray, EntityUid target) : base(user, hypospray, target) { }
}


[Serializable, NetSerializable]
public sealed partial class HyposprayDoAfterEvent : SimpleDoAfterEvent
{
}