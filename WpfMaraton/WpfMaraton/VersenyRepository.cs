using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfMaraton
{
    /// <summary>
    /// A verseny lebonyolításához szükséges adatokat kezeli. Az adatok az [eredmenyek.csv] és a [futok.csv] szöveges állományokban találhatóak.
    /// </summary>
    abstract class VersenyRepository
    {
        protected enum AdatForrasa { futok, eredmenyek };
        protected Dictionary<AdatForrasa, String> allomanyNevek
            = new Dictionary<AdatForrasa, string> {
                { AdatForrasa.futok, "futok.csv" },
                { AdatForrasa.eredmenyek, "eredmenyek.csv" }
            };

        List<Futo> futok; //A futók adatait tárolja
        List<Eredmeny> eredmenyek; //Az eredményeket tárolja
        public List<Futo> Futok { get => futok; set => futok = value; }
        public List<Eredmeny> Eredmenyek { get => eredmenyek; set => eredmenyek = value; }

        /// <summary>
        /// Betölti és eltárolja az [eredmenyek.csv] és a [futok.csv] fájlok tartalmát
        /// I/O hiba esetén kivételt dob, ahol az üzenetben szerepelteti melyik állománynál lépett fel a hiba. 
        /// </summary>
        public abstract void AdatokBetoltese();

        /// <summary>
        /// Új futó adatait veszi fel és egyben letárolja a [futok.csv] állományban hozzáfűzéssel.
        /// </summary>
        /// <param name="uj">Új versenyző adatai</param>
        public abstract void UjFuto(Futo uj);

        /// <summary>
        /// Új eredményt rögzít és egyben letárolja az [eredmenyek.csv] fájlban is az új adatsor hozzáfűzésével.
        /// </summary>
        /// <param name="uj">Futott új eredmény</param>
        public abstract void UjEredmeny(Eredmeny uj);

        /// <summary>
        /// A megadott eredményt törli az adatok közül. A törléssel egyidőben a szöveges állomány is felülíródik.
        /// </summary>
        /// <param name="torolni">A törlendő eredmény. Amennyiben nem létezik, akkor ne történjen semmi! (fájl felülírása sem)</param>
        public abstract void TorolEredmenyt(Eredmeny torolni);

        /// <summary>
        /// Felülírja a teljes szöveges állományt az aktuális értékekkel.
        /// </summary>
        /// <param name="mitKellIrni">futok -> [futok.csv] állományt írja felül a List<Futo> aktuális értékével</param>
        /// <param name="mitKellIrni">eredmenyek -> [eredmenyek.csv] állományt írja felül a List<Eredmeny> aktuális értékével</param>
        private void AdatokFelulirasa(AdatForrasa mitKellIrni)
        {
            switch (mitKellIrni)
            {
                case AdatForrasa.futok:
                    File.WriteAllLines(allomanyNevek[AdatForrasa.futok], futok.Select(obj => obj.ToString()).ToList());
                    break;
                case AdatForrasa.eredmenyek:
                    File.WriteAllLines(allomanyNevek[AdatForrasa.eredmenyek], eredmenyek.Select(obj => obj.ToString()).ToList());
                    break;
            }
        }
        /// <summary>
        /// Elkészíti a csapatok Dictionary adatszerkezetét, ahol a csapat sorszáma a kulcs (KEY) és a csapatban lévő versenyzők száma az érték (VALUE)
        /// </summary>
        public abstract Dictionary<int, int> Csapatok();

        /// <summary>
        /// Csapat taglista készítése
        /// </summary>
        /// <param name="csapatSzama">A csapat sorszáma</param>
        /// <returns>A csapat tagjainak listája</returns>
        public abstract List<Futo> Csapattagok(int csapatSzama);

        /// <summary>
        /// Meghatározza a csapat öszes idejét. A tagok futott idejét összeadva kapjuk meg. (Aki többször futott, annak több adata lesz az összegben.)
        /// </summary>
        /// <param name="csapatSzama">A csapat sorszáma</param>
        /// <returns>A csapat összes ideje</returns>
        public abstract int CsapatOsszideje(int csapatSzama);

    }
}
