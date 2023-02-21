namespace BackEnd.Models.Clases
{
    public class Prediccion
    {
        public int Id { get; set; }
        public Tipo_Resultado tipo_Resultado { get; set; }

        public Partido partido { get; set; }

        public Usuario usuario { get; set; }

        public int idPuntuacionUsuario { get; set; }
        public int idPenca { get; set; }
    }
}
