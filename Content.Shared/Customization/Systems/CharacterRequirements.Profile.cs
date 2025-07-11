// SPDX-FileCopyrightText: 2024 DEATHB4DEFEAT
// SPDX-FileCopyrightText: 2025 Raikyr0
// SPDX-FileCopyrightText: 2025 Timfa
// SPDX-FileCopyrightText: 2025 VMSolidus
// SPDX-FileCopyrightText: 2025 sleepyyapril
//
// SPDX-License-Identifier: AGPL-3.0-or-later AND MIT

using System.Linq;
using Content.Shared.Clothing.Loadouts.Prototypes;
using Content.Shared.Humanoid;
using Content.Shared.Humanoid.Prototypes;
using Content.Shared.Mind;
using Content.Shared.Preferences;
using Content.Shared.Prototypes;
using Content.Shared.Roles;
using Content.Shared.Traits;
using JetBrains.Annotations;
using Robust.Shared.Configuration;
using Robust.Shared.Enums;
using Robust.Shared.Physics;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.Customization.Systems;


/// <summary>
///     Requires the profile to be within an age range
/// </summary>
[UsedImplicitly, Serializable, NetSerializable]
public sealed partial class CharacterAgeRequirement : CharacterRequirement
{
    [DataField(required: true)]
    public int Min;

    [DataField]
    public int Max = Int32.MaxValue;

    public override bool IsValid(
        JobPrototype job,
        HumanoidCharacterProfile profile,
        Dictionary<string, TimeSpan> playTimes,
        bool whitelisted,
        IPrototype prototype,
        IEntityManager entityManager,
        IPrototypeManager prototypeManager,
        IConfigurationManager configManager,
        out string? reason,
        int depth = 0,
        MindComponent? mind = null
    )
    {
        var localeString = "";

        if (Max == Int32.MaxValue || Min <= 0)
            localeString = Max == Int32.MaxValue ? "character-age-requirement-minimum-only" : "character-age-requirement-maximum-only";
        else
            localeString = "character-age-requirement-range";

        reason = Loc.GetString(
            localeString,
            ("inverted", Inverted),
            ("min", Min),
            ("max", Max));
        return profile.Age >= Min && profile.Age <= Max;
    }
}

/// <summary>
///     Requires the profile to be a certain gender
/// </summary>
[UsedImplicitly, Serializable, NetSerializable]
public sealed partial class CharacterGenderRequirement : CharacterRequirement
{
    [DataField(required: true)]
    public Gender Gender;

    public override bool IsValid(
        JobPrototype job,
        HumanoidCharacterProfile profile,
        Dictionary<string, TimeSpan> playTimes,
        bool whitelisted,
        IPrototype prototype,
        IEntityManager entityManager,
        IPrototypeManager prototypeManager,
        IConfigurationManager configManager,
        out string? reason,
        int depth = 0,
        MindComponent? mind = null
    )
    {
        reason = Loc.GetString(
            "character-gender-requirement",
            ("inverted", Inverted),
            ("gender", Loc.GetString($"humanoid-profile-editor-pronouns-{Gender.ToString().ToLower()}-text")));
        return profile.Gender == Gender;
    }
}

/// <summary>
///     Requires the profile to be a certain sex
/// </summary>
[UsedImplicitly, Serializable, NetSerializable]
public sealed partial class CharacterSexRequirement : CharacterRequirement
{
    [DataField(required: true)]
    public Sex Sex;

    public override bool IsValid(
        JobPrototype job,
        HumanoidCharacterProfile profile,
        Dictionary<string, TimeSpan> playTimes,
        bool whitelisted,
        IPrototype prototype,
        IEntityManager entityManager,
        IPrototypeManager prototypeManager,
        IConfigurationManager configManager,
        out string? reason,
        int depth = 0,
        MindComponent? mind = null
    )
    {
        reason = Loc.GetString(
            "character-sex-requirement",
            ("inverted", Inverted),
            ("sex", Loc.GetString($"humanoid-profile-editor-sex-{Sex.ToString().ToLower()}-text")));
        return profile.Sex == Sex;
    }
}

/// <summary>
///     Requires the profile to be a certain species
/// </summary>
[UsedImplicitly, Serializable, NetSerializable]
public sealed partial class CharacterSpeciesRequirement : CharacterRequirement
{
    [DataField(required: true)]
    public List<ProtoId<SpeciesPrototype>> Species;

    public override bool IsValid(
        JobPrototype job,
        HumanoidCharacterProfile profile,
        Dictionary<string, TimeSpan> playTimes,
        bool whitelisted,
        IPrototype prototype,
        IEntityManager entityManager,
        IPrototypeManager prototypeManager,
        IConfigurationManager configManager,
        out string? reason,
        int depth = 0,
        MindComponent? mind = null
    )
    {
        const string color = "green";
        reason = Loc.GetString(
            "character-species-requirement",
            ("inverted", Inverted),
            ("species", $"[color={color}]{string.Join($"[/color], [color={color}]",
                Species.Select(s => Loc.GetString(prototypeManager.Index(s).Name)))}[/color]"));

        return Species.Contains(profile.Species);
    }
}

/// <summary>
///     Requires the profile to be within a certain height range
/// </summary>
[UsedImplicitly, Serializable, NetSerializable]
public sealed partial class CharacterHeightRequirement : CharacterRequirement
{
    /// <summary>
    ///     The minimum height of the profile in centimeters
    /// </summary>
    [DataField]
    public float Min = int.MinValue;

    /// <summary>
    ///     The maximum height of the profile in centimeters
    /// </summary>
    [DataField]
    public float Max = int.MaxValue;

    public override bool IsValid(
        JobPrototype job,
        HumanoidCharacterProfile profile,
        Dictionary<string, TimeSpan> playTimes,
        bool whitelisted,
        IPrototype prototype,
        IEntityManager entityManager,
        IPrototypeManager prototypeManager,
        IConfigurationManager configManager,
        out string? reason,
        int depth = 0,
        MindComponent? mind = null
    )
    {
        const string color = "yellow";
        var species = prototypeManager.Index<SpeciesPrototype>(profile.Species);

        reason = Loc.GetString(
            "character-height-requirement",
            ("inverted", Inverted),
            ("color", color),
            ("min", Min),
            ("max", Max));

        var height = profile.Height * species.AverageHeight;
        return height >= Min && height <= Max;
    }
}

/// <summary>
///     Requires the profile to be within a certain width range
/// </summary>
[UsedImplicitly, Serializable, NetSerializable]
public sealed partial class CharacterWidthRequirement : CharacterRequirement
{
    /// <summary>
    ///     The minimum width of the profile in centimeters
    /// </summary>
    [DataField]
    public float Min = int.MinValue;

    /// <summary>
    ///     The maximum width of the profile in centimeters
    /// </summary>
    [DataField]
    public float Max = int.MaxValue;

    public override bool IsValid(
        JobPrototype job,
        HumanoidCharacterProfile profile,
        Dictionary<string, TimeSpan> playTimes,
        bool whitelisted,
        IPrototype prototype,
        IEntityManager entityManager,
        IPrototypeManager prototypeManager,
        IConfigurationManager configManager,
        out string? reason,
        int depth = 0,
        MindComponent? mind = null
    )
    {
        const string color = "yellow";
        var species = prototypeManager.Index<SpeciesPrototype>(profile.Species);

        reason = Loc.GetString(
            "character-width-requirement",
            ("inverted", Inverted),
            ("color", color),
            ("min", Min),
            ("max", Max));

        var width = profile.Width * species.AverageWidth;
        return width >= Min && width <= Max;
    }
}

/// <summary>
///     Requires the profile to be within a certain weight range
/// </summary>
[UsedImplicitly, Serializable, NetSerializable]
public sealed partial class CharacterWeightRequirement : CharacterRequirement
{
    /// <summary>
    ///     Minimum weight of the profile in kilograms
    /// </summary>
    [DataField]
    public float Min = int.MinValue;

    /// <summary>
    ///     Maximum weight of the profile in kilograms
    /// </summary>
    [DataField]
    public float Max = int.MaxValue;

    public override bool IsValid(
        JobPrototype job,
        HumanoidCharacterProfile profile,
        Dictionary<string, TimeSpan> playTimes,
        bool whitelisted,
        IPrototype prototype,
        IEntityManager entityManager,
        IPrototypeManager prototypeManager,
        IConfigurationManager configManager,
        out string? reason,
        int depth = 0,
        MindComponent? mind = null
    )
    {
        const string color = "green";
        var species = prototypeManager.Index<SpeciesPrototype>(profile.Species);
        prototypeManager.Index(species.Prototype).TryGetComponent<FixturesComponent>(out var fixture);

        if (fixture == null)
        {
            reason = null;
            return false;
        }

        var weight = MathF.Round(
            MathF.PI * MathF.Pow(
                fixture.Fixtures["fix1"].Shape.Radius
                * ((profile.Width + profile.Height) / 2),
                2)
            * fixture.Fixtures["fix1"].Density);

        reason = Loc.GetString(
            "character-weight-requirement",
            ("inverted", Inverted),
            ("color", color),
            ("min", Min),
            ("max", Max));

        return weight >= Min && weight <= Max;
    }
}

/// <summary>
///     Requires the profile to have one of the specified traits
/// </summary>
[UsedImplicitly, Serializable, NetSerializable]
public sealed partial class CharacterTraitRequirement : CharacterRequirement
{
    [DataField(required: true)]
    public List<ProtoId<TraitPrototype>> Traits;

    public override bool IsValid(
        JobPrototype job,
        HumanoidCharacterProfile profile,
        Dictionary<string, TimeSpan> playTimes,
        bool whitelisted,
        IPrototype prototype,
        IEntityManager entityManager,
        IPrototypeManager prototypeManager,
        IConfigurationManager configManager,
        out string? reason,
        int depth = 0,
        MindComponent? mind = null
    )
    {
        const string color = "lightblue";
        reason = Loc.GetString(
            "character-trait-requirement",
            ("inverted", Inverted),
            ("traits", $"[color={color}]{string.Join($"[/color], [color={color}]",
                Traits.Select(t => Loc.GetString($"trait-name-{t}")))}[/color]"));

        return Traits.Any(t => profile.TraitPreferences.Contains(t.ToString()));
    }
}

/// <summary>
///     Requires the profile to have one of the specified loadouts
/// </summary>
[UsedImplicitly, Serializable, NetSerializable]
public sealed partial class CharacterLoadoutRequirement : CharacterRequirement
{
    [DataField(required: true)]
    public List<ProtoId<LoadoutPrototype>> Loadouts;

    public override bool IsValid(
        JobPrototype job,
        HumanoidCharacterProfile profile,
        Dictionary<string, TimeSpan> playTimes,
        bool whitelisted,
        IPrototype prototype,
        IEntityManager entityManager,
        IPrototypeManager prototypeManager,
        IConfigurationManager configManager,
        out string? reason,
        int depth = 0,
        MindComponent? mind = null
    )
    {
        const string color = "lightblue";
        reason = Loc.GetString(
            "character-loadout-requirement",
            ("inverted", Inverted),
            ("loadouts", $"[color={color}]{string.Join($"[/color], [color={color}]",
                Loadouts.Select(l => Loc.GetString($"loadout-name-{l}")))}[/color]"));

        return Loadouts.Any(l => profile.LoadoutPreferences.Select(l => l.LoadoutName).Contains(l.ToString()));
    }
}

/// <summary>
///     Requires the profile to not have any more than X of the specified traits, loadouts, etc, in a group
/// </summary>
[UsedImplicitly, Serializable, NetSerializable]
public sealed partial class CharacterItemGroupRequirement : CharacterRequirement
{
    [DataField(required: true)]
    public ProtoId<CharacterItemGroupPrototype> Group;

    public override bool IsValid(
        JobPrototype job,
        HumanoidCharacterProfile profile,
        Dictionary<string, TimeSpan> playTimes,
        bool whitelisted,
        IPrototype prototype,
        IEntityManager entityManager,
        IPrototypeManager prototypeManager,
        IConfigurationManager configManager,
        out string? reason,
        int depth = 0,
        MindComponent? mind = null
    )
    {
        var group = prototypeManager.Index(Group);

        // Get the count of items in the group that are in the profile
        var items = group.Items.Select(item => item.TryGetValue(profile, prototypeManager, out _) ? item.ID : null)
            .Where(id => id != null)
            .ToList();
        var count = items.Count;

        // If prototype is selected, decrease the count. Or increase it via negative number. Not my monkey, not my circus.
        if (items.ToList().Contains(prototype.ID))
        {
            // This disgusting ELIF nest requires an engine PR to make less terrible.
            if (prototypeManager.TryIndex<LoadoutPrototype>(prototype.ID, out var loadoutPrototype))
                count -= loadoutPrototype.Slots;
            else if (prototypeManager.TryIndex<TraitPrototype>(prototype.ID, out var traitPrototype))
                count -= traitPrototype.ItemGroupSlots;
            else count--;
        }

        reason = Loc.GetString(
            "character-item-group-requirement",
            ("inverted", Inverted),
            ("group", Loc.GetString($"character-item-group-{Group}")),
            ("max", group.MaxItems));

        return count < group.MaxItems;
    }
}
