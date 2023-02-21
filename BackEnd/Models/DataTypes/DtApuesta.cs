using BackEnd.Models.Clases;

namespace BackEnd.Models.DataTypes
{
    public class DtApuesta
    {
        public int idCompetencia { get; set; }

        public int idUsuario { get; set; }

        public int idPenca { get; set; }

        public int idParticipante { get; set; }
    }
}
