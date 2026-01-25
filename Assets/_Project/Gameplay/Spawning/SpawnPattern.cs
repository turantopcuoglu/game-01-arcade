using UnityEngine;
using System.Collections.Generic;

namespace Project.Gameplay.Spawning
{
	/// <summary>
	/// Defines obstacle types and their behaviors.
	/// </summary>
	public enum ObstacleType
	{
		Static,      // Standard non-moving block
		Sweeper,     // Moves left-right across lanes
		Gate         // Two blocks with gap in between
	}

	/// <summary>
	/// Represents a single obstacle in a pattern.
	/// </summary>
	[System.Serializable]
	public struct ObstacleData
	{
		public float zOffset;        // Distance from chunk start
		public float xPosition;      // Lane position (-2.2 to 2.2)
		public ObstacleType type;
		public float gateGapWidth;   // Only for Gate type
		public float sweeperSpeed;   // Only for Sweeper type
		public float sweeperRange;   // Only for Sweeper type

		public static ObstacleData Static(float z, float x)
		{
			return new ObstacleData
			{
				zOffset = z,
				xPosition = x,
				type = ObstacleType.Static
			};
		}

		public static ObstacleData Sweeper(float z, float x, float speed = 2f, float range = 2f)
		{
			return new ObstacleData
			{
				zOffset = z,
				xPosition = x,
				type = ObstacleType.Sweeper,
				sweeperSpeed = speed,
				sweeperRange = range
			};
		}

		public static ObstacleData Gate(float z, float gapCenter, float gapWidth = 1.5f)
		{
			return new ObstacleData
			{
				zOffset = z,
				xPosition = gapCenter,
				type = ObstacleType.Gate,
				gateGapWidth = gapWidth
			};
		}
	}

	/// <summary>
	/// Represents a coin in a pattern.
	/// </summary>
	[System.Serializable]
	public struct CoinData
	{
		public float zOffset;
		public float xPosition;

		public CoinData(float z, float x)
		{
			zOffset = z;
			xPosition = x;
		}
	}

	/// <summary>
	/// Pattern types for different gameplay experiences.
	/// </summary>
	public enum PatternType
	{
		SafeRun,       // Easy path, moderate coins
		RiskyLeft,     // Coins on left, narrow passage
		RiskyRight,    // Coins on right, narrow passage
		Slalom,        // Zigzag through obstacles
		GateRush,      // Multiple gates with coins through centers
		FakeReward,    // Lure with coins, then force direction change
		StraightRush,  // Used during Rush segments - straight coin line
		Empty          // Breathing room, no obstacles
	}

	/// <summary>
	/// Complete spawn pattern for a chunk.
	/// </summary>
	public class SpawnPattern
	{
		public PatternType Type { get; private set; }
		public float Length { get; private set; }           // Chunk length in units
		public List<ObstacleData> Obstacles { get; private set; }
		public List<CoinData> Coins { get; private set; }
		public bool IsRiskRoute { get; private set; }       // For risk/reward tracking
		public int ExpectedCoinCount => Coins.Count;

		public SpawnPattern(PatternType type, float length)
		{
			Type = type;
			Length = length;
			Obstacles = new List<ObstacleData>();
			Coins = new List<CoinData>();
		}

		public SpawnPattern SetRisky(bool risky)
		{
			IsRiskRoute = risky;
			return this;
		}

		public SpawnPattern AddObstacle(ObstacleData obstacle)
		{
			Obstacles.Add(obstacle);
			return this;
		}

		public SpawnPattern AddCoin(float z, float x)
		{
			Coins.Add(new CoinData(z, x));
			return this;
		}

		public SpawnPattern AddCoinTrail(float startZ, float x, int count, float spacing = 1.5f)
		{
			for (int i = 0; i < count; i++)
			{
				Coins.Add(new CoinData(startZ + i * spacing, x));
			}
			return this;
		}

		public SpawnPattern AddCoinArc(float startZ, float startX, float endX, int count, float zLength)
		{
			for (int i = 0; i < count; i++)
			{
				float t = (float)i / (count - 1);
				float z = startZ + t * zLength;
				float x = Mathf.Lerp(startX, endX, t);
				Coins.Add(new CoinData(z, x));
			}
			return this;
		}
	}

	/// <summary>
	/// Generates spawn patterns procedurally.
	/// </summary>
	public static class PatternGenerator
	{
		private const float MIN_X = -2.2f;
		private const float MAX_X = 2.2f;
		private const float LEFT_LANE = -1.5f;
		private const float CENTER_LANE = 0f;
		private const float RIGHT_LANE = 1.5f;

		/// <summary>
		/// Generate a pattern based on type and difficulty.
		/// </summary>
		public static SpawnPattern Generate(PatternType type, float length, float difficulty)
		{
			return type switch
			{
				PatternType.SafeRun => GenerateSafeRun(length, difficulty),
				PatternType.RiskyLeft => GenerateRiskyRoute(length, difficulty, true),
				PatternType.RiskyRight => GenerateRiskyRoute(length, difficulty, false),
				PatternType.Slalom => GenerateSlalom(length, difficulty),
				PatternType.GateRush => GenerateGateRush(length, difficulty),
				PatternType.FakeReward => GenerateFakeReward(length, difficulty),
				PatternType.StraightRush => GenerateStraightRush(length),
				PatternType.Empty => GenerateEmpty(length),
				_ => GenerateSafeRun(length, difficulty)
			};
		}

		/// <summary>
		/// SafeRun: Single lane, few obstacles, moderate coins.
		/// Player stays in one lane and collects coins safely.
		/// </summary>
		private static SpawnPattern GenerateSafeRun(float length, float difficulty)
		{
			var pattern = new SpawnPattern(PatternType.SafeRun, length);

			// Choose a safe lane (left or right)
			float safeLane = Random.value > 0.5f ? LEFT_LANE : RIGHT_LANE;
			float dangerLane = -safeLane;

			// Place 1-2 obstacles on the danger side
			int obstacleCount = 1 + (difficulty > 0.5f ? 1 : 0);
			float spacing = length / (obstacleCount + 1);

			for (int i = 0; i < obstacleCount; i++)
			{
				float z = spacing * (i + 1);
				pattern.AddObstacle(ObstacleData.Static(z, dangerLane));
			}

			// Coin trail on safe side
			int coinCount = Random.Range(5, 8);
			pattern.AddCoinTrail(2f, safeLane, coinCount, 1.5f);

			return pattern;
		}

		/// <summary>
		/// RiskyRoute: More coins but narrow passage.
		/// Player must navigate through tight spots for reward.
		/// </summary>
		private static SpawnPattern GenerateRiskyRoute(float length, float difficulty, bool leftSide)
		{
			var pattern = new SpawnPattern(leftSide ? PatternType.RiskyLeft : PatternType.RiskyRight, length);
			pattern.SetRisky(true);

			float coinLane = leftSide ? LEFT_LANE : RIGHT_LANE;
			float gateCenter = coinLane * 0.7f;  // Gate slightly towards coin side

			// Many coins on risky side
			int coinCount = Random.Range(8, 12);
			pattern.AddCoinTrail(1f, coinLane, coinCount, 1.2f);

			// Tight gate that player must pass through
			float gapWidth = Mathf.Lerp(1.8f, 1.2f, difficulty);  // Tighter with difficulty
			pattern.AddObstacle(ObstacleData.Gate(length * 0.4f, gateCenter, gapWidth));

			// Optional second obstacle on opposite side
			if (difficulty > 0.3f)
			{
				float blockLane = leftSide ? RIGHT_LANE : LEFT_LANE;
				pattern.AddObstacle(ObstacleData.Static(length * 0.7f, blockLane));
			}

			return pattern;
		}

		/// <summary>
		/// Slalom: Zigzag through alternating obstacles.
		/// Coins guide the path.
		/// </summary>
		private static SpawnPattern GenerateSlalom(float length, float difficulty)
		{
			var pattern = new SpawnPattern(PatternType.Slalom, length);

			int segments = 3 + (difficulty > 0.5f ? 1 : 0);
			float segmentLength = length / segments;

			bool startLeft = Random.value > 0.5f;

			for (int i = 0; i < segments; i++)
			{
				float segmentZ = segmentLength * i + 2f;
				bool isLeft = (i % 2 == 0) == startLeft;

				// Obstacle on one side
				float obstacleLane = isLeft ? LEFT_LANE : RIGHT_LANE;
				pattern.AddObstacle(ObstacleData.Static(segmentZ, obstacleLane));

				// Coins arc to the opposite side
				float coinStartX = isLeft ? RIGHT_LANE : LEFT_LANE;
				float coinEndX = isLeft ? LEFT_LANE : RIGHT_LANE;

				if (i < segments - 1)
				{
					pattern.AddCoinArc(segmentZ - 1f, coinStartX, coinEndX, 4, segmentLength * 0.8f);
				}
				else
				{
					// Last segment: straight trail
					pattern.AddCoinTrail(segmentZ, -obstacleLane, 4, 1.2f);
				}
			}

			return pattern;
		}

		/// <summary>
		/// GateRush: Multiple gates with coins through the center.
		/// Tests precision movement.
		/// </summary>
		private static SpawnPattern GenerateGateRush(float length, float difficulty)
		{
			var pattern = new SpawnPattern(PatternType.GateRush, length);

			int gateCount = 2 + (difficulty > 0.6f ? 1 : 0);
			float spacing = length / (gateCount + 1);
			float gapWidth = Mathf.Lerp(1.6f, 1.1f, difficulty);

			for (int i = 0; i < gateCount; i++)
			{
				float z = spacing * (i + 1);

				// Slightly offset gate centers for variety
				float gateOffset = Random.Range(-0.5f, 0.5f);
				pattern.AddObstacle(ObstacleData.Gate(z, gateOffset, gapWidth));

				// Coins right before each gate, through the center
				pattern.AddCoinTrail(z - 3f, gateOffset, 3, 1f);
			}

			return pattern;
		}

		/// <summary>
		/// FakeReward: Lure with coins, then force sudden direction change.
		/// Tests reaction time.
		/// </summary>
		private static SpawnPattern GenerateFakeReward(float length, float difficulty)
		{
			var pattern = new SpawnPattern(PatternType.FakeReward, length);
			pattern.SetRisky(true);

			// Choose bait side
			bool baitLeft = Random.value > 0.5f;
			float baitLane = baitLeft ? LEFT_LANE : RIGHT_LANE;
			float escapeLane = -baitLane;

			// Many coins on bait side (the lure)
			pattern.AddCoinTrail(1f, baitLane, 6, 1.2f);

			// Wall that blocks the bait lane (the trap)
			// Player must see it coming and switch lanes
			float wallZ = 8f + Random.Range(0f, 2f);
			pattern.AddObstacle(ObstacleData.Static(wallZ, baitLane));

			// Optional: second block to narrow escape
			if (difficulty > 0.5f)
			{
				pattern.AddObstacle(ObstacleData.Static(wallZ, CENTER_LANE));
			}

			// Reward coins on escape route
			pattern.AddCoinTrail(wallZ + 2f, escapeLane, 4, 1.3f);

			return pattern;
		}

		/// <summary>
		/// StraightRush: Used during Rush segments.
		/// Simple straight coin line, no obstacles.
		/// </summary>
		private static SpawnPattern GenerateStraightRush(float length)
		{
			var pattern = new SpawnPattern(PatternType.StraightRush, length);

			// Dense coin line down the center
			int coinCount = Mathf.FloorToInt(length / 1f);  // One coin per unit
			pattern.AddCoinTrail(0f, CENTER_LANE, coinCount, 1f);

			return pattern;
		}

		/// <summary>
		/// Empty: Breathing room between intense sections.
		/// </summary>
		private static SpawnPattern GenerateEmpty(float length)
		{
			var pattern = new SpawnPattern(PatternType.Empty, length);

			// Just a few coins scattered
			pattern.AddCoin(length * 0.25f, Random.Range(MIN_X, MAX_X));
			pattern.AddCoin(length * 0.5f, Random.Range(MIN_X, MAX_X));
			pattern.AddCoin(length * 0.75f, Random.Range(MIN_X, MAX_X));

			return pattern;
		}

		/// <summary>
		/// Select a random pattern type based on weights and difficulty.
		/// </summary>
		public static PatternType SelectRandomPattern(float difficulty, bool allowRisky = true)
		{
			// Weight distribution changes with difficulty
			float safeWeight = Mathf.Lerp(40f, 20f, difficulty);
			float riskyWeight = allowRisky ? Mathf.Lerp(10f, 25f, difficulty) : 0f;
			float slalomWeight = Mathf.Lerp(15f, 25f, difficulty);
			float gateWeight = Mathf.Lerp(10f, 20f, difficulty);
			float fakeWeight = allowRisky ? Mathf.Lerp(5f, 15f, difficulty) : 0f;
			float emptyWeight = Mathf.Lerp(20f, 5f, difficulty);

			float total = safeWeight + riskyWeight * 2 + slalomWeight + gateWeight + fakeWeight + emptyWeight;
			float roll = Random.Range(0f, total);

			float cumulative = 0f;

			cumulative += safeWeight;
			if (roll < cumulative) return PatternType.SafeRun;

			cumulative += riskyWeight;
			if (roll < cumulative) return PatternType.RiskyLeft;

			cumulative += riskyWeight;
			if (roll < cumulative) return PatternType.RiskyRight;

			cumulative += slalomWeight;
			if (roll < cumulative) return PatternType.Slalom;

			cumulative += gateWeight;
			if (roll < cumulative) return PatternType.GateRush;

			cumulative += fakeWeight;
			if (roll < cumulative) return PatternType.FakeReward;

			return PatternType.Empty;
		}
	}
}
