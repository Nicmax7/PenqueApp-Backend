using BackEnd.Models.Clases;

namespace BackEnd.Models.DataTypes
{
    public class DtPuntaje
    {
        public int idPuntuacion { get; set; }
        public string usuario { get; set; }
        public int puntaje { get; set; }

        public int posicion { get; set; }
    }
}
