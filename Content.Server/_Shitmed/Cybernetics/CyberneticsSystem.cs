// SPDX-FileCopyrightText: 2024 gluesniffler <159397573+gluesniffler@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 gluesniffler <linebarrelerenthusiast@gmail.com>
// SPDX-FileCopyrightText: 2024 sleepyyapril <***>
// SPDX-FileCopyrightText: 2025 sleepyyapril <123355664+sleepyyapril@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later AND MIT

using Content.Server.Emp;
using Content.Shared.Body.Part;
using Content.Shared.Body.Organ;
using Content.Shared._Shitmed.Body.Organ;
using Content.Shared._Shitmed.Body.Events;
using Content.Shared._Shitmed.Cybernetics;

namespace Content.Server._Shitmed.Cybernetics;

internal sealed class CyberneticsSystem : EntitySystem
{
    public override void Initialize()
    {
        SubscribeLocalEvent<CyberneticsComponent, EmpPulseEvent>(OnEmpPulse);
        SubscribeLocalEvent<CyberneticsComponent, EmpDisabledRemoved>(OnEmpDisabledRemoved);
    }
    private void OnEmpPulse(Entity<CyberneticsComponent> cyberEnt, ref EmpPulseEvent ev)
    {
        if (!cyberEnt.Comp.Disabled)
        {
            ev.Affected = true;
            ev.Disabled = true;
            cyberEnt.Comp.Disabled = true;

            if (HasComp<OrganComponent>(cyberEnt))
            {
                var disableEvent = new OrganEnableChangedEvent(false);
                RaiseLocalEvent(cyberEnt, ref disableEvent);
            }
            else if (HasComp<BodyPartComponent>(cyberEnt))
            {
                var disableEvent = new BodyPartEnableChangedEvent(false);
                RaiseLocalEvent(cyberEnt, ref disableEvent);
            }
        }
    }

    private void OnEmpDisabledRemoved(Entity<CyberneticsComponent> cyberEnt, ref EmpDisabledRemoved ev)
    {
        if (cyberEnt.Comp.Disabled)
        {
            cyberEnt.Comp.Disabled = false;
            if (HasComp<OrganComponent>(cyberEnt))
            {
                var enableEvent = new OrganEnableChangedEvent(true);
                RaiseLocalEvent(cyberEnt, ref enableEvent);
            }
            else if (HasComp<BodyPartComponent>(cyberEnt))
            {
                var enableEvent = new BodyPartEnableChangedEvent(true);
                RaiseLocalEvent(cyberEnt, ref enableEvent);
            }
        }
    }
}
