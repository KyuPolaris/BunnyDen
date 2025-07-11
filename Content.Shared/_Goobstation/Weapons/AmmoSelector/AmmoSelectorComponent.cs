// SPDX-FileCopyrightText: 2025 Eris <eris@erisws.com>
// SPDX-FileCopyrightText: 2025 sleepyyapril <123355664+sleepyyapril@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later AND MIT

using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._Goobstation.Weapons.AmmoSelector;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class AmmoSelectorComponent : Component
{
    [DataField, AutoNetworkedField]
    public HashSet<ProtoId<SelectableAmmoPrototype>> Prototypes = new();

    [DataField, AutoNetworkedField]
    public SelectableAmmoPrototype? CurrentlySelected;

    [DataField]
    public SoundSpecifier? SoundSelect = new SoundPathSpecifier("/Audio/Weapons/Guns/Misc/selector.ogg");
}
