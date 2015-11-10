﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HaloOnlineTagTool.Common;
using HaloOnlineTagTool.Serialization;

namespace HaloOnlineTagTool.TagStructures
{
	[TagStructure(Class = "unit", Size = 0x2C8)]
	public abstract class Unit : GameObject
	{
		public uint FlagsWarningHalo4Values;
		public DefaultTeamValue DefaultTeam;
		public ConstantSoundVolumeValue ConstantSoundVolume;
		public HaloTag HologramUnit;
		public List<MetagameProperty> MetagameProperties;
		public HaloTag IntegratedLightToggle;
		public Angle CameraFieldOfView;
		public float CameraStiffness;
		public short Flags2;
		public short Unknown6;
		public StringId CameraMarkerName;
		public StringId CameraSubmergedMarkerName;
		public Angle PitchAutoLevel;
		public Angle PitchRangeMin;
		public Angle PitchRangeMax;
		public List<CameraTrack> CameraTracks;
		public Angle Unknown7;
		public Angle Unknown8;
		public Angle Unknown9;
		public List<UnknownBlock> Unknown10;
		public short Flags3;
		public short Unknown11;
		public StringId CameraMarkerName2;
		public StringId CameraSubmergedMarkerName2;
		public Angle PitchAutoLevel2;
		public Angle PitchRangeMin2;
		public Angle PitchRangeMax2;
		public List<CameraTrack2> CameraTracks2;
		public Angle Unknown12;
		public Angle Unknown13;
		public Angle Unknown14;
		public List<UnknownBlock2> Unknown15;
		public HaloTag AssassinationResponse;
		public HaloTag AssassinationWeapon;
		public StringId AssasinationToolStowAnchor;
		public StringId AssasinationToolHandMarker;
		public StringId AssasinationToolMarker;
		public float AccelerationRangeI;
		public float AccelerationRangeJ;
		public float AccelerationRangeK;
		public float AccelerationActionScale;
		public float AccelerationAttachScale;
		public float SoftPingThreshold;
		public float SoftPingInterruptTime;
		public float HardPingThreshold;
		public float HardPingInterruptTime;
		public float FeignDeathThreshold;
		public float FeignDeathTime;
		public float DistanceOfEvadeAnimation;
		public float DistanceOfDiveAnimation;
		public float StunnedMovementThreshold;
		public float FeignDeathChance;
		public float FeignRepeatChance;
		public HaloTag SpawnedTurretCharacter;
		public short SpawnedActorCountMin;
		public short SpawnedActorCountMax;
		public float SpawnedVelocity;
		public Angle AimingVelocityMaximum;
		public Angle AimingAccelerationMaximum;
		public float CasualAimingModifier;
		public Angle LookingVelocityMaximum;
		public Angle LookingAccelerationMaximum;
		public StringId RightHandNode;
		public StringId LeftHandNode;
		public StringId PreferredGunNode;
		public HaloTag MeleeDamage;
		public HaloTag BoardingMeleeDamage;
		public HaloTag BoardingMeleeResponse;
		public HaloTag EjectionMeleeDamage;
		public HaloTag EjectionMeleeResponse;
		public HaloTag LandingMeleeDamage;
		public HaloTag FlurryMeleeDamage;
		public HaloTag ObstacleSmashMeleeDamage;
		public HaloTag ShieldPopDamage;
		public HaloTag AssassinationDamage;
		public MotionSensorBlipSizeValue MotionSensorBlipSize;
		public ItemScaleValue ItemScale;
		public List<Posture> Postures;
		public List<HudInterface> HudInterfaces;
		public List<DialogueVariant> DialogueVariants;
		public float Unknown16;
		public float Unknown17;
		public float Unknown18;
		public float Unknown19;
		public float GrenadeVelocity;
		public GrenadeTypeValue GrenadeType;
		public short GrenadeCount;
		public List<PoweredSeat> PoweredSeats;
		public List<Weapon> Weapons;
		public List<TargetTrackingBlock> TargetTracking;
		public List<Seat> Seats;
		public float EmpRadius;
		public HaloTag EmpEffect;
		public HaloTag BoostCollisionDamage;
		public float BoostPeakPower;
		public float BoostRisePower;
		public float BoostPeakTime;
		public float BoostFallPower;
		public float BoostDeadTime;
		public float LipsyncAttackWeight;
		public float LipsyncDecayWeight;
		public HaloTag DetachDamage;
		public HaloTag DetachedWeapon;

		public enum DefaultTeamValue : short
		{
			Default,
			Player,
			Human,
			Covenant,
			Flood,
			Sentinel,
			Heretic,
			Prophet,
			Guilty,
			Unused9,
			Unused10,
			Unused11,
			Unused12,
			Unused13,
			Unused14,
			Unused15,
		}

		public enum ConstantSoundVolumeValue : short
		{
			Silent,
			Medium,
			Loud,
			Shout,
			Quiet,
		}

		[TagStructure(Size = 0x8)]
		public class MetagameProperty
		{
			public UnitKindValue UnitKind;
			public UnitValue Unit;
			public ClassificationValue Classification;
			public sbyte Unknown;
			public short BasePointWorth;
			public short Unknown2;

			public enum UnitKindValue : sbyte
			{
				Actor,
				Vehicle,
			}

			public enum UnitValue : sbyte
			{
				Brute,
				Grunt,
				Jackal,
				Marine,
				Drone,
				Hunter,
				Unknown,
				FloodCarrier,
				FloodCombat,
				FloodPureform,
				Forerunner,
				Elite,
				Unknown2,
				Mongoose,
				Warthog,
				Scorpion,
				Hornet,
				Pelican,
				Shade,
				Unknown3,
				Ghost,
				Chopper,
				Mauler,
				Wraith,
				Banshee,
				Phantom,
				Scarab,
				Unknown4,
				Engineer,
			}

			public enum ClassificationValue : sbyte
			{
				Infantry,
				Leader,
				Hero,
				Specialist,
				LightVehicle,
				HeavyVehicle,
				GiantVehicle,
				MediumVehicle,
			}
		}

		[TagStructure(Size = 0x10)]
		public class CameraTrack
		{
			public HaloTag Track;
		}

		[TagStructure(Size = 0x4C)]
		public class UnknownBlock
		{
			public float Unknown;
			public float Unknown2;
			public float Unknown3;
			public float Unknown4;
			public float Unknown5;
			public float Unknown6;
			public float Unknown7;
			public float Unknown8;
			public float Unknown9;
			public float Unknown10;
			public float Unknown11;
			public float Unknown12;
			public float Unknown13;
			public float Unknown14;
			public float Unknown15;
			public float Unknown16;
			public float Unknown17;
			public float Unknown18;
			public float Unknown19;
		}

		[TagStructure(Size = 0x10)]
		public class CameraTrack2
		{
			public HaloTag Track;
		}

		[TagStructure(Size = 0x4C)]
		public class UnknownBlock2
		{
			public float Unknown;
			public float Unknown2;
			public float Unknown3;
			public float Unknown4;
			public float Unknown5;
			public float Unknown6;
			public float Unknown7;
			public float Unknown8;
			public float Unknown9;
			public float Unknown10;
			public float Unknown11;
			public float Unknown12;
			public float Unknown13;
			public float Unknown14;
			public float Unknown15;
			public float Unknown16;
			public float Unknown17;
			public float Unknown18;
			public float Unknown19;
		}

		public enum MotionSensorBlipSizeValue : short
		{
			Medium,
			Small,
			Large,
		}

		public enum ItemScaleValue : short
		{
			Human,
			Player,
			Covenant,
			Boss,
		}

		[TagStructure(Size = 0x10)]
		public class Posture
		{
			public StringId Name;
			public float PillOffsetI;
			public float PillOffsetJ;
			public float PillOffsetK;
		}

		[TagStructure(Size = 0x10)]
		public class HudInterface
		{
			public HaloTag UnitHudInterface;
		}

		[TagStructure(Size = 0x14)]
		public class DialogueVariant
		{
			public short VariantNumber;
			public short Unknown;
			public HaloTag Dialogue;
		}

		public enum GrenadeTypeValue : short
		{
			HumanFragmentation,
			CovenantPlasma,
			BruteSpike,
			Incendiary,
		}

		[TagStructure(Size = 0x8)]
		public class PoweredSeat
		{
			public float DriverPowerupTime;
			public float DriverPowerdownTime;
		}

		[TagStructure(Size = 0x10)]
		public class Weapon
		{
			public HaloTag Weapon2;
		}

		[TagStructure(Size = 0x38)]
		public class TargetTrackingBlock
		{
			public List<TrackingType> TrackingTypes;
			public float AcquireTime;
			public float GraceTime;
			public float DecayTime;
			public HaloTag TrackingSound;
			public HaloTag LockedSound;

			[TagStructure(Size = 0x4)]
			public class TrackingType
			{
				public StringId TrackingType2;
			}
		}

		[TagStructure(Size = 0xE4)]
		public class Seat
		{
			public uint Flags;
			public StringId SeatAnimation;
			public StringId SeatMarkerName;
			public StringId EntryMarkerSName;
			public StringId BoardingGrenadeMarker;
			public StringId BoardingGrenadeString;
			public StringId BoardingMeleeString;
			public StringId DetachWeaponString;
			public float PingScale;
			public float TurnoverTime;
			public float AccelerationRangeI;
			public float AccelerationRangeJ;
			public float AccelerationRangeK;
			public float AccelerationActionScale;
			public float AccelerationAttachScale;
			public float AiScariness;
			public AiSeatTypeValue AiSeatType;
			public short BoardingSeat;
			public float ListenerInterpolationFactor;
			public float YawRateBoundsMin;
			public float YawRateBoundsMax;
			public float PitchRateBoundsMin;
			public float PitchRateBoundsMax;
			public float Unknown;
			public float MinimumSpeedReference;
			public float MaximumSpeedReference;
			public float SpeedExponent;
			public short Unknown2;
			public short Unknown3;
			public StringId CameraMarkerName;
			public StringId CameraSubmergedMarkerName;
			public Angle PitchAutoLevel;
			public Angle PitchRangeMin;
			public Angle PitchRangeMax;
			public List<CameraTrack> CameraTracks;
			public Angle Unknown4;
			public Angle Unknown5;
			public Angle Unknown6;
			public List<UnknownBlock> Unknown7;
			public List<UnitHudInterfaceBlock> UnitHudInterface;
			public StringId EnterSeatString;
			public Angle YawRangeMin;
			public Angle YawRangeMax;
			public HaloTag BuiltInGunner;
			public float EntryRadius;
			public Angle EntryMarkerConeAngle;
			public Angle EntryMarkerFacingAngle;
			public float MaximumRelativeVelocity;
			public StringId InvisibleSeatRegion;
			public int RuntimeInvisibleSeatRegionIndex;

			public enum AiSeatTypeValue : short
			{
				None,
				Passenger,
				Gunner,
				SmallCargo,
				LargeCargo,
				Driver,
			}

			[TagStructure(Size = 0x10)]
			public class CameraTrack
			{
				public HaloTag Track;
			}

			[TagStructure(Size = 0x4C)]
			public class UnknownBlock
			{
				public float Unknown;
				public float Unknown2;
				public float Unknown3;
				public float Unknown4;
				public float Unknown5;
				public float Unknown6;
				public float Unknown7;
				public float Unknown8;
				public float Unknown9;
				public float Unknown10;
				public float Unknown11;
				public float Unknown12;
				public float Unknown13;
				public float Unknown14;
				public float Unknown15;
				public float Unknown16;
				public float Unknown17;
				public float Unknown18;
				public float Unknown19;
			}

			[TagStructure(Size = 0x10)]
			public class UnitHudInterfaceBlock
			{
				public HaloTag UnitHudInterface;
			}
		}
	}
}