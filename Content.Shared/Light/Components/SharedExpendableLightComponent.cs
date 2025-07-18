// SPDX-FileCopyrightText: 2023 deltanedas <39013340+deltanedas@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 deltanedas <@deltanedas:kde.org>
// SPDX-FileCopyrightText: 2024 DEATHB4DEFEAT <77995199+DEATHB4DEFEAT@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 VMSolidus <evilexecutive@gmail.com>
// SPDX-FileCopyrightText: 2025 sleepyyapril <123355664+sleepyyapril@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later AND MIT

using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared.Light.Components;

[NetworkedComponent]
public abstract partial class SharedExpendableLightComponent : Component
{

    [ViewVariables(VVAccess.ReadOnly)]
    public ExpendableLightState CurrentState { get; set; }

    [DataField("turnOnBehaviourID")]
    public string TurnOnBehaviourID { get; set; } = string.Empty;

    [DataField("fadeOutBehaviourID")]
    public string FadeOutBehaviourID { get; set; } = string.Empty;

    [DataField("glowDuration")]
    public float GlowDuration { get; set; } = 60 * 15f;

    [DataField("fadeOutDuration")]
    public float FadeOutDuration { get; set; } = 60 * 5f;

    [DataField("spentDesc")]
    public string SpentDesc { get; set; } = string.Empty;

    [DataField("spentName")]
    public string SpentName { get; set; } = string.Empty;

    [DataField("litSound")]
    public SoundSpecifier? LitSound { get; set; }

    [DataField("loopedSound")]
    public SoundSpecifier? LoopedSound { get; set; }

    [DataField("dieSound")]
    public SoundSpecifier? DieSound { get; set; } = null;
}

[Serializable, NetSerializable]
public enum ExpendableLightVisuals
{
    State,
    Behavior
}

[Serializable, NetSerializable]
public enum ExpendableLightState
{
    BrandNew,
    Lit,
    Fading,
    Dead
}
