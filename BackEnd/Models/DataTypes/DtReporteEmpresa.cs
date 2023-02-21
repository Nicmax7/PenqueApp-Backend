using BackEnd.Models.Clases;

namespace BackEnd.Models.DataTypes
{
    public class DtReporteEmpresa
    {
        public int numeroDeUsuarios { get; set; }
        public int numeroDePencas { get; set; }
        public float? premiosRepartidos { get; set; }
        public int pencasFinalizadas { get; set; }
        public int pencasActivas { get; set; }
        public int pencasPremium { get; set; }
        public int pencasIndividuales { get; set; }
        public int pencasDeEquipo { get; set; }
        public int mensajesEnForos { get; set; }
        public DtPenca[]? pencas { get; set; }
    }
}
