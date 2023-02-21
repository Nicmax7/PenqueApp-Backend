using BackEnd.Models.Clases;

namespace BackEnd.Models.DataTypes
{
    public class DtLiga_Individual
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int topeCompetencias { get; set; }
        public Tipo_Area tipoArea { get; set; }
    }
}
