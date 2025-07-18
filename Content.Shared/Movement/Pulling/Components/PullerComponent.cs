// SPDX-FileCopyrightText: 2024 Jezithyr <jezithyr@gmail.com>
// SPDX-FileCopyrightText: 2024 Mnemotechnican <69920617+Mnemotechnician@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 SimpleStation14 <130339894+SimpleStation14@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 sleepyyapril <flyingkarii@gmail.com>
// SPDX-FileCopyrightText: 2025 BramvanZijp <56019239+BramvanZijp@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 BramvanZijp <BramvanZijp@gmail.com>
// SPDX-FileCopyrightText: 2025 Eagle-0 <114363363+Eagle-0@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 sleepyyapril <123355664+sleepyyapril@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later AND MIT

using Content.Shared._Goobstation.MartialArts;
using Content.Shared._Goobstation.TableSlam; // Goobstation - Table Slam
using Content.Shared.Alert;
using Content.Shared.Movement.Pulling.Systems;
using Robust.Shared.GameStates;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Shared.Movement.Pulling.Components;

/// <summary>
/// Specifies an entity as being able to pull another entity with <see cref="PullableComponent"/>
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(true)]
[Access(typeof(PullingSystem), typeof(TableSlamSystem), typeof(SharedMartialArtsSystem))] // Goobstation - Table Slam
public sealed partial class PullerComponent : Component
{
    /// <summary>
    ///     Next time the puller change where they are pulling the target towards.
    /// </summary>
    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer)), AutoNetworkedField]
    public TimeSpan NextPushTargetChange;

    [DataField, AutoNetworkedField]
    public TimeSpan NextPushStop;

    [DataField]
    public TimeSpan PushChangeCooldown = TimeSpan.FromSeconds(0.1f), PushDuration = TimeSpan.FromSeconds(5f);

    // Before changing how this is updated, please see SharedPullerSystem.RefreshMovementSpeed
    public float WalkSpeedModifier => Pulling == default ? 1.0f : 0.95f;

    public float SprintSpeedModifier => Pulling == default ? 1.0f : 0.95f;
	
    /// <summary>
    /// whether or not to apply speed modifiers to the puller
    /// </summary>
	[AutoNetworkedField, DataField]
	public bool ApplySpeedModifier = true;

    /// <summary>
    ///     Entity currently being pulled/pushed if applicable.
    /// </summary>
    [AutoNetworkedField, DataField]
    public EntityUid? Pulling;

    /// <summary>
    ///     The position the entity is currently being pulled towards.
    /// </summary>
    [AutoNetworkedField, DataField]
    public EntityCoordinates? PushingTowards;

    /// <summary>
    ///     Does this entity need hands to be able to pull something?
    /// </summary>
    [DataField]
    public bool NeedsHands = true;

    /// <summary>
    ///     The maximum acceleration of pushing, in meters per second squared.
    /// </summary>
    [DataField]
    public float PushAcceleration = 0.3f;

    /// <summary>
    ///     The maximum speed to which the pulled entity may be accelerated relative to the push direction, in meters per second.
    /// </summary>
    [DataField]
    public float MaxPushSpeed = 1.2f;

    /// <summary>
    ///     The maximum distance between the puller and the point towards which the puller may attempt to pull it, in meters.
    /// </summary>
    [DataField]
    public float MaxPushRange = 2f;
    [DataField]
    public ProtoId<AlertPrototype> PullingAlert = "Pulling";

    // Goobstation start
    // Added Grab variables

    [DataField]
    public Dictionary<GrabStage, short> PullingAlertSeverity = new()
    {
        { GrabStage.No, 0 },
        { GrabStage.Soft, 1 },
        { GrabStage.Hard, 2 },
        { GrabStage.Suffocate, 3 },
    };

    [DataField, AutoNetworkedField]
    public GrabStage GrabStage = GrabStage.No;

    [DataField, AutoNetworkedField]
    public GrabStageDirection GrabStageDirection = GrabStageDirection.Increase;

    [AutoNetworkedField]
    public TimeSpan NextStageChange;

    [DataField]
    public TimeSpan StageChangeCooldown = TimeSpan.FromSeconds(1.5f);

    [AutoNetworkedField]
    public TimeSpan WhenCanThrow;

    /// <summary>
    ///     After initiating / upgrading to a hard combat grab, how long should you have to keep somebody grabbed to be able to throw them.
    /// </summary>
    [DataField]
    public TimeSpan ThrowDelayOnGrab = TimeSpan.FromSeconds(2f);

    [DataField]
    public Dictionary<GrabStage, float> EscapeChances = new()
    {
        { GrabStage.No, 1f },
        { GrabStage.Soft, 0.7f },
        { GrabStage.Hard, 0.4f },
        { GrabStage.Suffocate, 0.1f },
    };

    [DataField]
    public float SuffocateGrabStaminaDamage = 10f;

    [DataField]
    public float GrabThrowDamageModifier = 2f;

    [ViewVariables]
    public List<EntityUid> GrabVirtualItems = new();

    [ViewVariables]
    public Dictionary<GrabStage, int> GrabVirtualItemStageCount = new()
    {
        { GrabStage.Suffocate, 1 },
    };

    [DataField]
    public float StaminaDamageOnThrown = 100f;

    [DataField]
    public float GrabThrownSpeed = 7f;

    [DataField]
    public float ThrowingDistance = 4f;

    [DataField]
    public float SoftGrabSpeedModifier = 0.9f;

    [DataField]
    public float HardGrabSpeedModifier = 0.7f;

    [DataField]
    public float ChokeGrabSpeedModifier = 0.4f;
    // Goobstation end
}
