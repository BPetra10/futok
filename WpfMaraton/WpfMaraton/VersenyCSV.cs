using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
	class VersenyCSV : VersenyRepository
	{
		public override void AdatokBetoltese()
		{
			try
			{
				Eredmenyek = File.ReadAllLines("eredmenyek.csv").Skip(1).Select(data => new Eredmeny(data)).ToList();
				Futok = File.ReadAllLines("futok.csv").Skip(1).Select(data => new Futo(data)).ToList();
			}
			catch (Exception e)
			{
				MessageBox.Show("Hiba a beolvasáskor:" + e.Message);
				throw new FileNotFoundException("Hiba!");
			}
		}

		public override Dictionary<int, int> Csapatok()
		{
			var ossz = Futok.GroupBy(x => x.Csapat);
			Dictionary<int, int> csapatok = new Dictionary<int, int>();
			foreach (var item in ossz)
			{
				csapatok.Add(item.Key,item.Count());
			}
			return csapatok;
		}

		public override int CsapatOsszideje(int csapatSzama)
		{
			var azonositok = Futok.Where(x => x.Csapat == csapatSzama).Select(x=>x.Fid).ToList();
			int osszeg = 0;
			for (int i = 0; i < azonositok.Count; i++)
			{
				osszeg += Eredmenyek.Where(x => x.FutoID == azonositok[i]).Sum(x => x.Ido);
			}
			return osszeg;
		}

		public override List<Futo> Csapattagok(int csapatSzama)
		{
			var fut = Futok.Where(x=>x.Csapat==csapatSzama).ToList();
			return fut;
		}

		public override void TorolEredmenyt(Eredmeny torolni)
		{
			var torlendo = Eredmenyek.Where(x=>x.FutoID == torolni.FutoID && x.Ido==torolni.Ido && x.Kor==torolni.Kor).First();
			Eredmenyek.Remove(torlendo);
			StreamWriter sw = new StreamWriter("eredmenyek.csv");
			sw.WriteLine("futo;kor;ido");
			foreach (var item in Eredmenyek)
			{
				sw.WriteLine(item.FutoID+";"+item.Kor+";"+item.Ido);
			}
			sw.Close();
		}

		public override void UjEredmeny(Eredmeny uj)
		{
			Eredmenyek.Add(uj);
			using (StreamWriter sw = File.AppendText("eredmenyek.csv"))
			{
				sw.WriteLine(uj);
			}
		}

		public override void UjFuto(Futo uj)
		{
			Futok.Add(uj);
			using (StreamWriter sw = File.AppendText("futok.csv"))
			{
				sw.WriteLine(uj);
			}
		}
	}
}
