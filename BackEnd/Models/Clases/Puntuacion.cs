namespace BackEnd.Models.Clases
{
    public enum estado_Penca
    {
        Aceptado,
        Rechazado,
        Pendiente,
        Invitado
    }
    public class Puntuacion
    {
        public int id { get; set; }

        public int puntos { get; set; }

        public Penca penca { get; set; }

        public Usuario usuario { get; set; }

        public estado_Penca estado { get; set; }
    }
}
