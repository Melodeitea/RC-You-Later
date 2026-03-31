using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public static class Timer
{
    private static readonly Stopwatch stopwatch = new();
    private static readonly List<long> steps = new();

    public static bool IsRunning
    {
        get => stopwatch.IsRunning;
    }

    public static double ElapsedSeconds
    {
        get => stopwatch.ElapsedMilliseconds * 0.001f;
    }

    public static int StepsCount
    {
        get => steps.Count;
    }

    public static double GetStepElapsedSeconds(int index)
    {
        return steps[index] * 0.001f;
    }

    /// <summary>
    /// Reset the timer and remove any steps.
    /// </summary>
    public static void Reset()
    {
        stopwatch.Reset();
        steps.Clear();
    }

    public static void Start()
    {
        stopwatch.Start();
    }

    public static void Stop()
    {
        stopwatch.Stop();
    }

    public static void Step()
    {
		// Enregistre un checkpoint (temps en millisecondes)
		steps.Add(stopwatch.ElapsedMilliseconds);
    }

	public static void Save()
	{
		// code inspiré d'une video de brackeys que j'avais utilisé y a quelques temps parce qu'elle ne cree pas de pb 
		// quand je passe de linux à windows et inversement

		// ici le persistant data path me sauve de windows qui a pas la même hiérarchie de fichiers que linux,
		// et oui je sais techniquement je peux creer un fichier dans le repo git mais je me dis si je me repose sur ça,un build et cest mort

		string path = Application.persistentDataPath + "/score.txt";

		// Temps final actuel :
		// - soit le dernier step (si checkpoints)
		// - soit le temps total si aucun step
		long finalTime = steps.Count > 0 ? steps[steps.Count - 1] : stopwatch.ElapsedMilliseconds;


		if (File.Exists(path))
		{
			long bestTime = long.MaxValue;

			// On lit la DERNIÈRE ligne du fichier = ancien temps final
			using (StreamReader reader = new StreamReader(path))
			{
				string lastLine = null;

				while (!reader.EndOfStream)
				{
					lastLine = reader.ReadLine();
				}

				// On convertit en long si possible
				if (long.TryParse(lastLine, out long value))
				{
					bestTime = value;
				}
			}

			// pas meilleurs --> quit
			if (finalTime >= bestTime)
			{
				UnityEngine.Debug.Log("Score not saved (you suck)");
				return;
			}
		}

		// meilleur score --> save

		// On écrase le fichier (FileMode.Create)
		using (FileStream stream = new FileStream(path, FileMode.Create))
		using (StreamWriter writer = new StreamWriter(stream))
		{
			// On sauvegarde tous les steps
			foreach (long step in steps)
			{
				writer.WriteLine(step);
			}
		}
	}


	public static void Load()
    {
		// TODO : load our time steps from a file (if we have any)
		// and store them inside our steps variable (line 7 of this script)
		// to show them to the player before starting a race.
		
		string path = Application.persistentDataPath + "/score.txt";

		if (File.Exists(path))
		{
			steps.Clear();

			// Lecture du fichier ligne par ligne
			using (FileStream stream = new FileStream(path, FileMode.Open))
			using (StreamReader reader = new StreamReader(stream))
			{
				while (!reader.EndOfStream)
				{
					string line = reader.ReadLine();

					// On reconstruit les steps sauvegardés
					if (long.TryParse(line, out long value))
					{
						steps.Add(value);
					}
				}
			}

			UnityEngine.Debug.Log("Loaded save");
		}
		else
		{
			UnityEngine.Debug.Log("No save found");
		}
	}
}
