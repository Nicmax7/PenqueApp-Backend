using BackEnd.Models.Clases;

namespace BackEnd.Models.DataTypes
{
    public class DtPencasCompartida
    {
        public int id { get; set; }
        public string nombre { get; set; }

        public Tipo_Deporte tipoDeporte { get; set; }

        public float? entrada { get; set; }

        public float? pozo { get; set; }

        public int idLiga { get; set; }

        public Tipo_Liga Tipo_Liga { get; set; }

        public bool estado { get; set; }
        public bool estadoLiga { get; set; }

        public bool tieneAdmin { get; set; }

        public Tipo_Penca tipoPenca { get; set; }

    }
}
