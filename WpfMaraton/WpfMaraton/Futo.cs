using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfMaraton
{
	/// <summary>
	/// A versenyen résztvevő futó adatait tároló osztály
	/// </summary>
	class Futo
	{
		static int maxID = 0;  //az eddigi objektumok között létező legnagyobb ID

		int fid;
		String fnev;
		int szulev;
		int szulho;
		int csapat;
		bool ffi; //true = férfi; false = nő

		/// <summary>
		/// A konstruktor a paraméterében kapott sorból veszi ki az objektum adatait.
		/// </summary>
		/// <param name="adatSor">A beolvasott CSV fájl egy sora, ahol az adatokat ";" választja el egymástól.</param>
		public Futo(String adatSor)
		{
			//FELADAT!
			string[] tomb = adatSor.Split(';');
			fid = Convert.ToInt32(tomb[0]);
			fnev = tomb[1];
			szulev = Convert.ToInt32(tomb[2]);
			szulho = Convert.ToInt32(tomb[3]);
			csapat = Convert.ToInt32(tomb[4]);
			if (tomb[5] == "0")
				ffi = false;
			else
				ffi = true;
			//Különösen figyeljen a maxID osztályváltozó helyes beállítására!
			maxID = tomb[tomb.Length-1][0]+1;
		}


		/// <summary>
		/// Új objektum létrehozására használt konstruktor, ahol minden adatot meg kell külön adni.
		/// </summary>
		public Futo(string fnev, int szulev, int szulho, int csapat, bool ffi)
		{
			this.fid = Futo.maxID++;
			this.fnev = fnev;
			this.szulev = szulev;
			this.szulho = szulho;
			this.csapat = csapat;
			this.ffi = ffi;
		}

		public int Fid => fid;
		public String Nev => fnev;
		public int Ev => szulev;
		public int Honap => szulho;
		public int Csapat => csapat;
		public bool Neme => ffi;

		public override string ToString()
		{
			return $"{this.fid};{this.fnev};{this.szulev};{this.szulho};{this.csapat};{(this.ffi ? 1 : 0)}";
		}
	}
}
