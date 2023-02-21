using BackEnd.Models.Clases;

namespace BackEnd.Models.DataTypes
{
    public class DtPartido
    {
        public int id { get; set; }
        public DateTime fecha   { get; set; }

        public int Idlocal { get; set; }

        public int Idvisitante { get; set; }

        public string local { get; set; }

        public string visitante { get; set; }   

        public Tipo_Resultado resultado { get; set; }

        public Tipo_Deporte deporte { get; set; }
    }
}
