using System;

using System.Collections.Generic;
/// <summary>
/// Summary description for Class1
/// </summary>
/// 
namespace lecteurmp3_dotnet
{
	public class Dossier
	{
		public string Name { get; set; }
		public List<Fichier> dossier;
		public Dossier()
		{
			//
			// TODO: Add constructor logic here
			//
			dossier = new List<Fichier>();
		}
	}
}
