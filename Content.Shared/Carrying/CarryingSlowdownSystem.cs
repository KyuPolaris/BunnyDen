// SPDX-FileCopyrightText: 2023 Adrian16199 <144424013+Adrian16199@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 VMSolidus <evilexecutive@gmail.com>
// SPDX-FileCopyrightText: 2025 sleepyyapril <123355664+sleepyyapril@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later AND MIT

using Content.Shared.Movement.Systems;
using Robust.Shared.GameStates;

namespace Content.Shared.Carrying
{
    public sealed class CarryingSlowdownSystem : EntitySystem
    {
        [Dependency] private readonly MovementSpeedModifierSystem _movementSpeed = default!;

        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<CarryingSlowdownComponent, ComponentGetState>(OnGetState);
            SubscribeLocalEvent<CarryingSlowdownComponent, ComponentHandleState>(OnHandleState);
            SubscribeLocalEvent<CarryingSlowdownComponent, RefreshMovementSpeedModifiersEvent>(OnRefreshMoveSpeed);
        }

        public void SetModifier(EntityUid uid, float walkSpeedModifier, float sprintSpeedModifier, CarryingSlowdownComponent? component = null)
        {
            if (!Resolve(uid, ref component))
                return;

            component.WalkModifier = walkSpeedModifier;
            component.SprintModifier = sprintSpeedModifier;
            _movementSpeed.RefreshMovementSpeedModifiers(uid);
        }
        private void OnGetState(EntityUid uid, CarryingSlowdownComponent component, ref ComponentGetState args)
        {
            args.State = new CarryingSlowdownComponentState(component.WalkModifier, component.SprintModifier);
        }

        private void OnHandleState(EntityUid uid, CarryingSlowdownComponent component, ref ComponentHandleState args)
        {
            if (args.Current is not CarryingSlowdownComponentState state)
                return;

            component.WalkModifier = state.WalkModifier;
            component.SprintModifier = state.SprintModifier;
            _movementSpeed.RefreshMovementSpeedModifiers(uid);
        }
        private void OnRefreshMoveSpeed(EntityUid uid, CarryingSlowdownComponent component, RefreshMovementSpeedModifiersEvent args)
        {
            args.ModifySpeed(component.WalkModifier, component.SprintModifier);
        }
    }
}
