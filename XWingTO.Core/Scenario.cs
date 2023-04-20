using System.ComponentModel;

namespace XWingTO.Core;

public enum Scenario
{
	[Description("Assault At The Satellite Array")]
	AssaultAtTheSatelliteArray = 1,
	[Description("Chance Encounter")]
	ChanceEncounter = 2,
	[Description("Salvage Mission")]
	SalvageMission = 3,
	[Description("Scramble The Transmissions")]
	ScrambleTheTransmissions = 4
}