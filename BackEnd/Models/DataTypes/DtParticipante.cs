using BackEnd.Models.Clases;

namespace BackEnd.Models.DataTypes
{
    public class DtParticipante
    {
        public int Id { get; set; }
        public string nombre { get; set; }
        public Tipo_Area Area { get; set; }
        public string pais { get; set; }
    }
}
