using System;
using UnityEngine;

namespace Pathfinding
{
	public class ModifierConverter
	{
		public static bool AllBits(ModifierData a, ModifierData b)
		{
			return (a & b) == b;
		}

		public static bool AnyBits(ModifierData a, ModifierData b)
		{
			return (a & b) != ModifierData.None;
		}

		public static ModifierData Convert(Path p, ModifierData input, ModifierData output)
		{
			if (!ModifierConverter.CanConvert(input, output))
			{
				Debug.LogError(string.Concat(new object[]
				{
					"Can't convert ",
					input,
					" to ",
					output
				}));
				return ModifierData.None;
			}
			if (ModifierConverter.AnyBits(input, output))
			{
				return input;
			}
			if (ModifierConverter.AnyBits(input, ModifierData.Nodes) && ModifierConverter.AnyBits(output, ModifierData.Vector))
			{
				p.vectorPath.Clear();
				for (int i = 0; i < p.vectorPath.get_Count(); i++)
				{
					p.vectorPath.Add(p.path.get_Item(i).position);
				}
				return ModifierData.VectorPath | (ModifierConverter.AnyBits(input, ModifierData.StrictNodePath) ? ModifierData.StrictVectorPath : ModifierData.None);
			}
			Debug.LogError(string.Concat(new object[]
			{
				"This part should not be reached - Error in ModifierConverted\nInput: ",
				input,
				" (",
				(int)input,
				")\nOutput: ",
				output,
				" (",
				(int)output,
				")"
			}));
			return ModifierData.None;
		}

		public static bool CanConvert(ModifierData input, ModifierData output)
		{
			ModifierData b = ModifierConverter.CanConvertTo(input);
			return ModifierConverter.AnyBits(output, b);
		}

		public static ModifierData CanConvertTo(ModifierData a)
		{
			if (a == ModifierData.All)
			{
				return ModifierData.All;
			}
			ModifierData modifierData = a;
			if (ModifierConverter.AnyBits(a, ModifierData.Nodes))
			{
				modifierData |= ModifierData.VectorPath;
			}
			if (ModifierConverter.AnyBits(a, ModifierData.StrictNodePath))
			{
				modifierData |= ModifierData.StrictVectorPath;
			}
			if (ModifierConverter.AnyBits(a, ModifierData.StrictVectorPath))
			{
				modifierData |= ModifierData.VectorPath;
			}
			return modifierData;
		}
	}
}
