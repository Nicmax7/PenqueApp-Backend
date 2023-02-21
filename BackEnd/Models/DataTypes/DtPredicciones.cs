using BackEnd.Models.Clases;

namespace BackEnd.Models.DataTypes
{
    public class DtPredicciones
    {
        public int idPartido { get; set; }

        public int idUsuario { get; set; }

        public int idPenca { get; set; }

        public Tipo_Resultado tipo { get; set; }
    }
}
