using System;

namespace UnityEngine.SceneManagement
{
	public class SceneManager
	{
		public static void LoadScene(string name)
		{
			Application.LoadLevel(name);
		}

		public static void LoadScene(int buildIndex)
		{
			Application.LoadLevel(buildIndex);
		}
	}
}
