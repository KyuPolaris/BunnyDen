using System.Linq;
using Content.Server._Goobstation.Flashbang;
using Content.Server.Flash.Components;
using Content.Shared.Flash.Components;
using Content.Server.Light.EntitySystems;
using Content.Server.Popups;
using Content.Server.Stunnable;
using Content.Shared._Goobstation.Flashbang;
using Content.Shared.Charges.Components;
using Content.Shared.Charges.Systems;
using Content.Shared.Eye.Blinding.Components;
using Content.Shared.Flash;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Inventory;
using Content.Shared.Physics;
using Content.Shared.Tag;
using Content.Shared.Throwing;
using Content.Shared.Weapons.Melee.Events;
using Robust.Server.Audio;
using Robust.Server.GameObjects;
using Robust.Shared.Audio;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using InventoryComponent = Content.Shared.Inventory.InventoryComponent;
using Content.Shared.Traits.Assorted.Components;
using Robust.Shared.Random;
using Content.Shared.Eye.Blinding.Systems;

namespace Content.Server.Flash
{
    internal sealed class FlashSystem : SharedFlashSystem
    {
        [Dependency] private readonly AppearanceSystem _appearance = default!;
        [Dependency] private readonly AudioSystem _audio = default!;
        [Dependency] private readonly SharedChargesSystem _charges = default!;
        [Dependency] private readonly EntityLookupSystem _entityLookup = default!;
        [Dependency] private readonly IGameTiming _timing = default!;
        [Dependency] private readonly SharedTransformSystem _transform = default!;
        [Dependency] private readonly SharedInteractionSystem _interaction = default!;
        [Dependency] private readonly InventorySystem _inventory = default!;
        [Dependency] private readonly PopupSystem _popup = default!;
        [Dependency] private readonly StunSystem _stun = default!;
        [Dependency] private readonly TagSystem _tag = default!;
        [Dependency] private readonly IRobustRandom _random = default!;
        [Dependency] private readonly BlindableSystem _blindingSystem = default!;

        public override void Initialize()
        {
            base.Initialize();

            SubscribeLocalEvent<FlashComponent, MeleeHitEvent>(OnFlashMeleeHit);
            // ran before toggling light for extra-bright lantern
            SubscribeLocalEvent<FlashComponent, UseInHandEvent>(OnFlashUseInHand, before: new []{ typeof(HandheldLightSystem) });
            SubscribeLocalEvent<FlashComponent, ThrowDoHitEvent>(OnFlashThrowHitEvent);
            SubscribeLocalEvent<InventoryComponent, FlashAttemptEvent>(OnInventoryFlashAttempt);
            SubscribeLocalEvent<FlashImmunityComponent, FlashAttemptEvent>(OnFlashImmunityFlashAttempt);
            SubscribeLocalEvent<PermanentBlindnessComponent, FlashAttemptEvent>(OnPermanentBlindnessFlashAttempt);
            SubscribeLocalEvent<TemporaryBlindnessComponent, FlashAttemptEvent>(OnTemporaryBlindnessFlashAttempt);
        }

        private void OnFlashMeleeHit(EntityUid uid, FlashComponent comp, MeleeHitEvent args)
        {
            if (!args.IsHit ||
                !args.HitEntities.Any() ||
                !UseFlash(uid, comp))
                return;

            args.Handled = true;
            foreach (var e in args.HitEntities)
            {
                Flash(e, args.User, uid, comp.FlashDuration, comp.SlowTo, melee: true, stunDuration: comp.MeleeStunDuration);
            }
        }

        private void OnFlashUseInHand(EntityUid uid, FlashComponent comp, UseInHandEvent args)
        {
            if (args.Handled || !UseFlash(uid, comp))
                return;

            args.Handled = true;
            FlashArea(uid, args.User, comp.Range, comp.AoeFlashDuration, comp.SlowTo, true, comp.Probability);
        }

        private void OnFlashThrowHitEvent(EntityUid uid, FlashComponent comp, ThrowDoHitEvent args)
        {
            if (!UseFlash(uid, comp))
                return;

            FlashArea(uid, args.User, comp.Range, comp.AoeFlashDuration, comp.SlowTo, false, comp.Probability);
        }

        private bool UseFlash(EntityUid uid, FlashComponent comp)
        {
            if (comp.Flashing)
                return false;

            TryComp<LimitedChargesComponent>(uid, out var charges);
            if (_charges.IsEmpty(uid, charges))
                return false;

            _charges.UseCharge(uid, charges);
            _audio.PlayPvs(comp.Sound, uid);
            comp.Flashing = true;
            _appearance.SetData(uid, FlashVisuals.Flashing, true);

            if (_charges.IsEmpty(uid, charges))
            {
                _appearance.SetData(uid, FlashVisuals.Burnt, true);
                _tag.AddTag(uid, "Trash");
                _popup.PopupEntity(Loc.GetString("flash-component-becomes-empty"), uid);
            }

            uid.SpawnTimer(400, () =>
            {
                _appearance.SetData(uid, FlashVisuals.Flashing, false);
                comp.Flashing = false;
            });

            return true;
        }

        public void Flash(EntityUid target,
            EntityUid? user,
            EntityUid? used,
            float flashDuration,
            float slowTo,
            bool displayPopup = true,
            FlashableComponent? flashable = null,
            bool melee = false,
            TimeSpan? stunDuration = null)
        {
            if (!Resolve(target, ref flashable, false))
                return;

            var attempt = new FlashAttemptEvent(target, user, used);
            RaiseLocalEvent(target, attempt, true);

            if (attempt.Cancelled)
                return;

            flashDuration *= flashable.DurationMultiplier;

            flashable.LastFlash = _timing.CurTime;
            flashable.Duration = flashDuration / 1000f; // TODO: Make this sane...
            Dirty(target, flashable);

            if (TryComp<BlindableComponent>(target, out var blindable)
                && !blindable.IsBlind
                && _random.Prob(flashable.EyeDamageChance))
                _blindingSystem.AdjustEyeDamage((target, blindable), flashable.EyeDamage);

            if (displayPopup && user != null && target != user && Exists(user.Value))
            {
                _popup.PopupEntity(Loc.GetString("flash-component-user-blinds-you",
                    ("user", Identity.Entity(user.Value, EntityManager))), target, target);
            }

            if (HasComp<FlashSoundSuppressionComponent>(target))
                return;

            if (stunDuration != null)
            {
                // stunmeta
                _stun.TryKnockdown(target, stunDuration.Value, true);
            }
            else
            {
                _stun.TrySlowdown(target, TimeSpan.FromSeconds(flashDuration/1000f), true,
                slowTo, slowTo);
            }
        }

        public void FlashArea(Entity<FlashComponent?> source, EntityUid? user, float range, float duration, float slowTo = 0.8f, bool displayPopup = false, float probability = 1f, SoundSpecifier? sound = null)
        {
            var transform = Transform(source);
            var mapPosition = _transform.GetMapCoordinates(transform);
            var flashableQuery = GetEntityQuery<FlashableComponent>();

            foreach (var entity in _entityLookup.GetEntitiesInRange(transform.Coordinates, range))
            {
                if (!_random.Prob(probability))
                    continue;

                if (!flashableQuery.TryGetComponent(entity, out var flashable))
                    continue;

                // Check for unobstructed entities while ignoring the mobs with flashable components.
                if (!_interaction.InRangeUnobstructed(entity, mapPosition, range, flashable.CollisionGroup, predicate: (e) => flashableQuery.HasComponent(e) || e == source.Owner))
                    continue;

                // They shouldn't have flash removed in between right?
                Flash(entity, user, source, duration, slowTo, displayPopup, flashableQuery.GetComponent(entity));

                var distance = (mapPosition.Position - _transform.GetMapCoordinates(entity).Position).Length(); // Goobstation
                RaiseLocalEvent(source, new AreaFlashEvent(range, distance, entity)); // Goobstation
            }

            _audio.PlayPvs(sound, source, AudioParams.Default.WithVolume(1f).WithMaxDistance(3f));
        }

        private void OnInventoryFlashAttempt(EntityUid uid, InventoryComponent component, FlashAttemptEvent args)
        {
            foreach (var slot in new[] { "head", "eyes", "mask" })
            {
                if (args.Cancelled)
                    break;
                if (_inventory.TryGetSlotEntity(uid, slot, out var item, component))
                    RaiseLocalEvent(item.Value, args, true);
            }
        }

        private void OnFlashImmunityFlashAttempt(EntityUid uid, FlashImmunityComponent component, FlashAttemptEvent args)
        {
            if(component.Enabled)
                args.Cancel();
        }

        private void OnPermanentBlindnessFlashAttempt(EntityUid uid, PermanentBlindnessComponent component, FlashAttemptEvent args)
        {
            args.Cancel();
        }

        private void OnTemporaryBlindnessFlashAttempt(EntityUid uid, TemporaryBlindnessComponent component, FlashAttemptEvent args)
        {
            args.Cancel();
        }
    }

    public sealed class FlashAttemptEvent : CancellableEntityEventArgs
    {
        public readonly EntityUid Target;
        public readonly EntityUid? User;
        public readonly EntityUid? Used;

        public FlashAttemptEvent(EntityUid target, EntityUid? user, EntityUid? used)
        {
            Target = target;
            User = user;
            Used = used;
        }
    }
}
