using Content.Server.Maps;
using Content.Server.PresetPicker;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;

namespace Content.Server.GameTicking.Presets
{
    /// <summary>
    ///     A round-start setup preset, such as which antagonists to spawn.
    /// </summary>
    [Prototype("gamePreset")]
    public sealed partial class GamePresetPrototype : IPrototype
    {
        [IdDataField]
        public string ID { get; private set; } = default!;

        [DataField("alias")]
        public string[] Alias = [];

        [DataField("name")]
        public string ModeTitle = "????";

        [DataField("description")]
        public string Description = string.Empty;

        [DataField("showInVote")]
        public bool ShowInVote;

        [DataField("minPlayers")]
        public int? MinPlayers;

        [DataField("maxPlayers")]
        public int? MaxPlayers;

        [DataField]
        public bool HighDanger;

        [DataField]
        public bool VoteOnly;

        [DataField("rules", customTypeSerializer: typeof(PrototypeIdListSerializer<EntityPrototype>))]
        public IReadOnlyList<string> Rules { get; private set; } = Array.Empty<string>();

        /// <summary>
        /// If specified, the gamemode will only be run with these maps.
        /// If none are elligible, the global fallback will be used.
        /// </summary>
        [DataField("supportedMaps", customTypeSerializer: typeof(PrototypeIdSerializer<GameMapPoolPrototype>))]
        public string? MapPool;
    }
}
