using BackEnd.Models.Clases;

namespace BackEnd.Models.DataTypes
{
    public class DtPenca
    {
        public int id { get; set; }

        public string nombre { get; set; }

        public bool estado { get; set; }

        public DateTime fecha_Creacion { get; set; }

        public Tipo_Deporte tipo_Deporte { get; set; }

        public Tipo_Liga tipo_Liga { get; set; }

        public Tipo_Plan? tipo_Plan { get; set; }

        public float? entrada { get; set; }

        public float? pozo { get; set; }

        public string color { get; set; }

        public Tipo_Penca tipoPenca { get; set; }

        public bool tieneAdmin { get; set; }
    }
}
