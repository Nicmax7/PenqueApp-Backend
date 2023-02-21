using BackEnd.Models.Clases;

namespace BackEnd.Models.DataTypes
{
    public class DtCompetencia
    {
        public int Id { get; set; }
        public Tipo_Area Area { get; set; }
        public DateTime fecha_competencia { get; set; }
        public string nombre { get; set; }
        public int topeParticipantes { get; set; }
        public bool estado { get; set; }
    }
}
