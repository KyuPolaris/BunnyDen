using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Content.Shared._Funkystation.Atmos.HFR;

namespace Content.Shared._Funkystation.Atmos.Prototypes
{
    [Prototype("fusionRecipe")]
    public sealed class FusionRecipePrototype : IPrototype
    {
        [IdDataField]
        public string ID { get; private set; } = default!;

        [DataField("name")]
        public string Name { get; private set; } = default!;

        [DataField("negativeTemperatureMultiplier")]
        public float NegativeTemperatureMultiplier { get; set; }

        [DataField("positiveTemperatureMultiplier")]
        public float PositiveTemperatureMultiplier { get; set; }

        [DataField("energyConcentrationMultiplier")]
        public float EnergyConcentrationMultiplier { get; set; }

        [DataField("fuelConsumptionMultiplier")]
        public float FuelConsumptionMultiplier { get; set; }

        [DataField("gasProductionMultiplier")]
        public float GasProductionMultiplier { get; set; }

        [DataField("temperatureChangeMultiplier")]
        public float TemperatureChangeMultiplier { get; set; }

        [DataField("meltdownFlags")]
        public HypertorusFlags MeltdownFlags { get; set; }

        [DataField("requirements")]
        public List<string> Requirements { get; set; } = default!;

        [DataField("primaryProducts")]
        public List<string> PrimaryProducts { get; set; } = default!;

        [DataField("secondaryProducts")]
        public List<string> SecondaryProducts { get; set; } = default!;
    }
}