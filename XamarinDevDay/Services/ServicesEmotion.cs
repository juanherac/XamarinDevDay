using System.IO;

using System.Linq;

using System.Threading.Tasks;

using System.Collections.Generic;

using Microsoft.ProjectOxford.Emotion;

namespace XamarinDevDay.Services

{

	public class ServiceEmotions

	{

		static string key = "c3b18ba8009b458da0d1b1e49f42d405";

		public static async Task<Dictionary<string, float>> GetEmotions(Stream stream)

		{

			EmotionServiceClient cliente = new EmotionServiceClient(key);

			var emotions = await cliente.RecognizeAsync(stream);

			if (emotions == null || emotions.Count() == 0)

				return null;

			return emotions[0].Scores.ToRankedList().ToDictionary(x => x.Key, x => x.Value);

		}

	}

}