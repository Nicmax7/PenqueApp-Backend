using BackEnd.Models.Clases;
using System;
using System.Runtime.InteropServices;

namespace BackEnd.Models.Clases
{
    public class Empresa
    {
        public int  id { get; set; }
        public string email { get; set; }
        public string nombre { get; set; }
        public string pass { get; set; }
        public float? billetera { get; set; }
        public Tipo_Rol tipoRol { get; set; }

        public List<Chat> chats { get; set; }

        public List<Penca> pencas_empresa { get; set; }

        //Constructores

        public Empresa()
        {
            chats = new List<Chat>();
            pencas_empresa = new List<Penca>();
        }
        public void agregarFondos(float monto)
        {
            this.billetera += monto;
        }

        public void sustraerFondos(float monto)
        {
            this.billetera -= monto;
        }

       
    }
}
