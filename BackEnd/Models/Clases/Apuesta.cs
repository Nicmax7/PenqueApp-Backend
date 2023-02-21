namespace BackEnd.Models.Clases
{
    public class Apuesta
    {
        public int id { get; set; }
        public Competencia competencia { get; set; }
        public Usuario usuario { get; set; }

        public int idGanador { get; set; }

        public int idPuntuacionUsuario { get; set; }
        public int idPenca { get; set; }


    }
}
