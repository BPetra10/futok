using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfMaraton
{
    class Eredmeny
    {
        int futoID;
        int kor;
        int ido;

        public Eredmeny(int futoID, int kor, int ido)
        {
            this.futoID = futoID;
            this.kor = kor;
            this.ido = ido;
        }

        public Eredmeny(String adatSor)
        {
            //FELADAT!
            string[] tomb = adatSor.Split(';');
            futoID = Convert.ToInt32(tomb[0]);
            kor = Convert.ToInt32(tomb[1]);
            ido = Convert.ToInt32(tomb[2]);
        }

        public int FutoID { get => futoID;}
        public int Kor { get => kor; }
        public int Ido { get => ido;}

        public override string ToString()
        {
            return $"{this.futoID};{this.kor};{this.ido}";
        }
    }
}
