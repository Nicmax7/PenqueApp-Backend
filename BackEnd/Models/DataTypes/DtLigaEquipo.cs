using BackEnd.Models.Clases;

namespace BackEnd.Models.DataTypes
{
    public class DtLigaEquipo
    {
        public string nombreLiga { get; set; }
        public int tope { get; set; }

        public int id { get; set; }

        public Tipo_Deporte tipoDeporte { get; set; }
    }
}
