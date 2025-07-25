// SPDX-FileCopyrightText: 2023 chromiumboy
// SPDX-FileCopyrightText: 2024 BramvanZijp
// SPDX-FileCopyrightText: 2024 Nemanja
// SPDX-FileCopyrightText: 2024 metalgearsloth
// SPDX-FileCopyrightText: 2025 Jakumba
// SPDX-FileCopyrightText: 2025 Solaris
// SPDX-FileCopyrightText: 2025 sleepyyapril
//
// SPDX-License-Identifier: AGPL-3.0-or-later AND MIT

using Content.Shared.Access.Components;
using Content.Shared.Access.Systems;
using Content.Shared.Database;
using Content.Shared.Examine;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Verbs;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Shared.Prototypes;

namespace Content.Shared.Weapons.Ranged.Systems;

public sealed class BatteryWeaponFireModesSystem : EntitySystem
{
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly SharedPopupSystem _popupSystem = default!;
    [Dependency] private readonly AccessReaderSystem _accessReaderSystem = default!;
    [Dependency] private readonly SharedAppearanceSystem _appearanceSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<BatteryWeaponFireModesComponent, ActivateInWorldEvent>(OnInteractHandEvent);
        SubscribeLocalEvent<BatteryWeaponFireModesComponent, GetVerbsEvent<Verb>>(OnGetVerb);
        SubscribeLocalEvent<BatteryWeaponFireModesComponent, ExaminedEvent>(OnExamined);
    }

    private void OnExamined(EntityUid uid, BatteryWeaponFireModesComponent component, ExaminedEvent args)
    {
        if (component.FireModes.Count < 2)
            return;

        var fireMode = GetMode(component);

        if (!_prototypeManager.TryIndex<EntityPrototype>(fireMode.Prototype, out var proto))
            return;

        args.PushMarkup(Loc.GetString("gun-set-fire-mode", ("mode", proto.Name)));
    }

    private BatteryWeaponFireMode GetMode(BatteryWeaponFireModesComponent component)
    {
        return component.FireModes[component.CurrentFireMode];
    }

    private void OnGetVerb(EntityUid uid, BatteryWeaponFireModesComponent component, GetVerbsEvent<Verb> args)
    {
        if (!args.CanAccess || !args.CanInteract || !args.CanComplexInteract)
            return;

        if (component.FireModes.Count < 2)
            return;

        if (TryComp<AccessReaderComponent>(uid, out var accessReader) && !_accessReaderSystem.IsAllowed(args.User, uid, accessReader))
            return;

        for (var i = 0; i < component.FireModes.Count; i++)
        {
            var fireMode = component.FireModes[i];
            var entProto = _prototypeManager.Index<EntityPrototype>(fireMode.Prototype);
            var index = i;

            var v = new Verb
            {
                Priority = 1,
                Category = VerbCategory.SelectType,
                Text = entProto.Name,
                Disabled = i == component.CurrentFireMode,
                Impact = LogImpact.Low,
                DoContactInteraction = true,
                Act = () =>
                {
                    SetFireMode(uid, component, index, args.User);
                }
            };

            args.Verbs.Add(v);
        }
    }

    private void OnInteractHandEvent(EntityUid uid, BatteryWeaponFireModesComponent component, ActivateInWorldEvent args)
    {
        if (!args.Complex)
            return;

        if (component.FireModes.Count < 2)
            return;

        CycleFireMode(uid, component, args.User);
    }

    private void CycleFireMode(EntityUid uid, BatteryWeaponFireModesComponent component, EntityUid user)
    {
        if (component.FireModes.Count < 2)
            return;

        var index = (component.CurrentFireMode + 1) % component.FireModes.Count;
        SetFireMode(uid, component, index, user);
    }

    public bool TrySetFireMode(EntityUid uid, BatteryWeaponFireModesComponent component, int index, EntityUid? user = null)
    {
        if (index < 0 || index >= component.FireModes.Count)
            return false;

        SetFireMode(uid, component, index, user);

        return true;
    }

    private void SetFireMode(EntityUid uid, BatteryWeaponFireModesComponent component, int index, EntityUid? user = null)
    {
        var fireMode = component.FireModes[index];
        component.CurrentFireMode = index;
        Dirty(uid, component);

        if (!_prototypeManager.TryIndex<EntityPrototype>(fireMode.Prototype, out var prototype))
            return;

        if (TryComp<AppearanceComponent>(uid, out var appearance))
            _appearanceSystem.SetData(uid, BatteryWeaponFireModeVisuals.State, prototype.ID, appearance);
        if (user != null)
        {
            _popupSystem.PopupClient(Loc.GetString("gun-set-fire-mode", ("mode", prototype.Name)), uid, user.Value);
        }

        if (TryComp(uid, out ProjectileBatteryAmmoProviderComponent? projectileBatteryAmmoProviderComponent))
        {
            // TODO: Have this get the info directly from the batteryComponent when power is moved to shared.
            var OldFireCost = projectileBatteryAmmoProviderComponent.FireCost;
            projectileBatteryAmmoProviderComponent.Prototype = fireMode.Prototype;
            projectileBatteryAmmoProviderComponent.FireCost = fireMode.FireCost;

            float FireCostDiff = (float) fireMode.FireCost / (float) OldFireCost;
            projectileBatteryAmmoProviderComponent.Shots = (int) Math.Round(projectileBatteryAmmoProviderComponent.Shots / FireCostDiff);
            projectileBatteryAmmoProviderComponent.Capacity = (int) Math.Round(projectileBatteryAmmoProviderComponent.Capacity / FireCostDiff);

            Dirty(uid, projectileBatteryAmmoProviderComponent);
            var updateClientAmmoEvent = new UpdateClientAmmoEvent();
            RaiseLocalEvent(uid, ref updateClientAmmoEvent);


        }
    }
}
