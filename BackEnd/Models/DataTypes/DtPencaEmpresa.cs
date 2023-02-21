using BackEnd.Models.Clases;

namespace BackEnd.Models.DataTypes
{
    public class DtPencaEmpresa
    {
        public int id { get; set; }
        public string nombre { get; set; }

        public Tipo_Deporte tipoDeporte { get; set; }

        public int idLiga { get; set; }

        public Tipo_Plan? tipoPlan { get; set; }

        public float? premioFinal { get; set; }

        public float? entrada { get; set; }

        public int idEmpresa { get; set; }

        public Tipo_Liga tipo_Liga { get; set; }

    }
}
