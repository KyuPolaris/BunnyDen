// SPDX-FileCopyrightText: 2021 Pancake <Pangogie@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021 metalgearsloth <comedian_vs_clown@hotmail.com>
// SPDX-FileCopyrightText: 2022 Paul Ritter <ritter.paul1@googlemail.com>
// SPDX-FileCopyrightText: 2022 mirrorcult <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2022 wrexbe <81056464+wrexbe@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Kara <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2023 Visne <39844191+Visne@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 DEATHB4DEFEAT <77995199+DEATHB4DEFEAT@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 sleepyyapril <123355664+sleepyyapril@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later AND MIT

using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Speech.Components
{
    [Prototype("accent")]
    public sealed partial class ReplacementAccentPrototype : IPrototype
    {
        [ViewVariables]
        [IdDataField]
        public string ID { get; private set; } = default!;

        /// <summary>
        ///     If this array is non-null, the full text of anything said will be randomly replaced with one of these words.
        /// </summary>
        [DataField("fullReplacements")]
        public string[]? FullReplacements;

        /// <summary>
        ///     If this dictionary is non-null and <see cref="FullReplacements"/> is null, any keys surrounded by spaces
        ///     (words) will be replaced by the value, attempting to intelligently keep capitalization.
        /// </summary>
        [DataField("wordReplacements")]
        public Dictionary<string, string>? WordReplacements;
    }

    /// <summary>
    /// Replaces full sentences or words within sentences with new strings.
    /// </summary>
    [RegisterComponent]
    public sealed partial class ReplacementAccentComponent : Component
    {
        [DataField("accent", customTypeSerializer: typeof(PrototypeIdSerializer<ReplacementAccentPrototype>), required: true)]
        public string Accent = default!;

        /// <summary>
        /// Allows you to substitute words, not always, but with some chance
        /// </summary>
        [DataField]
        public float ReplacementChance = 1f;
    }
}
