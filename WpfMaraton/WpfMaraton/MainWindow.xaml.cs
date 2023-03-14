using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfMaraton
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		VersenyCSV csv = new VersenyCSV();
		public MainWindow()
		{
			InitializeComponent();
			//Adatbetöltés, Adatok megjelenítése:
			csv.AdatokBetoltese();
			dg_futoadatok.ItemsSource = csv.Futok;
			dg_eredmenyadatok.ItemsSource = csv.Eredmenyek;
		}
		private void lekerdezesek_GotFocus(object sender, RoutedEventArgs e)
		{
			//Ez azért szükséges, hogyha felvennénk egy futót, vagy törlnénk/rögzítenénk eredményt, minden listBox az aktuális adatot mutassa.
			//lb_osszido.Items.Clear();
			//lb_kisebbHat.Items.Clear();
			//Futók számának meghatározása:
			int szam = csv.Futok.Count();
			lbl_fszam.Content = "A futók száma: " + szam;
			double atlag = csv.Eredmenyek.Average(x => x.Ido);
			//Átlagos köridő meghatározása:
			lbl_atlag.Content = "Átlagos köridő: " + Math.Round(atlag, 2);
			//Legkisebb időt elérő futó neve, ideje, és progressbar-on ábrázolás:
			int legkisebb = csv.Eredmenyek.Min(x => x.Ido);
			int idLegkisebb = csv.Eredmenyek.Where(x => x.Ido == legkisebb).Select(x => x.FutoID).First();
			string Nevlegkisebb = csv.Futok.Where(x => x.Fid == idLegkisebb).Select(x => x.Nev).First();
			lbl_legkisebbNevIdo.Content = "Legkisebb időt elérő neve, ideje: " + Nevlegkisebb + ", " + legkisebb;
			//A progressbar maximum értéke, a futók átlaga, és az azon megjelenített value=érték a legkisebb idő alatt futóé.
			//A zöld színű rész, az a legkisebb futóé, a háttér, ami szürke az azt jelenti, hogy átlagosan a többi futónak mennyi többletidő kellett.
			pb_legkisHely.Maximum = atlag;
			pb_legkisHely.Value = legkisebb;
			//A férfiak és nők százalékos eloszlásának meghatározása, és progressbar-on ábrázolás:
			int ferfi = csv.Futok.Where(x => x.Neme == true).Count();
			double ferfiSzazalek = Math.Round((double)100 / szam * ferfi, 2);
			lbl_ferfiNoEloszlas.Content = $"Férfiak: {ferfiSzazalek} Nők: {100 - ferfiSzazalek}";
			//A progressbar maximum értéke a 100%. A value-nak a férféiak százalékát adtam meg.
			//Mivel a hátteret rózsaszínre állítottam, a rózsaszín érték a nők aránya, a kék pedig a férfiaké.
			pb_ferfiNoEloszlas.Maximum = 100;
			pb_ferfiNoEloszlas.Value = ferfiSzazalek;
			//Azoknak  a csapatoknak az azonosítóját írja ki, ahol kevesebben vannak, mint 6:
			var kisebb6 = csv.Csapatok().Where(x => x.Value < 6).Select(x => x.Key).OrderBy(x => x).ToList();
			lb_kisebbHat.ItemsSource = kisebb6;
			//Összidő kiíratás, és fájlba mentése:
			var csapatSzamok = csv.Futok.Select(x => x.Csapat).Distinct().ToList();
			List<int> Osszesit = new List<int>();
			foreach (var item in csapatSzamok)
			{
				int csapSzIdo = csv.CsapatOsszideje(item);
				Osszesit.Add(csapSzIdo);
			}
			List<string> ListaKesz = new List<string>();

			for (int i = 0; i < csapatSzamok.Count(); i++)
			{
				ListaKesz.Add(csapatSzamok[i] + ";" + Osszesit[i]);
			}
			ListaKesz = ListaKesz.OrderBy(x => Convert.ToInt32(x.Split(';')[1])).ToList();
			lb_osszido.ItemsSource = ListaKesz;
			File.WriteAllLines("eredmeny.txt", ListaKesz);
			//Legidősebb női versenyző, aki ajándékot kap:
			int legregebbiSzulEv = csv.Futok.Where(x => x.Neme == false).Select(x => x.Ev).OrderBy(x => x).First();
			int legregebbSzulHo = csv.Futok.Where(x => x.Neme == false).Select(x => x.Honap).OrderBy(x => x).First();
			var legid = csv.Futok.Where(x => x.Ev == legregebbiSzulEv && x.Honap == legregebbSzulHo && x.Neme == false).Select(x => x.Nev).ToList();
			Random rnd = new Random();
			int randomGen = rnd.Next(1, legid.Count() + 1);
			lbl_legidNoAj.Content = "Ajándékot kap: " + legid[randomGen - 1];
		}

		static List<int> azonositok = new List<int>();
		int fazon = 0;
		int ido = 0;

		/// <summary>
		/// Az eredmény oldalon vizsgálja, hogy a megadott futóazonosító érvényes-e, illetve, hogy a futó ideje nem lehet 0.
		/// </summary>
		private void FutoAzEllenoriz()
		{
			if (azonositok.Contains(fazon) && ido != 0)
			{
				btn_eredmenybevitel.IsEnabled = true;
			}
			else
			{
				btn_eredmenybevitel.IsEnabled = false;
			}
		}
		private void txt_fazon_TextChanged(object sender, TextChangedEventArgs e)
		{
			double parsedValue;

			if (!double.TryParse(txt_fazon.Text, out parsedValue) || txt_fazon.Text.Substring(0, 1) == "0")
			{
				txt_fazon.Text = "";
				btn_eredmenybevitel.IsEnabled = false;
			}
			else
			{
				fazon = Convert.ToInt32(txt_fazon.Text);
				azonositok = csv.Futok.Select(x => x.Fid).ToList();
				FutoAzEllenoriz();
			}
		}

		private void txt_ido_TextChanged(object sender, TextChangedEventArgs e)
		{
			double parsedValue;

			if (!double.TryParse(txt_ido.Text, out parsedValue) || txt_ido.Text.Substring(0, 1) == "0")
			{
				txt_ido.Text = "";
				btn_eredmenybevitel.IsEnabled = false;
			}
			else
			{
				ido = Convert.ToInt32(txt_ido.Text);
				FutoAzEllenoriz();
			}
		}

		private void txt_csapsor_TextChanged(object sender, TextChangedEventArgs e)
		{
			int soraz = 0;
			lb_nevek.Items.Clear();
			double parsedValue;
			if (!double.TryParse(txt_csapsor.Text, out parsedValue) || txt_csapsor.Text.Substring(0, 1) == "0")
			{
				txt_csapsor.Text = "";
			}
			else
			{
				soraz = Convert.ToInt32(txt_csapsor.Text);
				var csapatsorszam = csv.Futok.Select(x => x.Csapat).Distinct().ToList();
				if (csapatsorszam.Contains(soraz))
				{
					var Nevek = csv.Csapattagok(soraz).Select(x => x.Nev).ToList();
					foreach (var item in Nevek)
					{
						lb_nevek.Items.Add(item);
					}
				}
			}
		}

		private void dg_eredmenyadatok_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (dg_eredmenyadatok.SelectedIndex != -1)
			{
				btn_eredmenytorles.IsEnabled = true;
			}
		}

		/// <summary>
		/// A törlés és eredményfelvitel után a dg_eredmenyadatok DataGridet frissíti.
		/// </summary>
		private void DgEredFrissit()
		{
			dg_eredmenyadatok.ItemsSource = default;
			dg_eredmenyadatok.ItemsSource = csv.Eredmenyek;
		}
		private void btn_eredmenytorles_Click(object sender, RoutedEventArgs e)
		{
			string kijeloltSor = dg_eredmenyadatok.SelectedCells[0].Item.ToString();
			string[] AdatSplit = kijeloltSor.Split(';');
			int FutoAzon = Convert.ToInt32(AdatSplit[0]);
			int FutoKor = Convert.ToInt32(AdatSplit[1]);
			int FutoIdo = Convert.ToInt32(AdatSplit[2]);
			Eredmeny eredmeny = new Eredmeny(FutoAzon, FutoKor, FutoIdo);
			csv.TorolEredmenyt(eredmeny);
			DgEredFrissit();
			btn_eredmenytorles.IsEnabled = false;
		}

		private void btn_eredmenybevitel_Click(object sender, RoutedEventArgs e)
		{
			Eredmeny erFelvesz = new Eredmeny(Convert.ToInt32(txt_fazon.Text), cb_kor.SelectedIndex + 1, Convert.ToInt32(txt_ido.Text));
			csv.UjEredmeny(erFelvesz);
			DgEredFrissit();
			txt_fazon.Clear();
			txt_ido.Clear();
			cb_kor.SelectedIndex = 0;
			ido = 0;
			fazon = 0;
			btn_eredmenybevitel.IsEnabled = false;
		}

		private void txt_SzulDat_TextChanged(object sender, TextChangedEventArgs e)
		{
			double parsedValue;
			if (!double.TryParse(txt_SzulDat.Text, out parsedValue) || txt_SzulDat.Text.Substring(0, 1) == "0")
			{
				txt_SzulDat.Text = "";
			}
			Ellenorzesek();
		}

		private void txt_Nev_TextChanged(object sender, TextChangedEventArgs e)
		{
			Ellenorzesek();
		}

		/// <summary>
		/// A Születési dátum és Név megadása alapján létező listázásokat tartalmazza.
		/// </summary>
		List<Futo> SzulLista = new List<Futo>();
		List<Futo> NevLista = new List<Futo>();
		List<Futo> Ketmezos = new List<Futo>();
		private void Ellenorzesek()
		{
			lb_SzulVagyNevLista.Items.Clear();
			Ketmezos.Clear();
			if (txt_SzulDat.Text != "" && txt_Nev.Text != "")
			{
				Ketmezos = csv.Futok.Where(x => x.Nev == txt_Nev.Text && x.Ev == Convert.ToInt32(txt_SzulDat.Text)).ToList();
				if (Ketmezos.Count() != 0)
				{
					foreach (var item in Ketmezos)
					{
						lb_SzulVagyNevLista.Items.Add(item);
					}
				}
			}
			else if (txt_SzulDat.Text != "" && txt_Nev.Text == "")
			{
				int szulDat = Convert.ToInt32(txt_SzulDat.Text);
				SzulLista = csv.Futok.Where(x => x.Ev == szulDat).ToList();
				foreach (var item in SzulLista)
				{
					lb_SzulVagyNevLista.Items.Add(item);
				}
			}
			else if (txt_SzulDat.Text == "" && txt_Nev.Text != "")
			{
				NevLista = csv.Futok.Where(x => x.Nev == txt_Nev.Text).ToList();
				foreach (var item in NevLista)
				{
					lb_SzulVagyNevLista.Items.Add(item);
				}
			}
			else if (txt_SzulDat.Text == "" && txt_Nev.Text == "")
			{
				lb_SzulVagyNevLista.Items.Clear();
			}
		}

		int honap = 0;
		private void txt_szho_TextChanged(object sender, TextChangedEventArgs e)
		{
			honap = 0;
			double parsedValue;
			if (!double.TryParse(txt_szho.Text, out parsedValue) || txt_szho.Text.Substring(0, 1) == "0" || Convert.ToInt32(txt_szho.Text) > 12)
			{
				txt_szho.Text = "";
			}
			else
			{
				honap = Convert.ToInt32(txt_szho.Text);
			}
			MindenBevittE();
		}

		int ev = 0;
		private void txt_szev_TextChanged(object sender, TextChangedEventArgs e)
		{
			ev = 0;
			double parsedValue;
			if (!double.TryParse(txt_szev.Text, out parsedValue) || txt_szev.Text.Substring(0, 1) == "0")
			{
				txt_szev.Text = "";
			}
			else
			{
				ev = Convert.ToInt32(txt_szev.Text);
				if (2021 - ev < 18)
				{
					MessageBox.Show("A versenyhez induláshoz 18 évesnek kell lenned!");
					txt_szev.Text = "";
				}
			}
			MindenBevittE();
		}

		int csapazon = 0;
		private void txt_csapaz_TextChanged(object sender, TextChangedEventArgs e)
		{
			csapazon = 0;
			double parsedValue;
			if (!double.TryParse(txt_csapaz.Text, out parsedValue) || txt_csapaz.Text.Substring(0, 1) == "0")
			{
				txt_csapaz.Text = "";
			}
			else if (txt_csapaz.Text.Length >= 3)
			{
				int csapaz = Convert.ToInt32(txt_csapaz.Text);
				var kevesebbenMint6 = csv.Csapatok().Where(x => x.Value < 6).Select(x => x.Key).ToList();
				if (kevesebbenMint6.Contains(csapaz))
				{
					csapazon = Convert.ToInt32(txt_csapaz.Text);
				}
				else
				{
					txt_csapaz.Text = "";
				}
			}
			MindenBevittE();
		}

		int CursorWas;
		string WhatItWas;
		string nev;
		private void txt_fnev_TextChanged(object sender, TextChangedEventArgs e)
		{
			nev = "";
			if (Regex.IsMatch(txt_fnev.Text, "^[a-zA-Z ÁÉÚŐÓÜÖÍáéúőóüöí]*$"))
			{
				WhatItWas = txt_fnev.Text;
				nev = txt_fnev.Text;
			}
			else
			{
				CursorWas = txt_fnev.SelectionStart == 0 ? 0 : txt_fnev.SelectionStart - 1;
				txt_fnev.Text = WhatItWas;
				txt_fnev.SelectionStart = CursorWas;
			}
			MindenBevittE();
		}

		/// <summary>
		/// Az új futó beviteléhez ellenőrzi, hogy minden mező ki van töltve, vagy nem.
		/// </summary>
		private void MindenBevittE()
		{
			if (honap != 0 && ev != 0 && csapazon != 0 && nev != "")
			{
				btn_adatbevitel.IsEnabled = true;
			}
			else
			{
				btn_adatbevitel.IsEnabled = false;
			}
		}

		private void btn_adatbevitel_Click(object sender, RoutedEventArgs e)
		{
			int futoAzonositoja = csv.Futok.Max(x=>x.Fid)+1;
			int nem;
			if (cb_fnem.SelectedIndex == 0)
				nem = 1;
			else
				nem = 0;
			string adatsor = futoAzonositoja+";"+ nev + ";" + ev + ";" + honap + ";" + csapazon+";"+nem;
			Futo f = new Futo(adatsor);
			csv.UjFuto(f);
			dg_futoadatok.ItemsSource = default;
			dg_futoadatok.ItemsSource = csv.Futok;
			txt_fnev.Text = "";
			txt_szev.Text = "";
			txt_szho.Text = "";
			txt_SzulDat.Text = "";
			txt_csapaz.Text = "";
			cb_fnem.SelectedIndex = 0;
			nev ="";
			ev = 0;
			honap = 0;
			csapazon = 0;
		}
	}
}
