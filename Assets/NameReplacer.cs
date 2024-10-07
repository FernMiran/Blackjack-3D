using UnityEngine;

namespace Utilities
{
	public class NameReplacer : MonoBehaviour
	{
		[SerializeField]
		string newFirstWord = "Prefix";

		[ContextMenu("Replace")]
		private void Replace()
		{
			// Call the method to replace the first word of all child game objects' names
			ReplaceFirstWordOfChildrenNames(gameObject, newFirstWord);
		}

		private void ReplaceFirstWordOfChildrenNames(GameObject parent, string newFirstWord)
		{
			if (parent == null || string.IsNullOrEmpty(newFirstWord))
			{
				Debug.LogWarning("Invalid parent game object or new first word.");
				return;
			}

			// Iterate through all child game objects
			foreach (Transform child in parent.transform)
			{
				// Split the child's name into words
				string[] words = child.name.Split(' ');

				// Replace the first word
				if (words.Length > 0)
				{
					words[0] = newFirstWord;
				}

				// Join the words back into a single string
				child.name = string.Join(" ", words);

				Debug.Log($"Child GameObject's new name: {child.name}");
			}

			DestroyImmediate(this);
		}
	}
}